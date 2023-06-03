import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Pacient } from '../view-pacient/view-pacient.component';

@Component({
  selector: 'app-view-comanda',
  templateUrl: './view-comanda.component.html'
})
export class ViewComandaComponent implements OnInit {
  comanda = new Comanda();
  http2: any;
  baseUrl: string;
  medNou: Medicament = new Medicament;
  public medicamente: Medicament[] = [];
  public pacienti: Pacient[] = [];
  public id_medicament: number | undefined;
  public id_pacient: number | undefined;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this.http2 = http;
    this.baseUrl = baseUrl;
    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (v) => { this.medicamente = v }
    });
    http.get<Pacient[]>(baseUrl + 'listapacienti').subscribe({
      next: (v) => { this.pacienti = v }
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length > 0) {
        this.comanda.id = 0;
        const encodedData = params['comanda'];
        const decodedData = JSON.parse(decodeURIComponent(encodedData));
        this.comanda = decodedData;
      }
      else {
        this.comanda.data = new Date();
        this.comanda.pret = 0;
        this.comanda.status = 0;
      }
    });
  }

  salveazaClicked() {
    this.http.post(this.baseUrl + 'comanda/adauga', this.comanda).subscribe({
      next: (v) => {
        this.router.navigate(['/fetch-data-comenzi']);
      }
    });
  }

  aprobaClicked() {
    this.http.post(this.baseUrl + 'comanda/aproba', this.comanda).subscribe({
      next: (response) => {
        const message = response; // Access the message property using bracket notation
        console.log(message);
        this.comanda.status = 1;
      },
      error: (response) => {
        // Handle errors from the HTTP request if needed
      }
    });
  }

  adaugaMedicament() {
    this.medNou.pret = this.medNou.cantitate * this.medNou.pret;
    this.comanda.pret += this.medNou.pret;

    const medicamentExistent = this.comanda.medicamente.find(med => med.id == this.id_medicament)
    if (medicamentExistent) {
      medicamentExistent.cantitate += this.medNou.cantitate;
      medicamentExistent.pret += this.medNou.pret;
    }
    else {
      this.comanda.medicamente.push(this.medNou);
    }

    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }

  alegeMedicament() {
    var medTemplate = this.medicamente.find(med => med.id == this.id_medicament) as Medicament;

    this.medNou = {
      ...medTemplate,
      cantitate: 1
    };
  }

  afiseazaAdaugaMedicament() {
    this.medNou = new Medicament;
    this.medNou.cantitate = 1;

    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }

  getForma(forma: string): string {
    switch (forma) {
      case 'COMP':
        return 'Comprimate';
      case 'COPF':
        return 'Comprimate filmate';
      case 'COPS':
        return 'Comprimate de supt';
      case 'COPE':
        return 'Comprimate efervescente';
      case 'CAPS':
        return 'Capsule';
      case 'TABL':
        return 'Tablete';
      case 'SUPO':
        return 'Supozitoare';
      case 'SIRO':
        return 'Sirop';
      default:
        return '';
    }
  }

  getDate(date: Date) {
    date = new Date(date);

    return date.toLocaleString('ro', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}

export class Comanda {
  id: any;
  status: any;
  data: any;
  pret: any;
  medicamente: Medicament[] = [];
  iD_Pacient: any;
}

export class Medicament {
  id: any;
  denumire: any;
  cantitate: any;
  forma: any;
  pret: any;
  descriere: any;
}
