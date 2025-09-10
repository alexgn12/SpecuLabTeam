import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // No añadir Authorization a endpoints de autenticación
    const isAuthEndpoint =
      req.url.includes('/api/Auth/login') ||
      req.url.includes('/api/Auth/refresh-token') ||
      req.url.includes('/api/Auth/logout') ||
      req.url.includes('/api/Auth/register');

    const token = this.authService.getToken();

    if (token && !isAuthEndpoint) {
      const cloned = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
      return next.handle(cloned);
    }
    return next.handle(req);
  }
}
