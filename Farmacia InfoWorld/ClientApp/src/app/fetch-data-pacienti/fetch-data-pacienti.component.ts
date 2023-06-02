import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-fetch-data-pacienti',
  templateUrl: './fetch-data-pacienti.component.html'
})
export class PacientComponent {
  public pacs: Pacient[] = [];
  http2: any;
  baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http2 = http;
    this.baseUrl = baseUrl;
    http.get<Pacient[]>(baseUrl + 'listapacienti').subscribe({
      next: (v) => { this.pacs = v }
    });
  }

  modificaClicked(pac: Pacient) {
    const queryParams = encodeURIComponent(JSON.stringify(pac));
    const modifyPageUrl = `/view-pacient?pacient=${queryParams}`;
    window.location.href = modifyPageUrl;
  }

  stergeClicked() {
    const selectedRows = this.pacs.filter((row) => row.selectat);
    selectedRows.forEach((row) => {
      const index = this.pacs.indexOf(row);
      if (index > -1) {
        this.http2.post(this.baseUrl + 'pacient/sterge', row).subscribe({ next: () => { } });
        this.pacs.splice(index, 1);
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
