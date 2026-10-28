/**
 * Request payload for participant and employee authentication.
 */
export interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Request payload for registering a new participant.
 */
export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

/**
 * Authentication response returned by the API upon successful registration or login.
 */
export interface AuthenticationResponse {
  accessToken: string;
  expiresAt: string;
  email: string;
  role: string;
}

/**
 * Client-side authenticated user session model stored in browser storage.
 */
export interface UserSession {
  email: string;
  role: string;
  token: string;
  expiresAt: string;
}
