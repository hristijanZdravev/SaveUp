import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_auth/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TPipe } from '../_pipes/t.pipe';

@Component({
  selector: 'app-main',
  imports: [CommonModule, TPipe],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent implements OnInit {
  currentYear = new Date().getFullYear();

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
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
