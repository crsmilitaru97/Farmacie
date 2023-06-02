import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view-comanda',
  templateUrl: './view-comanda.component.html'
})
export class ViewComandaComponent implements OnInit {

  comanda = new Comanda();

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.comanda.id = 0;
      const encodedData = params['pacient'];
      const decodedData = JSON.parse(decodeURIComponent(encodedData));
      this.comanda = decodedData;
    });
  }

  salveazaClicked() {
    this.http.post('https://localhost:44462/' + 'adaugacomanda', this.comanda).subscribe({
      next: (v) => { }
    });
  }

  modificaClicked() {
    //this.http.post('https://localhost:44462/' + 'adaugapacient', this.pacient).subscribe({
    //  next: (v) => { }
    //});
  }

  addRow() {

  }

}

export class Comanda {
  id: any;
  pacient: any;
}
