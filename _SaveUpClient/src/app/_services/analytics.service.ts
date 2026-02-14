import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DashboardStatsDto {
  totalWorkouts: number;
  totalSets: number;
  totalVolume: number;
  activeDays?: number;
}

export interface RecentWorkoutDto {
  id: string;
  title: string;
  date: string;
}

export interface TopExerciseDto {
  exercise: string;
  count: number;
}

export interface AnalyticsOverviewDto {
  totalWorkouts: number;
  totalVolume: number;
  topExercise: TopExerciseDto | null;
}

export interface AnalyticsSummaryDto {
  totalWorkouts: number;
  totalSets: number;
  totalVolume: number;
  avgRepsPerSet: number;
  avgSetsPerWorkout: number;
  activeDays: number;
  consistencyScore: number;
}

export interface SetsPerDayDto {
  date: string;
  sets: number;
}

export interface BodyPartDistributionDto {
  bodyPart: string;
  count: number;
}

export interface BodyPartDto {
  id: string;
  name: string;
}

export interface BodyPartStatsDto {
  totalSets: number;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getDashboardStats(days = 7): Observable<DashboardStatsDto> {
    return this.http.get<DashboardStatsDto>(`${this.apiBaseUrl}/dashboard/stats`, {
      params: { days }
    });
  }

  getRecentWorkouts(limit = 5): Observable<RecentWorkoutDto[]> {
    return this.http.get<RecentWorkoutDto[]>(`${this.apiBaseUrl}/dashboard/recent-workouts`, {
      params: { limit }
    });
  }

  getAnalyticsOverview(): Observable<AnalyticsOverviewDto> {
    return this.http.get<AnalyticsOverviewDto>(`${this.apiBaseUrl}/analytics/overview`);
  }

  getAnalyticsSummary(days = 7): Observable<AnalyticsSummaryDto> {
    return this.http.get<AnalyticsSummaryDto>(`${this.apiBaseUrl}/analytics/summary`, {
      params: { days }
    });
  }

  getSetsPerDay(days = 7): Observable<SetsPerDayDto[]> {
    return this.http.get<SetsPerDayDto[]>(`${this.apiBaseUrl}/analytics/sets-per-day`, {
      params: { days }
    });
  }

  getBodyPartDistribution(days = 7): Observable<BodyPartDistributionDto[]> {
    return this.http.get<BodyPartDistributionDto[]>(`${this.apiBaseUrl}/analytics/body-part-distribution`, {
      params: { days }
    });
  }

  getBodyParts(): Observable<BodyPartDto[]> {
    return this.http.get<BodyPartDto[]>(`${this.apiBaseUrl}/body-parts`);
  }

  getBodyPartStats(id: string): Observable<BodyPartStatsDto> {
    return this.http.get<BodyPartStatsDto>(`${this.apiBaseUrl}/body-parts/${id}/stats`);
  }
}
