import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-view-pacient',
  templateUrl: './view-pacient.component.html'
})
export class ViewPacientComponent implements OnInit {

  pacient: Pacient = new Pacient();

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private route: ActivatedRoute, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);
  }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length > 0) {
        const encodedData = params['pacient'];
        const decodedData = JSON.parse(decodeURIComponent(encodedData));
        this.pacient = decodedData;
      }
      else {
        this.addRow(0);
      }
    });
  }

  salveazaClicked() {
    if (this.pacient.cnp.length !== 13) {
      alert("CNP-ul trebuie sa aiba 13 cifre!");
      return;
    }
    if (this.pacient.telefon.length !== 10) {
      alert("Numarul de telefon trebuie sa aiba 10 cifre!");
      return;
    }
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.pacient.email)) {
      alert("Adresa de email introdusa nu este valida!");
      return;
    }

    if (!this.pacient.id) {
      this.http.post(this.baseUrl + 'pacient/adauga', this.pacient).subscribe({
        next: () => {
          this.router.navigate(['/pacienti']);
        }
      });
    }
    else {
      this.http.post(this.baseUrl + 'pacient/modifica', this.pacient).subscribe({
        next: () => {
          this.router.navigate(['/pacienti']);
        }
      });
    }
  }

  addRow(defaultSelected: number) {
    if (!this.pacient.id) {
      this.pacient.adrese = [];
    }

    const adresaNoua = new Adresa();
    adresaNoua.tip_Adresa = defaultSelected;
    this.pacient.adrese?.push(adresaNoua);
  }
}

export class Pacient {
  id: any | 0;
  nume: any;
  prenume: any;
  cnp: any;
  data_Nastere: any;
  telefon: any;
  email: any;
  adrese: Array<Adresa> | undefined;
}

export class Adresa {
  id: any;
  tip_Adresa: any;
  linie_Adresa: any;
  localitate: any;
  judet: any;
  cod_Postal: any;
  id_Pacient: any | 0;
}
