import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionsService, Transaction } from '../../pages/budget/transactions.service';

@Component({
  selector: 'transactions-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './transactions-page.component.html'
})
export class TransactionsPageComponent implements OnInit {
  transactions: Transaction[] = [];
  loading = false;
  error: string | null = null;

  filters = {
    month: '',
    type: ''
  };

  months = [
    { value: '01', label: 'Enero' },
    { value: '02', label: 'Febrero' },
    { value: '03', label: 'Marzo' },
    { value: '04', label: 'Abril' },
    { value: '05', label: 'Mayo' },
    { value: '06', label: 'Junio' },
    { value: '07', label: 'Julio' },
    { value: '08', label: 'Agosto' },
    { value: '09', label: 'Septiembre' },
    { value: '10', label: 'Octubre' },
    { value: '11', label: 'Noviembre' },
    { value: '12', label: 'Diciembre' }
  ];

  constructor(private tx: TransactionsService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch(): void {
    this.loading = true;
    this.error = null;

    const { month, type } = this.filters;
    const page = 1;
    const size = 20;
    const transactionType = type === 'INGRESO' || type === 'GASTO' ? type : undefined;

    this.tx.getTransactions({
      transactionType,
      page,
      size
    }).subscribe({
      next: (data: Transaction[]) => { this.transactions = data; this.loading = false; },
      error: (err: any) => { this.error = 'Error al cargar transacciones'; this.loading = false; }
    });
  }
}
