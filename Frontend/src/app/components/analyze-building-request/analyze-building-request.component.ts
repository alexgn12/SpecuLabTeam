import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { AnalyzeBuildingRequestService } from '../../services/analyze-building-request.service';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({

  
  selector: 'app-analyze-building-request',
  templateUrl: './analyze-building-request.component.html',
  styleUrls: ['./analyze-building-request.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class AnalyzeBuildingRequestComponent implements OnChanges {
  @Input() requestId: number | null = null;
  @Input() buildingCode: string | null = null;
  @Input() buildingName: string | null = null;
  result: any;
  loading = false;
  error: string | null = null;

  constructor(private service: AnalyzeBuildingRequestService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['requestId'] && this.requestId != null) {
      this.submit();
    }
  }

  submit() {
    if (this.requestId == null) return;
    this.loading = true;
    this.error = null;
    this.service.analyzeRequest({ requestId: this.requestId }).subscribe({
      next: res => {
        if (res && typeof res.answer === 'string') {
          try {
            this.result = JSON.parse(res.answer);
          } catch {
            this.result = res.answer;
          }
        } else {
          this.result = res;
        }
        this.loading = false;
      },
      error: err => {
        this.error = 'Error al analizar la solicitud';
        this.loading = false;
      }
    });
  }
}
