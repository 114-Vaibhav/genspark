import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { RouteItem, SearchResult } from '../../core/models';

@Component({
  selector: 'app-home-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './home-page.component.html'
})
export class HomePageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  readonly authService = inject(AuthService);
  private readonly apiService = inject(ApiService);
  private readonly router = inject(Router);

  readonly searchForm = this.formBuilder.group({
    source: ['', Validators.required],
    destination: ['', Validators.required],
    date: ['', Validators.required],
    isSingleLady: [false]
  });

  routes: RouteItem[] = [];
  cityOptions: string[] = [];
  results: SearchResult[] = [];
  hasSearched = false;
  isSearching = false;
  feedback = '';
  error = '';

  ngOnInit(): void {
    console.log('[HomePage] ngOnInit called');
    this.apiService.getRoutes().subscribe({
      next: (routes) => {
        console.log('[HomePage] routes loaded', { count: routes.length });
        this.routes = routes;
        this.cityOptions = [...new Set(routes.flatMap((route) => [route.source, route.destination]))].sort();
      }
    });
  }

  searchTrips(): void {
    console.log('[HomePage] searchTrips called', this.searchForm.getRawValue());
    if (this.searchForm.invalid) {
      console.warn('[HomePage] search form invalid', this.searchForm.getRawValue());
      this.searchForm.markAllAsTouched();
      return;
    }

    const { source, destination, date } = this.searchForm.getRawValue();
    this.isSearching = true;
    this.hasSearched = true;
    this.error = '';
    this.feedback = '';
    this.results = [];

    this.apiService.searchTrips(source!, destination!, date!).subscribe({
      next: (results) => {
        console.log('[HomePage] searchTrips success', { count: results.length, results });
        this.results = results;
        console.log('[HomePage] mapped trip ids', results.map((item) => item.trip.id));
        console.log('[HomePage] results array after assignment', { length: this.results.length, data: this.results });
        this.feedback = results.length
          ? `${results.length} trip option(s) found for ${source} to ${destination}.`
          : 'No trips matched this route and date yet.';
        this.isSearching = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('[HomePage] searchTrips failed', error);
        this.error = error.error ?? 'Unable to search trips right now.';
        this.isSearching = false;
        this.cdr.markForCheck();
      }
    });
  }

  continueToBooking(result: SearchResult): void {
    console.log('[HomePage] continueToBooking called', { tripId: result.trip.id, loggedIn: this.authService.isLoggedIn() });
    this.apiService.rememberTrip(result);

    const isSingleLady = this.searchForm.value.isSingleLady;

    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/booking', result.trip.id], { queryParams: { singleLady: isSingleLady } });
      return;
    }

    this.router.navigate(['/auth'], {
      queryParams: { redirect: `/booking/${result.trip.id}`, singleLady: isSingleLady }
    });
  }

  formatTime(value?: string): string {
    return value && value.length >= 5 ? value.slice(0, 5) : '--:--';
  }

  trackByTripId(_: number, result: SearchResult): string {
    return result.trip.id || `${result.route.id}-${result.bus.id}-${result.trip.journeyDate}`;
  }
}
