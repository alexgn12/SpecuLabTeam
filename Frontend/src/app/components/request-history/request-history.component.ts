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
    this.historyService.getHistory(
      this.page,
      this.pageSize,
      this.fromDate || undefined,
      this.toDate || undefined,
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
}
