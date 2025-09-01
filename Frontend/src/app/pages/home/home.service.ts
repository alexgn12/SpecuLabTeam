import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map } from 'rxjs';


export interface Summary {
  totalBudget: number;        // presupuesto total
  spent: number;              // gasto acumulado
  income: number;             // ingresos acumulados
  updatedAt: string;          // ISO
}

export interface RequestsByStatus {
  pending: number;
  approved: number;
  rejected: number;
}

export interface Transaction {
  transactionId: number;
  transactionDate: string;     // ISO
  amount: number;              // + ingreso / - gasto
  type: 'INGRESO' | 'GASTO';
  description: string;
}

export interface BuildingsByDistrict {
  district: string;
  count: number;
}

@Injectable({ providedIn: 'root' })
export class HomeService {
  // ⚠️ Ajusta estos endpoints a tu API real
  private base = 'https://localhost:7092/api';
  private endpoints = {
    summary: `${this.base}/Dashboard/summary`,
    requestsByStatus: `${this.base}/Dashboard/requests-by-status`,
    transactionsRecent: `${this.base}/Transactions/recent?take=5`,
    buildingsByDistrict: `${this.base}/Ang/buildings-count-by-district`,
  };

  constructor(private http: HttpClient) {}

  getSummary(): Observable<Summary> {
    // Ejemplo real:
    // return this.http.get<Summary>(this.endpoints.summary);
    // Fallback de ejemplo si aún no tienes endpoint:
    return of({
      totalBudget: 250000,
      spent: 142300,
      income: 168500,
      updatedAt: new Date().toISOString()
    });
  }

  getRequestsByStatus(): Observable<RequestsByStatus> {
    // return this.http.get<RequestsByStatus>(this.endpoints.requestsByStatus);
    return of({ pending: 3, approved: 18, rejected: 2 });
  }

  getRecentTransactions(): Observable<Transaction[]> {
    // return this.http.get<Transaction[]>(this.endpoints.transactionsRecent);
    return of([
      { transactionId: 101, transactionDate: '2025-08-22', amount: 1200, type: 'INGRESO', description: 'Alquiler 3B' },
      { transactionId: 102, transactionDate: '2025-08-20', amount: -450, type: 'GASTO', description: 'Fontanería A-2' },
      { transactionId: 103, transactionDate: '2025-08-18', amount: 1300, type: 'INGRESO', description: 'Alquiler 2A' },
      { transactionId: 104, transactionDate: '2025-08-14', amount: -220, type: 'GASTO', description: 'Pintura portal' },
      { transactionId: 105, transactionDate: '2025-08-10', amount: 900, type: 'INGRESO', description: 'Alquiler Estudio' },
    ]);
  }

  getBuildingsByDistrict(): Observable<BuildingsByDistrict[]> {
    return this.http.get<BuildingsByDistrict[]>(this.endpoints.buildingsByDistrict)
      .pipe(map(list => list.slice(0, 6))); // recorta a 6 para el widget
  }
}
