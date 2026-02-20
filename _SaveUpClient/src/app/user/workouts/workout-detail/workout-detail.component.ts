import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, catchError, debounceTime, distinctUntilChanged, of, switchMap, takeUntil } from 'rxjs';
import { WorkoutService, WorkoutDetails, WorkoutExercise, Exercise, Set as WorkoutSet, AddSetRequest, UpdateSetRequest } from '../../../_services/workout.service';
import { ExerciseService } from '../../../_services/exercise.service';
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
  filteredExercises: Exercise[] = [];
  exerciseQuery = '';
  exerciseSearchLoading = false;
  loading = false;
  error: string | null = null;
  showAddExerciseModal = false;
  selectedExerciseId = '';
  showExerciseDeleteConfirm = false;
  deleteExerciseId: string | null = null;
  deleteExerciseTitle: string | null = null;
  showSetDeleteConfirm = false;
  deleteSetId: string | null = null;
  deleteSetLabel: string | null = null;
  editingSetId: string | null = null;
  editingSet: { reps: number; weight: number } | null = null;
  draggedExerciseId: string | null = null;
  dragOverExerciseId: string | null = null;
  reordering = false;
  collapsedExerciseIds = new Set<string>();
  showExpandAllAction = false;
  private exerciseSearch$ = new Subject<string>();
  private destroy$ = new Subject<void>();
  private exerciseImageMap: Record<string, string> = {};
  private normalizedImageUrlMap: Record<string, string> = {};

  constructor(
    private workoutService: WorkoutService,
    private exerciseService: ExerciseService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.setupExerciseSearch();
    const workoutId = this.route.snapshot.paramMap.get('id');
    if (workoutId) {
      this.loadWorkoutDetails(workoutId);
      this.loadExercises();
    }
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
          const nextIds = new Set(workout.workoutExercises.map((x) => x.id));
          this.collapsedExerciseIds = new Set(
            [...this.collapsedExerciseIds].filter((id) => nextIds.has(id))
          );
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

  loadExercises() {
    this.workoutService.getAllExercises()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (exercises) => {
          this.exercises = exercises;
          this.exerciseImageMap = exercises.reduce((acc, exercise) => {
            if (exercise.imageUrl) {
              acc[exercise.id] = exercise.imageUrl;
            }
            return acc;
          }, {} as Record<string, string>);

          if (!this.exerciseQuery.trim()) {
            this.filteredExercises = exercises;
          }
        },
        error: (err) => {
          console.error('Error loading exercises:', err);
        }
      });
  }

  goBack() {
    this.router.navigate(['/workouts']);
  }

  openAddExerciseModal() {
    this.showAddExerciseModal = true;
    this.selectedExerciseId = '';
    this.exerciseQuery = '';
    this.filteredExercises = this.exercises;
  }

  closeAddExerciseModal() {
    this.showAddExerciseModal = false;
    this.selectedExerciseId = '';
    this.exerciseQuery = '';
    this.exerciseSearchLoading = false;
    this.filteredExercises = this.exercises;
  }

  selectExercise(exerciseId: string) {
    this.selectedExerciseId = exerciseId;
  }

  onExerciseQueryChange() {
    this.exerciseSearch$.next(this.exerciseQuery);
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

  removeExercise(workoutExerciseId: string, exerciseTitle: string) {
    this.deleteExerciseId = workoutExerciseId;
    this.deleteExerciseTitle = exerciseTitle;
    this.showExerciseDeleteConfirm = true;
  }

  onExerciseDeleteConfirm() {
    if (!this.deleteExerciseId || !this.workout) return;

    this.loading = true;
    this.workoutService.removeExerciseFromWorkout(this.deleteExerciseId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.showExerciseDeleteConfirm = false;
          this.deleteExerciseId = null;
          this.deleteExerciseTitle = null;
          this.loadWorkoutDetails(this.workout!.id);
        },
        error: (err) => {
          console.error('Error removing exercise:', err);
          this.error = 'Failed to remove exercise. Please try again.';
          this.loading = false;
          this.showExerciseDeleteConfirm = false;
          this.deleteExerciseId = null;
          this.deleteExerciseTitle = null;
        }
      });
  }

  onExerciseDeleteCancel() {
    this.showExerciseDeleteConfirm = false;
    this.deleteExerciseId = null;
    this.deleteExerciseTitle = null;
  }

  openAddSetModal(workoutExerciseId: string) {
    const workoutExercise = this.workout?.workoutExercises.find(we => we.id === workoutExerciseId);
    if (!workoutExercise) return;

    const nextSetNumber = workoutExercise.sets.length > 0 
      ? Math.max(...workoutExercise.sets.map(s => s.setNumber)) + 1 
      : 1;

    this.editingSetId = workoutExerciseId;
    this.editingSet = { reps: 10, weight: 0 };
    (this.editingSet as any).setNumber = nextSetNumber;
    (this.editingSet as any).workoutExerciseId = workoutExerciseId;
  }

  closeAddSetModal() {
    this.editingSetId = null;
    this.editingSet = null;
  }

  addSet() {
    if (!this.editingSet || !this.editingSetId) return;

    const repsValue = Number(this.editingSet.reps);
    const weightValue = Number(this.editingSet.weight);

    if (!this.isValidSetInputs(repsValue, weightValue)) {
      this.error = 'Reps must be a number greater than 0, and weight must be a valid number.';
      return;
    }

    const request: AddSetRequest = {
      setNumber: (this.editingSet as any).setNumber,
      reps: repsValue,
      weight: weightValue
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

  startEditSet(set: WorkoutSet, workoutExerciseId: string) {
    this.editingSetId = workoutExerciseId;
    this.editingSet = { reps: set.reps, weight: set.weight };
    (this.editingSet as any).setId = set.id;
  }

  updateSet() {
    if (!this.editingSet || !this.editingSetId) return;

    const setId = (this.editingSet as any).setId;
    if (!setId) return;

    const repsValue = Number(this.editingSet.reps);
    const weightValue = Number(this.editingSet.weight);

    if (!this.isValidSetInputs(repsValue, weightValue)) {
      this.error = 'Reps must be a number greater than 0, and weight must be a valid number.';
      return;
    }

    const request: UpdateSetRequest = {
      reps: repsValue,
      weight: weightValue
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

  deleteSet(setId: string, setNumber: number) {
    this.deleteSetId = setId;
    this.deleteSetLabel = `Set ${setNumber}`;
    this.showSetDeleteConfirm = true;
  }

  onSetDeleteConfirm() {
    if (!this.deleteSetId || !this.workout) return;

    this.loading = true;
    this.workoutService.deleteSet(this.deleteSetId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.showSetDeleteConfirm = false;
          this.deleteSetId = null;
          this.deleteSetLabel = null;
          this.loadWorkoutDetails(this.workout!.id);
        },
        error: (err) => {
          console.error('Error deleting set:', err);
          this.error = 'Failed to delete set. Please try again.';
          this.loading = false;
          this.showSetDeleteConfirm = false;
          this.deleteSetId = null;
          this.deleteSetLabel = null;
        }
      });
  }

  onSetDeleteCancel() {
    this.showSetDeleteConfirm = false;
    this.deleteSetId = null;
    this.deleteSetLabel = null;
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

  getNextSetNumber(sets: WorkoutSet[]): number {
    if (sets.length === 0) {
      return 1;
    }
    return Math.max(...sets.map(s => s.setNumber)) + 1;
  }

  trackByExerciseId(index: number, exercise: Exercise): string {
    return exercise.id;
  }

  getExerciseImageUrl(exerciseId: string): string | null {
    return this.exerciseImageMap[exerciseId] ?? null;
  }

  onExerciseDragStart(workoutExerciseId: string, event: DragEvent): void {
    if (this.reordering) {
      event.preventDefault();
      return;
    }

    this.draggedExerciseId = workoutExerciseId;
    if (event.dataTransfer) {
      event.dataTransfer.effectAllowed = 'move';
      event.dataTransfer.setData('text/plain', workoutExerciseId);
    }
  }

  onExerciseDragOver(targetWorkoutExerciseId: string, event: DragEvent): void {
    if (!this.draggedExerciseId || this.draggedExerciseId === targetWorkoutExerciseId) {
      return;
    }
    event.preventDefault();
    this.dragOverExerciseId = targetWorkoutExerciseId;
    if (event.dataTransfer) {
      event.dataTransfer.dropEffect = 'move';
    }
  }

  onExerciseDragLeave(targetWorkoutExerciseId: string): void {
    if (this.dragOverExerciseId === targetWorkoutExerciseId) {
      this.dragOverExerciseId = null;
    }
  }

  onExerciseDrop(targetWorkoutExerciseId: string, event: DragEvent): void {
    event.preventDefault();
    const sourceWorkoutExerciseId = this.draggedExerciseId || event.dataTransfer?.getData('text/plain') || null;
    this.dragOverExerciseId = null;

    if (!sourceWorkoutExerciseId || sourceWorkoutExerciseId === targetWorkoutExerciseId || !this.workout) {
      this.draggedExerciseId = null;
      return;
    }

    const currentExercises = [...this.workout.workoutExercises];
    const sourceIndex = currentExercises.findIndex((x) => x.id === sourceWorkoutExerciseId);
    const targetIndex = currentExercises.findIndex((x) => x.id === targetWorkoutExerciseId);

    if (sourceIndex < 0 || targetIndex < 0) {
      this.draggedExerciseId = null;
      return;
    }

    const previousOrder = [...currentExercises];
    const [movedItem] = currentExercises.splice(sourceIndex, 1);
    currentExercises.splice(targetIndex, 0, movedItem);

    this.workout = {
      ...this.workout,
      workoutExercises: currentExercises
    };

    this.persistWorkoutExerciseOrder(previousOrder);
    this.draggedExerciseId = null;
  }

  onExerciseDragEnd(): void {
    this.draggedExerciseId = null;
    this.dragOverExerciseId = null;
  }

  isExerciseCollapsed(workoutExerciseId: string): boolean {
    return this.collapsedExerciseIds.has(workoutExerciseId);
  }

  collapseAllExercises(): void {
    if (!this.workout || this.workout.workoutExercises.length === 0) {
      return;
    }

    this.collapsedExerciseIds = new Set(this.workout.workoutExercises.map((x) => x.id));
    this.showExpandAllAction = true;
  }

  expandAllExercises(): void {
    this.collapsedExerciseIds.clear();
    this.showExpandAllAction = false;
  }

  toggleCollapseExpandAction(): void {
    if (this.showExpandAllAction) {
      this.expandAllExercises();
      return;
    }

    this.collapseAllExercises();
  }

  toggleExerciseCollapsed(workoutExerciseId: string): void {
    if (this.collapsedExerciseIds.has(workoutExerciseId)) {
      this.collapsedExerciseIds.delete(workoutExerciseId);
      return;
    }

    this.collapsedExerciseIds.add(workoutExerciseId);
  }

  getNormalizedExerciseImageUrl(imageUrl: string | null | undefined): string | null {
    if (!imageUrl) {
      return null;
    }

    if (this.normalizedImageUrlMap[imageUrl]) {
      return this.normalizedImageUrlMap[imageUrl];
    }

    const normalizedUrl = this.normalizeCloudinaryToSquare(imageUrl);
    this.normalizedImageUrlMap[imageUrl] = normalizedUrl;
    return normalizedUrl;
  }

  private normalizeCloudinaryToSquare(imageUrl: string): string {
    const [urlWithoutQuery, queryString] = imageUrl.split('?');

    if (!urlWithoutQuery.includes('res.cloudinary.com') || !urlWithoutQuery.includes('/upload/')) {
      return imageUrl;
    }

    const [prefix, suffix] = urlWithoutQuery.split('/upload/');
    if (!prefix || !suffix) {
      return imageUrl;
    }

    const transformation = 'c_pad,b_white,w_474,h_474,f_auto,q_auto';
    const segments = suffix.split('/');
    const firstSegment = segments[0] ?? '';

    const isVersionSegment = /^v\d+$/.test(firstSegment);
    const looksLikeTransformSegment =
      firstSegment.includes(',') || /^[a-z]{1,3}_.+/.test(firstSegment);

    const publicIdPath = looksLikeTransformSegment && !isVersionSegment
      ? segments.slice(1).join('/')
      : suffix;

    if (!publicIdPath) {
      return imageUrl;
    }

    const normalizedBase = `${prefix}/upload/${transformation}/${publicIdPath}`;
    return queryString ? `${normalizedBase}?${queryString}` : normalizedBase;
  }

  private isValidSetInputs(reps: number, weight: number): boolean {
    return Number.isFinite(reps) && reps > 0 && Number.isFinite(weight) && weight >= 0;
  }

  private persistWorkoutExerciseOrder(previousOrder: WorkoutExercise[]): void {
    if (!this.workout) {
      return;
    }

    const request = {
      items: this.workout.workoutExercises.map((x, index) => ({
        workoutExerciseId: x.id,
        order: index + 1
      }))
    };

    this.reordering = true;
    this.workoutService.reorderWorkoutExercises(this.workout.id, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.reordering = false;
        },
        error: (err) => {
          console.error('Error reordering exercises:', err);
          this.error = 'Failed to save exercise order. Please try again.';
          if (this.workout) {
            this.workout = {
              ...this.workout,
              workoutExercises: [...previousOrder]
            };
          }
          this.reordering = false;
        }
      });
  }

  private setupExerciseSearch() {
    this.exerciseSearch$
      .pipe(
        debounceTime(220),
        distinctUntilChanged(),
        switchMap((query) => {
          const normalizedQuery = query.trim();

          if (!normalizedQuery) {
            this.exerciseSearchLoading = false;
            return of(this.exercises);
          }

          this.exerciseSearchLoading = true;
          return this.exerciseService.searchExercises(normalizedQuery).pipe(
            switchMap((results) => of(results as Exercise[])),
            catchError((err) => {
              console.error('Error searching exercises:', err);
              const fallback = this.exercises.filter((exercise) =>
                exercise.title.toLowerCase().includes(normalizedQuery.toLowerCase())
              );
              return of(fallback);
            })
          );
        }),
        takeUntil(this.destroy$)
      )
      .subscribe((results) => {
        this.filteredExercises = results;
        this.exerciseSearchLoading = false;
      });
  }
}
