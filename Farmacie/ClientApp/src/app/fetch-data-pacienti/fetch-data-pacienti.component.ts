import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-fetch-data-pacienti',
  templateUrl: './fetch-data-pacienti.component.html'
})
export class PacientComponent {

  public pacientiNefiltrati: Pacient[] = [];
  filtruPacient!: string;
  pacienti: Pacient[] = [];

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);
    if (!authService.esteFarmacist)
      this.router.navigate(['/']);

    http.get<Pacient[]>(baseUrl + 'listapacienti').subscribe({
      next: (result) => {
        this.pacientiNefiltrati = result
        this.pacienti = this.pacientiNefiltrati;
      }
    });
  }

  filterPacs() {
    if (this.filtruPacient) {
      this.pacienti = this.pacientiNefiltrati.filter(pac => {
        return this.getNumePrenume(pac).toLowerCase().includes(this.filtruPacient.toLowerCase()) ||
          pac.cnp.includes(this.filtruPacient);
      });
    } else {
      this.pacienti = this.pacientiNefiltrati;
    }
  }

  getNumePrenume(pac: Pacient) {
    return pac.nume + " " + pac.prenume;
  }

  modificaClicked(pac: Pacient) {
    const queryParams = encodeURIComponent(JSON.stringify(pac));
    const modifyPageUrl = `/pacient?pacient=${queryParams}`;
    this.router.navigateByUrl(modifyPageUrl);
  }

  stergeClicked() {
    const selectedRows = this.pacientiNefiltrati.filter((row) => row.selectat);
    selectedRows.forEach((row) => {
      const index = this.pacientiNefiltrati.indexOf(row);
      if (index > -1) {
        this.http.post(this.baseUrl + 'pacient/sterge', row).subscribe({ next: () => { } });
        this.pacientiNefiltrati.splice(index, 1);
      }
    });
  }
}

interface Pacient {
  selectat: boolean;
  id: number;
  nume: string;
  prenume: string;
  cnp: string;
  data_Nastere: Date;
  telefon: string;
  email: string;
}
