import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthRequest, AuthResponse, DecodedSession, LoginRequest, UserRole } from '../models';

const API_BASE_URL = 'http://localhost:5299';
const TOKEN_KEY = 'bus_booking_token';
const ROLE_KEY = 'bus_booking_role';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenState = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly roleState = signal<UserRole | null>((localStorage.getItem(ROLE_KEY) as UserRole | null) ?? null);

  readonly token = computed(() => this.tokenState());
  readonly role = computed(() => this.roleState());
  readonly isLoggedIn = computed(() => !!this.tokenState());
  readonly session = computed<DecodedSession>(() => this.decodeJwt(this.tokenState()));

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  register(payload: AuthRequest): Observable<string> {
    console.log('[AuthService] register called', {
      email: payload.email,
      role: payload.role,
      dob: payload.dob
    });
    return this.http.post(`${API_BASE_URL}/auth/register`, {
      ...payload,
      dob: new Date(payload.dob).toISOString()
    }, {
      responseType: 'text'
    });
  }

  login(payload: LoginRequest): Observable<AuthResponse> {
    console.log('[AuthService] login called', { email: payload.email });
    return this.http.post<AuthResponse>(`${API_BASE_URL}/auth/login`, payload).pipe(
      tap((response) => {
        console.log('[AuthService] login succeeded', { role: response.role });
        this.setSession(response.token, response.role);
      })
    );
  }

  logout(): void {
    console.log('[AuthService] logout called');
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLE_KEY);
    this.tokenState.set(null);
    this.roleState.set(null);
    this.router.navigateByUrl('/');
  }

  hasRole(...roles: UserRole[]): boolean {
    console.log('[AuthService] hasRole check', { roles, currentRole: this.roleState() });
    const currentRole = this.roleState();
    return !!currentRole && roles.includes(currentRole);
  }

  private setSession(token: string, role: UserRole): void {
    console.log('[AuthService] setSession called', { role });
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(ROLE_KEY, role);
    this.tokenState.set(token);
    this.roleState.set(role);
  }

  private decodeJwt(token: string | null): DecodedSession {
    console.log('[AuthService] decodeJwt called', { hasToken: !!token });
    if (!token) {
      return { email: null, role: null, userId: null };
    }

    try {
      const [, payload] = token.split('.');
      const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
      const decoded = JSON.parse(atob(normalized));

      return {
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? null,
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null,
        userId: decoded.UserId ?? null
      };
    } catch {
      return { email: null, role: null, userId: null };
    }
  }
}
