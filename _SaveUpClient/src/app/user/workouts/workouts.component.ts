import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { WorkoutService, Workout } from '../../_services/workout.service';
import { ConfirmDeleteComponent } from '../../common/confirm-delete/confirm-delete.component';

@Component({
  selector: 'app-workouts',
  imports: [CommonModule, FormsModule, ConfirmDeleteComponent],
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
  pageSize = 10;
  totalWorkouts = 0;

  // Filter
  selectedFilter = 0; // 0 = all, 7 = week, 14 = two weeks, 30 = month
  filterOptions = [
    { days: 0, label: 'All Time' },
    { days: 7, label: 'Last 7 Days' },
    { days: 14, label: 'Last 14 Days' },
    { days: 30, label: 'Last 30 Days' }
  ];

  private destroy$ = new Subject<void>();

  constructor(
    private workoutService: WorkoutService,
    private router: Router
  ) {}

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
            this.error = 'Failed to load workouts. Please try again.';
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
            this.error = 'Failed to load workouts. Please try again.';
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
          this.error = 'Failed to create workout. Please try again.';
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
          this.error = 'Failed to delete workout. Please try again.';
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
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
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
                this.error = 'Failed to copy workout. Please try again.';
                this.loading = false;
              }
            });
        },
        error: (err) => {
          console.error('Error fetching workout details:', err);
          this.error = 'Failed to copy workout. Please try again.';
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
          this.error = 'Failed to copy all exercises. Please try again.';
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
          this.error = 'Failed to copy all sets. Please try again.';
          this.loading = false;
        }
      });
  }
}

