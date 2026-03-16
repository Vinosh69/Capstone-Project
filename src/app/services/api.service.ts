import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Central API client using Axios.
 * - Single base URL and timeout
 * - Request interceptor adds Bearer token from localStorage
 * - Emits inside Angular zone so UI updates immediately
 */
export const API_BASE = 'http://localhost:5185';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) {}

  private resolveUrl(url: string): string {
    if (/^https?:\/\//i.test(url)) return url;
    if (url.startsWith('/')) return API_BASE + url;
    return `${API_BASE}/${url}`;
  }

  private normalizeOptions(options?: any) {
    return options ?? {};
  }

  get<T = unknown>(url: string, options?: any): Observable<T> {
    const opts = { ...this.normalizeOptions(options), observe: 'body' as const };
    return this.http.get<T>(this.resolveUrl(url), opts as any) as Observable<T>;
  }

  post<T = unknown>(url: string, data?: unknown, options?: any): Observable<T> {
    const opts = { ...this.normalizeOptions(options), observe: 'body' as const };
    return this.http.post<T>(this.resolveUrl(url), data, opts as any) as Observable<T>;
  }

  put<T = unknown>(url: string, data?: unknown, options?: any): Observable<T> {
    const opts = { ...this.normalizeOptions(options), observe: 'body' as const };
    return this.http.put<T>(this.resolveUrl(url), data, opts as any) as Observable<T>;
  }

  delete<T = unknown>(url: string, options?: any): Observable<T> {
    const opts = { ...this.normalizeOptions(options), observe: 'body' as const };
    return this.http.delete<T>(this.resolveUrl(url), opts as any) as Observable<T>;
  }

  postFormData<T = unknown>(url: string, formData: FormData, options?: any): Observable<T> {
    // For FormData, do not set Content-Type; the browser will add the boundary.
    const opts = { ...this.normalizeOptions(options), observe: 'body' as const };
    return this.http.post<T>(this.resolveUrl(url), formData, opts as any) as Observable<T>;
  }
}

