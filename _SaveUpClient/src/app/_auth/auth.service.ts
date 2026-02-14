import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private readonly keycloak: KeycloakService, private readonly router: Router) {}

  isLoggedIn(): boolean {
    return this.keycloak.isLoggedIn();
  }

  login() : void {
    this.keycloak.login({
      redirectUri: `${window.location.origin}/dashboard`
    });
  }

  register() : void {
    this.keycloak.register();
  }

  goToProfile() : void {
    window.location.href =`${environment.keycloakUrl}/realms/${environment.keycloakRealm}/account`;
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

  isAdmin(): boolean {
    const roles = this.getUserRoles();
    return roles.includes('admin');
  }

  isUser(): boolean {
    const roles = this.getUserRoles();
    return roles.includes('user');
  }
}
