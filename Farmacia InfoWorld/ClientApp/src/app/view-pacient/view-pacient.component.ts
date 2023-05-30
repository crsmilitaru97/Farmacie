import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view-pacient',
  templateUrl: './view-pacient.component.html'
})
export class ViewPacientComponent implements OnInit {

  pacient = new Pacient();

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      const encodedData = params['pacient'];
      const decodedData = JSON.parse(decodeURIComponent(encodedData));
      this.pacient = decodedData;
    });
  }

  salveazaClicked() {
    this.http.post('https://localhost:44462/' + 'adaugapacient', this.pacient).subscribe({
      next: (v) => { }
    });
  }

  addRow() {

  }

}

export class Pacient {
  selectat: any;
  id: any;
  nume: any;
  prenume: any;
  cnp: any;
  data_nastere: any;
  telefon: any;
  email: any;
}
