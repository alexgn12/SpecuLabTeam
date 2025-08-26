import { Component } from '@angular/core';
import { Formulario } from '../../components/formulario/formulario';

@Component({
  selector: 'sl-home',
  standalone: true,
  templateUrl: './home.html',
  styleUrl: './home.css',
  imports: [Formulario]
})
export class Home {

}
