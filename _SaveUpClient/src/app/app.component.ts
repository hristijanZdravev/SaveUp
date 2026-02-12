import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Client } from './models/client.model';
import { TransactionType } from './models/transaction-type.model';
import { Currency } from './models/currency.model';
import { HttpClient } from '@angular/common/http';
import { TransactionRequest } from './models/transaction.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TransactionResponse } from './models/transaction-response.model';
import { NavbarComponent } from './navbar/navbar.component';

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
export class AppComponent {
   title = 'my-app';
}
