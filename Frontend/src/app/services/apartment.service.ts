import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

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
  private baseUrl = `https://devdemoapi3.azurewebsites.net/api/Apartments`;

  constructor(private http: HttpClient) {}

  getApartmentById(id: number): Observable<Apartment> {
    return this.http.get<any>(`${this.baseUrl}/${id}`).pipe(
      map(res => res.value)
    );
  }
}
