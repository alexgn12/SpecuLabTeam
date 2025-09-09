
import { Component, OnInit } from '@angular/core';
import { PatrimonyService, ApprovedBuilding, IncomeApartment } from './patrimony.service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion'; 
import { PatrimonyDetailModalComponent } from '../../components/patrimony-detail-modal/patrimony-detail-modal.component';

@Component({
  selector: 'app-patrimony',
  templateUrl: './patrimony.html',
  styleUrls: ['./patrimony.css'],
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatExpansionModule],
  providers: [CurrencyPipe]
})
export class PatrimonyComponent implements OnInit {
  getApartmentsByBuildingCode(buildingCode: string): IncomeApartment[] {
    return this.incomeApartments.filter(a => a.buildingCode === buildingCode);
  }
  approvedBuildings: ApprovedBuilding[] = [];
  incomeApartments: IncomeApartment[] = [];
  loading = true;
  error: string | null = null;

  // Selector de pestaña
  selectedTab: 'buildings' | 'apartments' = 'buildings';

  // Mostrar todos los resultados sin paginación
  get pagedBuildings(): ApprovedBuilding[] {
    return this.approvedBuildings;
  }
  get pagedApartments(): IncomeApartment[] {
    return this.incomeApartments;
  }

  // --- Paginación (comentada, para restaurar en el futuro) ---
  /*
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
  */

  constructor(private patrimonyService: PatrimonyService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.patrimonyService.getPatrimony().subscribe({
      next: (data) => {
        this.approvedBuildings = data.approvedBuildings || [];
        this.incomeApartments = data.incomeApartments || [];
        this.loading = false;
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

  // Métodos de paginación eliminados: ahora se muestran todos los resultados

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
