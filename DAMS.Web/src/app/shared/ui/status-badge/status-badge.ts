import { Component, computed, input } from '@angular/core';

/**
 * Presentational badge that maps any domain status to a consistent Bootstrap colour.
 * Centralising the colour map keeps status styling identical across every feature.
 */
@Component({
  selector: 'app-status-badge',
  template: `<span class="badge {{ cssClass() }}">{{ label() }}</span>`
})
export class StatusBadge {
  readonly status = input.required<string>();

  private readonly colours: Record<string, string> = {
    Booked: 'bg-primary',
    Completed: 'bg-success',
    Cancelled: 'bg-secondary',
    NoShow: 'bg-danger',
    Paid: 'bg-success',
    Unpaid: 'bg-warning text-dark',
    Available: 'bg-success',
    Occupied: 'bg-danger',
    Maintenance: 'bg-warning text-dark',
    Admitted: 'bg-primary',
    Discharged: 'bg-secondary'
  };

  readonly cssClass = computed(() => this.colours[this.status()] ?? 'bg-secondary');
  readonly label = computed(() => (this.status() === 'NoShow' ? 'No-show' : this.status()));
}
