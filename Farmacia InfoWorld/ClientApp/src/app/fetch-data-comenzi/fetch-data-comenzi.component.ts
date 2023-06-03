import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data-comenzi',
  templateUrl: './fetch-data-comenzi.component.html'
})
export class ComenziComponent {
  selectedRows: any[] = [];
  public comenzi: Comanda[] = [];
  http2: any;
  baseUrl: string;
  message: string | undefined;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http2 = http;
    this.baseUrl = baseUrl;
    http.get<Comanda[]>(baseUrl + 'listacomenzi').subscribe({
      next: (v) => { this.comenzi = v }
    });
  }

  updateSelectedRows() {
    this.selectedRows = this.comenzi.filter(com => com.selectat);
  }

  modificaClicked(com: Comanda) {
    const queryParams = encodeURIComponent(JSON.stringify(com));
    const modifyPageUrl = `/view-comanda?comanda=${queryParams}`;
    window.location.href = modifyPageUrl;
  }

  stergeClicked() {
    this.selectedRows.forEach((row) => {
      const index = this.comenzi.indexOf(row);
      if (index > -1) {
        this.http2.post(this.baseUrl + 'comanda/sterge', row).subscribe({ next: () => { } });
        this.comenzi.splice(index, 1);
        this.updateSelectedRows();
      }
    });
  }

  aprobaClicked(com: Comanda) {
    this.http2.post(this.baseUrl + 'comanda/aproba', com).subscribe({
      next: (response: any) => {
        if (response.status === 'error') {
          this.message = response.message;
          this.showMesajAprobare();
        }
        else
          com.status = 1;
      }
    });
  }

  showMesajAprobare() {
    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }

  toggleTable(row: Comanda) {
    row.arataDetalii = !row.arataDetalii;
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
}


interface Comanda {
  selectat: boolean;
  id: number;
  numePacient: string;
  medicamente: Array<Medicament>;
  data: Date;
  status: number;
  pret: number;
  arataDetalii: boolean;
}

interface Medicament {
  denumire: string;
  cantitate: number;
  forma: string;
  pret: number;
}
