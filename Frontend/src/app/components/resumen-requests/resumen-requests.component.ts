import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResumenRequestsService, ResumenRequest } from '../../pages/home/resumen-requests.service';

@Component({
  selector: 'sl-resumen-requests',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './resumen-requests.component.html',
  styleUrls: ['./resumen-requests.component.css']
})
export class ResumenRequestsComponent implements OnInit {
  resumen: ResumenRequest[] = [];
  columnKeys: string[] = [];
  loading = true;
  error: string | null = null;

  constructor(private resumenService: ResumenRequestsService) {}

  ngOnInit(): void {
    this.resumenService.getResumenRequests().subscribe({
      next: (data) => {
        // Si la respuesta es un objeto, conviértelo en array de un solo elemento
        if (Array.isArray(data)) {
          this.resumen = data;
        } else if (data && typeof data === 'object') {
          this.resumen = [data];
        } else {
          this.resumen = [];
        }
        this.columnKeys = this.resumen.length > 0 ? Object.keys(this.resumen[0]) : [];
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Error al cargar el resumen de requests';
        this.loading = false;
      }
    });
  }
}
