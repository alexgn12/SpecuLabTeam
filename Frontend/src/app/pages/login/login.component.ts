import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginRequest } from '../../services/auth.service';

import { FormsModule } from '@angular/forms';
@Component({
	selector: 'app-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.css'],
	standalone: true,
	imports: [FormsModule]
})
export class LoginComponent {
	email = '';
	password = '';
	error: string | null = null;
	loading = false;

	constructor(private auth: AuthService, private router: Router) {}

	login() {
		this.error = null;
		this.loading = true;
		const credentials: LoginRequest = { email: this.email, password: this.password };
		// Implementa el login real usando AuthService
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
				this.error = err?.error?.message || 'Error al iniciar sesión';
				this.loading = false;
			}
		});
	}
}
