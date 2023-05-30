import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data-medicamente',
  templateUrl: './fetch-data-medicamente.component.html'
})
export class MedicamentComponent {
  public meds: Medicament[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (v) => { this.meds = v }
    });
  }

  modificaClicked(med: Medicament) {
    // Handle button click event
    // Add your desired functionality here
  }

  stergeClicked() {
    // Handle button click event
    // Add your desired functionality here
  }

  selectedChanged(med: Medicament) {
    med.selectat = !med.selectat;
  }
}

interface Medicament {
  selectat: boolean;
  id: number;
  denumire: string;
  gramaj: number;
  forma: string;
  descriere: string;
}
