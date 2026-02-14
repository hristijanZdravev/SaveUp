import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../_auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../_services/theme.service';
import { Subject, takeUntil } from 'rxjs';

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
export class NavbarComponent implements OnInit, OnDestroy {
  authenticated: boolean = false;
  userName: string = '';
  isDarkMode: boolean = true;
  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService, 
    private http: HttpClient,
    private themeService: ThemeService
  ) {
    this.updateAuthState();
  }

  ngOnInit() {
    // Update auth state on component init
    this.updateAuthState();
    
    // Subscribe to theme changes
    this.themeService.darkMode$
      .pipe(takeUntil(this.destroy$))
      .subscribe(isDark => {
        this.isDarkMode = isDark;
      });
    
    // Periodically check auth state (every 2 seconds) to handle Keycloak SSO
    setInterval(() => {
      this.updateAuthState();
    }, 2000);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateAuthState() {
    this.authenticated = this.authService.isLoggedIn();
    if (this.authenticated) {
      try {
        const rawName = this.authService.userName || 'User';
        this.userName = rawName.split('@')[0] || 'User';
      } catch (error) {
        this.userName = 'User';
      }
    } else {
      this.userName = '';
    }
  }

  login() {
    this.authService.login();
  }

  register() {
    this.authService.register();
  }
  goToProfile() {
    this.authService.goToProfile();
  }
  logout() {
    this.authService.logout();
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
