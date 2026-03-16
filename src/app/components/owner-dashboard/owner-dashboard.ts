import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { PropertyService } from '../../services/property';
import { ReservationService } from '../../services/reservation';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-owner-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './owner-dashboard.html',
  styleUrls: ['./owner-dashboard.css']
})
export class OwnerDashboard implements OnInit {

  properties: any[] = [];
  reservations: any[] = [];
  loadingProps = false;
  loadingRes = false;
  addingProperty = false;
  deletingIds = new Set<number>();
  updatingResIds = new Set<number>();

  newProp = {
    title: '',
    description: '',
    location: '',
    propertyType: '',
    pricePerNight: 0
  };

  selectedFiles: File[] = [];

  /** Pending reservations (new requests) for the notification section. */
  get pendingReservations(): any[] {
    return this.reservations.filter(r => r.status === 'Pending');
  }

  get pendingCount(): number {
    return this.pendingReservations.length;
  }

  constructor(
    private propertyService: PropertyService,
    private reservationService: ReservationService,
    private notifier: NotificationService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.userIsOwner()) {
      this.router.navigate(['/login']);
      return;
    }
    this.fetchProperties();
    this.fetchReservations();
  }

  fetchProperties(): void {
    this.loadingProps = true;
    this.cdr.markForCheck();
    this.propertyService.getMyProperties().pipe(
      finalize(() => { this.loadingProps = false; this.cdr.markForCheck(); })
    ).subscribe({
      next: (data: any) => {
        this.properties = Array.isArray(data) ? data : (data?.data ?? data?.items ?? (data != null ? [data] : []));
        this.cdr.markForCheck();
      },
      error: (err) => { console.warn('fetchProperties failed', err); this.cdr.markForCheck(); }
    });
  }

  fetchReservations(): void {
    this.loadingRes = true;
    this.cdr.markForCheck();
    this.reservationService.getOwnerReservations().pipe(
      finalize(() => { this.loadingRes = false; this.cdr.markForCheck(); })
    ).subscribe({
      next: (data: any) => {
        this.reservations = Array.isArray(data) ? data : (data?.data ?? data?.items ?? (data != null ? [data] : []));
        this.cdr.markForCheck();
      },
      error: (err) => { console.warn('fetchReservations failed', err); this.cdr.markForCheck(); }
    });
  }

  onFilesSelected(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  addProperty(): void {
    const payload = {
      title: this.newProp.title,
      description: this.newProp.description,
      location: this.newProp.location,
      propertyType: this.newProp.propertyType,
      pricePerNight: this.newProp.pricePerNight,
      rating: 0
    };
    this.addingProperty = true;
    this.propertyService.createProperty(payload).subscribe({
      next: (created: any) => {
        const filesToUpload = this.selectedFiles.length > 0 ? [...this.selectedFiles] : [];
        // Update UI immediately: add new property to list and clear form (no wait for image upload)
        this.properties = [...this.properties, created];
        this.doneAdding(true, true);
        if (filesToUpload.length > 0 && created?.id) {
          this.uploadImagesInBackground(created.id, filesToUpload);
        }
      },
      error: (err) => {
        console.warn('createProperty failed', err);
        this.notifier.add('Could not add property.', 'error');
        this.addingProperty = false;
      }
    });
  }

  /** Upload images after property is created; runs in background so UI stays responsive. */
  private uploadImagesInBackground(propertyId: number, files: File[]): void {
    this.propertyService.uploadImages(propertyId, files).subscribe({
      next: () => {
        this.notifier.add('Property images uploaded.', 'success');
        this.fetchProperties();
      },
      error: (err) => {
        console.warn('image upload failed', err);
        this.notifier.add('Property created, but image upload failed.', 'error');
        this.fetchProperties();
      }
    });
  }

  private doneAdding(showSuccess: boolean, skipRefresh = false): void {
    this.newProp = { title: '', description: '', location: '', propertyType: '', pricePerNight: 0 };
    this.selectedFiles = [];
    const input = document.getElementById('propertyImages') as HTMLInputElement;
    if (input) input.value = '';
    if (!skipRefresh) this.fetchProperties();
    if (showSuccess) this.notifier.add('Property added successfully!', 'success');
    this.addingProperty = false;
  }

  deleteProperty(p: any): void {
    const id = p?.id;
    if (typeof id === 'number') this.deletingIds.add(id);
    this.propertyService.deleteProperty(p.id).subscribe({
      next: () => {
        this.fetchProperties();
        this.notifier.add('Property deleted.', 'info');
      },
      error: (err) => {
        console.warn('deleteProperty failed', err);
        this.notifier.add('Could not delete property.', 'error');
      },
      complete: () => { if (typeof id === 'number') this.deletingIds.delete(id); }
    });
  }

  setReservationStatus(r: any, status: 'Confirmed' | 'Rejected'): void {
    const payload = { ...r, status };
    const id = r?.id;
    const title = r.property?.title || 'Property';
    const renterName = r.renter?.name || r.renter?.email || 'Renter';
    // Update UI immediately so it feels instant
    this.reservations = this.reservations.map(res => res.id === id ? { ...res, status } : res);
    this.cdr.markForCheck();
    if (typeof id === 'number') this.updatingResIds.add(id);
    this.reservationService.updateReservation(r.id, payload).subscribe({
      next: () => {
        if (status === 'Confirmed') {
          this.notifier.add(`Approved: ${title}. ${renterName} has been notified.`, 'success', 5000);
        } else {
          this.notifier.add(`Rejected: ${title}. ${renterName} has been notified.`, 'info', 4000);
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.warn('updateReservation failed', err);
        this.reservations = this.reservations.map(res => res.id === id ? { ...res, status: 'Pending' } : res);
        const msg = typeof err?.error === 'string' ? err.error : (err?.error?.message ?? err?.message);
        this.notifier.add(msg || 'Could not update reservation.', 'error');
        this.cdr.markForCheck();
      },
      complete: () => { if (typeof id === 'number') this.updatingResIds.delete(id); this.cdr.markForCheck(); }
    });
  }

  private userIsOwner(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return false;
      const payload = JSON.parse(atob(parts[1].replace(/-/g, '+').replace(/_/g, '/')));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'] ?? '';
      return role === 'Owner';
    } catch {
      return false;
    }
  }
}
