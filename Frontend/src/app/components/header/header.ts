import { Component } from '@angular/core';

@Component({
  selector: 'sl-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header {
  isMenuOpen = false;

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
}
