import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';

const firstNamePattern = /^\p{L}(?:[\p{L} ]*\p{L})?$/u;
const lastNamePattern = /^\p{L}[\p{L} ]{1,23}\p{L}$/u;
const phonePattern = /^ *0 *9(?: *\d){7} *$/;
const passwordPattern =
  /^(?!.*(?:012|123|234|345|456|567|678|789))(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\p{P}\p{S}]).{15,25}$/u;

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    RouterLink,
  ],
  templateUrl: './register.component.html',
  styleUrl: './auth.component.css',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly firstNamePattern = firstNamePattern;
  readonly lastNamePattern = lastNamePattern;
  readonly phonePattern = phonePattern;
  readonly passwordPattern = passwordPattern;

  firstName = '';
  lastName = '';
  email = '';
  phone = '';
  password = '';

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  onSubmit(isFormValid: boolean | null): void {
    this.errorMessage.set(null);

    if (!isFormValid) {
      this.errorMessage.set('Enter valid registration details');
      return;
    }

    this.submitRegistration();
  }

  private submitRegistration(): void {
    this.isSubmitting.set(true);

    this.authService
      .register(this.createRegisterRequest())
      .pipe(
        switchMap(() => this.authService.createSession(this.createLoginRequest())),
        finalize(() => {
          this.isSubmitting.set(false);
        }),
      )
      .subscribe({
        next: () => {
          void this.router.navigateByUrl('/home');
        },
        error: (error: Error) => {
          this.errorMessage.set(error.message);
        },
      });
  }

  private createRegisterRequest(): RegisterRequest {
    return {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password,
      phone: this.phone,
    };
  }

  private createLoginRequest(): LoginRequest {
    return {
      email: this.email,
      password: this.password,
    };
  }
}
