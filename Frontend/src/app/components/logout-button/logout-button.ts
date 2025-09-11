import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'sl-logout-button',
  standalone: true,
  styleUrls: ['./logout-button.css'],
  template: `
    <button class="logout-btn" (click)="logout()" title="Cerrar sesión">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <path stroke="currentColor" stroke-width="2" d="M16 17l5-5-5-5"/>
        <path stroke="currentColor" stroke-width="2" d="M21 12H9"/>
        <path stroke="currentColor" stroke-width="2" d="M12 19a7 7 0 1 1 0-14"/>
      </svg>
      <span class="logout-text d-none d-md-inline">Cerrar Sesión</span>
    </button>
  `
})
export class LogoutButtonComponent {
  constructor(private authService: AuthService, private router: Router) {}

  logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: () => {
        this.router.navigate(['/login']);
      }
    });
  }
}
