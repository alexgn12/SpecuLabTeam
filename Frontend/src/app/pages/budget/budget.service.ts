export interface MonthlyGastoIngreso {
  año: number;
  mes: number;
  transactionType: 'GASTO' | 'INGRESO';
  totalGasto: number;
}
export interface Transaction {
  transactionId: number;
  transactionDate: string;
  transactionTypeId: number;
  transactionType: string;
  description?: string;
  requestId?: number;
}
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ManagementBudget {
  managementBudgetId: string;
  currentAmount: number;
  initialAmount: number;
  lastUpdatedDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrlBudget = 'https://localhost:7092/api/managementbudgets';
  private apiUrlStatus = 'https://localhost:7092/api/status';
  private apiUrlRequest = 'https://localhost:7092/api/requests';
  private apiUrlBuildings =  'https://localhost:7092/api/Building';

  constructor(private https: HttpClient) {}

  getBudgets(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlBudget);
  }

  /**
   * Obtiene los datos de gasto e ingreso mensual desde el nuevo endpoint.
   */
  getMonthlyGastoIngreso(): Observable<MonthlyGastoIngreso[]> {
    return this.https.get<MonthlyGastoIngreso[]>(
      'https://localhost:7092/api/Ang/gasto-mensual'
    );
  }

  getStatus(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlStatus);
  }

  getRequests(size: number = 300): Observable<ManagementBudget[]> {
    const url = `${this.apiUrlRequest}?size=${size}`;
    return this.https.get<ManagementBudget[]>(url);
  }

  getBuildings(): Observable<any[]> {
    return this.https.get<any[]>(this.apiUrlBuildings);
  }
}

