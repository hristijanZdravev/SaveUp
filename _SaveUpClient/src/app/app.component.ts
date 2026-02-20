import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './navbar/navbar.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterModule,
    NavbarComponent
  ],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'PeakLift';

  ngOnInit(): void {
    document.title = this.title;
    this.setFavicon('assets/logo.png?v=3');
  }

  private setFavicon(href: string): void {
    this.setOrCreateIconLink('icon', href, 'image/png');
    this.setOrCreateIconLink('shortcut icon', href, 'image/png');
    this.setOrCreateIconLink('apple-touch-icon', href, 'image/png');
  }

  private setOrCreateIconLink(rel: string, href: string, type: string): void {
    let link = document.querySelector<HTMLLinkElement>(`link[rel='${rel}']`);
    if (!link) {
      link = document.createElement('link');
      link.setAttribute('rel', rel);
      document.head.appendChild(link);
    }
    link.setAttribute('href', href);
    link.setAttribute('type', type);
  }
}
