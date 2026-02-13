import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { ExerciseSearchResult, ExerciseService } from '../../../_services/exercise.service';
import { AddSetRequest, Exercise, UpdateSetRequest, WorkoutDetails, WorkoutService, Set } from '../../../_services/workout.service';
import { ConfirmDeleteComponent } from '../../../common/confirm-delete/confirm-delete.component';

@Component({
  selector: 'app-workout-detail',
  imports: [CommonModule, FormsModule, ConfirmDeleteComponent],
  templateUrl: './workout-detail.component.html',
  styleUrl: './workout-detail.component.css'
})
export class WorkoutDetailComponent implements OnInit, OnDestroy {
  workout: WorkoutDetails | null = null;
  exercises: Exercise[] = [];
  loading = false;
  error: string | null = null;
  showAddExerciseModal = false;
  selectedExerciseId = '';
  editingSetId: string | null = null;
  editingSet: { reps: number; weight: number } | null = null;
  
  // Delete confirmation modal
  showDeleteConfirm = false;
  deleteExerciseId: string | null = null;
  deleteExerciseTitle: string | null = null;
  
  // Exercise search properties
  searchQuery = '';
  searchResults: ExerciseSearchResult[] = [];
  showSearchResults = false;
  searching = false;
  private searchSubject$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private workoutService: WorkoutService,
    private exerciseService: ExerciseService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    const workoutId = this.route.snapshot.paramMap.get('id');
    if (workoutId) {
      this.loadWorkoutDetails(workoutId);
    }
    
    // Setup search debounce with faster response
    this.searchSubject$
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe((query) => {
        if (query.trim().length > 0) {
          this.performSearch(query);
        } else {
          this.searchResults = [];
          this.showSearchResults = false;
          this.searching = false;
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadWorkoutDetails(workoutId: string) {
    this.loading = true;
    this.error = null;
    this.workoutService.getWorkoutDetails(workoutId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (workout) => {
          this.workout = workout;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading workout details:', err);
          this.error = 'Failed to load workout details. Please try again.';
          this.loading = false;
        }
      });
  }

  onSearchChange(query: string) {
    this.searchQuery = query;
    // Show loading state immediately for better UX feedback
    if (query.trim().length > 0) {
      this.searching = true;
    }
    this.searchSubject$.next(query);
  }

  performSearch(query: string) {
    this.searching = true;
    this.exerciseService.searchExercises(query)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (results: ExerciseSearchResult[]) => {
          this.searchResults = results;
          this.showSearchResults = results.length > 0;
          this.searching = false;
        },
        error: (err: any) => {
          console.error('Error searching exercises:', err);
          this.searchResults = [];
          this.showSearchResults = false;
          this.searching = false;
        }
      });
  }

  selectExercise(exerciseId: string) {
    this.selectedExerciseId = exerciseId;
    this.addExercise();
  }

  goBack() {
    this.router.navigate(['/workouts']);
  }

  openAddExerciseModal() {
    this.showAddExerciseModal = true;
    this.selectedExerciseId = '';
    this.searchQuery = '';
    this.searchResults = [];
    this.showSearchResults = false;
  }

  closeAddExerciseModal() {
    this.showAddExerciseModal = false;
    this.selectedExerciseId = '';
    this.searchQuery = '';
    this.searchResults = [];
    this.showSearchResults = false;
  }

  addExercise() {
    if (!this.selectedExerciseId || !this.workout) {
      return;
    }

    this.loading = true;
    this.workoutService.addExerciseToWorkout(this.workout.id, this.selectedExerciseId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loadWorkoutDetails(this.workout!.id);
          this.closeAddExerciseModal();
        },
        error: (err) => {
          console.error('Error adding exercise:', err);
          this.error = 'Failed to add exercise. Please try again.';
          this.loading = false;
        }
      });
  }

  removeExercise(workoutExerciseId: string) {
    if (confirm('Are you sure you want to remove this exercise from the workout?')) {
      this.loading = true;
      this.workoutService.removeExerciseFromWorkout(workoutExerciseId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.loadWorkoutDetails(this.workout!.id);
          },
          error: (err) => {
            console.error('Error removing exercise:', err);
            this.error = 'Failed to remove exercise. Please try again.';
            this.loading = false;
          }
        });
    }
  }

  openAddSetModal(workoutExerciseId: string) {
    const workoutExercise = this.workout?.workoutExercises.find(we => we.id === workoutExerciseId);
    if (!workoutExercise) return;

    const nextSetNumber = workoutExercise.sets.length > 0 
      ? Math.max(...workoutExercise.sets.map(s => s.setNumber)) + 1 
      : 1;

    // Get the last set's reps and weight if it exists
    const lastSet = workoutExercise.sets.length > 0 
      ? workoutExercise.sets[workoutExercise.sets.length - 1]
      : null;

    this.editingSetId = workoutExerciseId;
    this.editingSet = { 
      reps: lastSet ? lastSet.reps : 10,
      weight: lastSet ? lastSet.weight : 0
    };
    (this.editingSet as any).setNumber = nextSetNumber;
    (this.editingSet as any).workoutExerciseId = workoutExerciseId;
  }

  closeAddSetModal() {
    this.editingSetId = null;
    this.editingSet = null;
  }

  addSet() {
    if (!this.editingSet || !this.editingSetId) return;

    const request: AddSetRequest = {
      setNumber: (this.editingSet as any).setNumber,
      reps: this.editingSet.reps,
      weight: this.editingSet.weight
    };

    this.loading = true;
    this.workoutService.addSet((this.editingSet as any).workoutExerciseId, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loadWorkoutDetails(this.workout!.id);
          this.closeAddSetModal();
        },
        error: (err) => {
          console.error('Error adding set:', err);
          this.error = 'Failed to add set. Please try again.';
          this.loading = false;
        }
      });
  }

  startEditSet(set: Set, workoutExerciseId: string) {
    this.editingSetId = workoutExerciseId;
    this.editingSet = { reps: set.reps, weight: set.weight };
    (this.editingSet as any).setId = set.id;
  }

  updateSet() {
    if (!this.editingSet || !this.editingSetId) return;

    const setId = (this.editingSet as any).setId;
    if (!setId) return;

    const request: UpdateSetRequest = {
      reps: this.editingSet.reps,
      weight: this.editingSet.weight
    };

    this.loading = true;
    this.workoutService.updateSet(setId, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loadWorkoutDetails(this.workout!.id);
          this.closeAddSetModal();
        },
        error: (err) => {
          console.error('Error updating set:', err);
          this.error = 'Failed to update set. Please try again.';
          this.loading = false;
        }
      });
  }

  deleteSet(setId: string) {
    if (confirm('Are you sure you want to delete this set?')) {
      this.loading = true;
      this.workoutService.deleteSet(setId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.loadWorkoutDetails(this.workout!.id);
          },
          error: (err) => {
            console.error('Error deleting set:', err);
            this.error = 'Failed to delete set. Please try again.';
            this.loading = false;
          }
        });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  isEditingSet(setId: string): boolean {
    return this.editingSetId !== null && (this.editingSet as any)?.setId === setId;
  }

  isAddingSet(workoutExerciseId: string): boolean {
    return this.editingSetId === workoutExerciseId && !(this.editingSet as any)?.setId;
  }

  getNextSetNumber(sets: Set[]): number {
    if (sets.length === 0) {
      return 1;
    }
    return Math.max(...sets.map(s => s.setNumber)) + 1;
  }
}
