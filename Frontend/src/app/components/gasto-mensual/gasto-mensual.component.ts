import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BudgetService } from '../../pages/budget/budget.service';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';

// Opcional: define la interfaz de la respuesta para evitar usar any
interface MonthlyGastoIngreso {
  año: number;
  mes: number;
  transactionType: 'GASTO' | 'INGRESO';
  amount?: number;
  totalGasto?: number;
}

type ViewMode = 'gasto' | 'ingreso';

@Component({
  selector: 'gasto-mensual',
  templateUrl: './gasto-mensual.component.html',
  styleUrls: ['./gasto-mensual.component.css'],
  standalone: true,
  imports: [CommonModule, NgChartsModule]
})
export class GastoMensualComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  loading = true;
  error: string | null = null;

  // Estado del selector (por defecto: gastos)
  view: ViewMode = 'gasto';

  monthlyExpensesLabels: string[] = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];
  monthlyExpensesData: number[] = Array(12).fill(0);
  monthlyIncomeData: number[] = Array(12).fill(0);

  // Configuración de datos del gráfico (1 sola serie visible)
  barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: this.monthlyExpensesLabels,
    datasets: []
  };

  // Configuración de opciones del gráfico
  barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    layout: { padding: 8 },
    plugins: {
      legend: {
        display: false,                 // oculto porque solo hay 1 serie visible
      },
      tooltip: {
        enabled: true,
        padding: 10,
        bodyFont: { size: 12 },
        callbacks: {
          label: (ctx) => {
            const val = ctx.parsed.y ?? 0;
            const nf = new Intl.NumberFormat('es-ES', {
              style: 'currency', currency: 'EUR', maximumFractionDigits: 0
            });
            return `${ctx.dataset.label}: ${nf.format(val)}`;
          }
        }
      },
      datalabels: { display: false } as any
    },
    scales: {
      x: {
        grid: { display: false, drawBorder: false },
        ticks: {
          color: '#64748b',
          font: { size: 12 },
          maxRotation: 0,
          autoSkip: true,
          padding: 6
        }
      },
      y: {
        grid: {
          color: '#e8edf5',
          lineWidth: 1,
          drawBorder: false
        },
        ticks: {
          color: '#64748b',
          font: { size: 12 },
          padding: 6,
          callback: (v) =>
            new Intl.NumberFormat('es-ES', {
              style: 'currency', currency: 'EUR', maximumFractionDigits: 0
            }).format(Number(v))
        }
      }
    },
    animation: {
      duration: 600,
      easing: 'easeOutQuart'
    }
  };

  barChartType: 'bar' = 'bar';

  years: number[] = [];
  selectedYear: number | null = null;
  allData: MonthlyGastoIngreso[] = [];

  constructor(private budgetService: BudgetService) {}

  ngOnInit(): void {
    this.fetchDataAndInit();
  }

  fetchDataAndInit() {
    this.loading = true;
    this.budgetService.getMonthlyGastoIngreso().subscribe({
      next: (result) => {
        this.allData = result;
        this.years = Array.from(new Set(result.map(r => r.año))).sort((a, b) => b - a);

        // Seleccionar año actual si existe, si no el más reciente
        const currentYear = new Date().getFullYear();
        this.selectedYear = this.years.includes(currentYear) ? currentYear : this.years[0];

        this.updateChartForYear();
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar datos de gasto/ingreso mensual';
        this.loading = false;
      }
    });
  }

  onYearChange(event: Event) {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedYear = parseInt(value, 10);
    this.updateChartForYear();
  }

  onSelect(mode: ViewMode) {
    if (this.view === mode) return;
    this.view = mode;
    this.buildSingleDataset();      // reconstruye la serie visible
    this.chart?.update();
  }

  updateChartForYear() {
    if (!this.selectedYear) return;

    const gastos = this.allData.filter(r => r.transactionType === 'GASTO' && r.año === this.selectedYear);
    const ingresos = this.allData.filter(r => r.transactionType === 'INGRESO' && r.año === this.selectedYear);

    const monthlyExpenses = Array(12).fill(0);
    const monthlyIncome = Array(12).fill(0);

    gastos.forEach(g => {
      if (g.mes >= 1 && g.mes <= 12) {
        monthlyExpenses[g.mes - 1] = g.amount ?? g.totalGasto ?? 0;
      }
    });

    ingresos.forEach(i => {
      if (i.mes >= 1 && i.mes <= 12) {
        // Nota: si tu API usa totalIngreso, cámbialo aquí.
        monthlyIncome[i.mes - 1] = i.amount ?? i.totalGasto ?? 0;
      }
    });

    this.monthlyExpensesData = monthlyExpenses;
    this.monthlyIncomeData = monthlyIncome;

    // Construye la vista con el modo actual (gasto/ingreso)
    this.buildSingleDataset();
  }

  private buildSingleDataset() {
    const isGasto = this.view === 'gasto';
    const data = isGasto ? this.monthlyExpensesData : this.monthlyIncomeData;

    const label = isGasto ? 'Gastos' : 'Ingresos';
    const backgroundColor = isGasto ? '#dc2626' : '#16a34a';
    const hoverBackgroundColor = isGasto ? '#ef4444' : '#22c55e';

    this.barChartData = {
      labels: this.monthlyExpensesLabels,
      datasets: [
        {
          data,
          label,
          backgroundColor,
          hoverBackgroundColor,
          borderColor: '#ffffff',
          borderWidth: 1,
          borderRadius: 8,
          borderSkipped: false as any
        }
      ]
    };
  }
}
