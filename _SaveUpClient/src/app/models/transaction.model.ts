import { Client } from "./client.model";
import { Currency } from "./currency.model";
import { TransactionType } from "./transaction-type.model";

export interface TransactionRequest {
  type: TransactionType;
  amount: number;
  currency: Currency;
  isDomestic: boolean;
  client: Client;
}
