import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { InvoiceService } from '../../../core/services/invoice.service';
import { AuthService } from '../../../core/services/auth.service';
import { Invoice } from '../../../core/models/invoice.model';
import { EmptyState, LoadingSpinner, PageHeader, StatusBadge } from '../../../shared/ui';

@Component({
  selector: 'app-invoice-list',
  imports: [RouterLink, CurrencyPipe, DatePipe, PageHeader, LoadingSpinner, EmptyState, StatusBadge],
  templateUrl: './invoice-list.html'
})
export class InvoiceList {
  private readonly service = inject(InvoiceService);
  private readonly auth = inject(AuthService);

  readonly invoices = signal<Invoice[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly canManage = this.auth.hasRole('Admin', 'Receptionist');

  readonly outstanding = computed(() =>
    this.invoices().filter((i) => i.status === 'Unpaid').reduce((sum, i) => sum + i.total, 0)
  );

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll().subscribe({
      next: (i) => {
        this.invoices.set(i);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load invoices.');
        this.loading.set(false);
      }
    });
  }
}
