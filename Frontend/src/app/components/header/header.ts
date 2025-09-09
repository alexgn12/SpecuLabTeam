import { Component, OnDestroy } from '@angular/core';
import { SignalRService, TransactionLiveDto, RequestLiveDto } from '../../services/signalr.service';

@Component({
  selector: 'sl-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header implements OnDestroy {
  isMenuOpen = false;
  notifications: Array<TransactionLiveDto | RequestLiveDto> = [];
  showNotifications = false;
  private subscriptions: any[] = [];

  constructor(private signalR: SignalRService) {
    // Suscribirse a eventos de SignalR
    this.subscriptions.push(
      this.signalR.transactionCreated$.subscribe(n => this.addNotification(n)),
      this.signalR.transactionUpdated$.subscribe(n => this.addNotification(n)),
      this.signalR.requestCreated$.subscribe(n => this.addNotification(n)),
      this.signalR.requestUpdated$.subscribe(n => this.addNotification(n))
    );
  }

  addNotification(n: TransactionLiveDto | RequestLiveDto | null) {
    if (!n) return;
    this.notifications.unshift(n);
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
