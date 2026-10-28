import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEnrollmentRequest, CreateEnrollmentResponse, Enrollment } from '../models/enrollment.model';
import { environment } from '../../../environments/environment';

/**
 * Service managing workshop enrollments, cancellations, and certificate downloads.
 */
@Injectable({
  providedIn: 'root'
})
export class EnrollmentService {
  private readonly apiUrl = `${environment.apiUrl}/enrollments`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Retrieves all enrollments belonging to the authenticated participant.
   */
  public getMyEnrollments(): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.apiUrl}/mine`);
  }

  /**
   * Enrolls the authenticated participant in a specified workshop.
   * Validates capacity, duplicate enrollments, schedule conflicts, and workshop future start date.
   * @param workshopId The unique identifier of the target workshop.
   */
  public enrollInWorkshop(workshopId: string): Observable<CreateEnrollmentResponse> {
    const payload: CreateEnrollmentRequest = { workshopId };
    return this.http.post<CreateEnrollmentResponse>(this.apiUrl, payload);
  }

  /**
   * Cancels an existing enrollment. Participants can only cancel up to 24 hours before workshop start.
   * @param enrollmentId The unique identifier of the enrollment to cancel.
   */
  public cancelEnrollment(enrollmentId: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${enrollmentId}/cancel`, {});
  }

  /**
   * Downloads the official PDF certificate for an active enrollment.
   * @param enrollmentId The unique identifier of the active enrollment.
   */
  public downloadCertificate(enrollmentId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${enrollmentId}/certificate`, {
      responseType: 'blob'
    });
  }
}
