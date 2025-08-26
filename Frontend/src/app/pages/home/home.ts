import { Component } from '@angular/core';
import { Formulario } from '../../components/formulario/formulario';
import { ZoneGrafo } from '../../components/zone-grafo/zone-grafo';
import { ResumenRequestsComponent } from '../../components/resumen-requests/resumen-requests.component';

@Component({
  selector: 'sl-home',
  standalone: true,
  templateUrl: './home.html',
  styleUrl: './home.css',
  imports: [Formulario, ZoneGrafo, ResumenRequestsComponent]
})
export class Home {

}
