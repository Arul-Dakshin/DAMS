import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html'
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  // Demo accounts let evaluators try each role with one click.
  readonly demoAccounts = [
    { role: 'Admin', email: 'admin@dams.com', password: 'Admin@123' },
    { role: 'Doctor', email: 'doctor@dams.com', password: 'Doctor@123' },
    { role: 'Receptionist', email: 'reception@dams.com', password: 'Reception@123' },
    { role: 'Patient', email: 'patient@dams.com', password: 'Patient@123' }
  ];

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    const { email, password } = this.form.getRawValue();
    this.auth.login(email, password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Login failed. Please try again.');
        this.loading.set(false);
      }
    });
  }

  fill(email: string, password: string): void {
    this.form.setValue({ email, password });
  }
}
