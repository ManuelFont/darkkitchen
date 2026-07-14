import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { LoginRequest } from '../models/login-request.model';
import { AuthService } from '../../../core/auth/auth.service';

type LoginMessageType = 'error' | 'success';

interface LoginMessage {
  text: string;
  type: LoginMessageType;
}

const invalidLoginFormMessage: LoginMessage = {
  text: 'Enter a valid email and password',
  type: 'error',
};

const successfulLoginMessage: LoginMessage = {
  text: 'Logged in successfully',
  type: 'success',
};

@Component({
  selector: 'app-login',
  imports: [FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './auth.component.css',
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  password = '';

  isSubmitting = signal(false);
  message = signal<LoginMessage | null>(null);

  onSubmit(isFormValid: boolean | null): void {
    this.clearMessage();

    if (!isFormValid) {
      this.showMessage(invalidLoginFormMessage);
      return;
    }

    this.submitLoginRequest(this.createLoginRequest());
  }

  private submitLoginRequest(request: LoginRequest): void {
    this.startSubmitting();

    this.authService.createSession(request).pipe(
      finalize(() => {
        this.stopSubmitting();
      }),
    ).subscribe({
      next: () => this.handleSuccessfulLogin(),
      error: (error: Error) => this.showErrorMessage(error.message),
    });
  }

  private handleSuccessfulLogin(): void {
    this.showMessage(successfulLoginMessage);
    void this.router.navigateByUrl('/home');
  }

  private createLoginRequest(): LoginRequest {
    return {
      email: this.email,
      password: this.password,
    };
  }

  private clearMessage(): void {
    this.message.set(null);
  }

  private showMessage(message: LoginMessage): void {
    this.message.set(message);
  }

  private showErrorMessage(text: string): void {
    this.showMessage({ text, type: 'error' });
  }

  private startSubmitting(): void {
    this.isSubmitting.set(true);
  }

  private stopSubmitting(): void {
    this.isSubmitting.set(false);
  }
}
