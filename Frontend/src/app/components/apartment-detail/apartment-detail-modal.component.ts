import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'apartment-detail-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './apartment-detail-modal.component.html',
  styleUrls: ['./apartment-detail-modal.component.css']
})
export class ApartmentDetailModalComponent {
  constructor(
    public dialogRef: MatDialogRef<ApartmentDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { apartment: any }
  ) {}
}
