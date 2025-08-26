import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BudgetService } from '../../pages/budget/budget.service';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
	selector: 'gasto-mensual',
	templateUrl: './gasto-mensual.component.html',
	styleUrls: ['./gasto-mensual.component.css'],
	standalone: true,
	imports: [CommonModule, BaseChartDirective]
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
			title: { display: true, text: 'Gasto a lo largo de los meses' }
		},
		scales: {
			x: {},
			y: { beginAtZero: true }
		}
	};
	barChartType: 'bar' = 'bar';

	constructor(private budgetService: BudgetService) {}

	ngOnInit(): void {
		this.loadMonthlyExpenses();
	}

	private loadMonthlyExpenses() {
		this.budgetService.getMonthlyGastoIngreso().subscribe({
			next: (result) => {
				const currentYear = new Date().getFullYear();
				const gastos = result.filter(r => r.transactionType === 'GASTO' && r.año === currentYear);
				const ingresos = result.filter(r => r.transactionType === 'INGRESO' && r.año === currentYear);
				const monthlyExpenses = Array(12).fill(0);
				const monthlyIncome = Array(12).fill(0);
				gastos.forEach(g => {
					if (g.mes >= 1 && g.mes <= 12) {
						monthlyExpenses[g.mes - 1] = g.totalGasto;
					}
				});
				ingresos.forEach(i => {
					if (i.mes >= 1 && i.mes <= 12) {
						monthlyIncome[i.mes - 1] = i.totalGasto;
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
				this.loading = false;
			},
			error: () => {
				this.error = 'Error al cargar datos de gasto/ingreso mensual';
				this.loading = false;
			}
		});
	}
}
