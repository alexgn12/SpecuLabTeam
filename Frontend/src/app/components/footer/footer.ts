import { Component } from '@angular/core';
import { LogoutButtonComponent } from "../logout-button/logout-button";

@Component({
  selector: 'sl-footer',
  standalone: true,
  templateUrl: './footer.html',
  styleUrls: ['./footer.css'],
  imports: [LogoutButtonComponent]
})
export class Footer {
  year = new Date().getFullYear();
}
