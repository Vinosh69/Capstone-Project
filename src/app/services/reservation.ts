import { Injectable } from '@angular/core';
import { timeout } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  private base = '/api/Reservation';

  constructor(private api: ApiService) {}

  createReservation(reservation: { propertyId: number; checkInDate: string; checkOutDate: string }) {
    return this.api.post(this.base, reservation).pipe(timeout(12000));
  }

  getMyReservations() {
    return this.api.get(`${this.base}/my`).pipe(timeout(12000));
  }

  getOwnerReservations() {
    return this.api.get(`${this.base}/owner`).pipe(timeout(12000));
  }

  updateReservation(id: number, reservation: unknown) {
    return this.api.put(`${this.base}/${id}`, reservation).pipe(timeout(12000));
  }

  cancelReservation(id: number) {
    return this.api.delete(`${this.base}/${id}`).pipe(timeout(12000));
  }
}
