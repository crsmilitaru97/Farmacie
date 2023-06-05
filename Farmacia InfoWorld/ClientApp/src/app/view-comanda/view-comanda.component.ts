import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';
import { Pacient } from '../view-pacient/view-pacient.component';

@Component({
  selector: 'app-view-comanda',
  templateUrl: './view-comanda.component.html'
})
export class ViewComandaComponent implements OnInit {

  comanda = new Comanda();
  comMedNou: ComandaMedicament = new ComandaMedicament;
  public medicamente: Medicament[] = [];
  public pacienti: Pacient[] = [];
  public id_pacient: number | undefined;

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private route: ActivatedRoute, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);

    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (result) => { this.medicamente = result }
    });

    if (this.authService.esteFarmacist) {
      http.get<Pacient[]>(baseUrl + 'listapacienti').subscribe({
        next: (result) => {
          this.pacienti = result
        }
      });
    }
    else {
      var pacient = new Pacient()
      pacient.id = this.authService.pacient.id;
      pacient.nume = this.authService.pacient.nume;
      pacient.prenume = this.authService.pacient.prenume;
      this.pacienti.push(pacient);
    }
  }

  ngOnInit() {
    this.route.queryParams.subscribe((params: { [x: string]: any; }) => {
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
    if (!this.comanda.id) {
      this.http.post(this.baseUrl + 'comanda/adauga', this.comanda).subscribe({
        next: (v) => {
          this.router.navigate(['/comenzi']);
        }
      });
    }
    else {
      this.http.post(this.baseUrl + 'comanda/modifica', this.comanda).subscribe({
        next: (v) => {
          this.router.navigate(['/comenzi']);
        }
      });
    }
  }

  aprobaClicked() {
    this.http.post(this.baseUrl + 'comanda/aproba', this.comanda).subscribe({
      next: (response) => {
        const message = response;
        console.log(message);
        this.comanda.status = 1;
      },
      error: (response) => {
      }
    });
  }

  adaugaMedicament() {
    this.comMedNou._medicament.pret = this.comMedNou.cantitate * this.comMedNou._medicament.pret;
    this.comanda.pret += this.comMedNou._medicament.pret;

    const comMedExistent = this.comanda.comandaMedicamente.find(comMed => comMed.iD_Medicament == this.comMedNou.iD_Medicament) as ComandaMedicament
    if (comMedExistent) {
      comMedExistent._status = 1;
      comMedExistent.cantitate += this.comMedNou.cantitate;
      comMedExistent._medicament.pret += this.comMedNou._medicament.pret;
    }
    else {
      this.comMedNou._status = 2;
      this.comanda.comandaMedicamente.push(this.comMedNou);
    }

    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }

  alegeMedicament() {
    const medTemplate = this.medicamente.find(med => med.id == this.comMedNou.iD_Medicament) as Medicament;

    this.comMedNou._medicament.denumire = medTemplate.denumire;
    this.comMedNou._medicament.id = this.comMedNou.iD_Medicament = medTemplate.id;
    this.comMedNou._medicament.pret = medTemplate.pret;
    this.comMedNou._medicament.forma = medTemplate.forma;
    this.comMedNou.cantitate = 1;
  }

  afiseazaAdaugaMedicament() {
    this.comMedNou = new ComandaMedicament;
    this.comMedNou.cantitate = 1;

    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }
}

export class Comanda {
  id: any;
  status: any;
  data: any;
  pret: any;
  comandaMedicamente: ComandaMedicament[] = [];
  iD_Pacient: any;
}

export class ComandaMedicament {
  id: any;
  cantitate: any;
  iD_Medicament: number = 0;
  _status: any;
  _medicament: Medicament = new Medicament;
}

export class Medicament {
  id: number = 0;
  denumire: any;
  cantitate: any;
  forma: any;
  pret: number = 0;
  descriere: any;
}
