import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Workshop } from '../../../core/models/workshop.model';
import { WorkshopService } from '../../../core/services/workshop.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { AuthService } from '../../../core/services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Component displaying the public catalog of available future workshops.
 * Allows searching by title, code, or instructor, and direct participant enrollment.
 */
@Component({
  selector: 'app-workshop-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe],
  templateUrl: './workshop-list.html',
  styleUrl: './workshop-list.css'
})
export class WorkshopListComponent implements OnInit {
  /** Master list of available workshops retrieved from the API */
  public workshops = signal<Workshop[]>([]);

  /** Filtered workshop list according to search criteria */
  public filteredWorkshops = signal<Workshop[]>([]);

  /** Current query term for search input */
  public searchTerm = '';

  /** Loading state indicator */
  public isLoading = signal(false);

  /** User-facing error message notification */
  public errorMessage = signal<string | null>(null);

  /** User-facing success message notification */
  public successMessage = signal<string | null>(null);

  /** Identifier of the workshop currently being enrolled in, for spinner feedback */
  public enrollingWorkshopId = signal<string | null>(null);

  constructor(
    private readonly workshopService: WorkshopService,
    private readonly enrollmentService: EnrollmentService,
    public readonly authService: AuthService,
    private readonly router: Router
  ) {}

  public ngOnInit(): void {
    this.loadWorkshops();
  }

  /**
   * Fetches available workshops from the public backend endpoint.
   */
  public loadWorkshops(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.workshopService.getAvailableWorkshops().subscribe({
      next: (data) => {
        this.workshops.set(data);
        this.applyFilter();
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar los talleres disponibles. Por favor verifica que la API esté en ejecución.');
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Filters the workshops by title, unique code, instructor name, or description.
   */
  public applyFilter(): void {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      this.filteredWorkshops.set(this.workshops());
      return;
    }

    const filtered = this.workshops().filter((w) =>
      w.title.toLowerCase().includes(term) ||
      w.code.toLowerCase().includes(term) ||
      w.instructorName.toLowerCase().includes(term) ||
      w.description.toLowerCase().includes(term)
    );
    this.filteredWorkshops.set(filtered);
  }

  /**
   * Handles the enrollment process for a selected workshop.
   * Prompts unauthenticated visitors to log in, and displays domain error responses.
   * @param workshop The target workshop to enroll in.
   */
  public onEnroll(workshop: Workshop): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/workshops' } });
      return;
    }

    if (!this.authService.isParticipant()) {
      this.errorMessage.set('Solo los usuarios con rol de Participante pueden inscribirse a talleres.');
      return;
    }

    if (workshop.availableSeats <= 0) {
      this.errorMessage.set('Lo sentimos, este taller no tiene cupos disponibles.');
      return;
    }

    const confirmed = window.confirm(`¿Deseas inscribirte en el taller "${workshop.title}"?`);
    if (!confirmed) {
      return;
    }

    this.enrollingWorkshopId.set(workshop.id);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.enrollmentService.enrollInWorkshop(workshop.id).subscribe({
      next: (res) => {
        this.enrollingWorkshopId.set(null);
        this.successMessage.set(res.message || `¡Inscripción exitosa en "${workshop.title}"!`);
        // Reload workshops to update seat counts
        this.loadWorkshops();
        window.scrollTo({ top: 0, behavior: 'smooth' });
      },
      error: (error: HttpErrorResponse) => {
        this.enrollingWorkshopId.set(null);
        if (error.error?.detail) {
          this.errorMessage.set(error.error.detail);
        } else if (error.error?.message) {
          this.errorMessage.set(error.error.message);
        } else if (error.status === 409) {
          this.errorMessage.set('No es posible realizar la inscripción debido a conflicto de cupos, horario o inscripción duplicada.');
        } else {
          this.errorMessage.set('Ocurrió un error al procesar tu inscripción.');
        }
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }
}
