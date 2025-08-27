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
    indexAxis: 'y',
    layout: { padding: { left: 140, right: 120, top: 16, bottom: 12 } },
    plugins: {
      legend: { display: false },
      title: { display: false },
      tooltip: { enabled: false },
      datalabels: { clamp: true }
    },
    // Siempre inicializamos scales
    scales: {
      x: {
        grid: { display: false, drawBorder: false },
        ticks: { display: false, font: { size: 16 }, padding: 10 }
      },
      y: {
        grid: { display: true, drawBorder: false, color: 'rgba(13,27,42,0.05)' },
        ticks: { display: false, font: { size: 18 }, padding: 16 }
      }
    },
    elements: {
      bar: {
        borderRadius: 10,
        borderSkipped: false
      }
    }
  };

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: 'rgba(13,27,42,0.08)',
        borderRadius: 50,
        barThickness: 6,
        categoryPercentage: 0.52,
        barPercentage: 0.5,
        order: 1,
        datalabels: { display: false }
      },
      {
        data: [],
        backgroundColor: '#0b3a6b',
        borderRadius: 50,
        barThickness: 6,
        categoryPercentage: 0.52,
        barPercentage: 0.5,
        order: 2,
        datalabels: {
          labels: {
            name: {
              display: true,
              anchor: 'start',
              align: 'start',
              color: '#96989bff',
              font: { size: 14, weight: 500 },
              offset: 8,
              clip: false,
              formatter: (_: number, ctx: any) => String(ctx.chart.data.labels?.[ctx.dataIndex] ?? '')
            },
            value: {
              display: true,
              anchor: 'end',
              align: 'end',
              color: '#0d1b2a',
              font: { size: 16, weight: 700 },
              offset: 8,
              clip: false,
              formatter: (v: number) => `${v}`
            }
          }
        }
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
        const max = Math.max(...values);
        const track = Array(values.length).fill(max);
        this.barChartData = {
          labels,
          datasets: [
            { ...this.barChartData.datasets[0], data: track },
            { ...this.barChartData.datasets[1], data: values }
          ]
        };
        // Ajusta el máximo del eje X para acortar las barras
  const currentScales = (this.barChartOptions && this.barChartOptions.scales) ? this.barChartOptions.scales : {};
        this.barChartOptions = {
          ...this.barChartOptions,
          scales: {
            ...currentScales,
            x: {
              ...(currentScales["x"] || {}),
              max: max * 1.2
            },
            y: {
              ...(currentScales["y"] || {})
            }
          }
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


