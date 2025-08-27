import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'sl-formulario',
  templateUrl: './formulario.html',
  styleUrls: ['./formulario.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule]
})
export class Formulario {
  form: FormGroup;
  enviado = false;
  successMsg = '';
  errorMsg = '';

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private router: Router) {
    this.form = this.fb.group({
      buildingCode: ['', Validators.required],
      description: ['', Validators.required]
    });

    this.route.paramMap.subscribe(params => {
      const code = params.get('buildingCode');
      if (code) {
        this.form.patchValue({ buildingCode: code });
      }
    });
  }

  enviarPeticion() {
    this.enviado = true;
    this.successMsg = '';
    this.errorMsg = '';
    if (this.form.valid) {
      // Aquí iría la llamada real al backend
      this.successMsg = '¡Petición enviada correctamente a la empresa de mantenimiento!';
      alert(this.successMsg);
      setTimeout(() => {
        this.router.navigate(['/requests']);
      }, 1000);
      this.form.reset();
      this.enviado = false;
    } else {
      this.errorMsg = 'Por favor, rellena todos los campos correctamente.';
    }
  }
}
