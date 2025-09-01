import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface EstadoResumen {
  estado: string;
  total: number;
}

export interface ResumenRequestResponse {
  totalGeneral: number;
  porEstado: EstadoResumen[];
}

@Injectable({ providedIn: 'root' })
export class ResumenRequestsService {
  private apiUrl = 'https://devdemoapi3.azurewebsites.net/api/Ang/resumen-requests';

  constructor(private http: HttpClient) {}

  getResumenRequests(): Observable<ResumenRequestResponse> {
    return this.http.get<ResumenRequestResponse>(this.apiUrl);
  }
}
