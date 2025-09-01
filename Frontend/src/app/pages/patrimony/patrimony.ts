
import { Component, OnInit } from '@angular/core';
import { PatrimonyService, ApprovedBuilding, IncomeApartment } from './patrimony.service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { PatrimonyDetailModalComponent } from './patrimony-detail-modal.component';

@Component({
  selector: 'app-patrimony',
  templateUrl: './patrimony.html',
  styleUrls: ['./patrimony.css'],
  standalone: true,
  imports: [CommonModule, MatDialogModule, PatrimonyDetailModalComponent],
  providers: [CurrencyPipe]
})
export class PatrimonyComponent implements OnInit {
  approvedBuildings: ApprovedBuilding[] = [];
  incomeApartments: IncomeApartment[] = [];
  loading = true;
  error: string | null = null;

  // Selector de pestaña
  selectedTab: 'buildings' | 'apartments' = 'buildings';

  // Paginación
  buildingsPage = 1;
  apartmentsPage = 1;
  readonly pageSize = 6;

  get pagedBuildings(): ApprovedBuilding[] {
    const start = (this.buildingsPage - 1) * this.pageSize;
    return this.approvedBuildings.slice(start, start + this.pageSize);
  }
  get pagedApartments(): IncomeApartment[] {
    const start = (this.apartmentsPage - 1) * this.pageSize;
    return this.incomeApartments.slice(start, start + this.pageSize);
  }
  get buildingsTotalPages(): number {
    return Math.max(1, Math.ceil(this.approvedBuildings.length / this.pageSize));
  }
  get apartmentsTotalPages(): number {
    return Math.max(1, Math.ceil(this.incomeApartments.length / this.pageSize));
  }

  constructor(private patrimonyService: PatrimonyService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.patrimonyService.getPatrimony().subscribe({
      next: (data) => {
        this.approvedBuildings = data.approvedBuildings || [];
        this.incomeApartments = data.incomeApartments || [];
        this.loading = false;
        this.buildingsPage = 1;
        this.apartmentsPage = 1;
      },
      error: () => {
        this.error = 'Error al cargar los datos de patrimonio';
        this.loading = false;
      }
    });
  }

  selectTab(tab: 'buildings' | 'apartments') {
    this.selectedTab = tab;
  }

  nextPage(tab: 'buildings' | 'apartments') {
    if (tab === 'buildings' && this.buildingsPage < this.buildingsTotalPages) {
      this.buildingsPage++;
    } else if (tab === 'apartments' && this.apartmentsPage < this.apartmentsTotalPages) {
      this.apartmentsPage++;
    }
  }

  prevPage(tab: 'buildings' | 'apartments') {
    if (tab === 'buildings' && this.buildingsPage > 1) {
      this.buildingsPage--;
    } else if (tab === 'apartments' && this.apartmentsPage > 1) {
      this.apartmentsPage--;
    }
  }

  viewBuildingDetails(building: ApprovedBuilding) {
    this.dialog.open(PatrimonyDetailModalComponent, {
      data: { type: 'building', item: building },
      panelClass: 'mat-dialog-rounded',
      autoFocus: false
    });
  }

  viewApartmentDetails(apartment: IncomeApartment) {
    this.dialog.open(PatrimonyDetailModalComponent, {
      data: { type: 'apartment', item: apartment },
      panelClass: 'mat-dialog-rounded',
      autoFocus: false
    });
  }
}
