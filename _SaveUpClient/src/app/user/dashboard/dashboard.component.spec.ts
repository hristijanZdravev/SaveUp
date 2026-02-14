import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { DashboardComponent } from './dashboard.component';
import { AnalyticsService } from '../../_services/analytics.service';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async () => {
    const analyticsServiceMock = {
      getDashboardStats: () => of({ totalWorkouts: 0, totalSets: 0, totalVolume: 0, activeDays: 0 }),
      getAnalyticsSummary: () => of({ totalWorkouts: 0, totalSets: 0, totalVolume: 0, avgRepsPerSet: 0, avgSetsPerWorkout: 0, activeDays: 0, consistencyScore: 0 }),
      getRecentWorkouts: () => of([])
    };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [{ provide: AnalyticsService, useValue: analyticsServiceMock }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
