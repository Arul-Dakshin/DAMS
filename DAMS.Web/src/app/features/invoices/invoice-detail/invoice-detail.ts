import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { InvoiceService } from '../../../core/services/invoice.service';
import { AuthService } from '../../../core/services/auth.service';
import { Invoice } from '../../../core/models/invoice.model';

@Component({
  selector: 'app-invoice-detail',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './invoice-detail.html'
})
export class InvoiceDetail {
  private readonly service = inject(InvoiceService);
  private readonly route = inject(ActivatedRoute);
  private readonly auth = inject(AuthService);

  readonly invoice = signal<Invoice | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly paying = signal(false);
  readonly canManage = this.auth.hasRole('Admin', 'Receptionist');

  constructor() {
    this.load();
  }

  load(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.service.getById(id).subscribe({
      next: (i) => {
        this.invoice.set(i);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load this invoice.');
        this.loading.set(false);
      }
    });
  }

  markPaid(): void {
    const inv = this.invoice();
    if (!inv) return;
    this.paying.set(true);
    this.service.markPaid(inv.id).subscribe({
      next: (i) => {
        this.invoice.set(i);
        this.paying.set(false);
      },
      error: (e) => {
        this.error.set(e?.error?.message ?? 'Could not mark as paid.');
        this.paying.set(false);
      }
    });
  }

  print(): void {
    window.print();
  }
}
