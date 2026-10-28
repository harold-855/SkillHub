import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Component handling participant and employee login credentials.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit {
  public loginForm!: FormGroup;
  public isLoading = false;
  public errorMessage: string | null = null;
  public successMessage: string | null = null;
  private returnUrl: string = '/workshops';

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  public ngOnInit(): void {
    // If the user is already authenticated, redirect immediately to the workshops view
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/workshops']);
      return;
    }

    // Capture the optional return URL from route parameters
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/workshops';

    // Show a congratulatory notification if redirected from the registration flow
    const registered = this.route.snapshot.queryParams['registered'];
    if (registered) {
      this.successMessage = '¡Registro completado exitosamente! Por favor inicia sesión.';
    }

    // Initialize reactive login form with required validators
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  /**
   * Submits the credentials to the backend login endpoint.
   */
  public onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        // Parse RFC 7807 ProblemDetails or fallback error structures
        if (error.error?.detail) {
          this.errorMessage = error.error.detail;
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.status === 401 || error.status === 400) {
          this.errorMessage = 'Credenciales inválidas. Verifica tu correo y contraseña.';
        } else {
          this.errorMessage = 'Ocurrió un error al intentar iniciar sesión. Por favor intenta más tarde.';
        }
      }
    });
  }
}
