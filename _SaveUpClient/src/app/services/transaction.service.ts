import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Client } from '../models/client.model';
import { TransactionType } from '../models/transaction-type.model';
import { Currency } from '../models/currency.model';
import { Observable } from 'rxjs';
import { TransactionRequest } from '../models/transaction.model';
import { TransactionResponse } from '../models/transaction-response.model';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
   private base = 'https://localhost:7141/api';

  constructor(private http: HttpClient) {}

  getClients(): Observable<Client[]> {
    return this.http.get<Client[]>(`${this.base}/Common/clients`);
  }

  getTypes(): Observable<TransactionType[]> {
    return this.http.get<TransactionType[]>(`${this.base}/Common/type`);
  }

  getCurrencies(): Observable<Currency[]> {
    return this.http.get<Currency[]>(`${this.base}/Common/currency`);
  }

  calculateTransaction(tx: TransactionRequest): Observable<TransactionResponse> {
    return this.http.post<TransactionResponse>(`${this.base}/Transaction/calculate`, tx);
  }

  calculateBatch(requests: TransactionRequest[]): Observable<any> {
    return this.http.post(`${this.base}/Transaction/calculate-batch`, requests);
  }
}
