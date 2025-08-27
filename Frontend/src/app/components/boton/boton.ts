import { Component, Input } from '@angular/core';

import { CommonModule } from '@angular/common';
@Component({
  selector: 'sl-boton',
  template: `
    <button [ngClass]="btnClass" class="btn w-100 mt-2">
      {{ label }}
    </button>
  `,
  standalone: true,
  imports: [CommonModule]
})
export class Boton {
  @Input() tipo: 'aceptar' | 'rechazar' | 'presupuesto' = 'aceptar';

  get btnClass() {
    switch (this.tipo) {
      case 'aceptar':
        return 'btn-outline-success';
      case 'rechazar':
        return 'btn-outline-danger';
      case 'presupuesto':
        return 'btn-outline-warning';
      default:
        return 'btn-outline-secondary';
    }
  }

  get label() {
    switch (this.tipo) {
      case 'aceptar':
        return 'Aceptar';
      case 'rechazar':
        return 'Rechazar';
      case 'presupuesto':
        return 'Solicitar Presupuesto';
      default:
        return '';
    }
  }
}
