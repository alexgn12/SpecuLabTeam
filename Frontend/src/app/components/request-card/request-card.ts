import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Boton } from '../boton/boton';
import { IRequest } from '../../pages/requests/requests.service';
import { DetalleRequest } from '../detalle-request/detalle-request';
import { RequestsService } from '../../pages/requests/requests.service';
import { Router } from '@angular/router';
import { AnalyzeBuildingRequestComponent } from '../analyze-building-request/analyze-building-request.component';

@Component({
  selector: 'sl-request-card',
  templateUrl: './request-card.html',
  styleUrls: ['./request-card.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, Boton, DetalleRequest, AnalyzeBuildingRequestComponent]
})
export class RequestCard {
  // Recibo el objeto completo para mayor comodidad
  @Input()
  request!: IRequest;

  @Input()
  building?: {
    buildingName: string;
    street: string;
    district: string;
    floorCount: number;
    yearBuilt: number;
    buildingCode: string;
  };

  @Output() statusChanged = new EventEmitter<void>();

  showDetails = false;
  showIARecommendation = false;
  iaRequestId: number | null = null;

  constructor(private requestsService: RequestsService, private router: Router) {}

  get totalAmount(): number {
    return (this.request?.buildingAmount || 0) + (this.request?.maintenanceAmount || 0);
  }

  get buildingName(): string {
    return this.building?.buildingName || 'N/A';
  }

  get showTotalAmount(): boolean {
    return (
      this.request.statusType !== 'Recibido' &&
      typeof this.request.buildingAmount === 'number' &&
      typeof this.request.maintenanceAmount === 'number' &&
      this.request.maintenanceAmount > 0
    );
  }
  get titleText(): string {
  // Derivamos un título “bonito” según el estado (como en tu mock)
  switch (this.request?.statusType) {
    case 'Recibido':
      return 'Nueva solicitud de compra';
    case 'Pendiente':
      return 'Presupuesto requiere revisión';
    case 'Aprobado':
      return 'Solicitud aprobada';
    case 'Rechazado':
      return 'Solicitud rechazada';
    default:
      return 'Solicitud de compra';
  }
}


  get totalAmountText(): string {
    return this.showTotalAmount
      ? ((this.request.buildingAmount || 0) + (this.request.maintenanceAmount || 0)).toString()
      : 'Por determinar';
  }


  aceptarRequest() {
    if (confirm('¿Quieres aceptar esta petición?')) {
      // statusId = 3 para Aprobado
      const buildingCode = this.building?.buildingCode || '';
      const comment = `El edificio ${buildingCode} ha sido aceptado`;
      this.requestsService.updateRequestStatusPatch(this.request.requestId, 3, comment).subscribe({
        next: () => {
          alert('La petición ha sido aceptada');
          this.statusChanged.emit();
        },
        error: err => alert('Error al aceptar la petición')
      });
    }
  }

  rechazarRequest() {
    if (confirm('¿Quieres rechazar esta petición?')) {
      // statusId = 4 para Rechazado
      const buildingCode = this.building?.buildingCode || '';
      const comment = `El edificio ${buildingCode} ha sido rechazado`;
      this.requestsService.updateRequestStatusPatch(this.request.requestId, 4, comment).subscribe({
        next: () => {
          alert('La petición ha sido rechazada');
          this.statusChanged.emit();
        },
        error: err => alert('Error al rechazar la petición')
      });
    }
  }

  openIARecommendation() {
    this.iaRequestId = this.request.requestId;
    this.showIARecommendation = true;
  }
  closeIARecommendation() {
    this.showIARecommendation = false;
    this.iaRequestId = null;
  }
}
