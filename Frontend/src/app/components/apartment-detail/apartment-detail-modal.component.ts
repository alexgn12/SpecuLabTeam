import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'apartment-detail-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './apartment-detail-modal.component.html',
  styleUrls: ['./apartment-detail-modal.component.css']
})
export class ApartmentDetailModalComponent {
  @Input() apartment: any;
  @Input() close!: () => void;
}
