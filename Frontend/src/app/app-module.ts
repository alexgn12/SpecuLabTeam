import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
// import { MatPaginatorModule } from '@angular/material/paginator'; // Instalar Angular Material para habilitar
import { AppRoutingModule } from './app-routing-module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './services/auth.interceptor';
import { App } from './app';
import { Header } from './components/header/header';
import { Footer } from './components/footer/footer';
import { HomeComponent } from './pages/home/home';
import { Requests } from './pages/requests/requests';
// import { History } from './pages/history/history';
import { Budget } from './pages/budget/budget';

import { InfoCard } from './components/info-card/info-card';
import { LoginComponent } from './pages/login/login.component';
import { TransactionsPageComponent } from "./components/transactions-page/transactions-page.component";
import { ResumenRequestsComponent } from './components/resumen-requests/resumen-requests.component';
import { ZoneGrafo } from './components/zone-grafo/zone-grafo';
import { RequestHistoryComponent } from './components/request-history/request-history.component';
import { AnalyzeBuildingRequestComponent } from './components/analyze-building-request/analyze-building-request.component';
import { AuthGuard } from './services/auth.guard';
import { LogoutButtonComponent } from "./components/logout-button/logout-button";
@NgModule({
  declarations: [
    App,
    Header,
  // History,
  // RentabilidadComponent se importa como standalone en imports
  ],
  imports: [
    HomeComponent,
    BrowserModule,
    CommonModule,
    FormsModule,
    // MatPaginatorModule, // Descomentar cuando Angular Material esté instalado
    AppRoutingModule,
    HttpClientModule,
    Budget,
    InfoCard,
    TransactionsPageComponent,
    ResumenRequestsComponent,
    ZoneGrafo,
    RequestHistoryComponent,
    Footer,
    AnalyzeBuildingRequestComponent,
    LoginComponent,
    LogoutButtonComponent
],
  providers: [
    provideBrowserGlobalErrorListeners(),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    AuthGuard
  ],
  bootstrap: [App]
})
export class AppModule { }
