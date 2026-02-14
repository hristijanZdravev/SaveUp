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

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy {
  stats: DashboardStatsDto | null = null;
  summary: AnalyticsSummaryDto | null = null;
  recentWorkouts: RecentWorkoutDto[] = [];
  selectedDays = 7;
  readonly rangeOptions = [
    { label: 'All Time', value: 0 },
    { label: 'Last 7 Days', value: 7 },
    { label: 'Last 14 Days', value: 14 },
    { label: 'Last 30 Days', value: 30 }
  ];

  loading = false;
  error: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(private analyticsService: AnalyticsService) {}

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
          this.error = 'Could not load dashboard data. Try again in a moment.';
          this.loading = false;
        }
      });
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
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric'
    });
  }

  formatVolume(volume: number): string {
    return Math.round(volume).toLocaleString('en-US');
  }

  formatDecimal(value: number): string {
    return value.toLocaleString('en-US', {
      minimumFractionDigits: 1,
      maximumFractionDigits: 1
    });
  }
}
