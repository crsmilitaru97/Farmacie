import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view-medicament',
  templateUrl: './view-medicament.component.html'
})
export class ViewMedicamentComponent implements OnInit {

  medicament = new Medicament();

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      const encodedData = params['medicament'];
      const decodedData = JSON.parse(decodeURIComponent(encodedData));
      this.medicament = decodedData;
    });
  }

  salveazaClicked() {
    this.http.post('https://localhost:44462/' + 'adaugamedicament', this.medicament).subscribe({
      next: (v) => { }
    });
  }

  addRow() {

  }

}

export class Medicament {
  denumire: any;
  concentratie: any;
  forma: any;
  descriere: any;
}
