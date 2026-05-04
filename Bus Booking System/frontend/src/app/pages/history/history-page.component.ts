import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { ApiService } from '../../core/services/api.service';
import { Booking, TicketResponse } from '../../core/models';

@Component({
  selector: 'app-history-page',
  imports: [CommonModule],
  templateUrl: './history-page.component.html'
})
export class HistoryPageComponent implements OnInit {
  bookings: Booking[] = [];
  feedback = '';
  error = '';
  isLoading = true;

  private readonly cdr = inject(ChangeDetectorRef);
  constructor(private readonly apiService: ApiService) {}

  ngOnInit(): void {
    console.log('[HistoryPage] ngOnInit called');
    this.loadHistory();
  }

  cancelBooking(bookingId: string): void {
    console.log('[HistoryPage] cancelBooking called', { bookingId });
    this.feedback = '';
    this.error = '';

    this.apiService.cancelBooking(bookingId).subscribe({
      next: (response) => {
        console.log('[HistoryPage] cancelBooking success', response);
        this.feedback = `Booking cancelled. Refund amount: INR ${response.refund}.`;
        this.loadHistory();
      },
      error: (error) => {
        console.error('[HistoryPage] cancelBooking failed', error);
        this.error = error.error ?? 'Cancellation failed.';
        this.cdr.detectChanges();
      }
    });
  }

  downloadTicket(bookingId: string): void {
    console.log('[HistoryPage] downloadTicket called', { bookingId });
    this.apiService.getTicket(bookingId).subscribe({
      next: (ticket) => this.saveTicket(ticket),
      error: (error) => {
        console.error('[HistoryPage] downloadTicket failed', error);
        this.error = error.error ?? 'Ticket download failed.';
        this.cdr.detectChanges();
      }
    });
  }

  private loadHistory(): void {
    console.log('[HistoryPage] loadHistory called');
    this.isLoading = true;
    this.apiService.getHistory().subscribe({
      next: (bookings) => {
        console.log('[HistoryPage] loadHistory success', { count: bookings.length });
        this.bookings = bookings;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('[HistoryPage] loadHistory failed', error);
        this.error = error.error ?? 'Unable to load booking history.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private saveTicket(ticket: TicketResponse): void {
    console.log('[HistoryPage] saveTicket called', { bookingId: ticket.bookingId });
    const content = [
      'Bus Booking Ticket',
      `Booking Id: ${ticket.bookingId}`,
      `Journey Date: ${new Date(ticket.journeyDate).toLocaleDateString()}`,
      `Pickup: ${ticket.pickupAddress}`,
      `Drop: ${ticket.dropAddress}`,
      `Seats: ${ticket.seats.join(', ')}`,
      `Status: ${ticket.status}`,
      `Amount: ${ticket.amount}`
    ].join('\n');

    const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = `ticket-${ticket.bookingId}.txt`;
    anchor.click();
    URL.revokeObjectURL(url);
  }
}
