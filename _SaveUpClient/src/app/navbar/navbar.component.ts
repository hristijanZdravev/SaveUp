import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../_auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../_services/theme.service';
import { Subject, takeUntil } from 'rxjs';
import { TPipe } from '../_pipes/t.pipe';
import { I18nService } from '../_services/i18n.service';

@Component({
  selector: 'app-navbar',
  imports: [
    CommonModule,
    RouterModule,
    TPipe
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
    private themeService: ThemeService,
    public i18n: I18nService
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
        const fallbackUser = this.i18n.t('nav.user.fallback');
        const rawName = this.authService.userName || fallbackUser;
        this.userName = rawName.split('@')[0] || fallbackUser;
      } catch (error) {
        this.userName = this.i18n.t('nav.user.fallback');
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
