import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ApprovedBuilding {
  buildingId: number;
  buildingCode: string;
  buildingName: string;
  street: string;
  district: string;
  createdDate: string;
  floorCount: number;
  yearBuilt: number;
}

export interface IncomeApartment {
  apartmentId: number;
  apartmentCode: string;
  apartmentDoor: string;
  apartmentFloor: string;
  apartmentPrice: number;
  numberOfRooms: number;
  numberOfBathrooms: number;
  buildingId: number;
  hasLift: boolean;
  hasGarage: boolean;
  createdDate: string;
}

export interface PatrimonyResponse {
  approvedBuildings: ApprovedBuilding[];
  incomeApartments: IncomeApartment[];
}

@Injectable({
  providedIn: 'root'
})
export class PatrimonyService {
  private apiUrl = 'https://localhost:7092/api/Ang/aprobados-e-ingresos';

  constructor(private http: HttpClient) {}

  getPatrimony(): Observable<PatrimonyResponse> {
    return this.http.get<PatrimonyResponse>(this.apiUrl);
  }
}
