import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { WorkshopDetails } from '../../../core/models/workshop.model';
import { WorkshopService } from '../../../core/services/workshop.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Component providing in-depth workshop details, including instructor contact,
 * specialty, schedule, capacity statistics, and enrollment actions.
 */
@Component({
  selector: 'app-workshop-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './workshop-detail.html',
  styleUrl: './workshop-detail.css'
})
export class WorkshopDetailComponent implements OnInit {
  /** Workshop detailed data */
  public workshop = signal<WorkshopDetails | null>(null);

  /** Data retrieval loading indicator */
  public isLoading = signal(true);

  /** User-facing error message notification */
  public errorMessage = signal<string | null>(null);

  /** User-facing success message notification */
  public successMessage = signal<string | null>(null);

  /** Enrollment submission indicator */
  public isEnrolling = signal(false);

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly workshopService: WorkshopService,
    private readonly enrollmentService: EnrollmentService,
    public readonly authService: AuthService
  ) {}

  public ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/workshops']);
      return;
    }
    this.loadDetails(id);
  }

  /**
   * Fetches workshop details by ID.
   * @param id The GUID of the requested workshop.
   */
  public loadDetails(id: string): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.workshopService.getWorkshopById(id).subscribe({
      next: (data) => {
        this.workshop.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se encontró el taller solicitado o no está disponible.');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Initiates enrollment from the details view.
   */
  public onEnroll(): void {
    const current = this.workshop();
    if (!current) return;

    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/workshops/${current.id}` } });
      return;
    }

    if (!this.authService.isParticipant()) {
      this.errorMessage.set('Solo los participantes registrados pueden inscribirse en talleres.');
      return;
    }

    if (current.availableSeats <= 0) {
      this.errorMessage.set('No hay cupos disponibles para este taller.');
      return;
    }

    const confirmed = window.confirm(`¿Confirmas tu inscripción en el taller "${current.title}"?`);
    if (!confirmed) return;

    this.isEnrolling.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.enrollmentService.enrollInWorkshop(current.id).subscribe({
      next: (res) => {
        this.isEnrolling.set(false);
        this.successMessage.set(res.message || '¡Te has inscrito exitosamente a este taller!');
        // Refresh details to update seat count
        this.loadDetails(current.id);
      },
      error: (error: HttpErrorResponse) => {
        this.isEnrolling.set(false);
        if (error.error?.detail) {
          this.errorMessage.set(error.error.detail);
        } else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        } else if (error.status === 409) {
          this.errorMessage.set('Conflicto con la inscripción (cupo agotado, inscripción activa duplicada o cruce de horario con otro taller).');
        } else {
          this.errorMessage.set('Ocurrió un error al intentar inscribirte en el taller.');
        }
      }
    });
  }
}
