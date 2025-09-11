import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  login(credentials: LoginRequest) {
    // Ajusta la URL según tu backend
    return this.http.post<{ accessToken: string }>(
      'https://localhost:7092/api/Auth/login',
      credentials,
      { withCredentials: true }
    );
  }
  private _accessToken = signal<string | null>(null);
  private _tokenExpiry = signal<Date | null>(null);

  /**
   * Devuelve los roles del usuario extraídos del JWT (claim 'roles' o 'role')
   */
  getUserRoles(): string[] {
    const token = this._accessToken();
    if (!token) return [];
    const payload = this.parseJwt(token);
    // Soporta 'roles' como array o string, o 'role' como string
    if (Array.isArray(payload.roles)) return payload.roles;
    if (typeof payload.roles === 'string') return [payload.roles];
    if (typeof payload.role === 'string') return [payload.role];
    return [];
  }

  // ...el resto de la clase AuthService...


  /**
   * Devuelve si el usuario está autenticado (token válido y no expirado)
   */
  public isAuthenticated(): boolean {
    const token = this._accessToken();
    const expiry = this._tokenExpiry();
    return token !== null && expiry !== null && expiry > new Date();
  }


  constructor(private http: HttpClient) {
    // Cargar token y expiración desde localStorage si existen
    const token = localStorage.getItem('access_token');
    const expiry = localStorage.getItem('access_token_expiry');
    if (token && expiry) {
      this._accessToken.set(token);
      this._tokenExpiry.set(new Date(expiry));
    }
  }

  refreshToken(): Observable<any> {
    return this.http.post('https://localhost:7092/api/Auth/refresh-token', {}, { withCredentials: true });
  }

  logout(): Observable<any> {
    return this.http.post('/api/Auth/logout', {}, { withCredentials: true });
  }

  setAccessToken(token: string): void {
    const payload = this.parseJwt(token);
    this._accessToken.set(token);
    const expiry = new Date(payload.exp * 1000);
    this._tokenExpiry.set(expiry);
    localStorage.setItem('access_token', token);
    localStorage.setItem('access_token_expiry', expiry.toISOString());
  }

  getAccessToken(): string | null {
    return this.isAuthenticated() ? this._accessToken() : null;
  }

  clearToken(): void {
    this._accessToken.set(null);
    this._tokenExpiry.set(null);
    localStorage.removeItem('access_token');
    localStorage.removeItem('access_token_expiry');
  }

  // Decodifica el JWT para extraer el payload
  private parseJwt(token: string): any {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch {
      return {};
    }
  }
}
