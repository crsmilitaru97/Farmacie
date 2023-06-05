import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { Helpers } from '../helpers';

@Component({
  selector: 'app-fetch-data-comenzi',
  templateUrl: './fetch-data-comenzi.component.html'
})
export class ComenziComponent {

  public comenzi: Comanda[] = [];
  selectedRows: any[] = [];
  mesajAprobareComanda: string | undefined;

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, private router: Router, public authService: AuthService, public helpers: Helpers) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);

    http.get<Comanda[]>(baseUrl + 'listacomenzi').subscribe({
      next: (result) => {
        if (this.authService.esteFarmacist) {
          this.comenzi = result
        }
        else {
          this.comenzi = result.filter(com => { return com.iD_Pacient == this.authService.pacient?.id });
        }
      }
    });
  }

  updateSelectedRows() {
    this.selectedRows = this.comenzi.filter(com => com.selectat);
  }

  modificaClicked(com: Comanda) {
    const queryParams = encodeURIComponent(JSON.stringify(com));
    const modifyPageUrl = `/comanda?comanda=${queryParams}`;
    this.router.navigateByUrl(modifyPageUrl);
  }

  stergeClicked() {
    this.selectedRows.forEach((row) => {
      const index = this.comenzi.indexOf(row);
      if (index > -1) {
        this.http.post(this.baseUrl + 'comanda/sterge', row).subscribe({ next: () => { } });
        this.comenzi.splice(index, 1);
        this.updateSelectedRows();
      }
    });
  }

  aprobaClicked(com: Comanda) {
    this.http.post(this.baseUrl + 'comanda/aproba', com).subscribe({
      next: (response: any) => {
        if (response.status === 'error') {
          this.mesajAprobareComanda = response.message;
          this.toggleMesajAprobare();
        }
        else
          com.status = 1;
      }
    });
  }

  toggleMesajAprobare() {
    var popupContainer = document.getElementById('popupContainer');
    popupContainer?.classList.toggle('show');
  }

  toggleTable(row: Comanda) {
    row.arataDetalii = !row.arataDetalii;
  }
}

interface Comanda {
  selectat: boolean;
  id: number;
  numePacient: string;
  comandaMedicamente: ComandaMedicament[];
  data: Date;
  status: number;
  pret: number;
  arataDetalii: boolean;
  iD_Pacient: number;
}

interface ComandaMedicament {
  id: any;
  cantitate: any;
  id_Medicament: any;
  _medicament: Medicament;
}

interface Medicament {
  id: any;
  denumire: any;
  cantitate: any;
  forma: any;
  pret: any;
  descriere: any;
}
