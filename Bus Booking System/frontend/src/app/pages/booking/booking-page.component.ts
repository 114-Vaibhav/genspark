import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';

import { ApiService } from '../../core/services/api.service';
import { Booking, SearchResult, Seat, TicketResponse } from '../../core/models';

@Component({
  selector: 'app-booking-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './booking-page.component.html'
})
export class BookingPageComponent implements OnInit {

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);

  travelersForm = this.fb.group({
    travelers: this.fb.array<FormGroup>([])
  });

  context: SearchResult | null = null;
  seatMatrix: (Seat | null)[][] = [];
  selectedSeats: string[] = [];

  booking: Booking | null = null;
  ticket: TicketResponse | null = null;

  feedback = '';
  error = '';
  isLoading = true;
  isSubmitting = false;
  seatsLocked = false;
  platformFeePercentage = 10; // Default, will be fetched from backend

  get travelers(): FormArray<FormGroup> {
    return this.travelersForm.get('travelers') as FormArray<FormGroup>;
  }

  isSingleLady = false;

  ngOnInit(): void {
    const tripId = this.route.snapshot.paramMap.get('tripId');

    if (!tripId) {
      this.error = 'Trip not found';
      this.isLoading = false;
      return;
    }

    const singleLadyParam = this.route.snapshot.queryParamMap.get('singleLady');
    this.isSingleLady = singleLadyParam === 'true';

    // Fetch platform fee from backend
    this.api.getPlatformFee().subscribe({
      next: (fee) => {
        this.platformFeePercentage = fee.platformFeePercentage;
      },
      error: () => {
        // Use default if fetch fails
        this.platformFeePercentage = 10;
      }
    });

    this.api.getTripDetails(tripId).pipe(
      switchMap((ctx) => {

        this.context = {
          ...ctx,
          bookedSeatNumbers: ctx.bookedSeatNumbers || [],
          lockedSeatNumbers: ctx.lockedSeatNumbers || [],
          femaleBookedSeatNumbers: ctx.femaleBookedSeatNumbers || []
        };

        return this.api.getLayout(ctx.bus!.layoutId);
      })
    ).subscribe({
      next: (seats) => {
        this.buildSeatMatrix(seats);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = err?.message || 'Error loading data';
        this.isLoading = false;
      }
    });
  }

  buildSeatMatrix(seats: Seat[]) {
    const maxRow = Math.max(...seats.map(s => s.row), 0);
    const maxCol = Math.max(...seats.map(s => s.column), 0);

    this.seatMatrix = Array.from({ length: maxRow + 1 }, (_, r) =>
      Array.from({ length: maxCol + 1 }, (_, c) =>
        seats.find(s => s.row === r && s.column === c) || null
      )
    );
  }

  toggleSeat(seat: Seat) {
    if (this.seatsLocked || this.booking || this.isSeatUnavailable(seat.seatNumber)) return;

    const exists = this.selectedSeats.includes(seat.seatNumber);

    this.selectedSeats = exists
      ? this.selectedSeats.filter(s => s !== seat.seatNumber)
      : [...this.selectedSeats, seat.seatNumber];

    this.syncTravelers();
    this.cdr.detectChanges();
  }

  isSelected(seat: string) {
    return this.selectedSeats.includes(seat);
  }

  isSeatUnavailable(seat: string) {
    return !!this.context &&
      (this.context.bookedSeatNumbers.includes(seat) ||
       this.context.lockedSeatNumbers.includes(seat));
  }

  seatTone(seat: string) {
    if (this.isSingleLady && this.context?.femaleBookedSeatNumbers.includes(seat)) {
      return 'bg-pink-100 border-pink-300 text-pink-700 opacity-90 cursor-not-allowed';
    }
    
    if (this.isSeatUnavailable(seat)) {
      return 'bg-slate-200 border-slate-300 text-slate-400 opacity-70 cursor-not-allowed';
    }
    
    if (this.isSelected(seat)) {
      return 'bg-emerald-500 border-emerald-600 text-white shadow-md';
    }
    
    const traveler = this.travelers.controls.find(g => g.get('seatNumber')?.value === seat);
    if (traveler && traveler.get('gender')?.value === 'Female') {
      return 'bg-pink-100 border-pink-300 text-pink-700 shadow-md';
    }

    return 'bg-white border-slate-200 text-slate-700 hover:border-slate-400 hover:bg-slate-50';
  }

  syncTravelers() {
    const set = new Set(this.selectedSeats);

    for (let i = this.travelers.length - 1; i >= 0; i--) {
      if (!set.has(this.travelers.at(i).value.seatNumber)) {
        this.travelers.removeAt(i);
      }
    }

    this.selectedSeats.forEach(seat => {
      if (!this.travelers.controls.some(t => t.value.seatNumber === seat)) {
        this.travelers.push(this.fb.group({
          seatNumber: [seat],
          name: ['', Validators.required],
          age: [18, Validators.required],
          gender: [this.isSingleLady ? 'Female' : 'Male']
        }));
      }
    });
  }

  lockSeats() {
    if (!this.selectedSeats.length) return;
    
    this.isSubmitting = true;
    this.error = '';
    this.feedback = '';

    this.api.lockSeats(this.context!.trip.id, this.selectedSeats).subscribe({
      next: () => {
        this.seatsLocked = true;
        this.feedback = 'Seats locked securely. Please proceed to payment.';
        this.isSubmitting = false;
      },
      error: (err) => {
        this.error = err.error ?? 'Failed to lock seats.';
        this.isSubmitting = false;
      }
    });
  }

  createBooking() {
    if (!this.seatsLocked || this.travelersForm.invalid) return;

    // Validate single lady bookings
    if (this.isSingleLady) {
      const nonFemale = this.travelers.getRawValue().find((t: any) => t['gender'] !== 'Female');
      if (nonFemale) {
        this.error = 'Single lady booking requires all travelers to be female.';
        return;
      }
    }

    this.isSubmitting = true;
    this.error = '';
    this.feedback = '';

    this.api.createBooking({
      tripId: this.context!.trip.id,
      isSingleLady: this.isSingleLady,
      travelers: this.travelers.getRawValue().map(t => ({
        name: t['name'],
        age: Number(t['age']),
        gender: t['gender'],
        seatNumber: t['seatNumber']
      }))
    }).subscribe({
      next: (b) => {
        this.booking = b;
        this.feedback = 'Booking created successfully. Complete your payment below.';
        this.isSubmitting = false;
      },
      error: (err) => {
        this.error = err.error ?? 'Failed to create booking.';
        this.isSubmitting = false;
      }
    });
  }

  payNow() {
    if (!this.booking) return;

    this.isSubmitting = true;
    this.error = '';

    this.api.payForBooking(this.booking.id).subscribe({
      next: () => {
        this.api.getTicket(this.booking!.id).subscribe({
          next: (t) => {
            this.ticket = t;
            this.isSubmitting = false;
            this.feedback = 'Payment successful! Ticket generated.';
          },
          error: () => {
            this.error = 'Payment succeeded but failed to fetch ticket.';
            this.isSubmitting = false;
          }
        });
      },
      error: (err) => {
        this.error = err.error ?? 'Payment failed.';
        this.isSubmitting = false;
      }
    });
  }

  downloadTicket() {
    if (!this.ticket || !this.context) return;

    const content = [
      '==================================',
      '        BUS BOOKING TICKET        ',
      '==================================',
      `Booking ID: ${this.ticket.bookingId}`,
      `Date      : ${new Date(this.ticket.journeyDate).toLocaleDateString()}`,
      `Route     : ${this.context.route?.source} -> ${this.context.route?.destination}`,
      `Bus No    : ${this.context.bus?.busNumber}`,
      `Pickup    : ${this.ticket.pickupAddress}`,
      `Drop      : ${this.ticket.dropAddress}`,
      `Seats     : ${this.ticket.seats.join(', ')}`,
      `Amount    : $${this.ticket.amount}`,
      `Status    : ${this.ticket.status}`,
      '==================================',
      'Thank you for traveling with us!'
    ].join('\n');

    const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `Ticket_${this.ticket.bookingId.substring(0, 8)}.txt`;
    a.click();
    URL.revokeObjectURL(url);
  }
}
// import { CommonModule } from '@angular/common';
// import { Component, OnInit, inject } from '@angular/core';
// import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
// import { ActivatedRoute } from '@angular/router';
// import { ApiService } from '../../core/services/api.service';
// import { Booking, SearchResult, Seat, TicketResponse } from '../../core/models';
// import { switchMap } from 'rxjs/operators';
// import { ChangeDetectorRef } from '@angular/core';



// @Component({
//   selector: 'app-booking-page',
//   imports: [CommonModule, ReactiveFormsModule],
//   templateUrl: './booking-page.component.html'
// })
// export class BookingPageComponent implements OnInit {
//   private readonly formBuilder = inject(FormBuilder);
//   private readonly route = inject(ActivatedRoute);
//   private readonly apiService = inject(ApiService);
//   private cdr = inject(ChangeDetectorRef);

//   readonly travelersForm = this.formBuilder.group({
//     travelers: this.formBuilder.array<FormGroup>([])
//   });

//   context: SearchResult | null = null;
//   seatMatrix: (Seat | null)[][] = [];
//   selectedSeats: string[] = [];
//   booking: Booking | null = null;
//   ticket: TicketResponse | null = null;
//   feedback = '';
//   error = '';
//   isLoading = true;
//   isSubmitting = false;
//   seatsLocked = false;

//   get travelers(): FormArray<FormGroup> {
//     return this.travelersForm.controls.travelers;
//   }

//   // ngOnInit(): void {
//   //   console.log('[BookingPage] ngOnInit called');
//   //   const tripId = this.route.snapshot.paramMap.get('tripId');

//   //   if (!tripId) {
//   //     this.error = 'Trip context is missing.';
//   //     this.isLoading = false;
//   //     return;
//   //   }

//   //   this.apiService.getTripDetails(tripId).subscribe({
//   //     next: (context) => {
//   //       console.log('[BookingPage] trip details loaded', { tripId: context.trip.id });
//   //       this.context = context;
//   //       this.apiService.getLayout(context.bus!.layoutId).subscribe({
//   //         next: (seats) => {
//   //           console.log('[BookingPage] layout loaded', { seats: seats.length });
//   //           this.buildSeatMatrix(seats);
//   //           this.isLoading = false;
//   //           if (!seats.length) {
//   //             this.feedback = 'This bus has no uploaded seat layout yet. Ask the operator to add seats first.';
//   //           }
//   //         },
//   //         error: (error) => {
//   //           console.error('[BookingPage] getLayout failed', error);
//   //           this.error = error.error ?? 'Unable to load seat layout.';
//   //           this.isLoading = false;
//   //         }
//   //       });
//   //     },
//   //     error: (error) => {
//   //       console.error('[BookingPage] getTripDetails failed', error);
//   //       this.error = error.error ?? 'Trip details were not found.';
//   //       this.isLoading = false;
//   //     }
//   //   });
//   // }

  

// ngOnInit(): void {
//   console.log('[BookingPage] ngOnInit called');

//   const tripId = this.route.snapshot.paramMap.get('tripId');

//   if (!tripId) {
//     this.error = 'Trip context is missing.';
//     // this.isLoading = false;
//     this.isLoading = false;
//     this.cdr.detectChanges();
//     return;
//   }

//   this.apiService.getTripDetails(tripId).pipe(
//     switchMap((context) => {
//       console.log('[BookingPage] trip details loaded', context);

//       this.context = context;

//       return this.apiService.getLayout(context.bus!.layoutId);
//     })
//   ).subscribe({
//     next: (seats) => {
//       console.log('[BookingPage] layout loaded', seats);

//       this.buildSeatMatrix(seats);
//       this.isLoading = false;

//       if (!seats.length) {
//         this.feedback = 'No seat layout available.';
//       }
//     },
//     error: (error) => {
//       console.error('[BookingPage] error', error);
//       this.error = error.error ?? 'Something went wrong.';
//       this.isLoading = false;
//     }
//   });
// }

//   toggleSeat(seat: Seat): void {
//     console.log('[BookingPage] toggleSeat called', { seatNumber: seat.seatNumber });
//     if (this.seatsLocked || this.booking || this.isSeatUnavailable(seat.seatNumber)) {
//       console.warn('[BookingPage] toggleSeat blocked', { seatNumber: seat.seatNumber });
//       return;
//     }

//     const exists = this.selectedSeats.includes(seat.seatNumber);
//     this.selectedSeats = exists
//       ? this.selectedSeats.filter((seatNumber) => seatNumber !== seat.seatNumber)
//       : [...this.selectedSeats, seat.seatNumber];

//     this.syncTravelers();
//     this.feedback = '';
//     this.error = '';
//   }

//   isSelected(seatNumber: string): boolean {
//     console.log('[BookingPage] isSelected called', { seatNumber });
//     return this.selectedSeats.includes(seatNumber);
//   }

//   isSeatUnavailable(seatNumber: string): boolean {
//     console.log('[BookingPage] isSeatUnavailable called', { seatNumber });
//     return !!this.context && (
//       this.context.bookedSeatNumbers.includes(seatNumber) ||
//       this.context.lockedSeatNumbers.includes(seatNumber)
//     );
//   }

//   seatTone(seatNumber: string): string {
//     console.log('[BookingPage] seatTone called', { seatNumber });
//     if (this.context?.femaleBookedSeatNumbers.includes(seatNumber)) {
//       return 'border-pink-300 bg-pink-100 text-pink-700 opacity-90';
//     }

//     if (this.isSeatUnavailable(seatNumber)) {
//       return 'border-slate-200 bg-slate-200 text-slate-400 opacity-70';
//     }

//     if (!this.isSelected(seatNumber)) {
//       return 'border-slate-200 bg-white text-slate-700 hover:border-slate-900';
//     }

//     const traveler = this.travelers.controls.find((group) => group.get('seatNumber')?.value === seatNumber);
//     const gender = traveler?.get('gender')?.value;

//     if (gender === 'Female') {
//       return 'border-pink-300 bg-pink-100 text-pink-700';
//     }

//     return 'border-cyan-300 bg-cyan-100 text-cyan-800';
//   }

//   lockSeats(): void {
//     console.log('[BookingPage] lockSeats called', { selectedSeats: this.selectedSeats });
//     if (!this.selectedSeats.length) {
//       this.error = 'Select at least one seat before locking.';
//       return;
//     }

//     this.isSubmitting = true;
//     this.error = '';
//     this.feedback = '';

//     this.apiService.lockSeats(this.context!.trip.id, this.selectedSeats).subscribe({
//       next: (message) => {
//         console.log('[BookingPage] lockSeats success', { message });
//         this.seatsLocked = true;
//         this.feedback = `${message}. Complete booking within 5 minutes.`;
//         this.isSubmitting = false;
//       },
//       error: (error) => {
//         console.error('[BookingPage] lockSeats failed', error);
//         this.error = error.error ?? 'Unable to lock selected seats.';
//         this.isSubmitting = false;
//       }
//     });
//   }

//   createBooking(): void {
//     console.log('[BookingPage] createBooking called', { travelers: this.travelers.length });
//     if (this.travelersForm.invalid) {
//       this.travelersForm.markAllAsTouched();
//       return;
//     }

//     if (!this.seatsLocked) {
//       this.error = 'Lock seats before creating the booking.';
//       return;
//     }

//     this.isSubmitting = true;
//     this.error = '';
//     this.feedback = '';

//     this.apiService.createBooking({
//       tripId: this.context!.trip.id,
//       travelers: this.travelers.getRawValue().map((traveler) => ({
//         name: traveler['name'],
//         age: Number(traveler['age']),
//         gender: traveler['gender'],
//         seatNumber: traveler['seatNumber']
//       }))
//     }).subscribe({
//       next: (booking) => {
//         console.log('[BookingPage] createBooking success', booking);
//         this.booking = booking;
//         this.feedback = 'Booking created. Proceed with the dummy payment to confirm it.';
//         this.isSubmitting = false;
//       },
//       error: (error) => {
//         console.error('[BookingPage] createBooking failed', error);
//         this.error = error.error ?? 'Booking creation failed.';
//         this.isSubmitting = false;
//       }
//     });
//   }

//   payNow(): void {
//     console.log('[BookingPage] payNow called', { bookingId: this.booking?.id });
//     if (!this.booking) {
//       this.error = 'Create a booking before payment.';
//       return;
//     }

//     this.isSubmitting = true;
//     this.error = '';
//     this.feedback = '';

//     this.apiService.payForBooking(this.booking.id).subscribe({
//       next: (message) => {
//         console.log('[BookingPage] payNow success', { message });
//         this.feedback = `${message}. Ticket is ready for download.`;
//         this.apiService.getTicket(this.booking!.id).subscribe({
//           next: (ticket) => {
//             console.log('[BookingPage] ticket loaded', ticket);
//             this.ticket = ticket;
//             this.isSubmitting = false;
//           },
//           error: () => {
//             console.error('[BookingPage] getTicket after payment failed');
//             this.isSubmitting = false;
//           }
//         });
//       },
//       error: (error) => {
//         console.error('[BookingPage] payNow failed', error);
//         this.error = error.error ?? 'Payment failed.';
//         this.isSubmitting = false;
//       }
//     });
//   }

//   downloadTicket(): void {
//     console.log('[BookingPage] downloadTicket called', { bookingId: this.ticket?.bookingId });
//     if (!this.ticket || !this.context) {
//       return;
//     }

//     const content = [
//       'Bus Booking Ticket',
//       `Booking Id: ${this.ticket.bookingId}`,
//       `Route: ${this.context.route?.source} -> ${this.context.route?.destination}`,
//       `Bus Number: ${this.context.bus?.busNumber}`,
//       `Journey Date: ${new Date(this.ticket.journeyDate).toLocaleDateString()}`,
//       `Seats: ${this.ticket.seats.join(', ')}`,
//       `Status: ${this.ticket.status}`,
//       `Amount: ${this.ticket.amount}`
//     ].join('\n');

//     const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
//     const url = URL.createObjectURL(blob);
//     const anchor = document.createElement('a');
//     anchor.href = url;
//     anchor.download = `ticket-${this.ticket.bookingId}.txt`;
//     anchor.click();
//     URL.revokeObjectURL(url);
//   }

//   private syncTravelers(): void {
//     console.log('[BookingPage] syncTravelers called', { selectedSeats: this.selectedSeats });
//     const seatSet = new Set(this.selectedSeats);

//     for (let index = this.travelers.length - 1; index >= 0; index -= 1) {
//       const seatNumber = this.travelers.at(index).get('seatNumber')?.value;
//       if (!seatSet.has(seatNumber)) {
//         this.travelers.removeAt(index);
//       }
//     }

//     this.selectedSeats.forEach((seatNumber) => {
//       const exists = this.travelers.controls.some((group) => group.get('seatNumber')?.value === seatNumber);

//       if (!exists) {
//         this.travelers.push(
//           this.formBuilder.group({
//             seatNumber: [seatNumber, Validators.required],
//             name: ['', Validators.required],
//             age: [18, [Validators.required, Validators.min(1)]],
//             gender: ['Male', Validators.required]
//           })
//         );
//       }
//     });
//   }

//   private buildSeatMatrix(seats: Seat[]): void {
//     console.log('[BookingPage] buildSeatMatrix called', { seats: seats.length });
//     const maxRow = Math.max(...seats.map((seat) => seat.row), 0);
//     const maxColumn = Math.max(...seats.map((seat) => seat.column), 0);

//     this.seatMatrix = Array.from({ length: maxRow + 1 }, (_, row) =>
//       Array.from({ length: maxColumn + 1 }, (_, column) =>
//         seats.find((seat) => seat.row === row && seat.column === column) ?? null
//       )
//     );
//   }
// }
