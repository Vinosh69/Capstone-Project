import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { filter } from 'rxjs/operators';

import { PropertyService } from '../../services/property';
import { ReservationService } from '../../services/reservation';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.css']
})
export class Home implements OnInit {

  properties: any[] = [];
  loading = false;
  loadError: string | null = null;
  reservingIds = new Set<number>();
  wishlist = new Set<number>();
  imageErrors: Record<number, boolean> = {};

  filters = {
    location: '',
    propertyType: '',
    checkIn: '',
    checkOut: ''
  };

  constructor(
    public propertyService: PropertyService,
    private reservationService: ReservationService,
    private notifier: NotificationService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  private hasNavigatedOnce = false;

  ngOnInit(): void {
    this.loadWishlist();
    this.fetchProperties();
    // If the user navigates away and comes back, refresh the listing once.
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => {
        const onHome = e.urlAfterRedirects === '/' || e.urlAfterRedirects === '';
        if (onHome && this.hasNavigatedOnce) {
          this.fetchProperties();
        }
        if (onHome) this.hasNavigatedOnce = true;
      });
  }

  private loadWishlist(): void {
    try {
      const raw = localStorage.getItem('favorites');
      this.wishlist = new Set(raw ? JSON.parse(raw) : []);
    } catch {
      this.wishlist = new Set();
    }
  }

  private persistWishlist(): void {
    localStorage.setItem('favorites', JSON.stringify([...this.wishlist]));
  }

  inWishlist(property: any): boolean {
    return property?.id != null && this.wishlist.has(property.id);
  }

  onImageError(propertyId: number): void {
    this.imageErrors = { ...this.imageErrors, [propertyId]: true };
  }

  goToReserve(property: any): void {
    if (!property?.id) return;
    const q: any = {};
    if (this.filters.checkIn) q.checkIn = this.filters.checkIn;
    if (this.filters.checkOut) q.checkOut = this.filters.checkOut;
    this.router.navigate(['/property', property.id], { queryParams: Object.keys(q).length ? q : undefined });
  }

  canQuickReserve(): boolean {
    return !!(this.filters.checkIn && this.filters.checkOut);
  }

  toggleWishlist(property: any): void {
    const id = property?.id;
    if (typeof id !== 'number') return;
    if (this.wishlist.has(id)) {
      this.wishlist.delete(id);
      this.notifier.add('Removed from wishlist.', 'info');
    } else {
      this.wishlist.add(id);
      this.notifier.add('Added to wishlist.', 'success');
    }
    this.persistWishlist();
  }

  fetchProperties(): void {
    this.loadError = null;
    this.loading = true;
    this.cdr.markForCheck();
    this.propertyService.getProperties().pipe(
      finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      })
    ).subscribe({
      next: (data: any) => {
        const list = Array.isArray(data) ? data : (data?.data ?? data?.items ?? (data != null ? [data] : []));
        this.properties = Array.isArray(list) ? list : [];
        this.loadError = null;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.warn('fetchProperties failed', err);
        this.loadError = err?.name === 'TimeoutError' || err?.message?.includes('timeout')
          ? 'Request timed out. Check your connection and try again.'
          : 'Could not load properties.';
        this.notifier.add(this.loadError, 'error');
        this.cdr.markForCheck();
      }
    });
  }

  search(): void {
    const opts: any = {
      location: this.filters.location || undefined,
      propertyType: this.filters.propertyType || undefined,
      checkIn: this.filters.checkIn || undefined,
      checkOut: this.filters.checkOut || undefined
    };
    this.loadError = null;
    this.loading = true;
    this.cdr.markForCheck();
    this.propertyService.searchProperties(opts).pipe(
      finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      })
    ).subscribe({
      next: (data: any) => {
        const list = Array.isArray(data) ? data : (data?.data ?? data?.items ?? (data != null ? [data] : []));
        this.properties = Array.isArray(list) ? list : [];
        this.loadError = null;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.warn('search failed', err);
        this.loadError = err?.name === 'TimeoutError' || err?.message?.includes('timeout')
          ? 'Search timed out. Try again.'
          : 'Search failed. Please try again.';
        this.notifier.add(this.loadError, 'error');
        this.cdr.markForCheck();
      }
    });
  }

  requestReservation(property: any): void {
    const propId = property?.id;
    const token = localStorage.getItem('token');
    if (!token) {
      this.notifier.add('Please login before making a reservation.', 'info');
      this.router.navigate(['/login']);
      return;
    }
    if (!this.filters.checkIn || !this.filters.checkOut) {
      this.notifier.add('Please select check-in and check-out dates before reserving.', 'error');
      return;
    }

    const payload = {
      propertyId: propId,
      checkInDate: this.filters.checkIn,
      checkOutDate: this.filters.checkOut
    };
    if (typeof propId === 'number') this.reservingIds.add(propId);

    this.reservationService.createReservation(payload).subscribe({
      next: () => {
        this.notifier.add('Reservation request sent! Check Trips for status (Pending → Approved when owner confirms).', 'success', 6000);
      },
      error: (err) => {
        console.warn('createReservation failed', err);
        const msg = typeof err?.error === 'string' ? err.error : (err?.error?.message ?? err?.message);
        this.notifier.add(msg || 'Could not create reservation.', 'error');
      },
      complete: () => {
        if (typeof propId === 'number') this.reservingIds.delete(propId);
      }
    });
  }
}
