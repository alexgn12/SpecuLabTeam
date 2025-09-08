import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ToastService {
  private messageSubject = new BehaviorSubject<string>('');
  private showSubject = new BehaviorSubject<boolean>(false);

  message$ = this.messageSubject.asObservable();
  show$ = this.showSubject.asObservable();

  show(message: string) {
    this.messageSubject.next(message);
    this.showSubject.next(true);
  }

  hide() {
    this.showSubject.next(false);
  }
}
