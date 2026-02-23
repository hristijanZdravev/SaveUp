import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject, forkJoin, takeUntil } from 'rxjs';
import {
  AnalyticsSummaryDto,
  AnalyticsService,
  DashboardStatsDto,
  RecentWorkoutDto
} from '../../_services/analytics.service';
import { LocaleService } from '../../_services/locale.service';
import { I18nService } from '../../_services/i18n.service';
import { TPipe } from '../../_pipes/t.pipe';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink, TPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  stats: DashboardStatsDto | null = null;
  summary: AnalyticsSummaryDto | null = null;
  recentWorkouts: RecentWorkoutDto[] = [];
  selectedDays = 7;
  readonly rangeOptions = [
    { label: '', value: 0 },
    { label: '', value: 7 },
    { label: '', value: 14 },
    { label: '', value: 30 }
  ];

  loading = false;
  error: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private analyticsService: AnalyticsService,
    private localeService: LocaleService,
    private i18n: I18nService
  ) {
    this.setRangeOptions();
  }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    forkJoin({
      stats: this.analyticsService.getDashboardStats(this.selectedDays),
      summary: this.analyticsService.getAnalyticsSummary(this.selectedDays),
      recentWorkouts: this.analyticsService.getRecentWorkouts(6)
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ stats, summary, recentWorkouts }) => {
          this.stats = stats;
          this.summary = summary;
          this.recentWorkouts = recentWorkouts;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading dashboard data:', err);
          this.error = this.i18n.t('error.dashboardLoad');
          this.loading = false;
        }
      });
  }

  private setRangeOptions(): void {
    this.rangeOptions[0].label = this.i18n.t('range.allTime');
    this.rangeOptions[1].label = this.i18n.t('range.last7');
    this.rangeOptions[2].label = this.i18n.t('range.last14');
    this.rangeOptions[3].label = this.i18n.t('range.last30');
  }

  get avgRepsPerSet(): number {
    return this.summary?.avgRepsPerSet ?? 0;
  }

  get activeDays(): number {
    return this.summary?.activeDays ?? this.stats?.activeDays ?? 0;
  }

  get avgSetsPerWorkout(): number {
    return this.summary?.avgSetsPerWorkout ?? 0;
  }

  get consistencyScore(): number {
    return this.summary?.consistencyScore ?? 0;
  }

  onRangeChange(days: number): void {
    if (this.selectedDays === days || this.loading) {
      return;
    }
    this.selectedDays = days;
    this.loadDashboardData();
  }

  formatDate(dateString: string): string {
    return this.localeService.formatShortDate(dateString);
  }

  formatVolume(volume: number): string {
    return Math.round(volume).toLocaleString(this.localeService.intlLocale);
  }

  formatDecimal(value: number): string {
    return value.toLocaleString(this.localeService.intlLocale, {
      minimumFractionDigits: 1,
      maximumFractionDigits: 1
    });
  }
}
