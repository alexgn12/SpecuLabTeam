import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
// Update the import path below to the correct relative location of building.service
import { Building } from '../../services/building.service';
// If the correct path is different, adjust '../services/building.service' accordingly.

@Component({
  selector: 'building-detail-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './building-detail-modal.component.html',
  styleUrls: ['./building-detail-modal.component.css']
})
export class BuildingDetailModalComponent {
  @Input() building: Building | null = null;
  @Input() close!: () => void;
}
