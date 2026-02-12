import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_auth/auth.service';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-main',
  imports: [CommonModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent implements OnInit {

  constructor(public authService: AuthService, private http: HttpClient) {
  }

  ngOnInit() {
  
  }
}
