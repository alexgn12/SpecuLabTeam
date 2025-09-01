import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { ApprovedBuilding, IncomeApartment } from './patrimony.service';

@Component({
  selector: 'app-patrimony-detail-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  template: `
<div class="modal-content">
  <div class="modal-header">
  <h2 mat-dialog-title class="modal-title">
    Detalles
  </h2>
  <span class="type-chip" 
        [class.is-building]="data.type==='building'" 
        [class.is-apartment]="data.type==='apartment'">
    <ng-container *ngIf="data.type === 'building'; else aptIcon">
      🏢 Edificio
    </ng-container>
    <ng-template #aptIcon>
      🏠 Apartamento
    </ng-template>
  </span>
</div>

  <div mat-dialog-content class="modal-body">
    <ng-container *ngIf="data.type === 'building'; else apartmentBlock">
      <div class="field"><span class="label">Código: </span><span class="value">{{ data.item.buildingCode }}</span></div>
      <div class="field"><span class="label">Nombre: </span><span class="value">{{ data.item.buildingName }}</span></div>
      <div class="field"><span class="label">Dirección: </span><span class="value">{{ data.item.street }}</span></div>
      <div class="field"><span class="label">Distrito: </span><span class="value">{{ data.item.district }}</span></div>
      <div class="field"><span class="label">Año: </span><span class="value">{{ data.item.yearBuilt }}</span></div>
      <div class="field"><span class="label">Plantas: </span><span class="value">{{ data.item.floorCount }}</span></div>
      <div class="field"><span class="label">Fecha alta: </span><span class="value">{{ data.item.createdDate | date:'mediumDate' }}</span></div>
    </ng-container>

    <ng-template #apartmentBlock>
      <div class="field"><span class="label">Código: </span><span class="value">{{ data.item.apartmentCode }}</span></div>
      <div class="field"><span class="label">Puerta: </span><span class="value">{{ data.item.apartmentDoor }}</span></div>
      <div class="field"><span class="label">Piso: </span><span class="value">{{ data.item.apartmentFloor }}</span></div>
      <div class="field"><span class="label">Precio: </span><span class="value">{{ data.item.apartmentPrice | currency:'EUR':'symbol':'1.0-0' }}</span></div>
      <div class="field"><span class="label">Habitaciones: </span><span class="value">{{ data.item.numberOfRooms }}</span></div>
      <div class="field"><span class="label">Baños: </span><span class="value">{{ data.item.numberOfBathrooms }}</span></div>
      <div class="field"><span class="label">Ascensor: </span><span class="value">{{ data.item.hasLift ? 'Sí' : 'No' }}</span></div>
      <div class="field"><span class="label">Garaje: </span><span class="value">{{ data.item.hasGarage ? 'Sí' : 'No' }}</span></div>
      <div class="field"><span class="label">Fecha de alta: </span><span class="value">{{ data.item.createdDate | date:'mediumDate' }}</span></div>
    </ng-template>
  </div>

  <div mat-dialog-actions align="end" class="modal-actions">
    <button mat-button mat-dialog-close class="btn-cool-close">
      <span class="btn-cool-close-icon">✕</span>
      <span class="btn-cool-close-text">Cerrar</span>
    </button>
  </div>
</div>

  `,
  styleUrls: ['./patrimony-detail-modal.component.css']
})
export class PatrimonyDetailModalComponent {
  constructor(
    public dialogRef: MatDialogRef<PatrimonyDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { type: 'building' | 'apartment', item: ApprovedBuilding | IncomeApartment }
  ) {}
}
