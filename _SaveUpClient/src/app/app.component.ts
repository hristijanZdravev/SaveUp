import { Component, OnInit } from '@angular/core';
import { RouterModule, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './navbar/navbar.component';
import { AuthService } from './_auth/auth.service';
import { ThemeService } from './_services/theme.service';

@Component({
  selector: 'app-root',
    imports:[
    CommonModule,
    RouterModule,
    NavbarComponent
  ],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
   title = 'my-app';
   showLandingPage = false;
   currentYear = new Date().getFullYear();

   constructor(
     private router: Router,
     private authService: AuthService,
     private themeService: ThemeService
   ) {}

   ngOnInit() {
     // Initialize theme service (applies saved theme preference)
     // Theme service is already initialized in its constructor, but we ensure it's loaded
     
     // Show landing page only if user is not authenticated and not on a protected route
     this.showLandingPage = !this.authService.isLoggedIn();
   }

   navigateToApp() {
     if (this.authService.isLoggedIn() && this.authService.isUser()) {
       this.router.navigate(['/dashboard']);
     } else {
       this.authService.login();
     }
   }

   scrollToFeatures() {
     const featuresElement = document.getElementById('features');
     if (featuresElement) {
       featuresElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
     }
   }
}
