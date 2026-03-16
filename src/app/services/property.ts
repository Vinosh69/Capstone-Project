import { Injectable } from '@angular/core';
import { ApiService, API_BASE } from './api.service';
import { timeout, throwError } from 'rxjs';

const PROPERTY_LIST_TIMEOUT_MS = 12000;
const SEARCH_TIMEOUT_MS = 12000;

@Injectable({
  providedIn: 'root'
})
export class PropertyService {

  private baseUrl = '/api/Property';

  constructor(private api: ApiService) {}

  getImageUrl(relativePath: string | null | undefined): string {
    if (!relativePath) return '';
    const path = relativePath.startsWith('/') ? relativePath : '/' + relativePath;
    return API_BASE + path;
  }

  getProperties() {
    return this.api.get(this.baseUrl).pipe(timeout(PROPERTY_LIST_TIMEOUT_MS));
  }

  getBookedDates(propertyId: number) {
    return this.api.get<{ checkIn: string; checkOut: string }[]>(`${this.baseUrl}/${propertyId}/booked-dates`);
  }

  getPropertyById(id: string | number) {
    const numId = typeof id === 'string' ? parseInt(id, 10) : id;
    if (isNaN(numId) || numId < 1) {
      return throwError(() => ({ status: 400, error: { message: 'Invalid property id' } }));
    }
    return this.api.get(`${this.baseUrl}/${numId}`).pipe(timeout(15000));
  }

  getTopRated(propertyType?: string, count = 10) {
    let url = `${this.baseUrl}/top-rated?count=${count}`;
    if (propertyType) url += `&propertyType=${encodeURIComponent(propertyType)}`;
    return this.api.get(url);
  }

  searchProperties(filters: {
    checkIn?: string;
    checkOut?: string;
    location?: string;
    propertyType?: string;
    featureIds?: number[];
  }) {
    const params = new URLSearchParams();
    if (filters.checkIn) params.set('checkIn', filters.checkIn);
    if (filters.checkOut) params.set('checkOut', filters.checkOut);
    if (filters.location) params.set('location', filters.location);
    if (filters.propertyType) params.set('propertyType', filters.propertyType);
    filters.featureIds?.forEach(fid => params.append('featureIds', fid.toString()));
    const qs = params.toString();
    return this.api.get(`${this.baseUrl}/search${qs ? '?' + qs : ''}`).pipe(timeout(SEARCH_TIMEOUT_MS));
  }

  getMyProperties() {
    return this.api.get(`${this.baseUrl}/my`);
  }

  createProperty(property: unknown) {
    return this.api.post(this.baseUrl, property);
  }

  deleteProperty(id: number) {
    return this.api.delete(`${this.baseUrl}/${id}`);
  }

  uploadImages(propertyId: number, files: File[]) {
    const formData = new FormData();
    files.forEach(f => formData.append('images', f));
    return this.api.postFormData(`${this.baseUrl}/${propertyId}/images`, formData);
  }
}
