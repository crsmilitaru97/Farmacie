import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-view-loturi',
  templateUrl: './view-loturi.component.html'
})
export class ViewLoturiComponent implements OnInit {
  public medicamente: Medicament[] = [];
  public id_medicament: number | undefined;
  public loturi: Lot[] = [];
  baseUrl: string;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (v) => { this.medicamente = v }
    });
  }

  ngOnInit(): void {
  }

  alegeMedicament() {
    this.loturi = [];
    this.http.get<Lot[]>(this.baseUrl + 'medicament/loturi?id_medicament=' + this.id_medicament).subscribe({
      next: (v) => { this.loturi = v }
    });
  }

  adaugaStoc() {
  }
}

export class Lot {
  id: any;
  data_Expirare: any;
  cantitate: any;
  id_Medicament: any;
}

export class Medicament {
  id: any;
  denumire: any;
}
