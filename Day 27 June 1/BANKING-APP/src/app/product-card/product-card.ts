import { Component } from '@angular/core';
import { ProductCardModel } from '../models/ProductCard.model';
// @Component({
//   selector: 'app-product-card',
//   imports: [],
//   templateUrl: './product-card.html',
//   styleUrl: './product-card.css',
// })

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.html',
  styleUrls: ['./product-card.css']
})
export class ProductCard{

  product: ProductCardModel = new ProductCardModel(
    'iPhone 15',
    79999,
    'Latest Apple smartphone with advanced camera features.',
    'https://m.media-amazon.com/images/I/71d7rfSl0wL._AC_UF1000,1000_QL80_.jpg'  );

}