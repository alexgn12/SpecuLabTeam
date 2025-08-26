import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ResumenRequest {
  // Define aquí los campos que devuelve el endpoint /api/Ang/resumen-requests
  // Por ahora lo dejamos como any[] hasta conocer la estructura real
  [key: string]: any;
}

@Injectable({ providedIn: 'root' })
export class ResumenRequestsService {
  private apiUrl = 'https://localhost:7092/api/Ang/resumen-requests';

  constructor(private http: HttpClient) {}

  getResumenRequests(): Observable<ResumenRequest[]> {
    return this.http.get<ResumenRequest[]>(this.apiUrl);
  }
}
