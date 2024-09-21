import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppStatusService {
  isSignedIn: boolean = false
  isAdmin: boolean = false
  isMaster: boolean = false
  isUser: boolean = false
  constructor() { }
}
