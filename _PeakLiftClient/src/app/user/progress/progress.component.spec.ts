import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ProgressComponent } from './progress.component';
import { AnalyticsService } from '../../_services/analytics.service';

describe('ProgressComponent', () => {
  let component: ProgressComponent;
  let fixture: ComponentFixture<ProgressComponent>;

  beforeEach(async () => {
    const analyticsServiceMock = {
      getAnalyticsSummary: () => of({ totalWorkouts: 0, totalSets: 0, totalVolume: 0, avgRepsPerSet: 0, avgSetsPerWorkout: 0, activeDays: 0, consistencyScore: 0 }),
      getDashboardStats: () => of({ totalWorkouts: 0, totalSets: 0, totalVolume: 0, activeDays: 0 }),
      getSetsPerDay: () => of([]),
      getBodyPartDistribution: () => of([]),
      getBodyParts: () => of([]),
      getBodyPartStats: () => of({ totalSets: 0 })
    };

    await TestBed.configureTestingModule({
      imports: [ProgressComponent],
      providers: [{ provide: AnalyticsService, useValue: analyticsServiceMock }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
