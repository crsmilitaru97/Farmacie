import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-view-pacient',
  templateUrl: './view-pacient.component.html'
})
export class ViewPacientComponent implements OnInit {

  pacient = new Pacient();
  baseUrl: string;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
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
    if (!this.pacient.id) {
      this.http.post(this.baseUrl + 'pacient/adauga', this.pacient).subscribe({
        next: (v) => {
          this.router.navigate(['/fetch-data-pacienti']);
        }
      });
    }
    else {
      this.http.post(this.baseUrl + 'pacient/modifica', this.pacient).subscribe({
        next: (v) => {
          this.router.navigate(['/fetch-data-pacienti']);
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
