import { Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';

export interface TransactionLiveDto {
  transactionId: number;
  type: string; // "INGRESO" | "GASTO"
  amount: number;
  description: string;
  utcCreated: string; // ISO date string
}

export interface RequestLiveDto {
  requestId: number;
  description: string;
  buildingAmount: number;
  maintenanceAmount: number;
  statusId: number;
  statusType: string;
  buildingId: number;
  buildingCode: string;
  requestDate: string; // ISO date string
}

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private transactionCreatedSubject = new BehaviorSubject<TransactionLiveDto | null>(null);
  private transactionUpdatedSubject = new BehaviorSubject<TransactionLiveDto | null>(null);
  private requestCreatedSubject = new BehaviorSubject<RequestLiveDto | null>(null);
  private requestUpdatedSubject = new BehaviorSubject<RequestLiveDto | null>(null);

  transactionCreated$: Observable<TransactionLiveDto | null> = this.transactionCreatedSubject.asObservable();
  transactionUpdated$: Observable<TransactionLiveDto | null> = this.transactionUpdatedSubject.asObservable();
  requestCreated$: Observable<RequestLiveDto | null> = this.requestCreatedSubject.asObservable();
  requestUpdated$: Observable<RequestLiveDto | null> = this.requestUpdatedSubject.asObservable();

  constructor(private ngZone: NgZone) {
    this.startConnection();
  }

  private startConnection(): void {
    const hubUrl = 'https://devdemoapi3.azurewebsites.net/hubs/live?topics=transactions,requests';
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(hubUrl, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('TransactionCreated', (dto: TransactionLiveDto) => {
      this.ngZone.run(() => this.transactionCreatedSubject.next(dto));
    });
    this.hubConnection.on('TransactionUpdated', (dto: TransactionLiveDto) => {
      this.ngZone.run(() => this.transactionUpdatedSubject.next(dto));
    });
    this.hubConnection.on('RequestCreated', (dto: RequestLiveDto) => {
      this.ngZone.run(() => this.requestCreatedSubject.next(dto));
    });
    this.hubConnection.on('RequestUpdated', (dto: RequestLiveDto) => {
      this.ngZone.run(() => this.requestUpdatedSubject.next(dto));
    });

    this.hubConnection.start()
      .catch(err => console.error('Error al conectar con SignalR:', err));
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }
}
