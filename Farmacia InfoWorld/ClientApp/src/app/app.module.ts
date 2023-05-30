import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { MedicamentComponent } from './fetch-data-medicamente/fetch-data-medicamente.component';
import { PacientComponent } from './fetch-data-pacienti/fetch-data-pacienti.component';
import { ComenziComponent } from './fetch-data-comenzi/fetch-data-comenzi.component';
import { ViewPacientComponent } from './view-pacient/view-pacient.component';
import { ViewMedicamentComponent } from './view-medicament/view-medicament.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    MedicamentComponent,
    PacientComponent,
    ComenziComponent,
    ViewPacientComponent,
    ViewMedicamentComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data-medicamente', component: MedicamentComponent },
      { path: 'fetch-data-pacienti', component: PacientComponent },
      { path: 'fetch-data-comenzi', component: ComenziComponent },
      { path: 'view-pacient', component: ViewPacientComponent },
      { path: 'view-medicament', component: ViewMedicamentComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
