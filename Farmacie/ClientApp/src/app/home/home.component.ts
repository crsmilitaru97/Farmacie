import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(private router: Router, public authService: AuthService) {
    if (!authService.esteConectat)
      this.router.navigate(['/conectare']);
  }
}
