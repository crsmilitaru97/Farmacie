import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Helpers {

  constructor() { }

  getDataTimp(date: Date) {
    date = new Date(date);

    return date.toLocaleString('ro', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  getData(date: Date) {
    date = new Date(date);

    return date.toLocaleString('ro', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  getForma(forma: string): string {
    switch (forma) {
      case 'COMP':
        return 'Comprimate';
      case 'COPF':
        return 'Comprimate filmate';
      case 'COPS':
        return 'Comprimate de supt';
      case 'COPE':
        return 'Comprimate efervescente';
      case 'CAPS':
        return 'Capsule';
      case 'TABL':
        return 'Tablete';
      case 'SUPO':
        return 'Supozitoare';
      case 'SIRO':
        return 'Sirop';
      default:
        return '';
    }
  }
}
