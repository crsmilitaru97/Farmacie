import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-fetch-data-medicamente',
  templateUrl: './fetch-data-medicamente.component.html'
})
export class MedicamentComponent {

  public medicamente: Medicament[] = [];

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);

    http.get<Medicament[]>(baseUrl + 'listamedicamente').subscribe({
      next: (result) => { this.medicamente = result }
    });
  }

  modificaClicked(med: Medicament) {
    const queryParams = encodeURIComponent(JSON.stringify(med));
    const modifyPageUrl = `/medicament?medicament=${queryParams}`;
    this.router.navigateByUrl(modifyPageUrl);
  }

  stergeClicked() {
    const selectedRows = this.medicamente.filter((row) => row.selectat);
    selectedRows.forEach((row) => {
      const index = this.medicamente.indexOf(row);
      if (index > -1) {
        this.http.post(this.baseUrl + 'medicament/sterge', row).subscribe({ next: () => { } });
        this.medicamente.splice(index, 1);
      }
    });
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
