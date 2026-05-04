import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import {
  Booking,
  BookingRequest,
  Bus,
  OperatorBooking,
  PendingOperator,
  RevenueRow,
  RouteItem,
  SearchResult,
  Seat,
  TicketResponse,
  Trip,
  TripPassenger,
  UpdateProfileRequest,
  UserProfile
} from '../models';

const API_BASE_URL = 'http://localhost:5299';
const SELECTED_TRIP_KEY = 'bus_booking_selected_trip';

interface TripApiResponse {
  id: string;
  busId: string;
  routeId: string;
  layoutId: string;
  busNumber: string;
  totalSeats: number;
  availableSeats: number;
  source: string;
  destination: string;
  distance: number;
  journeyDate: string;
  departureTime: string;
  arrivalTime: string;
  travelMinutes: number;
  price: number;
  bookedSeatNumbers: string[];
  lockedSeatNumbers: string[];
  femaleBookedSeatNumbers: string[];
}

type RawApiRecord = Record<string, unknown>;

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private readonly http: HttpClient) {}

  getRoutes(): Observable<RouteItem[]> {
    console.log('[ApiService] getRoutes called');
    return this.http.get<RouteItem[]>(`${API_BASE_URL}/routes`);
  }

  getBuses(): Observable<Bus[]> {
    console.log('[ApiService] getBuses called');
    return this.http.get<Bus[]>(`${API_BASE_URL}/bus`);
  }

  getMyBuses(): Observable<Bus[]> {
    console.log('[ApiService] getMyBuses called');
    return this.http.get<Bus[]>(`${API_BASE_URL}/bus/my-buses`);
  }

  searchTrips(source: string, destination: string, date: string): Observable<SearchResult[]> {
    console.log('[ApiService] searchTrips called', { source, destination, date });
    return this.http.get<TripApiResponse[]>(`${API_BASE_URL}/trip-search`, {
      params: { source, destination, date }
    }).pipe(map((results) => results.map((result) => this.mapTripResult(result))));
  }

  rememberTrip(result: SearchResult): void {
    console.log('[ApiService] rememberTrip called', { tripId: result.trip.id });
    localStorage.setItem(SELECTED_TRIP_KEY, JSON.stringify(result));
  }

  getRememberedTrip(tripId: string): SearchResult | null {
    console.log('[ApiService] getRememberedTrip called', { tripId });
    const raw = localStorage.getItem(SELECTED_TRIP_KEY);

    if (!raw) {
      return null;
    }

    try {
      const parsed = JSON.parse(raw) as SearchResult;
      return parsed.trip.id === tripId ? parsed : null;
    } catch {
      return null;
    }
  }

  getTripDetails(tripId: string): Observable<SearchResult> {
    console.log('[ApiService] getTripDetails called', { tripId });
    return this.http.get<TripApiResponse>(`${API_BASE_URL}/trip/${tripId}`).pipe(
      map((result) => this.mapTripResult(result))
    );
  }

  getLayout(layoutId: string): Observable<Seat[]> {
    console.log('[ApiService] getLayout called', { layoutId });
    return this.http.get<Seat[]>(`${API_BASE_URL}/layout/${layoutId}`);
  }

  lockSeats(tripId: string, seatNumbers: string[]): Observable<string> {
    console.log('[ApiService] lockSeats called', { tripId, seatNumbers });
    return this.http.post(`${API_BASE_URL}/booking/lock-seat`, { tripId, seatNumbers }, {
      responseType: 'text'
    });
  }

  createBooking(payload: BookingRequest): Observable<Booking> {
    console.log('[ApiService] createBooking called', { tripId: payload.tripId, travelers: payload.travelers.length });
    return this.http.post<Booking>(`${API_BASE_URL}/booking/create`, payload);
  }

  payForBooking(bookingId: string): Observable<string> {
    console.log('[ApiService] payForBooking called', { bookingId });
    return this.http.post(`${API_BASE_URL}/payment`, {}, {
      params: { bookingId },
      responseType: 'text'
    });
  }

  getTicket(bookingId: string): Observable<TicketResponse> {
    console.log('[ApiService] getTicket called', { bookingId });
    return this.http.get<RawApiRecord>(`${API_BASE_URL}/ticket/${bookingId}`).pipe(
      map((result) => this.mapTicketResponse(result))
    );
  }

  getHistory(): Observable<Booking[]> {
    console.log('[ApiService] getHistory called');
    return this.http.get<RawApiRecord[]>(`${API_BASE_URL}/history`).pipe(
      map((results) => results.map((result) => this.mapBooking(result)))
    );
  }

  cancelBooking(bookingId: string): Observable<{ refund: number }> {
    console.log('[ApiService] cancelBooking called', { bookingId });
    return this.http.post<{ refund: number }>(`${API_BASE_URL}/cancel/${bookingId}`, {});
  }

  createLayout(name: string, totalSeats: number, busType: string, configuration: string): Observable<{ id: string }> {
    console.log('[ApiService] createLayout called', { name, totalSeats, busType, configuration });
    return this.http.post<{ id: string }>(`${API_BASE_URL}/layout`, { name, totalSeats, busType, configuration });
  }

  addBus(payload: {
    busNumber: string;
    totalSeats: number;
    layoutId: string;
    isActive: boolean;
  }): Observable<Bus> {
    console.log('[ApiService] addBus called', payload);
    return this.http.post<Bus>(`${API_BASE_URL}/bus`, payload);
  }

  createTrip(payload: {
    busId: string;
    routeId: string;
    journeyDate: string;
    departureTime: string;
    arrivalTime: string;
    price: number;
    pickupAddress: string;
    dropAddress: string;
  }): Observable<Trip> {
    console.log('[ApiService] createTrip called', payload);
    return this.http.post<Trip>(`${API_BASE_URL}/trip`, payload);
  }

  toggleBus(busId: string): Observable<{ isActive: boolean }> {
    console.log('[ApiService] toggleBus called', busId);
    return this.http.put<{ isActive: boolean }>(`${API_BASE_URL}/bus/${busId}/toggle`, {});
  }



  getPlatformFee(): Observable<{ platformFeePercentage: number }> {
    return this.http.get<{ platformFeePercentage: number }>(`${API_BASE_URL}/admin/settings/platform-fee`);
  }

  updatePlatformFee(platformFeePercentage: number): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${API_BASE_URL}/admin/settings/platform-fee`, platformFeePercentage);
  }

  approveOperator(operatorId: string): Observable<string> {
    console.log('[ApiService] approveOperator called', { operatorId });
    return this.http.post(`${API_BASE_URL}/admin/approve-operator/${operatorId}`, {}, {
      responseType: 'text'
    });
  }

  getRevenue(): Observable<RevenueRow[]> {
    console.log('[ApiService] getRevenue called');
    return this.http.get<RawApiRecord[]>(`${API_BASE_URL}/admin/revenue`).pipe(
      map((results) => results.map((result) => this.mapRevenueRow(result)))
    );
  }

  getPendingOperators(): Observable<PendingOperator[]> {
    console.log('[ApiService] getPendingOperators called');
    return this.http.get<RawApiRecord[]>(`${API_BASE_URL}/admin/operators/pending`).pipe(
      map((results) => results.map((result) => this.mapPendingOperator(result)))
    );
  }

  addRoute(payload: { source: string; destination: string; distance: number; pickupPoints: string[]; dropPoints: string[] }): Observable<RouteItem> {
    console.log('[ApiService] addRoute called', payload);
    return this.http.post<RouteItem>(`${API_BASE_URL}/routes`, payload);
  }

  // Profile Endpoints
  getProfile(): Observable<UserProfile> {
    console.log('[ApiService] getProfile called');
    return this.http.get<UserProfile>(`${API_BASE_URL}/user/profile`);
  }

  updateProfile(payload: UpdateProfileRequest): Observable<string> {
    console.log('[ApiService] updateProfile called', payload);
    return this.http.put(`${API_BASE_URL}/user/profile`, payload, { responseType: 'text' });
  }

  // Operator Bookings
  getOperatorBookings(): Observable<OperatorBooking[]> {
    console.log('[ApiService] getOperatorBookings called');
    return this.http.get<OperatorBooking[]>(`${API_BASE_URL}/operator/bookings`);
  }

  getTripPassengers(tripId: string): Observable<TripPassenger[]> {
    console.log('[ApiService] getTripPassengers called', { tripId });
    return this.http.get<TripPassenger[]>(`${API_BASE_URL}/operator/trips/${tripId}/bookings`);
  }

  // Admin Toggles
  rejectOperator(operatorId: string): Observable<string> {
    console.log('[ApiService] rejectOperator called', { operatorId });
    return this.http.post(`${API_BASE_URL}/admin/reject-operator/${operatorId}`, {}, { responseType: 'text' });
  }

  toggleOperator(operatorId: string): Observable<string> {
    console.log('[ApiService] toggleOperator called', { operatorId });
    return this.http.put(`${API_BASE_URL}/admin/operator/${operatorId}/toggle`, {}, { responseType: 'text' });
  }

  toggleRoute(routeId: string): Observable<string> {
    console.log('[ApiService] toggleRoute called', { routeId });
    return this.http.put(`${API_BASE_URL}/admin/routes/${routeId}/toggle`, {}, { responseType: 'text' });
  }

  private mapTripResult(result: TripApiResponse | RawApiRecord): SearchResult {
    const raw = result as RawApiRecord;
    const tripId = this.readString(raw, 'id', 'Id');
    const busId = this.readString(raw, 'busId', 'BusId');
    const routeId = this.readString(raw, 'routeId', 'RouteId');
    const layoutId = this.readString(raw, 'layoutId', 'LayoutId');
    const busNumber = this.readString(raw, 'busNumber', 'BusNumber') || 'Bus pending details';
    const source = this.readString(raw, 'source', 'Source') || 'Source';
    const destination = this.readString(raw, 'destination', 'Destination') || 'Destination';
    const journeyDateRaw = this.readString(raw, 'journeyDate', 'JourneyDate');
    const journeyDate = new Date(journeyDateRaw).toISOString();
    const departureTime = this.normalizeTime(this.readUnknown(raw, 'departureTime', 'DepartureTime'));
    const arrivalTime = this.normalizeTime(this.readUnknown(raw, 'arrivalTime', 'ArrivalTime'));
    const price = this.readNumber(raw, 'price', 'Price');
    const totalSeats = this.readNumber(raw, 'totalSeats', 'TotalSeats');
    const availableSeats = this.readNumber(raw, 'availableSeats', 'AvailableSeats');
    const travelMinutes = this.readNumber(raw, 'travelMinutes', 'TravelMinutes');
    const distance = this.readNumber(raw, 'distance', 'Distance');
    const bookedSeatNumbers = this.readStringArray(raw, 'bookedSeatNumbers', 'BookedSeatNumbers');
    const lockedSeatNumbers = this.readStringArray(raw, 'lockedSeatNumbers', 'LockedSeatNumbers');
    const femaleBookedSeatNumbers = this.readStringArray(raw, 'femaleBookedSeatNumbers', 'FemaleBookedSeatNumbers');

    console.log('[ApiService] mapTripResult called', {
      raw,
      normalized: { tripId, busId, routeId, busNumber, source, destination, availableSeats, departureTime, arrivalTime }
    });

    if (!tripId) {
      console.warn('[ApiService] mapTripResult missing trip id', raw);
    }

    return {
      trip: {
        id: tripId,
        busId,
        routeId,
        journeyDate,
        departureTime,
        arrivalTime,
        price,
        pickupAddress: this.readString(raw, 'pickupAddress', 'PickupAddress'),
        dropAddress: this.readString(raw, 'dropAddress', 'DropAddress')
      },
      bus: {
        id: busId,
        operatorId: this.readString(raw, 'operatorId', 'OperatorId'),
        busNumber,
        totalSeats,
        layoutId,
        isActive: true
      },
      route: {
        id: routeId,
        source,
        destination,
        distance,
        isActive: true,
        pickupPoints: [],
        dropPoints: []
      },
      availableSeats,
      travelMinutes,
      bookedSeatNumbers,
      lockedSeatNumbers,
      femaleBookedSeatNumbers,
      pickupAddress: this.readString(raw, 'pickupAddress', 'PickupAddress'),
      dropAddress: this.readString(raw, 'dropAddress', 'DropAddress')
    };
  }

  private mapRevenueRow(result: RawApiRecord): RevenueRow {
    return {
      operatorId: this.readString(result, 'operatorId', 'OperatorId'),
      revenue: this.readNumber(result, 'revenue', 'Revenue')
    };
  }

  private mapPendingOperator(result: RawApiRecord): PendingOperator {
    return {
      operatorId: this.readString(result, 'operatorId', 'OperatorId'),
      userId: this.readString(result, 'userId', 'UserId'),
      name: this.readString(result, 'name', 'Name'),
      email: this.readString(result, 'email', 'Email'),
      approved: this.readBoolean(result, 'approved', 'Approved'),
      isActive: this.readBoolean(result, 'isActive', 'IsActive')
    };
  }

  private mapBooking(result: RawApiRecord): Booking {
    return {
      id: this.readString(result, 'id', 'Id'),
      userId: this.readString(result, 'userId', 'UserId'),
      tripId: this.readString(result, 'tripId', 'TripId'),
      totalAmount: this.readNumber(result, 'totalAmount', 'TotalAmount'),
      platformFee: this.readNumber(result, 'platformFee', 'PlatformFee'),
      refundAmount: this.readNumber(result, 'refundAmount', 'RefundAmount'),
      status: this.readString(result, 'status', 'Status'),
      createdAt: this.readString(result, 'createdAt', 'CreatedAt')
    };
  }

  private mapTicketResponse(result: RawApiRecord): TicketResponse {
    return {
      bookingId: this.readString(result, 'bookingId', 'BookingId'),
      status: this.readString(result, 'status', 'Status'),
      amount: this.readNumber(result, 'amount', 'Amount'),
      journeyDate: this.readString(result, 'journeyDate', 'JourneyDate'),
      seats: this.readStringArray(result, 'seats', 'Seats'),
      pickupAddress: this.readString(result, 'pickupAddress', 'PickupAddress'),
      dropAddress: this.readString(result, 'dropAddress', 'DropAddress')
    };
  }

  private readUnknown(source: RawApiRecord, ...keys: string[]): unknown {
    for (const key of keys) {
      if (key in source) {
        return source[key];
      }
    }

    return undefined;
  }

  private readString(source: RawApiRecord, ...keys: string[]): string {
    const value = this.readUnknown(source, ...keys);
    return typeof value === 'string' ? value : value == null ? '' : String(value);
  }

  private readNumber(source: RawApiRecord, ...keys: string[]): number {
    const value = this.readUnknown(source, ...keys);
    if (typeof value === 'number') {
      return value;
    }
    if (typeof value === 'string') {
      const parsed = Number(value);
      return Number.isFinite(parsed) ? parsed : 0;
    }

    return 0;
  }

  private readBoolean(source: RawApiRecord, ...keys: string[]): boolean {
    const value = this.readUnknown(source, ...keys);
    if (typeof value === 'boolean') {
      return value;
    }
    if (typeof value === 'string') {
      return value.toLowerCase() === 'true';
    }

    return false;
  }

  private readStringArray(source: RawApiRecord, ...keys: string[]): string[] {
    const value = this.readUnknown(source, ...keys);
    if (Array.isArray(value)) {
      return value.map((item) => String(item));
    }

    return [];
  }

  private normalizeTime(value: unknown): string {
    if (typeof value === 'string') {
      // Handle TimeSpan format (HH:mm:ss) or partial formats
      if (value.match(/^\d{1,2}:\d{2}(:\d{2})?$/)) {
        return value;
      }
      return value;
    }

    return '00:00:00';
  }
}
