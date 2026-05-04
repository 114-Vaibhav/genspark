import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { PendingOperator, RevenueRow, RouteItem } from '../../core/models';

@Component({
  selector: 'app-admin-page',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './admin-page.component.html'
})
export class AdminPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(ApiService);

  revenueRows: RevenueRow[] = [];
  pendingOperators: PendingOperator[] = [];
  routesList: RouteItem[] = [];
  feedback = '';
  error = '';
  isLoading = true;
  platformFeePercentage = 10;

  readonly approvalForm = this.formBuilder.group({
    operatorId: ['', Validators.required]
  });

  readonly routeForm = this.formBuilder.group({
    source: ['', Validators.required],
    destination: ['', Validators.required],
    distance: [0, [Validators.required, Validators.min(1)]],
    pickupPoints: ['', Validators.required],
    dropPoints: ['', Validators.required]
  });

  ngOnInit(): void {
    console.log('[AdminPage] ngOnInit called');
    this.loadPendingOperators();
    this.loadRevenue();
    this.loadRoutes();
    this.loadPlatformFee();
  }

  loadPlatformFee(): void {
    this.apiService.getPlatformFee().subscribe({
      next: (res) => this.platformFeePercentage = res.platformFeePercentage,
      error: (err) => console.error('Failed to load platform fee', err)
    });
  }

  updatePlatformFee(): void {
    this.apiService.updatePlatformFee(this.platformFeePercentage).subscribe({
      next: (res) => {
        this.feedback = res.message;
        this.error = '';
      },
      error: (err) => this.error = err.error ?? 'Failed to update platform fee'
    });
  }

  loadRoutes(): void {
    this.apiService.getRoutes().subscribe({
      next: (routes) => this.routesList = routes,
      error: (err) => console.error('Failed to load routes', err)
    });
  }

  approveOperator(operatorId?: string): void {
    console.log('[AdminPage] approveOperator called', { operatorId });
    const resolvedOperatorId = operatorId || this.approvalForm.getRawValue().operatorId;

    if (!resolvedOperatorId) {
      this.approvalForm.markAllAsTouched();
      return;
    }

    this.apiService.approveOperator(resolvedOperatorId).subscribe({
      next: (message) => {
        console.log('[AdminPage] approveOperator success', { message });
        this.feedback = message;
        this.error = '';
        this.approvalForm.reset();
        this.loadPendingOperators();
      },
      error: (error) => {
        console.error('[AdminPage] approveOperator failed', error);
        this.error = error.error ?? 'Approval failed.';
      }
    });
  }

  rejectOperator(operatorId: string): void {
    this.apiService.rejectOperator(operatorId).subscribe({
      next: (msg) => {
        this.feedback = msg;
        this.loadPendingOperators();
      },
      error: (err) => this.error = err.error ?? 'Rejection failed.'
    });
  }

  toggleRoute(routeId: string): void {
    this.apiService.toggleRoute(routeId).subscribe({
      next: (msg) => {
        this.feedback = msg;
        this.loadRoutes();
      },
      error: (err) => this.error = err.error ?? 'Toggle route failed.'
    });
  }

  createRoute(): void {
    console.log('[AdminPage] createRoute called', this.routeForm.getRawValue());
    if (this.routeForm.invalid) {
      this.routeForm.markAllAsTouched();
      return;
    }

    const value = this.routeForm.getRawValue();
    const pickupArr = value.pickupPoints?.split(',').map(s => s.trim()).filter(s => s) || [];
    const dropArr = value.dropPoints?.split(',').map(s => s.trim()).filter(s => s) || [];

    this.apiService.addRoute({
      source: value.source!,
      destination: value.destination!,
      distance: Number(value.distance),
      pickupPoints: pickupArr,
      dropPoints: dropArr
    }).subscribe({
      next: () => {
        console.log('[AdminPage] createRoute success');
        this.feedback = 'Route added successfully.';
        this.error = '';
        this.routeForm.reset({ source: '', destination: '', distance: 0, pickupPoints: '', dropPoints: '' });
        this.loadRoutes();
      },
      
      error: (err) => {
  console.error('[AdminPage] createRoute failed', err);

  if (err.status === 400 ) {
    this.error = err.error; // "Route already exists"
    this.feedback = '';
  } else {
    this.error = 'Route creation failed.';
    this.feedback = '';
  }
}

   


    });
  }

  loadRevenue(): void {
    console.log('[AdminPage] loadRevenue called');
    this.isLoading = true;
    this.apiService.getRevenue().subscribe({
      next: (revenueRows) => {
        console.log('[AdminPage] loadRevenue success', { count: revenueRows.length });
        this.revenueRows = revenueRows;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('[AdminPage] loadRevenue failed', error);
        this.error = error.error ?? 'Unable to load revenue.';
        this.isLoading = false;
      }
    });
  }

  private loadPendingOperators(): void {
    console.log('[AdminPage] loadPendingOperators called');
    this.apiService.getPendingOperators().subscribe({
      next: (operators) => {
        console.log('[AdminPage] loadPendingOperators success', { count: operators.length });
        this.pendingOperators = operators;
      },
      error: (error) => {
        console.error('[AdminPage] loadPendingOperators failed', error);
        this.error = error.error ?? 'Unable to load pending operators.';
      }
    });
  }
}
