import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResumenRequestsService, ResumenRequest } from './resumen-requests.service';

type Item = { status: string; count: number };

@Component({
  selector: 'app-resumen-requests',
  standalone: true,
  imports: [CommonModule],
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
      next: (data) => {
        this.items = this.normalize(data);
        this.loading = false;
      },
      error: (e) => {
        console.error(e);
        this.error = 'No se pudo cargar el resumen de solicitudes';
        this.loading = false;
      }
    });
  }

  /** Normaliza distintas formas de respuesta del endpoint */
  private normalize(data: ResumenRequest[]): Item[] {
    if (!data || data.length === 0) return [];

    // Caso 1: array de objetos {status, count}
    if (typeof data[0]?.status === 'string' && typeof data[0]?.count === 'number') {
      return (data as any as Item[]).sort((a, b) => b.count - a.count);
    }

    // Caso 2: un único objeto con claves {pending: n, approved: n, ...}
    if (data.length === 1 && typeof data[0] === 'object') {
      const obj = data[0] as Record<string, any>;
      return Object.keys(obj)
        .filter(k => typeof obj[k] === 'number')
        .map(k => ({ status: k, count: obj[k] as number }))
        .sort((a, b) => b.count - a.count);
    }

    // Caso 3: array de objetos con cualquier shape; tomamos pares clave-numérico
    const map = new Map<string, number>();
    for (const row of data) {
      for (const [k, v] of Object.entries(row)) {
        if (typeof v === 'number') map.set(k, (map.get(k) ?? 0) + v);
      }
    }
    return Array.from(map.entries())
      .map(([status, count]) => ({ status, count }))
      .sort((a, b) => b.count - a.count);
  }

  /** Mapea nombres comunes a clases visuales */
  badgeClass(s: string): string {
    const key = s.toLowerCase();
    if (key.includes('pend')) return 'pending';
    if (key.includes('aprob') || key.includes('appr')) return 'approved';
    if (key.includes('rech') || key.includes('reject')) return 'rejected';
    return 'neutral';
  }

  label(s: string): string {
    // Normaliza etiquetas: Pending -> Pendientes, etc.
    const key = s.toLowerCase();
    if (key.includes('pend')) return 'Pendientes';
    if (key.includes('aprob') || key.includes('appr')) return 'Aprobadas';
    if (key.includes('rech') || key.includes('reject')) return 'Rechazadas';
    // Capitaliza por defecto
    return s.charAt(0).toUpperCase() + s.slice(1);
  }
}
