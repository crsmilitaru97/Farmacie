import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data-medicamente',
  templateUrl: './fetch-data-medicamente.component.html'
})
export class MedicamentComponent {
  public meds: Medicament[] = [];
  http2: any;
  baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http2 = http;
    this.baseUrl = baseUrl;
    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (v) => { this.meds = v }
    });
  }

  modificaClicked(med: Medicament) {
    const queryParams = encodeURIComponent(JSON.stringify(med));
    const modifyPageUrl = `/view-medicament?medicament=${queryParams}`;
    window.location.href = modifyPageUrl;
  }

  stergeClicked() {
    const selectedRows = this.meds.filter((row) => row.selectat);
    selectedRows.forEach((row) => {
      const index = this.meds.indexOf(row);
      if (index > -1) {
        this.http2.post(this.baseUrl + 'medicament/sterge', row).subscribe({ next: () => { } });
        this.meds.splice(index, 1);
      }
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

  getDescriere(descriere: string): string {
    if (descriere.length > 100) {
      return descriere.substring(0, 100).concat("...");
    }
    return descriere;
  }
}

interface Medicament {
  selectat: boolean;
  id: number;
  denumire: string;
  forma: string;
  descriere: string;
  pret: number;
}
