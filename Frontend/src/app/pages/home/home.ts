import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgxSwapyComponent } from '@omnedia/ngx-swapy';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';

import { ResumenRequestsComponent } from 'src/app/components/resumen-requests/resumen-requests.component';
import { InfoCard } from '../../components/info-card/info-card';
import { HomeService, Summary, Transaction, BuildingsByDistrict } from './home.service';
import { Subscription, combineLatest } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    NgxSwapyComponent,      // 👈 Swapy solo se usa en desktop (no se montará en móvil)
    NgChartsModule,
    ResumenRequestsComponent,
    InfoCard
  ],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  loading = true;
  error: string | null = null;

  summary?: Summary;
  transactions: Transaction[] = [];
  buildingsByDistrict: BuildingsByDistrict[] = [];

  // ======== Layout flags (desktop vs móvil) ========
  isMobile = false;
  templateCols = '1.05fr 1fr';
  gridGap = '16px';
  private mm = window.matchMedia('(max-width: 991.98px)');
  private onMM = () => {
    this.isMobile = this.mm.matches;
    this.templateCols = this.isMobile ? '1fr' : '1.05fr 1fr';
    this.gridGap = this.isMobile ? '12px' : '16px';
    // Cambia opciones del gráfico según el dispositivo
    this.barOptions = this.isMobile ? this.barOptionsMobile : this.barOptionsDesktop;
  };

  // ======== Chart config ========
  barData: ChartConfiguration<'bar'>['data'] = { labels: [], datasets: [] };

  // Desktop/tablet: interacciones activas
  private barOptionsDesktop: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    plugins: { legend: { display: false }, tooltip: { enabled: true } },
    layout: { padding: { left: 6, right: 6, top: 6, bottom: 6 } },
    scales: {
      x: { grid: { display: false }, ticks: { precision: 0 } },
      y: { grid: { display: false } }
    }
  };

  // Móvil: gráfico “pasivo” (no roba el gesto de scroll)
  private barOptionsMobile: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'y',
    events: [],                        // 👈 sin eventos de puntero
    plugins: { legend: { display: false }, tooltip: { enabled: false } },
    hover: { mode: undefined },
    layout: { padding: { left: 4, right: 4, top: 4, bottom: 4 } },
    scales: {
      x: { grid: { display: false }, ticks: { precision: 0 } },
      y: { grid: { display: false } }
    }
  };

  barOptions: ChartConfiguration<'bar'>['options'] = this.barOptionsDesktop;

  private sub = new Subscription();

  constructor(private home: HomeService) {}

  ngOnInit(): void {
    // Detectar móvil/desktop y preparar opciones desde el primer render
    this.onMM();
    this.mm.addEventListener('change', this.onMM);

    const s1 = this.home.getSummary();
    const s3 = this.home.getRecentTransactions();
    const s4 = this.home.getBuildingsByDistrict();

    this.sub.add(
      combineLatest([s1, s3, s4]).subscribe({
        next: ([summary, txs, byDist]) => {
          this.summary = summary;
          this.transactions = txs;
          this.buildingsByDistrict = byDist;

          this.barData = {
            labels: byDist.map(x => x.district),
            datasets: [{
              data: byDist.map(x => x.count),
              borderRadius: 6,
              barThickness: this.isMobile ? 12 : 16,
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
    this.mm.removeEventListener('change', this.onMM);
    this.sub.unsubscribe();
  }

  get spentPct(): number {
    if (!this.summary) return 0;
    const d = (this.summary.spent / this.summary.totalBudget) * 100;
    return Math.max(0, Math.min(100, Math.round(d)));
  }
}
