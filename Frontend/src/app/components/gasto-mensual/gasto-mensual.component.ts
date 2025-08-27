import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BudgetService } from '../../pages/budget/budget.service';
import { ChartConfiguration } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';

@Component({
	selector: 'gasto-mensual',
	templateUrl: './gasto-mensual.component.html',
	styleUrls: ['./gasto-mensual.component.css'],
	standalone: true,
		imports: [CommonModule, NgChartsModule]
})
export class GastoMensualComponent implements OnInit {
	loading = true;
	error: string | null = null;

	monthlyExpensesLabels: string[] = [
		'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
		'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
	];
	monthlyExpensesData: number[] = Array(12).fill(0);
	monthlyIncomeData: number[] = Array(12).fill(0);

	barChartData: ChartConfiguration<'bar'>['data'] = {
		labels: this.monthlyExpensesLabels,
		datasets: [
			{
				data: this.monthlyExpensesData,
				label: 'Gastos',
				backgroundColor: '#dc2626',
				borderRadius: 6
			},
			{
				data: this.monthlyIncomeData,
				label: 'Ingresos',
				backgroundColor: '#16a34a',
				borderRadius: 6
			}
		]
	};
	barChartOptions: ChartConfiguration<'bar'>['options'] = {
		responsive: true,
		plugins: {
			legend: { display: false },
			title: { display: true, text: 'Gasto a lo largo de los meses' },
			datalabels: { display: false } // Desactiva los datalabels sobre las barras
		},
		scales: {
			x: {},
			y: { beginAtZero: true }
		}
	};
	barChartType: 'bar' = 'bar';

	years: number[] = [];
	selectedYear: number | null = null;
	allData: any[] = [];

	constructor(private budgetService: BudgetService) {}

	ngOnInit(): void {
		this.fetchDataAndInit();
	}

	fetchDataAndInit() {
		this.loading = true;
		this.budgetService.getMonthlyGastoIngreso().subscribe({
			next: (result) => {
				this.allData = result;
				this.years = Array.from(new Set(result.map(r => r.año))).sort((a, b) => b - a);
				// Seleccionar año actual si existe, si no el más reciente
				const currentYear = new Date().getFullYear();
				this.selectedYear = this.years.includes(currentYear) ? currentYear : this.years[0];
				this.updateChartForYear();
				this.loading = false;
			},
			error: () => {
				this.error = 'Error al cargar datos de gasto/ingreso mensual';
				this.loading = false;
			}
		});
	}

		onYearChange(event: Event) {
			const value = (event.target as HTMLSelectElement).value;
			this.selectedYear = parseInt(value, 10);
			this.updateChartForYear();
		}

	updateChartForYear() {
		if (!this.selectedYear) return;
		const gastos = this.allData.filter(r => r.transactionType === 'GASTO' && r.año === this.selectedYear);
		const ingresos = this.allData.filter(r => r.transactionType === 'INGRESO' && r.año === this.selectedYear);
		const monthlyExpenses = Array(12).fill(0);
		const monthlyIncome = Array(12).fill(0);
		gastos.forEach(g => {
			if (g.mes >= 1 && g.mes <= 12) {
				monthlyExpenses[g.mes - 1] = (g as any).buildingAmount ?? g.totalGasto ?? 0;
			}
		});
		ingresos.forEach(i => {
			if (i.mes >= 1 && i.mes <= 12) {
				monthlyIncome[i.mes - 1] = (i as any).amount ?? i.totalGasto ?? 0;
			}
		});
		this.monthlyExpensesData = monthlyExpenses;
		this.monthlyIncomeData = monthlyIncome;
		this.barChartData = {
			labels: this.monthlyExpensesLabels,
			datasets: [
				{
					data: this.monthlyExpensesData,
					label: 'Gastos',
					backgroundColor: '#dc2626',
					borderRadius: 6
				},
				{
					data: this.monthlyIncomeData,
					label: 'Ingresos',
					backgroundColor: '#16a34a',
					borderRadius: 6
				}
			]
		};
	}
}
