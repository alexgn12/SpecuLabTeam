import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ResumenRequestsService, EstadoResumen, ResumenRequestResponse } from './resumen-requests.service';

type Item = { status: string; count: number };

@Component({
  selector: 'app-resumen-requests',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './resumen-requests.component.html',
  styleUrls: ['./resumen-requests.component.css']
})
export class ResumenRequestsComponent implements OnInit {
  loading = true;
  error: string | null = null;
  items: Item[] = [];

  constructor(private srv: ResumenRequestsService) {}

  ngOnInit(): void {
    this.srv.getResumenRequests().subscribe({
      next: (data: ResumenRequestResponse) => {
        this.items = (data.porEstado || []).map(e => ({ status: e.estado, count: e.total })).sort((a, b) => b.count - a.count);
        this.loading = false;
      },
      error: (e) => {
        console.error(e);
        this.error = 'No se pudo cargar el resumen de solicitudes';
        this.loading = false;
      }
    });
  }

  // Normalización eliminada, ya no es necesaria con la nueva estructura

  /** Mapea nombres comunes a clases visuales */
 badgeClass(s: string): string {
  const key = s.toLowerCase();
  if (key.includes('pend')) return 'pending';
  if (key.includes('rev') || key.includes('revisión') || key.includes('revision')) return 'review';
  if (key.includes('aprob') || key.includes('appr')) return 'approved';
  if (key.includes('deneg') || key.includes('rech') || key.includes('reject')) return 'rejected';
  return 'neutral';
}

label(s: string): string {
  const key = s.toLowerCase();
  if (key.includes('pend')) return 'Pendientes';
  if (key.includes('rev') || key.includes('revisión') || key.includes('revision')) return 'En Revisión';
  if (key.includes('aprob') || key.includes('appr')) return 'Aprobadas';
  if (key.includes('deneg') || key.includes('rech') || key.includes('reject')) return 'Denegadas';
  return s.charAt(0).toUpperCase() + s.slice(1);
}
};

