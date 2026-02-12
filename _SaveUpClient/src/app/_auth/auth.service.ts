import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private readonly keycloak: KeycloakService) {}

  isLoggedIn(): boolean {
    return this.keycloak.isLoggedIn();
  }

  login() : void {
      console.log(this.keycloak);
    this.keycloak.login();
  }

  logout(): void {
    this.keycloak.logout();
  }

  get userName(): string {
    return this.keycloak.getUsername();
  }
   
  getUserRoles(): any[] {
    return this.keycloak.getUserRoles()
  }
}
