/**
 * Lifecycle status of an enrollment.
 * Corresponds to SkillHub.Domain.Enums.EnrollmentStatus (Active = 1, Cancelled = 2).
 */
export enum EnrollmentStatus {
  Active = 1,
  Cancelled = 2
}

/**
 * Representation of a participant's enrollment in a workshop.
 */
export interface Enrollment {
  id: string;
  workshopId: string;
  workshopCode: string;
  workshopTitle: string;
  workshopStartDateTime: string;
  workshopEndDateTime: string;
  status: EnrollmentStatus | number | string;
  enrolledAt: string;
  cancelledAt?: string | null;
}

/**
 * Payload sent to create a new enrollment for the authenticated participant.
 */
export interface CreateEnrollmentRequest {
  workshopId: string;
}

/**
 * Response payload returned when an enrollment is successfully created.
 */
export interface CreateEnrollmentResponse {
  id: string;
  message: string;
}
