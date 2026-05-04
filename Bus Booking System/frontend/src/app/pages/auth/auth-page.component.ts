import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AuthRequest } from '../../core/models';

@Component({
  selector: 'app-auth-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './auth-page.component.html'
})
export class AuthPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  mode: 'login' | 'register' = 'login';
  redirectUrl = '/';
  feedback = '';
  error = '';
  isSubmitting = false;

  readonly loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(1)]]
  });

  readonly registerForm = this.formBuilder.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    mobileNo: ['', Validators.required],
    gender: ['Male', Validators.required],
    dob: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(1)]],
    role: ['User', Validators.required]
  });

  ngOnInit(): void {
    console.log('[AuthPage] ngOnInit called');
    this.redirectUrl = this.route.snapshot.queryParamMap.get('redirect') || '/';
    console.log('[AuthPage] redirectUrl resolved', { redirectUrl: this.redirectUrl });
  }

  setMode(mode: 'login' | 'register'): void {
    console.log('[AuthPage] setMode called', { mode });
    this.mode = mode;
    this.feedback = '';
    this.error = '';
  }

  submitLogin(): void {
    console.log('[AuthPage] submitLogin called', { email: this.loginForm.getRawValue().email });
    if (this.loginForm.invalid) {
      console.warn('[AuthPage] login form invalid', this.loginForm.getRawValue());
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.error = '';
    this.feedback = '';

    this.authService.login(this.loginForm.getRawValue() as { email: string; password: string }).subscribe({
      next: (response) => {
        console.log('[AuthPage] login response received', response);
        this.isSubmitting = false;

        if (this.redirectUrl !== '/') {
          this.router.navigateByUrl(this.redirectUrl);
          return;
        }

        if (response.role === 'Operator') {
          this.router.navigateByUrl('/operator');
          return;
        }

        if (response.role === 'Admin') {
          this.router.navigateByUrl('/admin');
          return;
        }

        this.router.navigateByUrl('/');
      },
      error: (error) => {
        console.error('[AuthPage] login failed', error);
        this.error = error.error ?? 'Login failed.';
        this.isSubmitting = false;
      }
    });
  }

  submitRegister(): void {
    console.log('[AuthPage] submitRegister called', {
      email: this.registerForm.getRawValue().email,
      role: this.registerForm.getRawValue().role
    });
    if (this.registerForm.invalid) {
      console.warn('[AuthPage] register form invalid', this.registerForm.getRawValue());
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.error = '';
    this.feedback = '';

    this.authService.register(this.registerForm.getRawValue() as AuthRequest).subscribe({
      next: (message) => {
        console.log('[AuthPage] register response received', { message });
        this.feedback = `${message}. You can log in now. Operators will need admin approval before using protected operator APIs.`;
        this.mode = 'login';
        this.loginForm.patchValue({
          email: this.registerForm.getRawValue().email
        });
        this.isSubmitting = false;
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('[AuthPage] register failed', error);
        this.error = error.error ?? 'Registration failed.';
        this.isSubmitting = false;
      }
    });

  }
}
