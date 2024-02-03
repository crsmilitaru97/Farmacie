import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  constructor(private router: Router, public authService: AuthService) {
  }

  deconectare() {
    this.authService.esteConectat = false;
  }

  viewPacient() {
    const queryParams = encodeURIComponent(JSON.stringify(this.authService.pacient));
    const modifyPageUrl = `/pacient?pacient=${queryParams}`;
    this.router.navigateByUrl(modifyPageUrl);
    //window.location.href = modifyPageUrl; Reseteaza clasele ex.: AuthService
  }
}
