import { Component } from '@angular/core';
import { AuthService } from '../_auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [
    CommonModule,
    RouterModule
  ],
  standalone: true,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  authenticated: boolean;

  constructor(private authService: AuthService, private http: HttpClient) {
    this.authenticated = this.authService.isLoggedIn();

  }

  login() {
    this.authService.login();
  }

  logout() {
    this.authService.logout();
  }
}
