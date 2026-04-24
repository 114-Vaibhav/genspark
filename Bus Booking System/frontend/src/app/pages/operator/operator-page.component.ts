import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { Bus, RouteItem, OperatorBooking, TripPassenger } from '../../core/models';

@Component({
  selector: 'app-operator-page',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './operator-page.component.html'
})
export class OperatorPageComponent implements OnInit {
  activeTab: 'management' | 'bookings' | 'passengers' = 'management';
  private readonly formBuilder = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly apiService = inject(ApiService);

  routes: RouteItem[] = [];
  buses: Bus[] = [];
  latestLayoutId = '';
  feedback = '';
  error = '';
  
  operatorBookings: OperatorBooking[] = [];
  tripPassengers: TripPassenger[] = [];
  selectedTripIdForPassengers = '';

  readonly layoutForm = this.formBuilder.group({
    name: ['', Validators.required],
    totalSeats: [40, [Validators.required, Validators.min(1)]],
    busType: ['Seater', Validators.required],
    configuration: ['2+2', Validators.required]
  });

  readonly busForm = this.formBuilder.group({
    busNumber: ['', Validators.required],
    totalSeats: [40, [Validators.required, Validators.min(1)]],
    layoutId: ['', Validators.required],
    isActive: [true, Validators.required]
  });

  readonly tripForm = this.formBuilder.group({
    busId: ['', Validators.required],
    routeId: ['', Validators.required],
    journeyDate: ['', Validators.required],
    departureTime: ['08:00', Validators.required],
    arrivalTime: ['12:00', Validators.required],
    price: [500, [Validators.required, Validators.min(1)]],
    pickupAddress: ['', Validators.required],
    dropAddress: ['', Validators.required]
  });

  ngOnInit(): void {
    console.log('[OperatorPage] ngOnInit called');
    this.loadMeta();
  }

  createLayout(): void {
    console.log('[OperatorPage] createLayout called', this.layoutForm.getRawValue());
    if (this.layoutForm.invalid) {
      this.layoutForm.markAllAsTouched();
      return;
    }

    const { name, totalSeats, busType, configuration } = this.layoutForm.getRawValue();
    this.apiService.createLayout(name!, Number(totalSeats), busType!, configuration!).subscribe({
      next: (layout) => {
        console.log('[OperatorPage] createLayout success', layout);
        this.latestLayoutId = layout.id;
        this.busForm.patchValue({ layoutId: layout.id, totalSeats: Number(totalSeats) });
        this.feedback = `Layout created. Seats auto-generated.`;
        this.error = '';
      },
      error: (error) => {
        console.error('[OperatorPage] createLayout failed', error);
        this.error = error.error ?? 'Layout creation failed.';
      }
    });
  }

  toggleBus(busId: string): void {
    this.apiService.toggleBus(busId).subscribe({
      next: (res) => {
        this.feedback = `Bus is now ${res.isActive ? 'Active' : 'Unavailable and bookings cancelled'}`;
        this.error = '';
        this.loadMeta();
      },
      error: (err) => {
        this.error = err.error ?? 'Failed to toggle bus';
      }
    });
  }

  addBus(): void {
    console.log('[OperatorPage] addBus called', this.busForm.getRawValue());
    if (this.busForm.invalid) {
      this.busForm.markAllAsTouched();
      return;
    }

    const value = this.busForm.getRawValue();
    this.apiService.addBus({
      busNumber: value.busNumber!,
      totalSeats: Number(value.totalSeats),
      layoutId: value.layoutId!,
      isActive: !!value.isActive
    }).subscribe({
      next: () => {
        console.log('[OperatorPage] addBus success');
        this.feedback = 'Bus added successfully.';
        this.error = '';
        this.busForm.patchValue({ busNumber: '' });
        this.loadMeta();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('[OperatorPage] addBus failed', error);
        this.error = error.error ?? 'Adding bus failed. Make sure the operator is approved.';
        this.cdr.detectChanges();
      }
    });
  }

  createTrip(): void {
    console.log('[OperatorPage] createTrip called', this.tripForm.getRawValue());
    if (this.tripForm.invalid) {
      this.tripForm.markAllAsTouched();
      return;
    }

    const value = this.tripForm.getRawValue();
    this.apiService.createTrip({
      busId: value.busId!,
      routeId: value.routeId!,
      journeyDate: value.journeyDate!,
      departureTime: value.departureTime!,
      arrivalTime: value.arrivalTime!,
      price: Number(value.price),
      pickupAddress: value.pickupAddress!,
      dropAddress: value.dropAddress!
    }).subscribe({
      next: () => {
        console.log('[OperatorPage] createTrip success');
        this.feedback = 'Trip created successfully.';
        this.error = '';
      },
      error: (error) => {
        console.error('[OperatorPage] createTrip failed', error);
        this.error = error.error ?? 'Trip creation failed.';
      }
    });
  }

  private loadMeta(): void {
    console.log('[OperatorPage] loadMeta called');
    this.apiService.getRoutes().subscribe({
      next: (routes) => {
        console.log('[OperatorPage] routes loaded', { count: routes.length });
        this.routes = routes;
      }
    });

    this.apiService.getMyBuses().subscribe({
      next: (buses) => {
        console.log('[OperatorPage] my buses loaded', { count: buses.length });
        this.buses = buses;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[OperatorPage] failed to load buses', err);
      }
    });
  }

  loadBookings(): void {
    this.apiService.getOperatorBookings().subscribe({
      next: (bookings) => {
        this.operatorBookings = bookings;
      },
      error: (err) => console.error('Failed to load operator bookings', err)
    });
  }

  loadPassengers(): void {
    if (!this.selectedTripIdForPassengers) return;
    this.apiService.getTripPassengers(this.selectedTripIdForPassengers).subscribe({
      next: (passengers) => {
        this.tripPassengers = passengers;
      },
      error: (err) => console.error('Failed to load trip passengers', err)
    });
  }

  setTab(tab: 'management' | 'bookings' | 'passengers') {
    this.activeTab = tab;
    if (tab === 'bookings') {
      this.loadBookings();
    }
  }
}
