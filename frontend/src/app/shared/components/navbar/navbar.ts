import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

/**
 * Top navigation header component featuring responsive navigation links,
 * authenticated user profile indicator, and session management actions.
 */
@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class NavbarComponent {
  constructor(
    public readonly authService: AuthService,
    private readonly router: Router
  ) {}

  /**
   * Logs out the currently authenticated user and navigates back to the public catalog.
   */
  public onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/workshops']);
  }
}
