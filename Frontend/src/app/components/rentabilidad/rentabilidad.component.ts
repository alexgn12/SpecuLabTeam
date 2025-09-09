
import { Component, Input, OnInit } from '@angular/core';
import { RentabilidadService, AnalyzeJsonResponse } from '../../services/rentabilidad.service';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';


@Component({
  selector: 'app-rentabilidad',
  templateUrl: './rentabilidad.component.html',
  styleUrls: ['./rentabilidad.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class RentabilidadComponent implements OnInit {
  chart?: Chart;
  chartReady: boolean = false;
  ngAfterViewInit(): void {
    // Cargar html2canvas globalmente
    import('html2canvas').then((module) => {
      (window as any).html2canvas = module.default;
    });

    // Crear gráfico de ejemplo si hay transacciones
    setTimeout(() => {
      if (this.transactions && this.transactions.length > 0) {
        const ctx = (document.getElementById('rentabilidadChart') as HTMLCanvasElement)?.getContext('2d');
        if (ctx) {
          const labels = this.transactions.map((t, i) => t.fecha || t.date || ('Tx ' + (i + 1)));
          const data = this.transactions.map(t => Number(t.monto || t.amount || 0));
          // Si no hay datos válidos, no crear el gráfico
          if (data.every(v => v === 0)) {
            this.chartReady = false;
            return;
          }
          this.chart = new Chart(ctx, {
            type: 'bar',
            data: {
              labels,
              datasets: [{
                label: 'Monto',
                data,
                backgroundColor: 'rgba(54, 162, 235, 0.5)'
              }]
            },
            options: {
              plugins: {
                legend: { display: false }
              },
              responsive: false,
              scales: {
                y: { beginAtZero: true }
              }
            }
          });
          this.chartReady = true;
        }
      }
    }, 500); // Espera para asegurar que el canvas esté en el DOM
  }
  @Input() summary?: any;
  @Input() transactions: any[] = [];
  answer?: string;
  loading = false;
  error?: string;

  questions = [
    {
      userPrompt: 'Realiza un análisis de rentabilidad basado en los datos reales de las transacciones de mi aplicación, mostrando tendencias y resultados clave.',
      label: '¿Cuál es el análisis de rentabilidad?'
    },
    {
      userPrompt: 'Analiza la rentabilidad de las transacciones monetarias realizadas en mi aplicación. ¿Qué tendencias, oportunidades de mejora y riesgos identificas? Dame recomendaciones prácticas para optimizar la rentabilidad.',
      label: '¿Cómo puedo mejorar la rentabilidad?'
    }
  ];

  askQuestion(question: string) {
    this.loading = true;
    this.error = undefined;
    this.answer = undefined;
    const dataToAnalyze = {
      summary: this.summary,
      transactions: this.transactions,
      userPrompt: question,
      systemPrompt: 'Eres un analista financiero. Tu trabajo es analizar los datos en formato JSON que se te proporcionan. Debes responder únicamente en base a esos datos, calculando métricas como beneficios, gastos, ingresos o cualquier información solicitada en el prompt del usuario. No inventes información fuera de los datos. Responde en 3-4 lineas claras y concisas.'
    };
    this.rentabilidadService.analyzeJson(dataToAnalyze).subscribe({
      next: (data) => {
        this.answer = data.answer;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al analizar rentabilidad';
        this.loading = false;
      }
    });
  }

  constructor(private rentabilidadService: RentabilidadService) {}



  ngOnInit(): void {
  }
}
