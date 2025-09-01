import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, AsyncPipe, DatePipe, CurrencyPipe, NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HomeService, Summary, RequestsByStatus, Transaction, BuildingsByDistrict } from './home.service';
import { Subscription, combineLatest } from 'rxjs';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ResumenRequestsComponent } from 'src/app/components/resumen-requests/resumen-requests.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, AsyncPipe, DatePipe, CurrencyPipe, NgChartsModule, NgClass, ResumenRequestsComponent],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  loading = true;
  error: string | null = null;

  summary?: Summary;
  reqStatus?: RequestsByStatus;
  transactions: Transaction[] = [];
  buildingsByDistrict: BuildingsByDistrict[] = [];

  // Chart config (barras horizontales)
  barData: ChartConfiguration<'bar'>['data'] = { labels: [], datasets: [] };
  barOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    plugins: { legend: { display: false } },
    layout: { padding: { left: 6, right: 6, top: 6, bottom: 6 } },
    scales: {
      x: { grid: { display: false }, ticks: { precision: 0 } },
      y: { grid: { display: false } }
    }
  };

  private sub = new Subscription();

  constructor(private home: HomeService) {}

  ngOnInit(): void {
    const s1 = this.home.getSummary();
    const s2 = this.home.getRequestsByStatus();
    const s3 = this.home.getRecentTransactions();
    const s4 = this.home.getBuildingsByDistrict();

    this.sub.add(
      combineLatest([s1, s2, s3, s4]).subscribe({
        next: ([summary, reqStatus, txs, byDist]) => {
          this.summary = summary;
          this.reqStatus = reqStatus;
          this.transactions = txs;
          this.buildingsByDistrict = byDist;

          // preparar chart
          this.barData = {
            labels: byDist.map(x => x.district),
            datasets: [{ data: byDist.map(x => x.count), borderRadius: 6, barThickness: 16, categoryPercentage: 0.7 }]
          };

          this.loading = false;
        },
        error: (e) => {
          this.error = 'No se pudo cargar el Dashboard';
          this.loading = false;
          console.error(e);
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  get spentPct(): number {
    if (!this.summary) return 0;
    const d = this.summary.spent / this.summary.totalBudget * 100;
    return Math.max(0, Math.min(100, Math.round(d)));
  }

  amountClass(a: number): string {
    return a >= 0 ? 'tx-income' : 'tx-expense';
  }
}
