import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.css'],
	standalone: true,
	imports: [CommonModule, FormsModule]
})
export class LoginComponent {
	email = '';
	password = '';
	error: string | null = null;
	loading = false;

	showPassword = false;


	constructor(private auth: AuthService, private router: Router) {}

	login() {
		this.error = null;
		// Validación simple de campos
		if (!this.email || !this.password) {
			this.error = 'Introduce tu email y contraseña.';
			return;
		}
		this.loading = true;
		const credentials: LoginRequest = { email: this.email, password: this.password };
		this.auth.login(credentials).subscribe({
			next: (resp: any) => {
				if (resp && resp.accessToken) {
					this.auth.setAccessToken(resp.accessToken);
					this.router.navigate(['/']);
				} else {
					this.error = 'Respuesta inválida del servidor.';
				}
				this.loading = false;
			},
			error: (err) => {
				if (err.status === 401) {
					this.error = 'Email o contraseña incorrectos.';
				} else if (err?.error?.message) {
					this.error = err.error.message;
				} else {
					this.error = 'Error al iniciar sesión';
				}
				this.loading = false;
			}
		});
	}
}
