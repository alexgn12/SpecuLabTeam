import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { SignalRService, TransactionLiveDto, RequestLiveDto } from '../../services/signalr.service';

@Component({
  selector: 'sl-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header implements OnDestroy {
  removeNotification(index: number) {
    this.notifications.splice(index, 1);
  }
  isMenuOpen = false;
  notifications: Array<(TransactionLiveDto | RequestLiveDto) & { notifType?: string }> = [];
  showNotifications = false;
  private subscriptions: any[] = [];

  constructor(private signalR: SignalRService, private router: Router) {
    // Suscribirse a eventos de SignalR
    this.subscriptions.push(
      this.signalR.transactionCreated$.subscribe(n => this.addNotification(n, 'Transacción creada')),
      this.signalR.transactionUpdated$.subscribe(n => this.addNotification(n, 'Transacción actualizada')),
      this.signalR.requestCreated$.subscribe(n => this.addNotification(n, 'Solicitud creada')),
      this.signalR.requestUpdated$.subscribe(n => {
        if (!n) return;
        // Si es actualización de mantenimiento
        const isMantenimiento = n.maintenanceAmount && n.maintenanceAmount > 0;
        const notifType = isMantenimiento ? 'Mantenimiento recibido' : 'Solicitud actualizada';
        this.addNotification(n, notifType);
      })
    );
  }

  onNotificationClick(n: TransactionLiveDto | RequestLiveDto) {
    // Si tiene transactionId es transacción, si tiene requestId es request
    if ((n as any).transactionId !== undefined) {
      // Es transacción: navegar a /budget y hacer scroll tras navegar
      this.router.navigate(['/budget']).then(() => {
        setTimeout(() => {
          const el = document.querySelector('#transactions-list, .transactions-list');
          if (el) {
            (el as HTMLElement).scrollIntoView({ behavior: 'smooth', block: 'start' });
          }
        }, 400);
      });
    } else if ((n as any).requestId !== undefined) {
      // Es request: navegar a /requests
      this.router.navigate(['/requests']);
    }
    this.showNotifications = false;
  }

  addNotification(n: TransactionLiveDto | RequestLiveDto | null, notifType?: string) {
    if (!n) return;
    const notif = { ...n, notifType };
    this.notifications.unshift(notif);
    if (this.notifications.length > 5) {
      this.notifications = this.notifications.slice(0, 5);
    }
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
    if (window.innerWidth < 992) {
      setTimeout(() => {
        const nav = document.getElementById('navbarNav');
        if (nav) nav.classList.toggle('show', this.isMenuOpen);
      }, 0);
    }
  }

  closeMenu() {
    this.isMenuOpen = false;
    if (window.innerWidth < 992) {
      setTimeout(() => {
        const nav = document.getElementById('navbarNav');
        if (nav) nav.classList.remove('show');
      }, 0);
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe && s.unsubscribe());
  }
}
