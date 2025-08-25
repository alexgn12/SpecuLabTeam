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
  private apiUrlTransactions = 'https://localhost:7092/api/transactions';
  private apiUrlStatus = 'https://localhost:7092/api/status';
  private apiUrlRequest = 'https://localhost:7092/api/requests';
  private apiUrlBuildings =  'https://localhost:7092/api/Building';

  constructor(private https: HttpClient) {}

  getBudgets(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlBudget);
  }

  getTransactions(): Observable<Transaction[]> {
    return this.https.get<Transaction[]>(this.apiUrlTransactions);
  }

  getStatus(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlStatus);
  }

  getRequests(): Observable<ManagementBudget[]> {
    return this.https.get<ManagementBudget[]>(this.apiUrlRequest);
  }

  getBuildings(): Observable<any[]> {
    return this.https.get<any[]>(this.apiUrlBuildings);
  }
}

