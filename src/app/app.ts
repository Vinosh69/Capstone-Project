import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { NotificationService } from './services/notification';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent implements OnInit {

  isLoggedIn = false;
  role: 'User' | 'Owner' | '' = '';

  constructor(
    private router: Router,
    public notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.refreshAuth();
    window.addEventListener('storage', () => this.refreshAuth());
    window.addEventListener('auth-change', () => this.refreshAuth());
  }

  refreshAuth(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      this.isLoggedIn = false;
      this.role = '';
      return;
    }
    const roleClaim = this.parseRole(token);
    this.isLoggedIn = roleClaim === 'User' || roleClaim === 'Owner';
    this.role = (this.isLoggedIn ? roleClaim : '') as 'User' | 'Owner' | '';
  }

  dismissNotification(id: number): void {
    this.notificationService.remove(id);
  }

  logout(): void {
    localStorage.removeItem('token');
    window.dispatchEvent(new Event('auth-change'));
    this.isLoggedIn = false;
    this.role = '';
    this.notificationService.clear();
    this.router.navigate(['/']);
  }

  private parseRole(token: string): string {
    try {
      const [, b64] = token.split('.');
      if (!b64) return '';
      const payload = JSON.parse(atob(b64.replace(/-/g, '+').replace(/_/g, '/')));
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'] ?? '';
    } catch {
      return '';
    }
  }
}
