import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InfoCard } from '../info-card/info-card';
import { BudgetService } from '../../pages/budget/budget.service';

@Component({
  selector: 'app-edificios-comprados-count',
  standalone: true,
  imports: [CommonModule, InfoCard],
  templateUrl: './edificios-comprados-count.html',
  styleUrls: ['./edificios-comprados-count.css']
})
export class EdificiosCompradosCountComponent implements OnInit {
  count: number | null = null;
  loading = true;
  error: string | null = null;

  constructor(private budgetService: BudgetService) {}

  ngOnInit() {
    this.budgetService.getEdificiosCompradosCount().subscribe({
      next: (data: any) => {
        this.count = data?.edificiosComprados ?? null;
        this.loading = false;
      },
      error: (err: any) => {
        this.error = 'Error al cargar el conteo de edificios comprados';
        this.loading = false;
      }
    });
  }
}
