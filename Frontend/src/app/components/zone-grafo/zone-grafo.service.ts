import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BuildingsCountByDistrict {
  district: string;
  count: number;
}
//Cambio pequeño
@Injectable({
  providedIn: 'root',
})
export class ZoneGrafoService {
  private apiUrl = 'https://devdemoapi3.azurewebsites.net/api/Ang/buildings-count-by-district';

  constructor(private http: HttpClient) {}

  getBuildingsCountByDistrict(): Observable<BuildingsCountByDistrict[]> {
    return this.http.get<BuildingsCountByDistrict[]>(this.apiUrl);
  }
}
