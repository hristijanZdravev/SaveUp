import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';

// Backend DTOs matching the C# controller
export interface Workout {
  id: string;
  title: string;
  date: string;
}

export interface Exercise {
  id: string;
  title: string;
  description?: string | null;
  imageUrl?: string | null;
  imagePublicId?: string | null;
  bodyGroup?: {
    id?: string;
    name: string;
  };
}

export interface Set {
  id: string;
  setNumber: number;
  reps: number;
  weight: number;
}

// Backend returns ExerciseTitle directly, not nested Exercise object
export interface WorkoutExercise {
  id: string;
  order?: number;
  exerciseId: string;
  exerciseTitle: string; // Changed from nested exercise object
  imageUrl?: string | null;
  imagePublicId?: string | null;
  sets: Set[];
}

export interface WorkoutDetails {
  id: string;
  title: string;
  date: string;
  workoutExercises: WorkoutExercise[];
}

export interface CreateWorkoutRequest {
  title: string;
  date: string;
}

export interface UpdateWorkoutRequest {
  title: string;
  date: string;
}

export interface AddExerciseRequest {
  exerciseId: string; // Backend expects this structure
}

export interface AddSetRequest {
  setNumber: number;
  reps: number;
  weight: number;
}

export interface UpdateSetRequest {
  reps: number;
  weight: number;
}

export interface ReorderWorkoutExerciseItem {
  workoutExerciseId: string;
  order: number;
}

export interface ReorderWorkoutExercisesRequest {
  items: ReorderWorkoutExerciseItem[];
}

export interface PaginatedResponse<T> {
  page: number;
  pageSize: number;
  total: number;
  data: T[];
}

@Injectable({
  providedIn: 'root'
})
export class WorkoutService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  // Get all workouts with pagination
  getAllWorkouts(page: number = 1, pageSize: number = 8): Observable<PaginatedResponse<Workout>> {
    return this.http.get<PaginatedResponse<Workout>>(
      `${this.apiBaseUrl}/workouts`,
      { params: { page: page.toString(), pageSize: pageSize.toString() } }
    );
  }

  // Filter workouts by days
  filterWorkoutsByDays(days: number): Observable<Workout[]> {
    return this.http.get<Workout[]>(
      `${this.apiBaseUrl}/workouts/filter`,
      { params: { days: days.toString() } }
    );
  }

  // Get workout details
  getWorkoutDetails(id: string): Observable<WorkoutDetails> {
    return this.http.get<WorkoutDetails & { exercises?: WorkoutExercise[] }>(`${this.apiBaseUrl}/workouts/${id}`).pipe(
      map((dto) => ({
        ...dto,
        workoutExercises: dto.workoutExercises ?? dto.exercises ?? []
      }))
    );
  }

  // Create workout
  createWorkout(request: CreateWorkoutRequest): Observable<Workout> {
    return this.http.post<Workout>(`${this.apiBaseUrl}/workouts`, request);
  }

  // Update workout
  updateWorkout(id: string, request: UpdateWorkoutRequest): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/workouts/${id}`, request);
  }

  // Add exercise to workout - backend expects { exerciseId: string }
  addExerciseToWorkout(workoutId: string, exerciseId: string): Observable<WorkoutExercise> {
    const request: AddExerciseRequest = { exerciseId };
    return this.http.post<WorkoutExercise>(
      `${this.apiBaseUrl}/workouts/${workoutId}/exercises`,
      request
    );
  }

  // Add set
  addSet(workoutExerciseId: string, request: AddSetRequest): Observable<Set> {
    return this.http.post<Set>(
      `${this.apiBaseUrl}/workouts/exercises/${workoutExerciseId}/sets`,
      request
    );
  }

  // Update set
  updateSet(setId: string, request: UpdateSetRequest): Observable<void> {
    return this.http.put<void>(
      `${this.apiBaseUrl}/workouts/sets/${setId}`,
      request
    );
  }

  // Delete set
  deleteSet(setId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/workouts/sets/${setId}`);
  }

  // Remove exercise from workout
  removeExerciseFromWorkout(workoutExerciseId: string): Observable<void> {
    return this.http.delete<void>(
      `${this.apiBaseUrl}/workouts/exercises/${workoutExerciseId}`
    );
  }

  // Reorder exercises within a workout
  reorderWorkoutExercises(workoutId: string, request: ReorderWorkoutExercisesRequest): Observable<void> {
    return this.http.put<void>(`${this.apiBaseUrl}/workouts/${workoutId}/reorder`, request);
  }

  // Delete workout
  deleteWorkout(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/workouts/${id}`);
  }

  // Get all exercises (for dropdown)
  getAllExercises(): Observable<Exercise[]> {
    return this.http.get<Exercise[]>(`${this.apiBaseUrl}/exercises`);
  }
}
