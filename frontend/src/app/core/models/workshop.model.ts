/**
 * Summary representation of a workshop available in the public catalog.
 */
export interface Workshop {
  id: string;
  code: string;
  title: string;
  description: string;
  instructorName: string;
  startDateTime: string;
  endDateTime: string;
  maximumCapacity: number;
  availableSeats: number;
}

/**
 * Detailed representation of a workshop including instructor contact and specialty information.
 */
export interface WorkshopDetails {
  id: string;
  code: string;
  title: string;
  description: string;
  instructorName: string;
  instructorEmail: string;
  instructorSpecialty: string;
  startDateTime: string;
  endDateTime: string;
  maximumCapacity: number;
  availableSeats: number;
}
