import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public esteConectat: boolean = false;
  public esteFarmacist: boolean = false;
  public pacient: Pacient = new Pacient;
}

export class Pacient {
  id: any;
  nume: any;
  prenume: any;
  cnp: any;
}
