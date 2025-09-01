import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, switchMap } from 'rxjs';


export interface Summary {
  totalBudget: number;        // presupuesto total
  spent: number;              // gasto acumulado
  income: number;             // ingresos acumulados
  updatedAt: string;          // ISO
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
  private base = 'https://devdemoapi3.azurewebsites.net/api';
  private endpoints = {
    summary: `${this.base}/Dashboard/summary`,
    requestsByStatus: `${this.base}/Ang/resumen-requests`,
  transactionsRecent: `${this.base}/transactions?take=4`,
    buildingsByDistrict: `${this.base}/Ang/buildings-count-by-district`,
    budgets: `${this.base}/managementbudgets`,
  };

  constructor(private http: HttpClient) {}

  getSummary(): Observable<Summary> {
    // Usar budgets para el total y transacciones para gasto/ingreso acumulado
    return this.http.get<any[]>(this.endpoints.budgets).pipe(
      map((budgets) => {
        let totalBudget = 0;
        let updatedAt = '';
        if (budgets && budgets.length > 0) {
          const latest = budgets.reduce((a, b) => new Date(a.lastUpdatedDate) > new Date(b.lastUpdatedDate) ? a : b);
          totalBudget = latest.currentAmount;
          updatedAt = latest.lastUpdatedDate;
        }
        return { totalBudget, spent: 0, income: 0, updatedAt };
      }),
      // Ahora combinamos con transacciones para calcular gasto/ingreso
      // Usamos switchMap para hacer la segunda petición
      switchMap(summary =>
        this.http.get<{ items: any[] }>(this.endpoints.transactionsRecent).pipe(
          map(res => {
            const items = res.items || [];
            const spent = items.filter(t => t.transactionType === 'GASTO').reduce((sum, t) => sum + (t.amount || 0), 0);
            const income = items.filter(t => t.transactionType === 'INGRESO').reduce((sum, t) => sum + (t.amount || 0), 0);
            return { ...summary, spent, income };
          })
        )
      )
    );
  }


  //transaction recent
  getRecentTransactions(): Observable<Transaction[]> {
    return this.http.get<{ items: any[] }>(this.endpoints.transactionsRecent).pipe(
      map(res => (res.items || [])
        .filter(t => t.transactionType === 'INGRESO' || t.transactionType === 'GASTO')
        .slice(0, 4)
        .map(t => ({
          transactionId: t.transactionId,
          transactionDate: t.transactionDate,
          amount: t.amount,
          type: t.transactionType,
          description: t.description
        }))
      )
    );
  }

  getBuildingsByDistrict(): Observable<BuildingsByDistrict[]> {
    return this.http.get<BuildingsByDistrict[]>(this.endpoints.buildingsByDistrict)
      .pipe(map(list => list.slice(0, 6))); // recorta a 6 para el widget
  }
}
