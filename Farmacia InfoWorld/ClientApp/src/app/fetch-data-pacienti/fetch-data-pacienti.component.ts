import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-fetch-data-pacienti',
  templateUrl: './fetch-data-pacienti.component.html'
})
export class PacientComponent {
  public pacs: Pacient[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Pacient[]>(baseUrl + 'listapacienti').subscribe({
      next: (v) => { this.pacs = v }
    });
  }

  stergeClicked() {
    const selectedRows = this.pacs.filter((row) => row.selectat);
    selectedRows.forEach((row) => {
      const index = this.pacs.indexOf(row);
      if (index > -1) {
        this.pacs.splice(index, 1);
      }
    });
  }

  modificaClicked(pac: Pacient) {
    const queryParams = encodeURIComponent(JSON.stringify(pac));
    const modifyPageUrl = `/view-pacient?pacient=${queryParams}`;
    window.location.href = modifyPageUrl;
  }

  selectedChanged(pac: Pacient) {
    pac.selectat = !pac.selectat;
  }
}

interface Pacient {
  selectat: boolean;
  id: number;
  nume: string;
  prenume: string;
  cnp: string;
  data_nastere: Date;
  telefon: string;
  email: string;
}
