import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { Role } from '../../core/models/auth.models';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: Role[];
}

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrl: './shell.css'
})
export class Shell {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly user = this.auth.user;
  readonly role = this.auth.role;

  // Nav items are role-filtered; new feature links are appended in later phases.
  private readonly allNav: NavItem[] = [
    { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard', roles: ['Admin', 'Doctor', 'Receptionist', 'Patient'] },
    { label: 'Patients', icon: 'bi-people', route: '/patients', roles: ['Admin', 'Doctor', 'Receptionist'] },
    { label: 'Doctors', icon: 'bi-person-badge', route: '/doctors', roles: ['Admin', 'Doctor', 'Receptionist'] },
    { label: 'Appointments', icon: 'bi-calendar-check', route: '/appointments', roles: ['Admin', 'Doctor', 'Receptionist', 'Patient'] },
    { label: 'My Profile', icon: 'bi-person-vcard', route: '/my-profile', roles: ['Patient'] }
  ];

  readonly navItems = computed<NavItem[]>(() => {
    const r = this.role();
    return r ? this.allNav.filter((n) => n.roles.includes(r)) : [];
  });

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
