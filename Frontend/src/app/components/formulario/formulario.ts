import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'sl-formulario',
  templateUrl: './formulario.html',
  styleUrls: ['./formulario.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class Formulario {
  form: FormGroup;
  enviado = false;
  successMsg = '';
  errorMsg = '';

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      buildingCode: ['', Validators.required],
      description: ['', Validators.required]
    });
  }

  enviarPeticion() {
    this.enviado = true;
    this.successMsg = '';
    this.errorMsg = '';
    if (this.form.valid) {
      // Aquí iría la llamada real al backend
      this.successMsg = '¡Petición enviada correctamente a la empresa de mantenimiento!';
      this.form.reset();
      this.enviado = false;
    } else {
      this.errorMsg = 'Por favor, rellena todos los campos correctamente.';
    }
  }
}
