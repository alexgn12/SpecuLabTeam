import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RequestStatusHistoryService, RequestStatusHistoryApiResponse } from '../../services/request-status-history.service';
import { RequestStatusHistoryDto } from '../../models/request-status-history.dto';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-request-history',
  standalone: true,
  templateUrl: './request-history.component.html',
  styleUrls: ['./request-history.component.css'],
  providers: [DatePipe],
  imports: [CommonModule, FormsModule]
})
export class RequestHistoryComponent implements OnInit {
  resetFilters(): void {
    this.requestId = null;
    this.fromDate = null;
    this.toDate = null;
    this.page = 1;
    this.fetchHistory();
  }
  formatToEuropean(date: string): string {
    if (!date) return '';
    const [year, month, day] = date.split('-');
    return `${day}/${month}/${year}`;
  }
  history: RequestStatusHistoryDto[] = [];
  page = 1;
  pageSize = 10;
  totalRecords = 0;
  fromDate: string | null = null;
  toDate: string | null = null;
  requestId: number | null = null;
  errorMsg: string | null = null;

  constructor(
    private historyService: RequestStatusHistoryService,
    private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.fetchHistory();
  }

  fetchHistory(): void {
    this.errorMsg = null;
    // Enviar fechas en formato YYYY-MM-DDTHH:mm:ss
    let fromDateParam = this.fromDate ? `${this.fromDate}T00:00:00` : undefined;
    let toDateParam = this.toDate ? `${this.toDate}T23:59:59` : undefined;
    this.historyService.getHistory(
      this.page,
      this.pageSize,
      fromDateParam,
      toDateParam,
      this.requestId || undefined
    ).subscribe({
      next: (response: RequestStatusHistoryApiResponse) => {
        this.history = response.histories;
        this.totalRecords = response.totalCount;
        if (!response.histories || response.histories.length === 0) {
          this.errorMsg = 'No se encontraron datos para los filtros seleccionados.';
        }
      },
      error: err => {
        this.history = [];
        this.totalRecords = 0;
        this.errorMsg = 'Error al consultar el historial. Verifica la conexión con el backend.';
        console.error('Error en la consulta de historial:', err);
      }
    });
  }

  // Ya no se necesita la conversión a ISO, se envía YYYY-MM-DD directamente

  onPageChange(event: any): void {
    this.page = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.fetchHistory();
  }

  applyFilters(): void {
    this.page = 1;
    this.fetchHistory();
  }

  formatDate(date: string): string {
    return this.datePipe.transform(date, 'dd/MM/yyyy HH:mm') || '';
  }

  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.fetchHistory();
    }
  }

  nextPage(): void {
    if (this.history.length === this.pageSize) {
      this.page++;
      this.fetchHistory();
    }
  }

  firstPage(): void {
    if (this.page !== 1) {
      this.page = 1;
      this.fetchHistory();
    }
  }

  lastPage(): void {
    const totalPages = Math.ceil(this.totalRecords / this.pageSize);
    if (this.page !== totalPages && totalPages > 0) {
      this.page = totalPages;
      this.fetchHistory();
    }
  }
}
