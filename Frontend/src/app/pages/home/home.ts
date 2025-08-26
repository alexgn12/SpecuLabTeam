import { Component } from '@angular/core';
import { Formulario } from '../../components/formulario/formulario';
import { ZoneGrafo } from '../../components/zone-grafo/zone-grafo';

@Component({
  selector: 'sl-home',
  standalone: true,
  templateUrl: './home.html',
  styleUrl: './home.css',
  imports: [Formulario, ZoneGrafo]
})
export class Home {

}
