import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Custom cross-field validator ensuring password and confirmPassword fields match.
 */
export function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirmPassword = group.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordsMismatch: true };
}

/**
 * Component handling self-registration for new SkillHub participants.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent implements OnInit {
  public registerForm!: FormGroup;
  public isLoading = false;
  public errorMessage: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public ngOnInit(): void {
    // Redirect if already authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/workshops']);
      return;
    }

    // Configure form controls with length and matching validations
    this.registerForm = this.fb.group(
      {
        fullName: ['', [Validators.required, Validators.minLength(2)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]]
      },
      { validators: passwordsMatchValidator }
    );
  }

  /**
   * Submits participant registration data to the backend API.
   */
  public onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.isLoading = false;
        // Redirect to login page upon successful account creation
        this.router.navigate(['/login'], { queryParams: { registered: 'true' } });
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        // Parse server problem details or property validation errors
        if (error.error?.detail) {
          this.errorMessage = error.error.detail;
        } else if (error.error?.message) {
          this.errorMessage = error.error.message;
        } else if (error.error?.errors) {
          const firstKey = Object.keys(error.error.errors)[0];
          this.errorMessage = error.error.errors[firstKey][0];
        } else if (error.status === 409) {
          this.errorMessage = 'El correo electrónico ya se encuentra registrado.';
        } else {
          this.errorMessage = 'Ocurrió un error al procesar el registro. Intenta nuevamente.';
        }
      }
    });
  }
}
