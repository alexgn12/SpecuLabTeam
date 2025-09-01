import { Component } from '@angular/core';

@Component({
  selector: 'sl-footer',
  standalone: true,
  templateUrl: './footer.html',
  styleUrls: ['./footer.css']
})
export class Footer {
  year = new Date().getFullYear();
}
