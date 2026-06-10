import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, tap, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) {}

  public getProductsFromDummyJson() {
    return this.http.get<any>('https://dummyjson.com/products').pipe(

      tap(() => {
        console.log('Products API called successfully');
      }),

      map(response => response),

      catchError(error => {
        console.error('Error fetching products:', error);
        return throwError(() => error);
      })

    );
  }

  public getProductDetailsFromDummyJson(productId: number) {
    return this.http
      .get<any>(`https://dummyjson.com/products/${productId}`)
      .pipe(

        tap(() => {
          console.log(`Product ${productId} fetched successfully`);
        }),

        map(response => response),

        catchError(error => {
          console.error('Error fetching product details:', error);
          return throwError(() => error);
        })

      );
  }
}