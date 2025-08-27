
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing-module';
import { HttpClientModule } from '@angular/common/http';
import { App } from './app';
import { Header } from './components/header/header';
import { Footer } from './components/footer/footer';
import { Home } from './pages/home/home';
import { Requests } from './pages/requests/requests';
import { Transactions } from './pages/transactions/transactions';
import { Budget } from './pages/budget/budget';

import { InfoCard } from './components/info-card/info-card';
import { TransactionsPageComponent } from "./components/transactions-page/transactions-page.component";
import { DetalleRequest } from './components/detalle-request/detalle-request';
import { ResumenRequestsComponent } from './components/resumen-requests/resumen-requests.component';
import { ZoneGrafo } from './components/zone-grafo/zone-grafo';

@NgModule({
  declarations: [
  App,
  Header,
  Footer,
  Transactions,
  DetalleRequest,
    
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    Budget,
    InfoCard,
    TransactionsPageComponent,
    ResumenRequestsComponent,
    ZoneGrafo
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
