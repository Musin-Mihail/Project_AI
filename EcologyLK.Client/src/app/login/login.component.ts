import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';
import { LoginUserDto } from '../models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  // Внедряем сервисы
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  // Signal для отображения ошибок
  error = signal<string | null>(null);

  // Форма входа
  loginForm = this.fb.group({
    email: ['admin@ecology.lk', [Validators.required, Validators.email]],
    password: ['AdminP@ssw0rd1!', Validators.required],
  });

  /**
   * Вызывается при отправке формы
   */
  onSubmit() {
    if (this.loginForm.invalid) {
      this.error.set('Пожалуйста, введите Email и пароль.');
      return;
    }

    this.error.set(null); // Сбрасываем ошибку

    const dto: LoginUserDto = {
      email: this.loginForm.value.email!,
      password: this.loginForm.value.password!,
    };

    // Вызываем сервис
    this.authService.login(dto).subscribe({
      next: (response) => {
        console.log('Вход успешен:', response.email);
        // При успехе - перенаправляем на главную страницу (Анкету)
        this.router.navigate(['/new-site']);
      },
      error: (err) => {
        console.error('Ошибка входа:', err);
        this.error.set('Ошибка входа. Проверьте Email или пароль.');
      },
    });
  }
}
