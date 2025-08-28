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
    layout: { padding: { left: 24, right: 24, top: 24, bottom: 32 } },
    plugins: {
      legend: { display: false },
      title: { display: false },
      tooltip: {
        enabled: true,
        callbacks: {
          label: (ctx: any) => `Operaciones: ${ctx.parsed.y}`
        }
      },
      datalabels: {
        anchor: 'end',
        align: 'end',
        color: '#222',
        font: { size: 16, weight: 700 },
        formatter: (v: number) => `${v}`
      }
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { color: '#0b3a6b', font: { size: 14, weight: '400' } }
      },
      y: {
        grid: { color: 'rgba(13,27,42,0.08)' },
        beginAtZero: true,
        ticks: { color: '#888', font: { size: 14 } }
      }
    },
    elements: {
      bar: {
        borderRadius: 12,
        borderSkipped: false,
        backgroundColor: (ctx: any) => {
          // Gradiente de verde a rojo según valor
          const v = ctx.parsed.y;
          if (v < 2) return '#22c55e'; // verde
          if (v < 4) return '#eab308'; // amarillo
          return '#ef4444'; // rojo
        }
      }
    }
  };

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        // backgroundColor se define en options.elements.bar.backgroundColor
        borderRadius: 12,
        barPercentage: 0.7,
        categoryPercentage: 0.7,
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


