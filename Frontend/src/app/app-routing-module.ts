import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Importa los componentes para asociarlos a las rutas
import { Home } from './pages/home/home';
import { Requests } from './pages/requests/requests';
import { Budget } from './pages/budget/budget';
import { History } from './pages/history/history';
import { Formulario } from './pages/formulario/formulario';
import { PatrimonyComponent } from './pages/patrimony/patrimony';

// Definición de rutas
import { RequestHistoryComponent } from './components/request-history/request-history.component';
const routes: Routes = [
  { path: '', component: Home },             // Ruta raíz: / 
  { path: 'requests', component: Requests }, // /requests
  { path: 'budget', component: Budget},     // /budget
  { path: 'history', component: History }, // /history (antiguo)
  { path: 'request-history', component: RequestHistoryComponent }, // /request-history (nuevo)
  { path: 'formulario', component: Formulario }, // /formulario
  { path: 'formulario/:buildingCode', component: Formulario }, // /formulario/:buildingCode
  { path: 'formulario/:buildingCode/:requestId', component: Formulario }, // /formulario/:buildingCode/:requestId
  { path: 'patrimony', component: PatrimonyComponent }, // /patrimony

  // Si quieres manejar rutas no encontradas (404)
  { path: '**', redirectTo: '', pathMatch: 'full' }    // Redirige a Home
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
