import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Building {
  buildingId: number;
  buildingCode?: string;
  buildingName?: string;
  street?: string;
  district?: string;
  createdDate?: string;
  floorCount?: number;
  yearBuilt?: number;
}

@Injectable({ providedIn: 'root' })
export class BuildingService {
  private baseUrl = `https://localhost:7092/api/Building`;

  constructor(private http: HttpClient) {}

  getBuildingById(id: number): Observable<Building> {
    return this.http.get<Building>(`${this.baseUrl}/${id}`);
  }
}
