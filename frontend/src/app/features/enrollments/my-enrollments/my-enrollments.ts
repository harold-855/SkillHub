import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Enrollment, EnrollmentStatus } from '../../../core/models/enrollment.model';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Component managing the authenticated participant's workshop enrollments,
 * providing real-time status badges, cancellation management (up to 24h prior),
 * and official PDF enrollment certificate downloads.
 */
@Component({
  selector: 'app-my-enrollments',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './my-enrollments.html',
  styleUrl: './my-enrollments.css'
})
export class MyEnrollmentsComponent implements OnInit {
  /** List of the participant's enrollments */
  public enrollments = signal<Enrollment[]>([]);

  /** Data retrieval loading indicator */
  public isLoading = signal(true);

  /** User-facing error message notification */
  public errorMessage = signal<string | null>(null);

  /** User-facing success message notification */
  public successMessage = signal<string | null>(null);

  /** Identifier of the enrollment currently being cancelled */
  public cancellingId = signal<string | null>(null);

  /** Identifier of the enrollment currently generating/downloading a PDF certificate */
  public downloadingId = signal<string | null>(null);

  constructor(private readonly enrollmentService: EnrollmentService) {}

  public ngOnInit(): void {
    this.loadEnrollments();
  }

  /**
   * Fetches the current participant's enrollments from the backend API.
   */
  public loadEnrollments(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.enrollmentService.getMyEnrollments().subscribe({
      next: (data) => {
        this.enrollments.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron obtener tus inscripciones. Por favor verifica tu sesión.');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Checks whether an enrollment is in Active status.
   * Handles numeric, string, and enum value variations.
   */
  public isActive(enrollment: Enrollment): boolean {
    return (
      enrollment.status === EnrollmentStatus.Active ||
      enrollment.status === 1 ||
      enrollment.status === 'Active'
    );
  }

  /**
   * Checks whether an enrollment is in Cancelled status.
   */
  public isCancelled(enrollment: Enrollment): boolean {
    return (
      enrollment.status === EnrollmentStatus.Cancelled ||
      enrollment.status === 2 ||
      enrollment.status === 'Cancelled'
    );
  }

  /**
   * Enforces the business rule: a participant can cancel up to 24 hours before workshop start.
   * @param enrollment The enrollment to verify.
   */
  public canCancel(enrollment: Enrollment): boolean {
    if (!this.isActive(enrollment)) {
      return false;
    }
    const workshopStart = new Date(enrollment.workshopStartDateTime).getTime();
    const now = Date.now();
    const hoursRemaining = (workshopStart - now) / (1000 * 60 * 60);
    return hoursRemaining >= 24;
  }

  /**
   * Requests cancellation of an enrollment from the API.
   * @param enrollment The enrollment to cancel.
   */
  public onCancel(enrollment: Enrollment): void {
    const confirmed = window.confirm(
      `¿Deseas cancelar tu inscripción en el taller "${enrollment.workshopTitle}"?\n\nRecuerda que solo se permite cancelar hasta 24 horas antes del inicio del taller.`
    );
    if (!confirmed) return;

    this.cancellingId.set(enrollment.id);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.enrollmentService.cancelEnrollment(enrollment.id).subscribe({
      next: () => {
        this.cancellingId.set(null);
        this.successMessage.set(`Inscripción en "${enrollment.workshopTitle}" cancelada exitosamente.`);
        this.loadEnrollments();
        window.scrollTo({ top: 0, behavior: 'smooth' });
      },
      error: (error: HttpErrorResponse) => {
        this.cancellingId.set(null);
        if (error.error?.detail) {
          this.errorMessage.set(error.error.detail);
        } else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        } else if (error.status === 409) {
          this.errorMessage.set('No fue posible cancelar: debe realizarse al menos 24 horas antes del inicio del taller.');
        } else {
          this.errorMessage.set('Ocurrió un error al intentar cancelar la inscripción.');
        }
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  /**
   * Downloads the official PDF certificate for an active enrollment.
   * Triggers an in-browser blob download using a temporary object URL.
   * @param enrollment The active enrollment to download the certificate for.
   */
  public onDownloadCertificate(enrollment: Enrollment): void {
    if (!this.isActive(enrollment)) {
      this.errorMessage.set('Solo se genera constancia para inscripciones activas.');
      return;
    }

    this.downloadingId.set(enrollment.id);
    this.errorMessage.set(null);

    this.enrollmentService.downloadCertificate(enrollment.id).subscribe({
      next: (blob: Blob) => {
        this.downloadingId.set(null);
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `SkillHub-Constancia-${enrollment.workshopCode}.pdf`;
        anchor.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error: HttpErrorResponse) => {
        this.downloadingId.set(null);
        if (error.error?.detail) {
          this.errorMessage.set(error.error.detail);
        } else {
          this.errorMessage.set('No se pudo generar la constancia en PDF. Verifica que la inscripción esté activa.');
        }
      }
    });
  }
}
