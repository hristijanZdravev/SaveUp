import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type AppLocale = 'en' | 'mk';

@Injectable({
  providedIn: 'root'
})
export class LocaleService {
  private readonly defaultLocale: AppLocale = 'en';
  private readonly localeSubject = new BehaviorSubject<AppLocale>(this.defaultLocale);

  readonly locale$ = this.localeSubject.asObservable();

  constructor() {
    this.applyLocale(this.defaultLocale);
  }

  get locale(): AppLocale {
    return this.localeSubject.value;
  }

  get intlLocale(): string {
    return this.locale === 'mk' ? 'mk-MK' : 'en-US';
  }

  formatShortDate(value: string | Date): string {
    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) {
      return '';
    }

    const day = date.getDate();
    const year = date.getFullYear();

    if (this.locale === 'mk') {
      const mkMonths = ['Јан', 'Феб', 'Мар', 'Апр', 'Мај', 'Јун', 'Јул', 'Авг', 'Сеп', 'Окт', 'Ное', 'Дек'];
      return `${mkMonths[date.getMonth()]} ${day}, ${year}`;
    }

    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }

  applyLocale(rawLocale: string | null | undefined): void {
    const resolved = this.resolveLocale(rawLocale);
    this.localeSubject.next(resolved);
    document.documentElement.lang = resolved;
  }

  private resolveLocale(rawLocale: string | null | undefined): AppLocale {
    if (!rawLocale) {
      return this.defaultLocale;
    }

    const normalized = rawLocale.trim().toLowerCase();
    if (normalized === 'mk' || normalized.startsWith('mk-')) {
      return 'mk';
    }

    if (normalized === 'en' || normalized.startsWith('en-')) {
      return 'en';
    }

    return this.defaultLocale;
  }
}
