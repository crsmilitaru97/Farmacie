import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { ComenziComponent } from './fetch-data-comenzi/fetch-data-comenzi.component';
import { MedicamentComponent } from './fetch-data-medicamente/fetch-data-medicamente.component';
import { PacientComponent } from './fetch-data-pacienti/fetch-data-pacienti.component';
import { HomeComponent } from './home/home.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ViewComandaComponent } from './view-comanda/view-comanda.component';
import { ViewLoturiComponent } from './view-loturi/view-loturi.component';
import { ViewMedicamentComponent } from './view-medicament/view-medicament.component';
import { ViewPacientComponent } from './view-pacient/view-pacient.component';
import { ViewUtilizatorComponent } from './view-utilizator/view-utilizator.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    MedicamentComponent,
    PacientComponent,
    ComenziComponent,
    ViewPacientComponent,
    ViewMedicamentComponent,
    ViewComandaComponent,
    ViewLoturiComponent,
    ViewUtilizatorComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'medicamente', component: MedicamentComponent },
      { path: 'pacienti', component: PacientComponent },
      { path: 'comenzi', component: ComenziComponent },
      { path: 'pacient', component: ViewPacientComponent },
      { path: 'medicament', component: ViewMedicamentComponent },
      { path: 'comanda', component: ViewComandaComponent },
      { path: 'loturi', component: ViewLoturiComponent },
      { path: 'conectare', component: ViewUtilizatorComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
