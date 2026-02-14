import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, forkJoin, map, of, switchMap, takeUntil } from 'rxjs';
import {
  AnalyticsSummaryDto,
  AnalyticsService,
  DashboardStatsDto,
  BodyPartDistributionDto,
  SetsPerDayDto
} from '../../_services/analytics.service';

interface BodyPartProgressItem {
  id: string;
  name: string;
  totalSets: number;
}

interface FrequencyChartPoint {
  label: string;
  count: number;
  heightPercent: number;
}

interface DistributionChartPoint {
  label: string;
  count: number;
  widthPercent: number;
}

@Component({
  selector: 'app-progress',
  imports: [CommonModule],
  templateUrl: './progress.component.html',
  styleUrl: './progress.component.css'
})
export class ProgressComponent implements OnInit, OnDestroy {
  summary: AnalyticsSummaryDto | null = null;
  stats: DashboardStatsDto | null = null;
  setsPerDay: SetsPerDayDto[] = [];
  distribution: BodyPartDistributionDto[] = [];
  bodyPartProgress: BodyPartProgressItem[] = [];
  selectedDays = 7;
  readonly rangeOptions = [
    { label: 'All Time', value: 0 },
    { label: 'Last 7 Days', value: 7 },
    { label: 'Last 14 Days', value: 14 },
    { label: 'Last 30 Days', value: 30 }
  ];

  frequencyChart: FrequencyChartPoint[] = [];
  distributionChart: DistributionChartPoint[] = [];

  loading = false;
  error: string | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(private analyticsService: AnalyticsService) {}

  ngOnInit(): void {
    this.loadProgress();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProgress(): void {
    this.loading = true;
    this.error = null;

    forkJoin({
      summary: this.analyticsService.getAnalyticsSummary(this.selectedDays),
      stats: this.analyticsService.getDashboardStats(this.selectedDays),
      setsPerDay: this.analyticsService.getSetsPerDay(this.selectedDays),
      distribution: this.analyticsService.getBodyPartDistribution(this.selectedDays),
      bodyParts: this.analyticsService.getBodyParts()
    })
      .pipe(
        switchMap(({ summary, stats, setsPerDay, distribution, bodyParts }) => {
          if (bodyParts.length === 0) {
            return of({ summary, stats, setsPerDay, distribution, bodyPartProgress: [] as BodyPartProgressItem[] });
          }

          const statsRequests = bodyParts.map((part) =>
            this.analyticsService.getBodyPartStats(part.id).pipe(
              map((stats) => ({ id: part.id, name: part.name, totalSets: stats.totalSets }))
            )
          );

          return forkJoin(statsRequests).pipe(
            map((bodyPartProgress) => ({ summary, stats, setsPerDay, distribution, bodyPartProgress }))
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: ({ summary, stats, setsPerDay, distribution, bodyPartProgress }) => {
          this.summary = summary;
          this.stats = stats;
          this.setsPerDay = setsPerDay;
          this.distribution = distribution;
          this.bodyPartProgress = bodyPartProgress.sort((a, b) => b.totalSets - a.totalSets);
          this.buildCharts();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading progress data:', err);
          this.error = 'Could not load progress analytics. Try again in a moment.';
          this.loading = false;
        }
      });
  }

  get totalTrackedSets(): number {
    return this.bodyPartProgress.reduce((sum, item) => sum + item.totalSets, 0);
  }

  get activeDays(): number {
    return this.summary?.activeDays ?? this.stats?.activeDays ?? 0;
  }

  get avgRepsPerSet(): number {
    return this.summary?.avgRepsPerSet ?? 0;
  }

  get avgSetsPerWorkout(): number {
    return this.summary?.avgSetsPerWorkout ?? 0;
  }

  get consistencyScore(): number {
    return this.summary?.consistencyScore ?? 0;
  }

  get consistencyRate(): number {
    if (this.selectedDays > 0) {
      return Math.min(100, Math.round((this.activeDays / this.selectedDays) * 100));
    }

    if (this.setsPerDay.length === 0) {
      return 0;
    }

    const sorted = [...this.setsPerDay]
      .map((entry) => new Date(entry.date))
      .sort((a, b) => a.getTime() - b.getTime());
    const first = sorted[0];
    const last = sorted[sorted.length - 1];
    const spanDays = Math.max(1, Math.floor((last.getTime() - first.getTime()) / (1000 * 60 * 60 * 24)) + 1);
    const activeUniqueDays = this.setsPerDay.filter((entry) => entry.sets > 0).length;

    return Math.min(100, Math.round((activeUniqueDays / spanDays) * 100));
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

  formatDayLabel(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric'
    });
  }

  bodyPartPercentage(totalSets: number): number {
    if (this.totalTrackedSets === 0) {
      return 0;
    }

    return Math.round((totalSets / this.totalTrackedSets) * 100);
  }

  onRangeChange(days: number): void {
    if (this.loading || this.selectedDays === days) {
      return;
    }
    this.selectedDays = days;
    this.loadProgress();
  }

  private buildCharts(): void {
    const maxSets = this.setsPerDay.reduce((max, entry) => Math.max(max, entry.sets), 0);
    this.frequencyChart = this.setsPerDay.slice(-14).map((entry) => {
      const heightPercent = maxSets > 0 ? Math.max(14, Math.round((entry.sets / maxSets) * 100)) : 0;
      return {
        label: this.formatDayLabel(entry.date),
        count: entry.sets,
        heightPercent
      };
    });

    const maxDistribution = this.distribution.reduce((max, entry) => Math.max(max, entry.count), 0);
    this.distributionChart = this.distribution
      .slice()
      .sort((a, b) => b.count - a.count)
      .map((entry) => {
        const widthPercent = maxDistribution > 0 ? Math.max(10, Math.round((entry.count / maxDistribution) * 100)) : 0;
        return {
          label: entry.bodyPart,
          count: entry.count,
          widthPercent
        };
      });
  }
}
