import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Boton } from '../boton/boton';
import { IRequest } from '../../pages/requests/requests.service';
import { DetalleRequest } from '../detalle-request/detalle-request';
import { RequestsService } from '../../pages/requests/requests.service';
import { Router } from '@angular/router';

@Component({
  selector: 'sl-request-card',
  templateUrl: './request-card.html',
  styleUrls: ['./request-card.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, Boton, DetalleRequest]
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

  showDetails = false;

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

  get totalAmountText(): string {
    return this.showTotalAmount
      ? ((this.request.buildingAmount || 0) + (this.request.maintenanceAmount || 0)).toString()
      : 'Por determinar';
  }

  aceptarRequest() {
    if (confirm('¿Quieres aceptar esta petición?')) {
      this.requestsService.updateRequestStatus(this.request.requestId, 'Aprobado').subscribe({
        next: () => window.location.reload(),
        error: err => alert('Error al aceptar la petición')
      });
    }
  }

  rechazarRequest() {
    if (confirm('¿Quieres rechazar esta petición?')) {
      this.requestsService.updateRequestStatus(this.request.requestId, 'Rechazado').subscribe({
        next: () => window.location.reload(),
        error: err => alert('Error al rechazar la petición')
      });
    }
  }
}
