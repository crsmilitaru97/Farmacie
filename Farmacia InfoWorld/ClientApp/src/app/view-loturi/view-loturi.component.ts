import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-view-loturi',
  templateUrl: './view-loturi.component.html'
})
export class ViewLoturiComponent {

  public medicamente: Medicament[] = [];
  public id_medicament: number | undefined;
  public loturi: Lot[] = [];
  public lotNou: Lot = new Lot;

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private route: ActivatedRoute, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);

    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (v) => { this.medicamente = v }
    });
  }

  alegeMedicament() {
    this.loturi = [];
    this.http.get<Lot[]>(this.baseUrl + 'medicament/loturi?id_medicament=' + this.id_medicament).subscribe({
      next: (v) => { this.loturi = v }
    });
    this.lotNou.id_Medicament = this.id_medicament;
  }

  adaugaStoc() {
    this.http.post(this.baseUrl + 'medicament/loturi/adauga', this.lotNou).subscribe({
      next: (v) => {
        this.loturi.push(this.lotNou);
        this.lotNou = new Lot;
        this.lotNou.id_Medicament = this.id_medicament;
      }
    });
  }
}

export class Lot {
  id: any;
  data_Expirare: Date = new Date();
  cantitate: number = 1;
  id_Medicament: any;
}

export class Medicament {
  id: any;
  denumire: any;
}
