import { Component, computed, inject, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats } from '../../core/models/dashboard.model';

interface QuickCard {
  title: string;
  description: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [BaseChartDirective, CurrencyPipe],
  templateUrl: './dashboard.html'
})
export class Dashboard {
  private readonly auth = inject(AuthService);
  private readonly dashboard = inject(DashboardService);

  readonly user = this.auth.user;
  readonly role = this.auth.role;
  readonly showStats = this.auth.hasRole('Admin', 'Receptionist');

  readonly stats = signal<DashboardStats | null>(null);
  readonly loadingStats = signal(false);

  readonly greeting = computed(() => {
    const h = new Date().getHours();
    return h < 12 ? 'Good morning' : h < 18 ? 'Good afternoon' : 'Good evening';
  });

  readonly bedOccupancyPct = computed(() => {
    const s = this.stats();
    if (!s || s.totalBeds === 0) return 0;
    return Math.round((s.occupiedBeds / s.totalBeds) * 100);
  });

  readonly revenueChart = computed<ChartConfiguration<'line'>['data']>(() => {
    const s = this.stats();
    return {
      labels: s?.revenueByMonth.map((r) => r.month) ?? [],
      datasets: [
        {
          data: s?.revenueByMonth.map((r) => r.total) ?? [],
          label: 'Revenue',
          fill: true,
          tension: 0.35,
          borderColor: '#0d6efd',
          backgroundColor: 'rgba(13,110,253,0.12)',
          pointBackgroundColor: '#0d6efd'
        }
      ]
    };
  });

  readonly statusChart = computed<ChartConfiguration<'doughnut'>['data']>(() => {
    const s = this.stats();
    return {
      labels: ['Booked', 'Completed', 'Cancelled', 'No-show'],
      datasets: [
        {
          data: s ? [s.appointmentsBooked, s.appointmentsCompleted, s.appointmentsCancelled, s.appointmentsNoShow] : [],
          backgroundColor: ['#0d6efd', '#198754', '#6c757d', '#dc3545']
        }
      ]
    };
  });

  readonly lineOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: false } },
    scales: { y: { beginAtZero: true } }
  };

  readonly doughnutOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { position: 'bottom' } }
  };

  readonly cards = computed<QuickCard[]>(() => {
    switch (this.role()) {
      case 'Doctor':
        return [
          { title: 'My Appointments', description: "Today's schedule & history", icon: 'bi-calendar-check', color: 'primary' },
          { title: 'Prescriptions', description: 'Diagnoses & medicines', icon: 'bi-file-medical', color: 'success' }
        ];
      default: // Patient
        return [
          { title: 'My Appointments', description: 'Upcoming & past visits', icon: 'bi-calendar-check', color: 'primary' },
          { title: 'My Prescriptions', description: 'Your medical records', icon: 'bi-file-medical', color: 'success' },
          { title: 'Billing', description: 'Your invoices', icon: 'bi-receipt', color: 'warning' }
        ];
    }
  });

  constructor() {
    if (this.showStats) {
      this.loadingStats.set(true);
      this.dashboard.getStats().subscribe({
        next: (s) => {
          this.stats.set(s);
          this.loadingStats.set(false);
        },
        error: () => this.loadingStats.set(false)
      });
    }
  }
}
