
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
  apartmentCount?: number;
  street?: string;
  issueTypeId: number = 7;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private requestsService: RequestsService
  ) {
    this.form = this.fb.group({
      buildingCode: ['', Validators.required],
      description: ['', Validators.required],
      apartmentCount: [{ value: '', disabled: true }, Validators.required],
      street: [{ value: '', disabled: true }, Validators.required],
  issueTypeId: [{ value: 7, disabled: true }, Validators.required]
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
    this.route.queryParamMap.subscribe(query => {
      const apartmentCount = query.get('apartmentCount');
      if (apartmentCount) {
        this.apartmentCount = +apartmentCount;
        this.form.patchValue({ apartmentCount: this.apartmentCount });
      }
      const street = query.get('street');
      if (street) {
        this.street = street;
        this.form.patchValue({ street: this.street });
      }
      // issueTypeId fijo en 7
      this.form.patchValue({ issueTypeId: 7 });
    });
  }

  enviarPeticion() {
    this.enviado = true;
    this.successMsg = '';
    this.errorMsg = '';
    if (this.form.valid) {
      // Construir el JSON para el endpoint
      const payload = {
        buildingCode: this.form.get('buildingCode')?.value,
        description: this.form.get('description')?.value,
        apartmentCount: this.apartmentCount,
        Address: this.street,
        issueTypeId: 7,
        MaintenanceCost: 1
      };
      // Llamada real al backend
      this.requestsService.createSolicitation(payload).subscribe({
        next: () => {
          this.successMsg = '¡Petición enviada correctamente a la empresa de mantenimiento!';
          // Cambiar estado a Pendiente (statusId = 2) si hay requestId
          if (this.requestId) {
            const buildingCode = this.form.get('buildingCode')?.value || '';
            const comment = `el edificio ${buildingCode} esta pendiente de presupuesto`;
            this.requestsService.updateRequestStatusPatch(this.requestId, 2, comment).subscribe({
              next: () => {
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
            alert(this.successMsg);
            setTimeout(() => {
              this.router.navigate(['/requests']);
            }, 1000);
            this.form.reset();
            this.enviado = false;
          }
        },
        error: () => {
          this.errorMsg = 'Error al enviar la petición al backend.';
        }
      });
    } else {
      this.errorMsg = 'Por favor, rellena todos los campos correctamente.';
    }
  }
}
