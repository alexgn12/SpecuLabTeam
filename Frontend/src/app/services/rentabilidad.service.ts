import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface AnalyzeJsonResponse {
  answer: string;
}

@Injectable({ providedIn: 'root' })

export class RentabilidadService {
  private apiUrl = 'https://devdemoapi3.azurewebsites.net/api/ServiceAI/analyze-json';

  constructor(private http: HttpClient) {}

  analyzeJson(data: any): Observable<AnalyzeJsonResponse> {
    return this.http.post<AnalyzeJsonResponse>(this.apiUrl, data);
  }
}
