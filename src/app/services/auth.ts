import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private authApi = 'http://localhost:5185/api/Auth';

  constructor(private http: HttpClient) {}

  login(credentials: { email: string; password: string }) {
    return this.http.post<{ token: string }>(`${this.authApi}/login`, credentials);
  }

  register(userData: { name: string; email: string; passwordHash: string; role: string }) {
    return this.http.post(this.authApi + '/register', userData);
  }
}
