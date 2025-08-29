import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'building-detail-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './building-detail-modal.component.html',
  styleUrls: ['./building-detail-modal.component.css']
})
export class BuildingDetailModalComponent {
  constructor(
    public dialogRef: MatDialogRef<BuildingDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { building: any }
  ) {}
}
