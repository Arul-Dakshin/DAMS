import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

interface QuickCard {
  title: string;
  description: string;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html'
})
export class Dashboard {
  private readonly auth = inject(AuthService);
  readonly user = this.auth.user;
  readonly role = this.auth.role;

  readonly greeting = computed(() => {
    const h = new Date().getHours();
    return h < 12 ? 'Good morning' : h < 18 ? 'Good afternoon' : 'Good evening';
  });

  // Placeholder feature cards — populated with live data in later phases.
  readonly cards = computed<QuickCard[]>(() => {
    switch (this.role()) {
      case 'Admin':
        return [
          { title: 'Patients', description: 'Registrations & demographics', icon: 'bi-people', color: 'primary' },
          { title: 'Doctors', description: 'Staff & schedules', icon: 'bi-person-badge', color: 'success' },
          { title: 'Beds', description: 'Bed availability (coming soon)', icon: 'bi-hospital', color: 'info' },
          { title: 'Revenue', description: 'Billing charts (coming soon)', icon: 'bi-graph-up-arrow', color: 'warning' }
        ];
      case 'Doctor':
        return [
          { title: 'My Appointments', description: 'Today\'s schedule', icon: 'bi-calendar-check', color: 'primary' },
          { title: 'Prescriptions', description: 'Diagnoses & meds (coming soon)', icon: 'bi-file-medical', color: 'success' }
        ];
      case 'Receptionist':
        return [
          { title: 'Register Patient', description: 'New patient intake', icon: 'bi-person-plus', color: 'primary' },
          { title: 'Book Appointment', description: 'Schedule a visit', icon: 'bi-calendar-plus', color: 'success' }
        ];
      default:
        return [
          { title: 'My Appointments', description: 'Upcoming visits', icon: 'bi-calendar-check', color: 'primary' },
          { title: 'My Prescriptions', description: 'Records (coming soon)', icon: 'bi-file-medical', color: 'success' }
        ];
    }
  });
}
