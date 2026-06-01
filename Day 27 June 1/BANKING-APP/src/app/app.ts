import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Customers } from './customers/customers';
import { ProductCard } from './product-card/product-card';
@Component({
  selector: 'app-root',
  imports: [Customers, ProductCard],
  // imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('BANKING-APP');
}
