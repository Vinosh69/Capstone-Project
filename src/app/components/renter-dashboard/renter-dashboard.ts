import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { ReservationService } from '../../services/reservation';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-renter-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './renter-dashboard.html',
  styleUrls: ['./renter-dashboard.css']
})
export class RenterDashboard implements OnInit {

  reservations: any[] = [];
  loading = false;
  loadError: string | null = null;
  cancellingIds = new Set<number>();

  /** All reservations as updates: Pending (awaiting approval), Confirmed (approved), Rejected. Most recent first, Pending first. */
  get statusUpdates(): { propertyTitle: string; status: string; checkInDate: string; checkOutDate: string }[] {
    const map = (r: any) => ({
      propertyTitle: r.property?.title ?? 'Property',
      status: r.status || 'Pending',
      checkInDate: r.checkInDate,
      checkOutDate: r.checkOutDate
    });
    const pending = this.reservations.filter(r => r.status === 'Pending').map(map);
    const decided = this.reservations.filter(r => r.status === 'Confirmed' || r.status === 'Rejected').map(map);
    return [...pending, ...decided];
  }

  get pendingCount(): number {
    return this.reservations.filter(r => r.status === 'Pending').length;
  }

  constructor(
    private reservationService: ReservationService,
    private router: Router,
    private notifier: NotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.hasRenterRole()) {
      this.router.navigate(['/login']);
      return;
    }
    this.fetchReservations();
  }

  fetchReservations(): void {
    this.loading = true;
    this.loadError = null;
    this.cdr.detectChanges();
    this.reservationService.getMyReservations().pipe(
      finalize(() => {
        this.loading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (data: any) => {
        this.reservations = Array.isArray(data) ? data : (data?.data ?? data?.items ?? (data != null ? [data] : []));
        this.loadError = null;
        // extra safety
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.warn('getMyReservations failed', err);
        const msg = typeof err?.response?.data === 'string'
          ? err.response.data
          : (err?.response?.data?.message ?? err?.message);
        this.loadError = msg || 'Could not load reservations.';
        this.notifier.add(this.loadError || 'Could not load reservations.', 'error');
        // extra safety
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  cancelReservation(res: any): void {
    const id = res?.id;
    if (typeof id === 'number') this.cancellingIds.add(id);
    this.reservationService.cancelReservation(res.id).subscribe({
      next: () => {
        this.fetchReservations();
        this.notifier.add('Reservation cancelled.', 'info');
      },
      error: (err) => {
        console.warn('cancelReservation failed', err);
        this.notifier.add('Could not cancel reservation.', 'error');
      },
      complete: () => { if (typeof id === 'number') this.cancellingIds.delete(id); }
    });
  }

  private hasRenterRole(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    try {
      const [, payloadB64] = token.split('.');
      if (!payloadB64) return false;
      const payload = JSON.parse(atob(payloadB64.replace(/-/g, '+').replace(/_/g, '/')));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'] ?? '';
      return role === 'User';
    } catch {
      return false;
    }
  }
}
