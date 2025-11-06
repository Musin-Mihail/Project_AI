import { Component, inject, Input, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinancialDocumentDto, FinancialDocumentStatus, FinancialDocumentType } from '../models';
import { FinancialDocumentService } from '../financial-document.service';

@Component({
  selector: 'app-financial-document-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './financial-document-list.component.html',
  styleUrl: './financial-document-list.component.scss',
})
export class FinancialDocumentListComponent implements OnInit {
  @Input({ required: true }) siteId!: number;

  private docService = inject(FinancialDocumentService);

  documents: WritableSignal<FinancialDocumentDto[]> = signal([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  // Enums для использования в шаблоне
  FinancialDocumentType = FinancialDocumentType;
  FinancialDocumentStatus = FinancialDocumentStatus;

  ngOnInit() {
    if (this.siteId > 0) {
      this.loadDocuments();
    }
  }

  loadDocuments() {
    this.isLoading.set(true);
    this.docService.getDocumentsForSite(this.siteId).subscribe({
      next: (data) => {
        this.documents.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Не удалось загрузить фин. документы: ' + err.message);
        this.isLoading.set(false);
      },
    });
  }

  // --- Вспомогательные функции для отображения Enums ---

  getDocumentTypeText(type: FinancialDocumentType): string {
    switch (type) {
      case FinancialDocumentType.Contract:
        return 'Договор';
      case FinancialDocumentType.Invoice:
        return 'Счет';
      case FinancialDocumentType.Act:
        return 'Акт';
      default:
        return 'Неизвестно';
    }
  }

  getDocumentStatusText(status: FinancialDocumentStatus): string {
    switch (status) {
      case FinancialDocumentStatus.Draft:
        return 'Черновик';
      case FinancialDocumentStatus.Sent:
        return 'Отправлен';
      case FinancialDocumentStatus.Paid:
        return 'Оплачен';
      case FinancialDocumentStatus.Overdue:
        return 'Просрочен';
      default:
        return 'Неизвестно';
    }
  }
}
