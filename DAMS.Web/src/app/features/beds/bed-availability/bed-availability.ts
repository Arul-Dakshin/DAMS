import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WardService } from '../../../core/services/ward.service';
import { AuthService } from '../../../core/services/auth.service';
import { Bed, BedStatus, Ward } from '../../../core/models/ward.model';
import { EmptyState, LoadingSpinner, PageHeader } from '../../../shared/ui';

@Component({
  selector: 'app-bed-availability',
  imports: [FormsModule, PageHeader, LoadingSpinner, EmptyState],
  templateUrl: './bed-availability.html',
  styleUrl: './bed-availability.css'
})
export class BedAvailability {
  private readonly wardService = inject(WardService);
  private readonly auth = inject(AuthService);

  readonly wards = signal<Ward[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly isAdmin = this.auth.hasRole('Admin');

  newWardName = '';
  newWardCategory = '';
  newBedNumber: Record<number, string> = {};

  readonly totals = computed(() => {
    const beds = this.wards().flatMap((w) => w.beds);
    return {
      total: beds.length,
      available: beds.filter((b) => b.status === 'Available').length,
      occupied: beds.filter((b) => b.status === 'Occupied').length,
      maintenance: beds.filter((b) => b.status === 'Maintenance').length
    };
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.wardService.getWards().subscribe({
      next: (w) => {
        this.wards.set(w);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load wards.');
        this.loading.set(false);
      }
    });
  }

  bedClass(status: BedStatus): string {
    const map: Record<BedStatus, string> = {
      Available: 'bed-available',
      Occupied: 'bed-occupied',
      Maintenance: 'bed-maintenance'
    };
    return map[status];
  }

  addWard(): void {
    if (!this.newWardName.trim()) return;
    this.wardService
      .createWard({ name: this.newWardName.trim(), category: this.newWardCategory.trim() || null })
      .subscribe({
        next: () => {
          this.newWardName = '';
          this.newWardCategory = '';
          this.load();
        },
        error: (e) => this.error.set(e?.error?.message ?? 'Could not add ward.')
      });
  }

  deleteWard(w: Ward): void {
    if (!confirm(`Delete ward "${w.name}" and all its beds?`)) return;
    this.wardService.deleteWard(w.id).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Could not delete ward.')
    });
  }

  addBed(w: Ward): void {
    const n = (this.newBedNumber[w.id] || '').trim();
    if (!n) return;
    this.wardService.addBed(w.id, n).subscribe({
      next: () => {
        this.newBedNumber[w.id] = '';
        this.load();
      },
      error: (e) => this.error.set(e?.error?.message ?? 'Could not add bed.')
    });
  }

  deleteBed(w: Ward, b: Bed): void {
    this.wardService.deleteBed(w.id, b.id).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Could not delete bed.')
    });
  }

  toggleMaintenance(b: Bed): void {
    const next: BedStatus = b.status === 'Maintenance' ? 'Available' : 'Maintenance';
    this.wardService.updateBedStatus(b.id, next).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Could not update bed.')
    });
  }
}
