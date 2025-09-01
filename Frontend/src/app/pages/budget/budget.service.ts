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
  private apiUrlBudget = 'https://devdemoapi3.azurewebsites.net/api/managementbudgets';
  private edificiosCompradosCountUrl = 'https://devdemoapi3.azurewebsites.net/api/Ang/edificios-comprados-count';

  constructor(private https: HttpClient) {}

  getBudgets(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlBudget);
  }

  /**
   * Obtiene los datos de gasto e ingreso mensual desde el nuevo endpoint.
   */
  getMonthlyGastoIngreso(): Observable<MonthlyGastoIngreso[]> {
    return this.https.get<MonthlyGastoIngreso[]>(
  'https://devdemoapi3.azurewebsites.net/api/Ang/gasto-mensual'
    );
  }

  /**
   * Obtiene el conteo de edificios comprados desde el endpoint.
   */
  getEdificiosCompradosCount(): Observable<number> {
    return this.https.get<number>(this.edificiosCompradosCountUrl);
  }




}

