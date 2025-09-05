
import { Component, Input, OnInit } from '@angular/core';
import { RentabilidadService, AnalyzeJsonResponse } from '../../services/rentabilidad.service';

@Component({
  selector: 'app-rentabilidad',
  templateUrl: './rentabilidad.component.html',
  styleUrls: ['./rentabilidad.component.css']
})
export class RentabilidadComponent implements OnInit {
  @Input() summary?: any;
  @Input() transactions: any[] = [];
  answer?: string;
  loading = true;
  error?: string;

  constructor(private rentabilidadService: RentabilidadService) {}

  ngOnInit(): void {
    const dataToAnalyze = {
    //   summary: this.summary,
    //   transactions: this.transactions,
      userPrompt: 'Quiero ver el análisis de rentabilidad',
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
