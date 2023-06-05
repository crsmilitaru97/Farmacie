import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-view-utilizator',
  templateUrl: './view-utilizator.component.html'
})
export class ViewUtilizatorComponent {
  utilizator = new Utilizator();
  afisareCampuriUtilizatorNou: boolean = false;
  pacient: Pacient = new Pacient();

  constructor(public http: HttpClient, public router: Router, @Inject('BASE_URL') public baseUrl: string, public authService: AuthService) { }

  conectare() {
    const params = new HttpParams().set('email', this.utilizator.email).set('parola', this.utilizator.parola);

    this.http.get<Utilizator>(this.baseUrl + 'conectare', { params }).subscribe({
      next: (result) => {
        if (result != null) {
          this.utilizator = result,
            this.authService.esteFarmacist = this.utilizator.tip == "FAR",
            this.authService.esteConectat = true,
            this.http.get<Pacient>(`${this.baseUrl}getpacient?id_pacient=${encodeURIComponent(this.utilizator.iD_Pacient)}`).subscribe({
              next: (result) => {
                this.authService.pacient = result,
                  this.router.navigate(['/']);
              }
            });
        }

        else
          this.toggleMesajEroareConectare();
      }
    });
  }

  inregistrare() {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.utilizator.email)) {
      alert("Adresa de email introdusă nu este valida!");
      return;
    }
    if (this.pacient.cnp.length !== 13) {
      alert("CNP-ul trebuie sa aiba 13 cifre!");
      return;
    }

    this.http.post(this.baseUrl + 'inregistrare', this.utilizator).subscribe({
      next: (result) => {
        this.pacient.email = this.utilizator.email;
        this.utilizator.id = result;
        this.http.post(this.baseUrl + 'pacient/adauga', this.pacient).subscribe({
          next: (result) => {
            this.authService.pacient = this.pacient;
            this.pacient.id = result;
            this.utilizator.iD_Pacient = this.pacient.id;
            this.http.post(this.baseUrl + 'utilizator/modifica', this.utilizator).subscribe({
              next: () => {
                this.router.navigate(['/']);
              }
            });
          }
        });
      }
    });
  }

  toggleCampuriUtilizatorNou() {
    this.afisareCampuriUtilizatorNou = !this.afisareCampuriUtilizatorNou;
  }

  toggleMesajEroareConectare() {
    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }
}

export class Utilizator {
  id: any;
  email: any;
  parola: any;
  tip: any;
  iD_Pacient: any;
}
export class Pacient {
  id: any;
  nume: any;
  prenume: any;
  cnp: any;
  email: any;
  data_Nastere: any = new Date();
}
