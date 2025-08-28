import { Component, OnInit } from '@angular/core';
import { PatrimonyService, ApprovedBuilding, IncomeApartment } from './patrimony.service';
import { CommonModule, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-patrimony',
  templateUrl: './patrimony.html',
  styleUrls: ['./patrimony.css'],
  standalone: true,
  imports: [CommonModule],
  providers: [CurrencyPipe]
})
export class PatrimonyComponent implements OnInit {
  approvedBuildings: ApprovedBuilding[] = [];
  incomeApartments: IncomeApartment[] = [];
  loading = true;
  error: string | null = null;

  constructor(private patrimonyService: PatrimonyService) {}

  ngOnInit(): void {
    this.patrimonyService.getPatrimony().subscribe({
      next: (data) => {
        this.approvedBuildings = data.approvedBuildings || [];
        this.incomeApartments = data.incomeApartments || [];
        this.loading = false;
      },
      error: () => {
        this.error = 'Error al cargar los datos de patrimonio';
        this.loading = false;
      }
    });
  }
}
