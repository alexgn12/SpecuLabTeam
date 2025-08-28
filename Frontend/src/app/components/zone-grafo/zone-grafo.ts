import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartConfiguration } from 'chart.js';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import DataLabelsPlugin from 'chartjs-plugin-datalabels';
import { ZoneGrafoService, BuildingsCountByDistrict } from './zone-grafo.service';

Chart.register(DataLabelsPlugin);


@Component({
  selector: 'sl-zone-grafo',
  standalone: true,
  templateUrl: './zone-grafo.html',
  styleUrl: './zone-grafo.css',
  imports: [CommonModule, NgChartsModule],
})
export class ZoneGrafo implements OnInit {
  public loading = true;
  public error: string | null = null;

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'x', // Barras verticales
    animation: {
      duration: 1200,
      easing: 'easeOutBounce'
    },
  layout: { padding: { left: 12, right: 12, top: 18, bottom: 24 } },
    plugins: {
      legend: { display: false },
      title: { display: false },
      tooltip: {
        enabled: true,
        backgroundColor: '#0b3a6b',
        titleColor: '#fff',
        bodyColor: '#fff',
        borderColor: '#0b3a6b',
        borderWidth: 1,
        callbacks: {
          label: (ctx: any) => `Operaciones: ${ctx.parsed.y}`
        }
      },
      datalabels: {
        anchor: 'end',
        align: 'end',
        color: '#0b3a6b',
        font: { size: 15, weight: 600 },
        formatter: (v: number) => `${v}`
      }
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { color: '#0b3a6b', font: { size: 13, weight: '500' } }
      },
      y: {
        grid: { color: 'rgba(13,27,42,0.06)' },
        beginAtZero: true,
        ticks: { color: '#b6c2d1', font: { size: 13 } }
      }
    },
    elements: {
      bar: {
        borderRadius: 10,
        borderSkipped: false,
        backgroundColor: (ctx: any) => {
          // Paleta corporativa: azul, verde, amarillo, naranja, rojo
          const v = ctx.parsed.y;
          if (v < 2) return '#16a34a'; // verde corporativo
          if (v < 4) return '#22d3ee'; // azul claro
          if (v < 7) return '#eab308'; // amarillo
          if (v < 10) return '#f97316'; // naranja
          return '#dc2626'; // rojo
        }
      }
    }
  };

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        borderRadius: 10,
        barPercentage: 0.35,
        categoryPercentage: 0.55,
        datalabels: { display: true }
      }
    ]
  };

  constructor(private zoneGrafoService: ZoneGrafoService) {}

  ngOnInit(): void {
    this.loading = true;
    this.error = null;
    this.zoneGrafoService.getBuildingsCountByDistrict().subscribe({
      next: (data: BuildingsCountByDistrict[]) => {
        const labels = data.map(d => d.district);
        const values = data.map(d => Number(d.count));
        this.barChartData = {
          labels,
          datasets: [
            { ...this.barChartData.datasets[0], data: values }
          ]
        };
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar los datos';
        this.loading = false;
      }
    });
  }
}


