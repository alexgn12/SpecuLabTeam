import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { RequestsService, IRequest } from './requests.service';
import { RequestCard } from '../../components/request-card/request-card';
import { InfoCard } from '../../components/info-card/info-card';


@Component({
  selector: 'sl-requests',
  templateUrl: './requests.html',
  styleUrls: ['./requests.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, RequestCard, InfoCard]
})
export class Requests implements OnInit, OnDestroy {
  requestsData: IRequest[] = [];
  resumen: any = {};
  page = 1;
  size = 10;
  selectedStatus: string = 'Recibido';
  private subscription?: Subscription;
  private resumenSubscription?: Subscription;

  getEstadoTotal(estado: string): number {
    if (!this.resumen.porEstado) return 0;
    const found = this.resumen.porEstado.find((e: any) => e.estado === estado);
    return found ? found.total : 0;
  }

  onStatusChange() {
    this.page = 1;
    this.loadRequests();
  }

  constructor(private requestsService: RequestsService) {}

  ngOnInit() {
    this.loadRequests();
    this.resumenSubscription = this.requestsService.getResumenRequests().subscribe({
      next: (data) => this.resumen = data,
      error: err => console.error('Error al cargar resumen:', err)
    });
  }

  loadRequests() {
    this.subscription = this.requestsService.getRequests(this.page, this.size, this.selectedStatus)
      .subscribe({
        next: (requests: IRequest[]) => {
          this.requestsService.getAllBuildings().subscribe({
            next: (buildings: any[]) => {
              const buildingsMap = new Map(
                buildings.map(building => [Number(building.buildingId), building])
              );
              this.requestsData = requests.map(request => ({
                ...request,
                building: buildingsMap.get(Number(request.buildingId)) || {
                  buildingName: 'N/A',
                  street: 'N/A',
                  district: 'N/A',
                  floorCount: 0,
                  yearBuilt: 0,
                  buildingCode: 'N/A'
                }
              }));
            },
            error: err => console.error('Error al cargar buildings:', err)
          });
        },
        error: err => console.error('Error al cargar requests:', err)
      });
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.loadRequests();
    }
  }

  nextPage() {
    if (this.requestsData.length === this.size) {
      this.page++;
      this.loadRequests();
    }
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
    this.resumenSubscription?.unsubscribe();
  }
}
