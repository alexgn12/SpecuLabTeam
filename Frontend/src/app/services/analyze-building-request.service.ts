import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AnalyzeBuildingRequest {
  requestId: number;
}

@Injectable({ providedIn: 'root' })
export class AnalyzeBuildingRequestService {
  private apiUrl = 'https://localhost:7092/api/ServiceAI/analyze-building-request';

  constructor(private http: HttpClient) {}

  analyzeRequest(data: AnalyzeBuildingRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }
}
