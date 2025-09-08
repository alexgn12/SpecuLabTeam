import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface AnalyzeJsonResponse {
  answer: string;
}

@Injectable({ providedIn: 'root' })

export class RentabilidadService {
  private apiUrl = 'https://localhost:7092/api/ServiceAI/analyze-json'; // Cambia esto por la URL real de tu API

  constructor(private http: HttpClient) {}

  analyzeJson(data: any): Observable<AnalyzeJsonResponse> {
    return this.http.post<AnalyzeJsonResponse>(this.apiUrl, data);
  }
}
