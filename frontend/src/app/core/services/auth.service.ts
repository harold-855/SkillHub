import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthenticationResponse, LoginRequest, RegisterRequest, UserSession } from '../models/auth.model';
import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'skillhub_token';
const USER_KEY = 'skillhub_user';

/**
 * Service managing user authentication, JWT persistence, and reactive session state.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  
  // Reactive session state using modern Angular Signals
  private readonly currentUserSignal = signal<UserSession | null>(this.getStoredSession());

  /** Read-only signal exposing the active user session */
  public readonly currentUser = this.currentUserSignal.asReadonly();

  /** Computed signal indicating whether a user is currently authenticated */
  public readonly isAuthenticated = computed(() => !!this.currentUserSignal());

  /** Computed signal indicating whether the authenticated user has the Participant role */
  public readonly isParticipant = computed(() => this.currentUserSignal()?.role === 'Participant');

  /** Computed signal indicating whether the authenticated user has the Employee role */
  public readonly isEmployee = computed(() => this.currentUserSignal()?.role === 'Employee');

  constructor(private readonly http: HttpClient) {}

  /**
   * Registers a new participant account.
   * @param request Participant registration details.
   */
  public register(request: RegisterRequest): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${this.apiUrl}/register`, request);
  }

  /**
   * Authenticates a user with email and password, persisting the resulting JWT token.
   * @param request User login credentials.
   */
  public login(request: LoginRequest): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        this.saveSession({
          token: response.accessToken,
          email: response.email,
          role: response.role,
          expiresAt: response.expiresAt
        });
      })
    );
  }

  /**
   * Clears the current user session and authentication tokens from browser storage.
   */
  public logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.currentUserSignal.set(null);
  }

  /**
   * Retrieves the raw JWT token from current session or local storage.
   */
  public getToken(): string | null {
    const session = this.currentUserSignal();
    return session ? session.token : localStorage.getItem(TOKEN_KEY);
  }

  /**
   * Persists user session in localStorage and updates the reactive signal.
   */
  private saveSession(session: UserSession): void {
    localStorage.setItem(TOKEN_KEY, session.token);
    localStorage.setItem(USER_KEY, JSON.stringify(session));
    this.currentUserSignal.set(session);
  }

  /**
   * Reads and parses any existing user session from localStorage on startup.
   */
  private getStoredSession(): UserSession | null {
    const token = localStorage.getItem(TOKEN_KEY);
    const userStr = localStorage.getItem(USER_KEY);
    if (!token || !userStr) {
      return null;
    }

    try {
      return JSON.parse(userStr) as UserSession;
    } catch {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
      return null;
    }
  }
}
