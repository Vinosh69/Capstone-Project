import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { ReservationService } from '../../services/reservation';
import { NotificationService } from '../../services/notification';
import { MessageService } from '../../services/message';

@Component({
  selector: 'app-renter-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './renter-dashboard.html',
  styleUrls: ['./renter-dashboard.css']
})
export class RenterDashboard implements OnInit {

  reservations: any[] = [];
  loading = false;
  loadError: string | null = null;
  cancellingIds = new Set<number>();

  // Chat (renter <-> owner)
  loadingMessages = false;
  messages: any[] = [];
  messageText = '';
  selectedChatReservationId: number | null = null;
  selectedChatPropertyId: number | null = null;
  selectedChatReceiverId: number | null = null; // owner user id

  /** Reservation status summary for the top section (Pending first, newest first within each group). */
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
    private messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    if (!this.hasRenterRole()) {
      this.router.navigate(['/login']);
      return;
    }
    this.fetchReservations();
    this.fetchMessages();
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
        this.ensureChatSelection();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.warn('getMyReservations failed', err);
        const msg = typeof err?.response?.data === 'string'
          ? err.response.data
          : (err?.response?.data?.message ?? err?.message);
        this.loadError = msg || 'Could not load reservations.';
        this.notifier.add(this.loadError || 'Could not load reservations.', 'error');
        this.cdr.detectChanges();
      }
    });
  }

  fetchMessages(): void {
    this.loadingMessages = true;
    this.loadError = null;
    this.cdr.detectChanges();
    this.messageService.getMessages().pipe(
      finalize(() => {
        this.loadingMessages = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (data: any) => {
        this.messages = Array.isArray(data) ? data : (data?.data ?? data?.items ?? []);
      },
      error: (err) => {
        console.warn('fetchMessages failed', err);
        this.messages = [];
      }
    });
  }

  private ensureChatSelection(): void {
    if (this.selectedChatReservationId != null && this.selectedChatPropertyId != null && this.selectedChatReceiverId != null) return;
    const first = this.reservations?.[0];
    if (!first) return;
    this.selectedChatReservationId = first.id ?? null;
    this.selectedChatPropertyId = first.propertyId ?? first.property?.id ?? null;
    this.selectedChatReceiverId = first.property?.ownerId ?? first.ownerId ?? null;
  }

  onChatReservationSelected(reservationId: number): void {
    const r = this.reservations.find(x => x.id === reservationId);
    if (!r) return;
    this.selectedChatReservationId = reservationId;
    this.selectedChatPropertyId = r.propertyId ?? r.property?.id ?? null;
    this.selectedChatReceiverId = r.property?.ownerId ?? null;
  }

  get chatMessages(): any[] {
    if (this.selectedChatPropertyId == null || this.selectedChatReceiverId == null) return [];
    return (this.messages ?? []).filter(m =>
      m?.propertyId === this.selectedChatPropertyId &&
      (m?.senderId === this.selectedChatReceiverId || m?.receiverId === this.selectedChatReceiverId)
    );
  }

  get selectedChatPartnerLabel(): string {
    if (this.selectedChatReservationId == null) return '';
    const r = this.reservations.find(x => x.id === this.selectedChatReservationId);
    if (!r) return '';
    return r?.property?.ownerName ?? r?.property?.ownerEmail ?? 'Owner';
  }

  sendMessage(): void {
    if (this.selectedChatReceiverId == null || this.selectedChatPropertyId == null) return;
    const text = this.messageText.trim();
    if (!text) return;

    const payload = {
      receiverId: this.selectedChatReceiverId,
      propertyId: this.selectedChatPropertyId,
      messageText: text
    };

    this.messageService.sendMessage(payload).subscribe({
      next: () => {
        this.messageText = '';
        this.fetchMessages();
        this.notifier.add('Message sent.', 'success', 3000);
      },
      error: (err) => {
        console.warn('sendMessage failed', err);
        const msg = typeof err?.error === 'string'
          ? err.error
          : (err?.error?.message ?? err?.message);
        this.notifier.add(msg || 'Could not send message.', 'error', 5000);
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
