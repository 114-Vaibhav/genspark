import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductModel } from '../../models/product.model';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-products',
  imports: [RouterLink],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products {

  products = signal<ProductModel[]>([]);
  cart = signal<ProductModel[]>([]);

  constructor(private productService: ProductService) {

    this.productService.getProductsFromDummyJson()
      .subscribe({
        next: (response: any) => {

          this.products.set(response.products);

        },
        error: (error) => {

          console.error(error);

        }
      });
  }
}