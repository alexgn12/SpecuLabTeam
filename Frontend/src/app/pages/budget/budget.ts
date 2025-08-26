import { Component, OnInit } from '@angular/core';

import { BudgetService, ManagementBudget } from './budget.service';
import { TransactionsService, Transaction } from './transactions.service';


import { CommonModule } from '@angular/common';

import { InfoCard } from '../../components/info-card/info-card';
import { TransactionsPageComponent } from "../../components/transactions-page/transactions-page.component";
import { GastoMensualComponent } from '../../components/gasto-mensual/gasto-mensual.component';

@Component({
  selector: 'sl-budget',
  templateUrl: './budget.html',
  styleUrls: ['./budget.css'],
  standalone: true,
  imports: [CommonModule, InfoCard, TransactionsPageComponent, GastoMensualComponent]
})

export class Budget implements OnInit {
  totalAmount: number = 0;
  totalPatrimony: number = 0;
  buildingsCount: number = 0;

  loading = true;
  error: string | null = null;


  constructor(private budgetService: BudgetService, private transactionsService: TransactionsService) {}

  ngOnInit(): void {
    this.loadBudgets();
    this.loadBuildings();
    this.loading = false;
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
    this.transactionsService.getTransactions({ size: 300 }).subscribe({
      next: (result: { items: Transaction[]; total: number }) => {
        const gastos = result.items.filter(tx => tx.type === 'GASTO');
        this.buildingsCount = gastos.length;
        // Sumar buildingAmount de cada gasto (si existe)
        this.totalPatrimony = gastos.reduce((sum, tx) => sum + (tx.buildingAmount || 0), 0);
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar transacciones de edificios';
        this.loading = false;
      }
    });
  }
}