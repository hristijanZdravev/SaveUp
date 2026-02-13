import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ExerciseSearchResult {
  id: string;
  title: string;
}

export interface ExerciseDetail extends ExerciseSearchResult {
  description?: string;
  bodyGroup?: {
    id: string;
    name: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class ExerciseService {
  private readonly apiBaseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  // Search exercises by query
  searchExercises(query: string): Observable<ExerciseSearchResult[]> {
    return this.http.get<ExerciseSearchResult[]>(
      `${this.apiBaseUrl}/exercises/search`,
      { params: { query } }
    );
  }

  // Get all exercises
  getAllExercises(): Observable<ExerciseDetail[]> {
    return this.http.get<ExerciseDetail[]>(`${this.apiBaseUrl}/exercises`);
  }

  // Get exercise by ID
  getExerciseById(id: string): Observable<ExerciseDetail> {
    return this.http.get<ExerciseDetail>(`${this.apiBaseUrl}/exercises/${id}`);
  }

  // Get exercises by body part
  getExercisesByBodyPart(bodyPart: string): Observable<ExerciseSearchResult[]> {
    return this.http.get<ExerciseSearchResult[]>(
      `${this.apiBaseUrl}/exercises/by-body-part/${bodyPart}`
    );
  }
}
