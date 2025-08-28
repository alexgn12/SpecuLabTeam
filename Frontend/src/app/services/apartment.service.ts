import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Apartment {
  apartmentId: number;
  apartmentCode?: string;
  apartmentDoor?: string;
  apartmentFloor?: string;
  apartmentPrice?: number;
  numberOfRooms?: number;
  numberOfBathrooms?: number;
  buildingId?: number;
  hasLift?: boolean;
  hasGarage?: boolean;
  createdDate?: string;
}

@Injectable({ providedIn: 'root' })
export class ApartmentService {
  private baseUrl = `https://localhost:7092/api/Apartments`;

  constructor(private http: HttpClient) {}

  getApartmentById(id: number): Observable<Apartment> {
    return this.http.get<Apartment>(`${this.baseUrl}/${id}`);
  }
}
