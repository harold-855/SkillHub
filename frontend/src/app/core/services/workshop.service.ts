import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Workshop, WorkshopDetails } from '../models/workshop.model';
import { environment } from '../../../environments/environment';

/**
 * Service providing HTTP operations for querying workshops in the public catalog.
 */
@Injectable({
  providedIn: 'root'
})
export class WorkshopService {
  private readonly apiUrl = `${environment.apiUrl}/workshops`;

  constructor(private readonly http: HttpClient) {}

  /**
   * Retrieves all future workshops with their calculated available seats.
   * This endpoint is public and does not require authentication.
   */
  public getAvailableWorkshops(): Observable<Workshop[]> {
    return this.http.get<Workshop[]>(this.apiUrl);
  }

  /**
   * Retrieves detailed information for a specific workshop, including instructor contact details.
   * @param id The unique identifier of the workshop.
   */
  public getWorkshopById(id: string): Observable<WorkshopDetails> {
    return this.http.get<WorkshopDetails>(`${this.apiUrl}/${id}`);
  }
}
