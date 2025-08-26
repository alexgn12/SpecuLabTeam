import { Component, OnInit } from '@angular/core';

import { BudgetService, ManagementBudget } from './budget.service';
import { TransactionsService, Transaction } from './transactions.service';


import { CommonModule } from '@angular/common';

import { InfoCard } from '../../components/info-card/info-card';
import { ChartConfiguration } from 'chart.js';
import { TransactionsPageComponent } from "../../components/transactions-page/transactions-page.component";
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'sl-budget',
  templateUrl: './budget.html',
  styleUrls: ['./budget.css'],
  standalone: true,
  imports: [CommonModule, InfoCard, TransactionsPageComponent, BaseChartDirective]
})

export class Budget implements OnInit {
  totalAmount: number = 0;
  totalPatrimony: number = 0;
  buildingsCount: number = 0;

  loading = true;
  error: string | null = null;

  // Datos para el gráfico de barras de gasto mensual
  monthlyExpensesLabels: string[] = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];
  monthlyExpensesData: number[] = Array(12).fill(0);
  monthlyIncomeData: number[] = Array(12).fill(0);

  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: this.monthlyExpensesLabels,
    datasets: [
      {
        data: this.monthlyExpensesData,
        label: 'Gastos',
        backgroundColor: '#dc2626', // rojo
        borderRadius: 6
      },
      {
        data: this.monthlyIncomeData,
        label: 'Ingresos',
        backgroundColor: '#16a34a', // verde
        borderRadius: 6
      }
    ]
  };
  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    plugins: {
      legend: { display: false },
      title: { display: true, text: 'Gasto a lo largo de los meses' }
    },
    scales: {
      x: {},
      y: { beginAtZero: true }
    }
  };
  barChartType: 'bar' = 'bar';

  constructor(private budgetService: BudgetService, private transactionsService: TransactionsService) {}

  ngOnInit(): void {
    this.loadBudgets();
    this.loadBuildings();
    this.loadMonthlyExpenses();
    this.loading = false;
  }

  private loadMonthlyExpenses() {
    this.transactionsService.getTransactions({ size: 300 }).subscribe({
      next: (result: { items: Transaction[]; total: number }) => {
        const transactions = result.items;
        const monthlyExpenses = Array(12).fill(0);
        const monthlyIncome = Array(12).fill(0);
        transactions.forEach(tx => {
          const date = new Date(tx.transactionDate);
          const month = date.getMonth();
          if (tx.type === 'GASTO') {
            monthlyExpenses[month] += 1;
          } else if (tx.type === 'INGRESO') {
            monthlyIncome[month] += 1;
          }
        });
        this.monthlyExpensesData = monthlyExpenses;
        this.monthlyIncomeData = monthlyIncome;
        this.barChartData = {
          labels: this.monthlyExpensesLabels,
          datasets: [
            {
              data: this.monthlyExpensesData,
              label: 'Gastos',
              backgroundColor: '#dc2626',
              borderRadius: 6
            },
            {
              data: this.monthlyIncomeData,
              label: 'Ingresos',
              backgroundColor: '#16a34a',
              borderRadius: 6
            }
          ]
        };
      },
      error: () => {
        this.error = 'Error al cargar transacciones';
      }
    });
  }

  private loadBudgets() {
    this.budgetService.getBudgets().subscribe({
      next: (data: ManagementBudget[]) => {
        if (data && data.length > 0) {
          // Actualiza el presupuesto actual con el ultimo dato (ultima fecha)
          const latest = data.reduce((a, b) =>
            new Date(a.lastUpdatedDate) > new Date(b.lastUpdatedDate) ? a : b
          );
          this.totalAmount = latest.currentAmount;
        }
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar presupuestos';
        this.loading = false;
      }
    });
  }

  private loadBuildings() {
    this.budgetService.getRequests().subscribe({
      next: (requests: any[]) => {
        this.totalPatrimony = requests.reduce((sum, r) => sum + (r.buildingAmount || 0), 0);
        this.buildingsCount = new Set(requests.map(r => r.buildingId)).size;
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar requests';
        this.loading = false;
      }
    });
  }
}