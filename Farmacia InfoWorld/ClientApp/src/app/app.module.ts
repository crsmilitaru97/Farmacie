import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { MedicamentComponent } from './fetch-data-medicamente/fetch-data-medicamente.component';
import { PacientComponent } from './fetch-data-pacienti/fetch-data-pacienti.component';
import { ComenziComponent } from './fetch-data-comenzi/fetch-data-comenzi.component';
import { ViewPacientComponent } from './view-pacient/view-pacient.component';
import { ViewMedicamentComponent } from './view-medicament/view-medicament.component';
import { ViewComandaComponent } from './view-comanda/view-comanda.component';
import { ViewLoturiComponent } from './view-loturi/view-loturi.component';

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
    ViewLoturiComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'fetch-data-medicamente', component: MedicamentComponent },
      { path: 'fetch-data-pacienti', component: PacientComponent },
      { path: 'fetch-data-comenzi', component: ComenziComponent },
      { path: 'view-pacient', component: ViewPacientComponent },
      { path: 'view-medicament', component: ViewMedicamentComponent },
      { path: 'view-comanda', component: ViewComandaComponent },
      { path: 'view-loturi', component: ViewLoturiComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
