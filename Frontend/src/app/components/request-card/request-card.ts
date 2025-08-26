import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Boton } from '../boton/boton';
import { IRequest } from '../../pages/requests/requests.service';

@Component({
  selector: 'sl-request-card',
  templateUrl: './request-card.html',
  styleUrls: ['./request-card.css'],
  standalone: true,
  imports: [CommonModule, Boton]
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

  get totalAmount(): number {
    return (this.request?.buildingAmount || 0) + (this.request?.maintenanceAmount || 0);
  }

  get buildingName(): string {
    return this.building?.buildingName || 'N/A';
  }
}
