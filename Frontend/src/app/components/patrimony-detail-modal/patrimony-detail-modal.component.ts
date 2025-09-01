import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { ApprovedBuilding, IncomeApartment } from '../../pages/patrimony/patrimony.service';

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
    @Inject(MAT_DIALOG_DATA) 
    public data: { 
      type: 'building' | 'apartment', 
      item: ApprovedBuilding | IncomeApartment 
    }
  ) {}

  /** Cierra el modal manualmente si lo necesitas */
  close(): void {
    this.dialogRef.close();
  }
}
