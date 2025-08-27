import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IRequest } from '../../pages/requests/requests.service';

@Component({
  selector: 'sl-detalle-request',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './detalle-request.html',
  styleUrl: './detalle-request.css'
})
export class DetalleRequest {
  @Input() request!: IRequest;
  @Input() building?: {
    buildingName: string;
    street: string;
    district: string;
    floorCount: number;
    yearBuilt: number;
    buildingCode: string;
  };
}
