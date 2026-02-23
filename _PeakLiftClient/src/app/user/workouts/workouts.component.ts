import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { WorkoutService, Workout } from '../../_services/workout.service';
import { ConfirmDeleteComponent } from '../../common/confirm-delete/confirm-delete.component';
import { LocaleService } from '../../_services/locale.service';
import { I18nService } from '../../_services/i18n.service';
import { TPipe } from '../../_pipes/t.pipe';

@Component({
  selector: 'app-workouts',
  imports: [CommonModule, FormsModule, ConfirmDeleteComponent, TPipe],
  templateUrl: './workouts.component.html',
  styleUrl: './workouts.component.css'
})
export class WorkoutsComponent implements OnInit, OnDestroy {
  workouts: Workout[] = [];
  loading = false;
  error: string | null = null;
  showCreateModal = false;
  newWorkoutTitle = '';
  newWorkoutDate = new Date().toISOString().split('T')[0];

  // Delete confirmation modal
  showDeleteConfirm = false;
  deleteWorkoutId: string | null = null;
  deleteWorkoutTitle: string | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 9;
  totalWorkouts = 0;

  // Filter
  selectedFilter = 0; // 0 = all, 7 = week, 14 = two weeks, 30 = month
  filterOptions = [
    { days: 0, label: '' },
    { days: 7, label: '' },
    { days: 14, label: '' },
    { days: 30, label: '' }
  ];

  private destroy$ = new Subject<void>();

  constructor(
    private workoutService: WorkoutService,
    private router: Router,
    private localeService: LocaleService,
    private i18n: I18nService
  ) {
    this.setFilterLabels();
  }

  ngOnInit() {
    this.loadWorkouts();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadWorkouts() {
    this.loading = true;
    this.error = null;

    if (this.selectedFilter === 0) {
      // Load all workouts with pagination
      this.workoutService.getAllWorkouts(this.currentPage, this.pageSize)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (response) => {
            this.workouts = response.data;
            this.totalWorkouts = response.total;
            this.currentPage = response.page;
            this.loading = false;
          },
          error: (err) => {
            console.error('Error loading workouts:', err);
            this.error = this.i18n.t('error.workoutsLoad');
            this.loading = false;
          }
        });
    } else {
      // Load filtered workouts
      this.workoutService.filterWorkoutsByDays(this.selectedFilter)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (workouts) => {
            this.workouts = workouts;
            this.totalWorkouts = workouts.length;
            this.currentPage = 1; // Reset to first page
            this.loading = false;
          },
          error: (err) => {
            console.error('Error loading filtered workouts:', err);
            this.error = this.i18n.t('error.filteredWorkoutsLoad');
            this.loading = false;
          }
        });
    }
  }

  onFilterChange(days: number) {
    this.selectedFilter = days;
    this.currentPage = 1;
    this.loadWorkouts();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.getTotalPages()) return;
    this.currentPage = page;
    this.loadWorkouts();
  }

  nextPage() {
    if (this.currentPage < this.getTotalPages()) {
      this.goToPage(this.currentPage + 1);
    }
  }

  previousPage() {
    if (this.currentPage > 1) {
      this.goToPage(this.currentPage - 1);
    }
  }

  getTotalPages(): number {
    return Math.ceil(this.totalWorkouts / this.pageSize);
  }

  openWorkout(workoutId: string) {
    this.router.navigate(['/workouts', workoutId]);
  }

  openCreateModal() {
    this.showCreateModal = true;
    this.newWorkoutTitle = '';
    this.newWorkoutDate = new Date().toISOString().split('T')[0];
  }

  closeCreateModal() {
    this.showCreateModal = false;
    this.newWorkoutTitle = '';
  }

  createWorkout() {
    if (!this.newWorkoutTitle.trim()) {
      return;
    }

    this.loading = true;
    this.workoutService.createWorkout({
      title: this.newWorkoutTitle.trim(),
      date: this.newWorkoutDate
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (workout) => {
          this.closeCreateModal();
          this.router.navigate(['/workouts', workout.id]);
        },
        error: (err) => {
          console.error('Error creating workout:', err);
          this.error = this.i18n.t('error.workoutCreate');
          this.loading = false;
        }
      });
  }

  deleteWorkout(workoutId: string, event: Event) {
    event.stopPropagation();
    const workout = this.workouts.find(w => w.id === workoutId);
    if (workout) {
      this.deleteWorkoutId = workoutId;
      this.deleteWorkoutTitle = workout.title;
      this.showDeleteConfirm = true;
    }
  }

  onDeleteConfirm() {
    if (!this.deleteWorkoutId) return;
    
    const workoutId = this.deleteWorkoutId;
    this.loading = true;
    
    this.workoutService.deleteWorkout(workoutId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.showDeleteConfirm = false;
          this.deleteWorkoutId = null;
          this.deleteWorkoutTitle = null;
          // Reset to first page if we're on a filtered view or if workout was last on page
          if (this.workouts.length === 1) {
            this.currentPage = 1;
          }
          this.loadWorkouts();
        },
        error: (err: any) => {
          console.error('Error deleting workout:', err);
          this.error = this.i18n.t('error.workoutDelete');
          this.loading = false;
          this.showDeleteConfirm = false;
          this.deleteWorkoutId = null;
          this.deleteWorkoutTitle = null;
        }
      });
  }
  onDeleteCancel() {
    this.showDeleteConfirm = false;
    this.deleteWorkoutId = null;
    this.deleteWorkoutTitle = null;
  }

  formatDate(dateString: string): string {
    return this.localeService.formatShortDate(dateString);
  }

  copyWorkout(workoutId: string, event: Event) {
    event.stopPropagation();
    this.loading = true;
    this.error = null;

    // Get the original workout details
    this.workoutService.getWorkoutDetails(workoutId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (originalWorkout) => {
          // Create new workout with today's date
          const todayDate = new Date().toISOString().split('T')[0];
          this.workoutService.createWorkout({
            title: originalWorkout.title,
            date: todayDate
          })
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: (newWorkout) => {
                // Add all exercises from original workout
                this.addExercisesToNewWorkout(originalWorkout.workoutExercises, newWorkout.id, 0);
              },
              error: (err) => {
                console.error('Error creating copied workout:', err);
                this.error = this.i18n.t('error.workoutCopy');
                this.loading = false;
              }
            });
        },
        error: (err) => {
          console.error('Error fetching workout details:', err);
          this.error = this.i18n.t('error.workoutCopy');
          this.loading = false;
        }
      });
  }

  private addExercisesToNewWorkout(exercises: any[], newWorkoutId: string, index: number) {
    if (index >= exercises.length) {
      // All exercises added, reload
      this.loading = false;
      this.loadWorkouts();
      return;
    }

    const exercise = exercises[index];
    this.workoutService.addExerciseToWorkout(newWorkoutId, exercise.exerciseId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (addedExercise) => {
          // Add all sets for this exercise
          this.addSetsToExercise(exercise.sets, addedExercise.id, 0, exercises, newWorkoutId, index);
        },
        error: (err) => {
          console.error('Error adding exercise:', err);
          this.error = this.i18n.t('error.workoutCopy');
          this.loading = false;
        }
      });
  }

  private addSetsToExercise(sets: any[], workoutExerciseId: string, setIndex: number, exercises: any[], newWorkoutId: string, exerciseIndex: number) {
    if (setIndex >= sets.length) {
      // All sets added for this exercise, move to next exercise
      this.addExercisesToNewWorkout(exercises, newWorkoutId, exerciseIndex + 1);
      return;
    }

    const set = sets[setIndex];
    this.workoutService.addSet(workoutExerciseId, {
      setNumber: set.setNumber,
      reps: set.reps,
      weight: set.weight
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Move to next set
          this.addSetsToExercise(sets, workoutExerciseId, setIndex + 1, exercises, newWorkoutId, exerciseIndex);
        },
        error: (err: any) => {
          console.error('Error adding set:', err);
          this.error = this.i18n.t('error.workoutCopy');
          this.loading = false;
        }
      });
  }

  private setFilterLabels(): void {
    this.filterOptions[0].label = this.i18n.t('range.allTime');
    this.filterOptions[1].label = this.i18n.t('range.last7');
    this.filterOptions[2].label = this.i18n.t('range.last14');
    this.filterOptions[3].label = this.i18n.t('range.last30');
  }
}


