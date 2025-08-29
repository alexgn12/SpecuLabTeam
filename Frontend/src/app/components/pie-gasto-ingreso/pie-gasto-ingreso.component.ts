import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { Chart, ChartData, ChartOptions, TooltipItem } from 'chart.js';
import { CommonModule } from '@angular/common';
import { NgChartsModule } from 'ng2-charts';

@Component({
  selector: 'sl-pie-gasto-ingreso',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  templateUrl: './pie-gasto-ingreso.component.html',
  styleUrls: ['./pie-gasto-ingreso.component.css']
})
export class PieGastoIngresoComponent implements OnInit, OnChanges {
  @Input() ingreso: number = 0;
  @Input() gasto: number = 0;

  private nf = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 });

  // Texto central (lo calcula updateChartData y lo usa el plugin)
  private centerMain = 'Sin datos';
  private centerSub = '';

  pieChartData: ChartData<'doughnut', number[], string | string[]> = {
    labels: ['Ingresos', 'Gastos'],
    datasets: [
      {
        data: [0, 0],
        // Colores alineados con tu tema (verde éxito, rojo alerta)
        backgroundColor: ['#16a34a', '#ef4444'],
        hoverBackgroundColor: ['#22c55e', '#f87171'],
        borderColor: '#ffffff',
        borderWidth: 2,
        hoverOffset: 4,
        //cutout: '68%' as any // grosor del anillo
      }
    ]
  };

pieChartOptions: ChartOptions<'doughnut'> = {
  responsive: true,
  maintainAspectRatio: true,
  layout: { padding: 8 },
  cutout: '68%',             // ✅ aquí sí
  plugins: {
    legend: {
      display: true,
      position: 'bottom',
      labels: {
        usePointStyle: true,
        pointStyle: 'circle',
        boxWidth: 8,
        boxHeight: 8,
        padding: 16,
        font: { size: 12, weight: '500' },
        color: '#475569'
      }
    },
    tooltip: {
      padding: 10,
      bodyFont: { size: 12 },
      callbacks: {
        label: (ctx) => {
          const label = ctx.label || '';
          const value = (ctx.parsed as number) || 0;
          const total = (ctx.dataset.data as number[]).reduce((a, b) => a + b, 0);
          const pct = total > 0 ? ` (${Math.round((value / total) * 100)}%)` : '';
          const nf = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 });
          return `${label}: ${nf.format(value)}${pct}`;
        }
      }
    },
    datalabels: {
        display: false,
        formatter: (value, ctx) => {
          const label = ctx.chart.data.labels[ctx.dataIndex];
          return `${label}: ${this.nf.format(value)}`;
        }

    }
        
  }
};


  // Plugin para dibujar texto en el centro (total y % de gasto)
  private centerTextPlugin = {
    id: 'centerText',
    beforeDraw: (chart: Chart) => {
      const { ctx, chartArea } = chart;
      if (!ctx || !chartArea) return;

      ctx.save();
      const cx = (chartArea.left + chartArea.right) / 2;
      const cy = (chartArea.top + chartArea.bottom) / 2;

      // Línea principal
      ctx.fillStyle = '#0f172a';
      ctx.font = '600 16px system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(this.centerMain, cx, cy - 6);

      // Subtítulo
      if (this.centerSub) {
        ctx.fillStyle = '#64748b';
        ctx.font = '500 12px system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial';
        ctx.fillText(this.centerSub, cx, cy + 14);
      }
      ctx.restore();
    }
  };

  pieChartPlugins = [this.centerTextPlugin];

  ngOnInit() {
    this.updateChartData();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['ingreso'] || changes['gasto']) {
      this.updateChartData();
    }
  }

  private updateChartData() {
    const ingreso = Math.max(0, this.ingreso || 0);
    const gasto = Math.max(0, this.gasto || 0);
    const total = ingreso + gasto;

    // Actualiza dataset
    this.pieChartData = {
      labels: ['Ingresos', 'Gastos'],
      datasets: [
        {
          data: [ingreso, gasto],
          backgroundColor: ['#16a34a', '#ef4444'],
          hoverBackgroundColor: ['#22c55e', '#f87171'],
          borderColor: '#ffffff',
          borderWidth: 2,
          hoverOffset: 4,
          //cutout: '68%' as any
        }
      ]
    };

    // Texto centro: Total y % de gasto (o “Sin datos”)
    if (total === 0) {
      this.centerMain = 'Sin datos';
      this.centerSub = '';
    } else {
      this.centerMain = this.nf.format(total);
      const pctGasto = Math.round((gasto / total) * 100);
      this.centerSub = `Gasto ${pctGasto}%`;
    }
  }
}
