import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, map } from 'rxjs/operators';

export interface IRequest {
  requestId: number;
  buildingAmount: number;
  maintenanceAmount: number;
  description: string;
  statusType: string;
  buildingStreet: string;
  statusId: number;
  buildingId: number;
  building?: {
    buildingName: string;
    street: string;
    district: string;
    floorCount: number;
    yearBuilt: number;
    buildingCode: string;
  };
}

export interface IPaginatedRequests {
  items: IRequest[];
  total: number;
  page: number;
  size: number;
}

@Injectable({ providedIn: 'root' })
export class RequestsService {
  private apiUrl = 'https://localhost:7092/api/requests';

  constructor(private http: HttpClient) {}



  getRequests(page: number = 1, size: number = 10, status: string = '', sortBy: string = '', desc: boolean = true): Observable<IRequest[]> {
    let params: any = { Page: page, Size: size, Status: status, SortBy: sortBy, Desc: desc };
    return this.http.get<IRequest[]>(this.apiUrl, { params });
  }

  getBuildingById(buildingId: number): Observable<any> {
    const url = `https://localhost:7092/api/Building/${buildingId}`;
    return this.http.get<any>(url);
  }

  getAllBuildings(): Observable<any[]> {
    const pageSize = 100; // Ajusta según el límite del backend
    let allBuildings: any[] = [];
    let currentPage = 1;

    const fetchPage = (page: number): Observable<any[]> => {
      const url = `https://localhost:7092/api/Building?page=${page}&size=${pageSize}`;
      return this.http.get<any[]>(url);
    };

    return new Observable(observer => {
      const fetchNextPage = () => {
        fetchPage(currentPage).subscribe({
          next: (buildings) => {
            allBuildings = [...allBuildings, ...buildings];
            if (buildings.length === pageSize) {
              currentPage++;
              fetchNextPage();
            } else {
              observer.next(allBuildings);
              observer.complete();
            }
          },
          error: (err) => observer.error(err),
        });
      };
      fetchNextPage();
    });
  }

  getResumenRequests(): Observable<any> {
    return this.http.get<any>('https://localhost:7092/api/Ang/resumen-requests');
  }

  /**
   * Actualiza el estado de una request usando PATCH y operaciones JSON Patch.
   * @param requestId ID de la request
   * @param statusId Nuevo statusId (por ejemplo, 2 para Pendiente)
   */
  updateRequestStatusPatch(requestId: number, statusId: number): Observable<any> {
    const url = `${this.apiUrl}/${requestId}`;
    const body = [
      {
        path: "/statusid",
        op: "replace",
        value: statusId
      }
    ];
    return this.http.patch(url, body);
  }
}
