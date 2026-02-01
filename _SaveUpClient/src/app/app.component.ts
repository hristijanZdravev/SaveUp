import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Client } from './models/client.model';
import { TransactionType } from './models/transaction-type.model';
import { Currency } from './models/currency.model';
import { HttpClient } from '@angular/common/http';
import { TransactionRequest } from './models/transaction.model';
import { FormsModule } from '@angular/forms';
import { TransactionService } from './services/transaction.service';
import { CommonModule } from '@angular/common';
import { TransactionResponse } from './models/transaction-response.model';

@Component({
  selector: 'app-root',
  imports: [ FormsModule, CommonModule ],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  clients: Client[] = [];
  types: TransactionType[] = [];
  currencies: Currency[] = [];
  response?: TransactionResponse;


  request: TransactionRequest = {
    type: { id: 0, name: '' },
    amount: 0,
    currency: { id: 0, name: '' },
    isDomestic: false,
    client: { id: 0, creditScore: 0, segment: { id: 0, name: '' }, riskLevel: 0 }
  };

  constructor(private http: HttpClient, private transactionService: TransactionService) {}

  ngOnInit(): void {
  this.loadOptions();
  }

  loadOptions() {
    this.transactionService.getClients().subscribe(c => this.clients = c);
    this.transactionService.getTypes().subscribe(t => this.types = t);
    this.transactionService.getCurrencies().subscribe(cu => this.currencies = cu);
  }

  submitTransaction() {
  this.transactionService.calculateTransaction(this.request)
    .subscribe({
      next: res => {
        this.response = res;
      },
      error: () => alert('❌ Something went wrong.')
    });
  }

  compareById(o1: any, o2: any): boolean {
    return o1 && o2 ? o1.id === o2.id : o1 === o2;
  }
}
