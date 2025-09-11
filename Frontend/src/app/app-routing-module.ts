import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Importa los componentes para asociarlos a las rutas
import { HomeComponent } from './pages/home/home';
import { Requests } from './pages/requests/requests';
import { Budget } from './pages/budget/budget';
// import { History } from './pages/history/history';
import { Formulario } from './pages/formulario/formulario';
import { PatrimonyComponent } from './pages/patrimony/patrimony';

// Definición de rutas
import { RequestHistoryComponent } from './components/request-history/request-history.component';
import { LoginComponent } from './pages/login/login.component';
import { AuthGuard } from './services/auth.guard';
const routes: Routes = [
  { path: 'login', component: LoginComponent }, // /login (sin guard)
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },             // Ruta raíz: / 
  { path: 'requests', component: Requests, canActivate: [AuthGuard] }, // /requests
  { path: 'budget', component: Budget, canActivate: [AuthGuard] },     // /budget
  // { path: 'history', component: History }, // /history (antiguo)
  { path: 'request-history', component: RequestHistoryComponent, canActivate: [AuthGuard] }, // /request-history (nuevo)
  { path: 'formulario', component: Formulario, canActivate: [AuthGuard] }, // /formulario
  { path: 'formulario/:buildingCode', component: Formulario, canActivate: [AuthGuard] }, // /formulario/:buildingCode
  { path: 'formulario/:buildingCode/:requestId', component: Formulario, canActivate: [AuthGuard] }, // /formulario/:buildingCode/:requestId
  { path: 'patrimony', component: PatrimonyComponent, canActivate: [AuthGuard] }, // /patrimony

  // Si quieres manejar rutas no encontradas (404)
  { path: '**', redirectTo: '', pathMatch: 'full' }    // Redirige a Home
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
