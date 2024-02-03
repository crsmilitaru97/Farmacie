import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-view-medicament',
  templateUrl: './view-medicament.component.html'
})
export class ViewMedicamentComponent implements OnInit {

  medicament = new Medicament();

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private route: ActivatedRoute, private router: Router, public authService: AuthService, public helpers: Helpers) {

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
        next: () => {
          this.router.navigate(['/medicamente']);
        }
      });
    }
    else {
      this.http.post(this.baseUrl + 'medicament/modifica', this.medicament).subscribe({
        next: () => {
          this.router.navigate(['/medicamente']);
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
