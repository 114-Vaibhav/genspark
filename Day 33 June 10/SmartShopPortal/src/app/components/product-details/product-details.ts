import { Component, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ProductModel } from '../../models/product.model';

@Component({
  selector: 'app-product-details',
  imports: [],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails {

  product = signal<ProductModel | null>(null);

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {

    const productId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.productService
      .getProductDetailsFromDummyJson(productId)
      .subscribe({
        next: (response: any) => {

          this.product.set(response);

        },
        error: (error) => {

          console.error(error);

        }
      });
  }
}