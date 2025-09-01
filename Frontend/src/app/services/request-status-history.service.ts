import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RequestStatusHistoryDto } from '../models/request-status-history.dto';

export interface RequestStatusHistoryApiResponse {
  page: number;
  pageSize: number;
  totalCount: number;
  histories: RequestStatusHistoryDto[];
}

@Injectable({
  providedIn: 'root'
})
export class RequestStatusHistoryService {
  private apiUrl = 'https://devdemoapi3.azurewebsites.net/api/request-status-history';

  constructor(private http: HttpClient) { }

  getHistory(
    page: number,
    pageSize: number,
    fromDate?: string,
    toDate?: string,
    requestId?: number
  ): Observable<RequestStatusHistoryApiResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (fromDate) {
      params = params.set('fromDate', fromDate);
    }
    if (toDate) {
      params = params.set('toDate', toDate);
    }
    if (requestId) {
      params = params.set('requestId', requestId.toString());
    }

    return this.http.get<RequestStatusHistoryApiResponse>(this.apiUrl, { params });
  }
}
