import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, switchMap, EMPTY, finalize, distinctUntilChanged } from 'rxjs';

import { PropertyService } from '../../services/property';
import { ReservationService } from '../../services/reservation';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-property-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './property-details.html',
  styleUrls: ['./property-details.css']
})
export class PropertyDetails implements OnInit, OnDestroy {

  property: any = null;
  checkIn = '';
  checkOut = '';
  reserving = false;
  reservationSent = false;
  loading = true;
  loadError: string | null = null;
  notFound = false;
  imageErrorIndices = new Set<number>();
  minDate = '';

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    public propertyService: PropertyService,
    private reservationService: ReservationService,
    private notifier: NotificationService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    const today = new Date();
    this.minDate = today.toISOString().slice(0, 10);
  }

  get nightsCount(): number {
    if (!this.checkIn || !this.checkOut) return 0;
    const a = new Date(this.checkIn);
    const b = new Date(this.checkOut);
    const nights = Math.round((b.getTime() - a.getTime()) / (1000 * 60 * 60 * 24));
    return nights > 0 ? nights : 0;
  }

  get totalPrice(): number {
    if (!this.property?.pricePerNight || this.nightsCount <= 0) return 0;
    return Number(this.property.pricePerNight) * this.nightsCount;
  }

  onImageError(index: number): void {
    this.imageErrorIndices.add(index);
  }

  ngOnInit(): void {
    this.route.queryParamMap.pipe(takeUntil(this.destroy$)).subscribe(q => {
      const checkIn = q.get('checkIn');
      const checkOut = q.get('checkOut');
      if (checkIn) this.checkIn = checkIn;
      if (checkOut) this.checkOut = checkOut;
    });

    this.route.paramMap
      .pipe(
        takeUntil(this.destroy$),
        distinctUntilChanged((a, b) => a.get('id') === b.get('id')),
        switchMap(params => {
          const id = params.get('id');
          this.property = null;
          this.loadError = null;
          this.notFound = false;
          this.imageErrorIndices.clear();
          this.loading = true;
          this.cdr.markForCheck();
          if (!id) {
            this.loading = false;
            this.loadError = 'Missing property id.';
            this.cdr.markForCheck();
            return EMPTY;
          }
          return this.propertyService.getPropertyById(id).pipe(
            finalize(() => {
              this.loading = false;
              this.cdr.markForCheck();
            })
          );
        })
      )
      .subscribe({
        next: (data: any) => {
          this.property = data ?? null;
          this.loadError = null;
          this.cdr.markForCheck();
        },
        error: (err) => {
          const isTimeout = err?.name === 'TimeoutError' || err?.message?.includes('timeout');
          this.loadError = isTimeout
            ? 'Request timed out. Please check your connection and try again.'
            : (err?.error?.message || (err?.status === 404 ? 'Property not found.' : 'Failed to load property.'));
          this.notFound = err?.status === 404;
          this.notifier.add(this.loadError ?? 'Failed to load property.', 'error');
          this.cdr.markForCheck();
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  reserveProperty(): void {
    if (!this.property?.id) return;
    const token = localStorage.getItem('token');
    if (!token) {
      this.notifier.add('Please login before making a reservation.', 'info');
      this.router.navigate(['/login']);
      return;
    }
    if (!this.checkIn || !this.checkOut) {
      this.notifier.add('Please select check-in and check-out dates.', 'error');
      return;
    }

    const payload = {
      propertyId: this.property.id,
      checkInDate: this.checkIn,
      checkOutDate: this.checkOut
    };
    this.reserving = true;
    this.reservationSent = false;
    this.reservationService.createReservation(payload).subscribe({
      next: () => {
        this.reservationSent = true;
        this.notifier.add('Reservation request sent! Check Trips for status (Pending → Approved when owner confirms).', 'success', 6000);
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.warn('reserve failed', err);
        const msg = typeof err?.error === 'string' ? err.error : (err?.error?.message ?? err?.message);
        this.notifier.add(msg || 'Could not create reservation.', 'error');
        this.cdr.markForCheck();
      },
      complete: () => {
        this.reserving = false;
        this.cdr.markForCheck();
      }
    });
  }
}
