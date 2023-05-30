import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data-comenzi',
  templateUrl: './fetch-data-comenzi.component.html'
})
export class ComenziComponent {
  public comz: Comanda[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Comanda[]>(baseUrl + 'listacomenzi').subscribe({
      next: (v) => { this.comz = v }
    });
  }

  selectedChanged(com: Comanda) {
    com.selectat = !com.selectat;
  }

  aprobaClicked(com: Comanda) {
    // Handle button click event
    // Add your desired functionality here
  }

  modificaClicked(com: Comanda) {
    // Handle button click event
    // Add your desired functionality here
  }

  stergeClicked() {
    // Handle button click event
    // Add your desired functionality here
  }

  toggleTable(row: Comanda) {
    row.arataDetalii = !row.arataDetalii;
  }
}


interface Comanda {
  selectat: boolean;
  id: number;
  numePacient: string;
  medicamente: Array<Medicament>;
  data: Date;
  status: number;
  arataDetalii: boolean;
}

interface Medicament {
  denumire: string;
  cantitate: number;
  forma: string;
}
