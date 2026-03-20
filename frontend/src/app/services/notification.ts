import { Injectable } from '@angular/core';

export interface NotificationMessage {
  id: number;
  text: string;
  type: 'success' | 'info' | 'error';
}

@Injectable({ providedIn: 'root' })
export class NotificationService {

  msgIdCounter = 1;
  messages: NotificationMessage[] = [];

  add(text: string, type: 'success' | 'info' | 'error' = 'info', durationMs?: number): void {
    const id = this.msgIdCounter++;
    this.messages.push({ id, text, type });
    const duration = durationMs ?? (type === 'success' ? 5000 : 3000);
    setTimeout(() => this.remove(id), duration);
  }

  remove(id: number): void {
    this.messages = this.messages.filter(m => m.id !== id);
  }

  clear(): void {
    this.messages = [];
  }
}
