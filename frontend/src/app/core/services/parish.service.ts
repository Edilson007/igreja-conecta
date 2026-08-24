import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Parish } from '../models/parish';

@Injectable({ providedIn: 'root' })
export class ParishService {
  private readonly apiUrl = '/api/parishes';
  constructor(private readonly http: HttpClient) {}
  search(city = '', sector = '', query = '') {
    const params = new HttpParams().set('city', city).set('sector', sector).set('query', query);
    return this.http.get<Parish[]>(this.apiUrl, { params });
  }
}
