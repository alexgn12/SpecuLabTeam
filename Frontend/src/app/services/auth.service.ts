import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface LoginRequest {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenSubject = new BehaviorSubject<string | null>(null);
  token$ = this.tokenSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<any> {
    return this.http.post('/api/Auth/login', credentials, { withCredentials: true });
  }

  refreshToken(): Observable<any> {
    return this.http.post('/api/Auth/refresh-token', {}, { withCredentials: true });
  }

  logout(): Observable<any> {
    return this.http.post('/api/Auth/logout', {}, { withCredentials: true }).pipe(
      tap(() => this.tokenSubject.next(null))
    );
  }

  setToken(token: string) {
    this.tokenSubject.next(token);
    localStorage.setItem('access_token', token);
  }

  getToken(): string | null {
    return this.tokenSubject.value || localStorage.getItem('access_token');
  }

  clearToken() {
    this.tokenSubject.next(null);
    localStorage.removeItem('access_token');
  }
}
