import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, AsyncPipe, DatePipe, CurrencyPipe, NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HomeService, Summary, Transaction, BuildingsByDistrict } from './home.service';
import { Subscription, combineLatest } from 'rxjs';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { ResumenRequestsComponent } from 'src/app/components/resumen-requests/resumen-requests.component';
import { InfoCard } from '../../components/info-card/info-card';
import { NgxSwapyComponent } from '@omnedia/ngx-swapy';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, AsyncPipe, DatePipe, CurrencyPipe, NgChartsModule, NgClass, ResumenRequestsComponent, InfoCard, NgxSwapyComponent],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  loading = true;
  error: string | null = null;

  summary?: Summary;
  transactions: Transaction[] = [];
  buildingsByDistrict: BuildingsByDistrict[] = [];

  isMobile = false;
  templateCols = '1.05fr 1fr';   // desktop/tablet
  gridGap = '16px';

  private mm = window.matchMedia('(max-width: 991.98px)');
  private onMM = () => {
    this.isMobile = this.mm.matches;
    this.templateCols = this.isMobile ? '1fr' : '1.05fr 1fr';
    this.gridGap = this.isMobile ? '12px' : '16px';
  };


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
    const s3 = this.home.getRecentTransactions();
    const s4 = this.home.getBuildingsByDistrict();

    this.onMM();                       // aplica columnas correctas desde el inicio
    this.mm.addEventListener('change', this.onMM); // reacciona a cambios

    this.sub.add(
      combineLatest([s1, s3, s4]).subscribe({
        next: ([summary, txs, byDist]) => {
          this.summary = summary;
          this.transactions = txs;
          this.buildingsByDistrict = byDist;

          // preparar chart
          this.barData = {
            labels: byDist.map(x => x.district),
            datasets: [{
              data: byDist.map(x => x.count),
              borderRadius: 6,
              barThickness: 16,
              categoryPercentage: 0.7,
              backgroundColor: 'rgba(37, 99, 235, 0.35)',
              hoverBackgroundColor: 'rgba(37, 99, 235, 0.65)'
            }]
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
    this.mm.removeEventListener('change', this.onMM);
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
