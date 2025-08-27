
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RequestsService } from '../requests/requests.service';

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

  requestId?: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private requestsService: RequestsService
  ) {
    this.form = this.fb.group({
      buildingCode: ['', Validators.required],
      description: ['', Validators.required]
    });

    this.route.paramMap.subscribe(params => {
      const code = params.get('buildingCode');
      if (code) {
        this.form.patchValue({ buildingCode: code });
      }
      const reqId = params.get('requestId');
      if (reqId) {
        this.requestId = +reqId;
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
      // Cambiar estado a Pendiente si hay requestId
      if (this.requestId) {
        this.requestsService.updateRequestStatus(this.requestId, 'Pendiente').subscribe({
          next: () => {
            // Estado actualizado, continuar con el flujo normal
            alert(this.successMsg);
            setTimeout(() => {
              this.router.navigate(['/requests']);
            }, 1000);
            this.form.reset();
            this.enviado = false;
          },
          error: () => {
            this.errorMsg = 'Error al actualizar el estado de la solicitud.';
          }
        });
      } else {
        // Si no hay requestId, solo mostrar el mensaje y redirigir
        alert(this.successMsg);
        setTimeout(() => {
          this.router.navigate(['/requests']);
        }, 1000);
        this.form.reset();
        this.enviado = false;
      }
    } else {
      this.errorMsg = 'Por favor, rellena todos los campos correctamente.';
    }
  }
}
