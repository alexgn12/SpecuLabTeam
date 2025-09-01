import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TransactionsService, Transaction } from '../../pages/budget/transactions.service';
import { ApartmentService, Apartment } from '../../services/apartment.service';
import { BuildingService, Building } from '../../services/building.service';
import { ApartmentDetailModalComponent } from '../apartment-detail/apartment-detail-modal.component';
import { BuildingDetailModalComponent } from '../building-detail/building-detail-modal.component';
import { PaginacionComponent } from '../paginacion/paginacion.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'transactions-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ApartmentDetailModalComponent, BuildingDetailModalComponent, MatDialogModule, PaginacionComponent],
  templateUrl: './transactions-page.component.html',
  styleUrls: ['./transactions-page.component.css']
})
export class TransactionsPageComponent implements OnInit {
  transactions: Transaction[] = [];
  loading = false;
  error: string | null = null;

  filters = {
    month: '',
    year: '',
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

  years = Array.from({length: 5}, (_, i) => String(new Date().getFullYear() - i));

  currentPage = 1;
  totalPages = 1;
  pageSize = 20;
  totalItems = 0;

  private dialog = inject(MatDialog);

  constructor(
    private tx: TransactionsService,
    private apartmentService: ApartmentService,
    private buildingService: BuildingService
  ) {}

  onViewClick(t: Transaction) {
    if (t.type === 'INGRESO' && t.apartmentId) {
      this.apartmentService.getApartmentById(t.apartmentId).subscribe({
        next: (apartment) => {
          this.dialog.open(ApartmentDetailModalComponent, {
            data: { apartment },
            panelClass: 'custom-modal-panel'
          });
        },
        error: () => {
          this.dialog.open(ApartmentDetailModalComponent, {
            data: { apartment: null },
            panelClass: 'custom-modal-panel'
          });
        }
      });
    } else if (t.type === 'GASTO' && t.buildingId) {
      this.buildingService.getBuildingById(t.buildingId).subscribe({
        next: (building) => {
          this.dialog.open(BuildingDetailModalComponent, {
            data: { building },
            panelClass: 'custom-modal-panel'
          });
        },
        error: () => {
          this.dialog.open(BuildingDetailModalComponent, {
            data: { building: null },
            panelClass: 'custom-modal-panel'
          });
        }
      });
    }
  }



  ngOnInit(): void {
    this.fetch();
  }

  fetch(page: number = this.currentPage): void {
    this.loading = true;
    this.error = null;
    this.currentPage = page;

    const { month, year, type } = this.filters;
    const transactionType = type === 'INGRESO' || type === 'GASTO' ? type : undefined;

    this.tx.getTransactions({
      transactionType,
      page: this.currentPage,
      size: this.pageSize,
      month: month || undefined,
      year: year || undefined
    }).subscribe({
      next: (result: { items: Transaction[]; total: number }) => {
        this.transactions = result.items;
        this.totalItems = result.total;
        this.totalPages = Math.max(1, Math.ceil(this.totalItems / this.pageSize));
        this.loading = false;
      },
      error: (err: any) => { this.error = 'Error al cargar transacciones'; this.loading = false; }
    });
  }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.fetch(this.currentPage + 1);
    }
  }

  prevPage() {
    if (this.currentPage > 1) {
      this.fetch(this.currentPage - 1);
    }
  }

  firstPage() {
    this.fetch(1);
  }

  lastPage() {
    this.fetch(this.totalPages);
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.fetch(1);
  }

  resetFilters(): void {
    this.filters = {
      month: '',
      year: '',
      type: ''
    };
    this.currentPage = 1;
    this.fetch(1);
  }
}
