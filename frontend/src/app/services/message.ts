import { Injectable } from '@angular/core';
import { timeout } from 'rxjs';

import { ApiService } from './api.service';

export interface AppUserLite {
  id: number;
  name?: string;
  email?: string;
  role?: string;
}

export interface AppPropertyLite {
  id: number;
  title?: string;
  location?: string;
  propertyType?: string;
  pricePerNight?: number;
  ownerId?: number;
}

export interface AppMessage {
  id: number;
  senderId: number;
  receiverId: number;
  propertyId: number;
  messageText: string;
  createdAt: string;
  sender?: AppUserLite;
  receiver?: AppUserLite;
  property?: AppPropertyLite;
}

export interface SendMessageRequest {
  receiverId: number;
  propertyId: number;
  messageText: string;
}

const MESSAGES_TIMEOUT_MS = 12000;

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private baseUrl = '/api/Message';

  constructor(private api: ApiService) {}

  getMessages() {
    return this.api.get<AppMessage[]>(this.baseUrl).pipe(timeout(MESSAGES_TIMEOUT_MS));
  }

  sendMessage(payload: SendMessageRequest) {
    return this.api.post<AppMessage>(this.baseUrl, payload).pipe(timeout(MESSAGES_TIMEOUT_MS));
  }
}

