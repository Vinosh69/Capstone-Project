import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  name = '';
  email = '';
  password = '';
  role: 'User' | 'Owner' = 'User';
  feedback = '';
  submitting = false;

  constructor(private auth: Auth) {}

  submitRegister(): void {
    this.feedback = '';
    this.submitting = true;
    const payload = {
      name: this.name,
      email: this.email,
      passwordHash: this.password,
      role: this.role
    };
    this.auth.register(payload).subscribe({
      next: () => { this.feedback = 'Registration successful.'; },
      error: () => { this.feedback = 'Registration failed. Email may already be in use.'; },
      complete: () => { this.submitting = false; }
    });
  }
}
