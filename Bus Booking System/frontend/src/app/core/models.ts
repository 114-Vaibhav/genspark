export type UserRole = 'User' | 'Operator' | 'Admin';
export type RegistrableRole = 'User' | 'Operator';

export interface AuthRequest {
  name: string;
  email: string;
  mobileNo: string;
  gender: string;
  dob: string;
  password: string;
  role: RegistrableRole;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  role: UserRole;
}

export interface RouteItem {
  id: string;
  source: string;
  destination: string;
  distance: number;
  isActive: boolean;
  pickupPoints: string[];
  dropPoints: string[];
}

export interface Bus {
  id: string;
  operatorId: string;
  busNumber: string;
  totalSeats: number;
  layoutId: string;
  isActive: boolean;
}

export interface Trip {
  id: string;
  busId: string;
  routeId: string;
  journeyDate: string;
  departureTime: string;
  arrivalTime: string;
  price: number;
  pickupAddress: string;
  dropAddress: string;
}

export interface Seat {
  id: string;
  layoutId: string;
  seatNumber: string;
  row: number;
  column: number;
}

export interface TravelerPayload {
  name: string;
  age: number;
  gender: string;
  seatNumber: string;
}

export interface BookingRequest {
  tripId: string;
  isSingleLady: boolean;
  travelers: TravelerPayload[];
}

export interface Booking {
  id: string;
  userId: string;
  tripId: string;
  totalAmount: number;
  platformFee: number;
  refundAmount: number;
  status: string;
  createdAt: string;
}

export interface TicketResponse {
  bookingId: string;
  status: string;
  amount: number;
  journeyDate: string;
  seats: string[];
  pickupAddress: string;
  dropAddress: string;
}

export interface RevenueRow {
  operatorId: string;
  revenue: number;
}

export interface PendingOperator {
  operatorId: string;
  userId: string;
  name: string;
  email: string;
  approved: boolean;
  isActive: boolean;
}

export interface SearchResult {
  trip: Trip;
  bus: Bus;
  route: RouteItem;
  availableSeats: number;
  travelMinutes: number;
  bookedSeatNumbers: string[];
  lockedSeatNumbers: string[];
  femaleBookedSeatNumbers: string[];
  pickupAddress: string;
  dropAddress: string;
}

export interface DecodedSession {
  email: string | null;
  role: UserRole | null;
  userId: string | null;
}

export interface UserProfile {
  id: string;
  name: string;
  email: string;
  mobileNo: string;
  gender: string;
  dob: string;
  role: string;
}

export interface UpdateProfileRequest {
  name: string;
  mobileNo: string;
  gender: string;
  dob: string;
}

export interface OperatorBooking {
  id: string;
  totalAmount: number;
  platformFee: number;
  status: string;
  createdAt: string;
  journeyDate: string;
  source: string;
  destination: string;
  busNumber: string;
  userName: string;
}

export interface TripPassenger {
  id: string;
  name: string;
  age: number;
  gender: string;
  seatNumber: string;
  bookingStatus: string;
}
