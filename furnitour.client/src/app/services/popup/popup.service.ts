import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoadingComponent } from './loading/loading.component';
@Injectable({
  providedIn: 'root'
})
export class PopupService {

  constructor(private snackBar: MatSnackBar ) { }
  openSnackBar(message: string, action: string = 'Ok') {
    this.snackBar.open(message, action, {
      duration: 5000,
      panelClass: 'my-custom-snackbar'
    });
  }

  loadingSnackBar() {
    this.snackBar.openFromComponent(LoadingComponent, {
      duration: 100000, 
      panelClass: ['loading-snackbar'],

    });
  }

  closeSnackBar() {
    this.snackBar.dismiss();
  }
}
