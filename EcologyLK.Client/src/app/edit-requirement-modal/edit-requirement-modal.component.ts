import { Component, computed, EventEmitter, input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EcologicalRequirementDto, RequirementStatus, UpdateRequirementDto } from '../models';

@Component({
  selector: 'app-edit-requirement-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-requirement-modal.component.html',
  styleUrl: './edit-requirement-modal.component.scss',
})
export class EditRequirementModalComponent {
  private fb = inject(FormBuilder);

  // Входной сигнал (Input) с данными
  requirement = input<EcologicalRequirementDto | null>(null);

  // События (Outputs)
  @Output() save = new EventEmitter<UpdateRequirementDto>();
  @Output() close = new EventEmitter<void>();

  // Опции для <select>
  statusOptions = Object.keys(RequirementStatus)
    .filter((v) => isNaN(Number(v)))
    .map((key) => ({
      key: key,
      value: RequirementStatus[key as keyof typeof RequirementStatus],
    }));

  // Форма
  editForm = this.fb.group({
    status: [RequirementStatus.NotStarted, Validators.required],
    deadline: [null as string | null], // Используем string для <input type="date">
    responsiblePerson: [''],
  });

  // Вычисляемый сигнал для title
  title = computed(() => this.requirement()?.title || 'Редактирование');

  constructor() {
    // Отслеживаем изменения входного 'requirement'
    computed(() => {
      const req = this.requirement();
      if (req) {
        this.editForm.patchValue({
          status: req.status,
          // Преобразуем Date в YYYY-MM-DD для <input type="date">
          deadline: req.deadline ? this.formatDateForInput(req.deadline) : null,
          responsiblePerson: req.responsiblePerson,
        });
      }
    });
  }

  // Преобразование Date -> YYYY-MM-DD
  private formatDateForInput(date: Date | string): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  onSave() {
    if (this.editForm.invalid || !this.requirement()) {
      return;
    }

    const formVal = this.editForm.value;

    const dto: UpdateRequirementDto = {
      status: Number(formVal.status), // <select> возвращает string
      deadline: formVal.deadline ? new Date(formVal.deadline) : undefined,
      responsiblePerson: formVal.responsiblePerson ?? undefined,
    };

    this.save.emit(dto);
  }

  onClose() {
    this.close.emit();
  }
}
