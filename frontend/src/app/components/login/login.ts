import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {

  email = '';
  password = '';
  showPassword = false;
  errorMessage = '';
  submitting = false;

  constructor(
    private auth: Auth,
    private router: Router,
    private notifier: NotificationService
  ) {}

  submitLogin(): void {
    this.errorMessage = '';
    this.submitting = true;
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: (res: any) => {
        if (!res?.token) return;
        localStorage.setItem('token', res.token);
        window.dispatchEvent(new Event('auth-change'));
        const role = this.getRoleFromToken(res.token);
        this.notifier.add('Login successful!', 'success');
        if (role === 'Owner') this.router.navigate(['/owner']);
        else if (role === 'User') this.router.navigate(['/renter']);
        else this.router.navigate(['/']);
      },
      error: () => {
        this.errorMessage = 'Invalid email or password.';
      },
      complete: () => { this.submitting = false; }
    });
  }

  private getRoleFromToken(token: string): string {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return '';
      const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'] ?? '';
    } catch {
      return '';
    }
  }
}
