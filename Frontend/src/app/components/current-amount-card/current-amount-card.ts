import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { InfoCard } from '../info-card/info-card';

@Component({
  selector: 'sl-current-amount-card',
  standalone: true,
  imports: [CommonModule, InfoCard],
  templateUrl: './current-amount-card.html',
  styleUrls: ['./current-amount-card.css']
})
export class CurrentAmountCard {
  amount: number | null = null;
  loading = true;
  error = false;

  constructor(private http: HttpClient) {
    this.fetchAmount();
  }

  fetchAmount() {
    this.http.get<any>('https://localhost:7092/api/Ang/current-amount').subscribe({
      next: (data) => {
        this.amount = typeof data === 'number' ? data : (data?.currentAmount ?? null);
        this.loading = false;
      },
      error: () => {
        this.error = true;
        this.loading = false;
      }
    });
  }
}
