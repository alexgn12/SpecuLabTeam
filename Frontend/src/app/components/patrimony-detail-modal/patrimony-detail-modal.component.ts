import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ApprovedBuilding, IncomeApartment } from '../../pages/patrimony/patrimony.service';

import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-patrimony-detail-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './patrimony-detail-modal.component.html',
  styleUrls: ['./patrimony-detail-modal.component.css']
})
export class PatrimonyDetailModalComponent {
  constructor(
    public dialogRef: MatDialogRef<PatrimonyDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { type: 'building' | 'apartment', item: ApprovedBuilding | IncomeApartment }
  ) {}
}
