
import { Component, Input, OnInit } from '@angular/core';
import { RentabilidadService, AnalyzeJsonResponse } from '../../services/rentabilidad.service';
import { CommonModule } from '@angular/common';
import { jsPDF } from 'jspdf';

@Component({
  selector: 'app-rentabilidad',
  templateUrl: './rentabilidad.component.html',
  styleUrls: ['./rentabilidad.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class RentabilidadComponent {
  @Input() summary?: any;
  @Input() transactions: any[] = [];
  answer?: string;
  loading = false;
  error?: string;

 // QUITAR COMENTARIOS CUANDO SE USE Y BORRAR EL NG ON INIT

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

  downloadPdf() {
    const doc = new jsPDF();
    let content = 'Análisis de Rentabilidad\n\n';
    content += this.answer ? this.answer : 'No hay análisis disponible.';
    doc.text(content, 10, 10);
    doc.save('analisis-rentabilidad.pdf');
  }

  constructor(private rentabilidadService: RentabilidadService) {}



  // ngOnInit(): void {
  //   const dataToAnalyze = {
  //   //   summary: this.summary,
  //   //   transactions: this.transactions,
  //     userPrompt: 'Quiero ver el análisis de rentabilidad',
  //     systemPrompt: 'Devuelve el análisis de rentabilidad en español.'
  //   };

    // SUSTITUIR EL NG ON INIT POR ESTA FUNCION DE ABAJO Y LLAMARLA DESDE EL HTML

     askQuestion(question: string) {
    this.loading = true;
    this.error = undefined;
    this.answer = undefined;
    const dataToAnalyze = {
      summary: this.summary,
      transactions: this.transactions,
      userPrompt: question,
      systemPrompt: 'Devuelve el análisis de rentabilidad en español.'
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
}
