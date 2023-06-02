import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-view-medicament',
  templateUrl: './view-medicament.component.html'
})
export class ViewMedicamentComponent implements OnInit {

  medicament = new Medicament();
  baseUrl: string;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length > 0) {
        const encodedData = params['medicament'];
        const decodedData = JSON.parse(decodeURIComponent(encodedData));
        this.medicament = decodedData;
      }
      else {
        this.medicament.forma = "COMP";
      }
    });
  }

  salveazaClicked() {
    if (!this.medicament.id) {
      this.http.post(this.baseUrl + 'medicament/adauga', this.medicament).subscribe({
        next: (v) => {
          this.router.navigate(['/fetch-data-medicamente']);
        }
      });
    }
    else {
      this.http.post(this.baseUrl + 'medicament/modifica', this.medicament).subscribe({
        next: (v) => {
          this.router.navigate(['/fetch-data-medicamente']);
        }
      });
    }
  }
}

export class Medicament {
  id: any;
  denumire: any;
  forma: any;
  descriere: any;
  pret: any;
}
