import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MassForm {
  day: string;
  time: string;
  description?: string;
}

export interface ParishForm {
  id?: string;
  name: string;
  sector: string;
  city: string;
  state: string;
  address: string;
  phone?: string;
  imageUrl?: string;
  isPremium: boolean;
  massSchedules: MassForm[];
  communities?: CommunityForm[];
}
export interface CommunityForm { id?: string; name: string; address: string; phone?: string; imageUrl?: string; massSchedules: MassForm[]; }

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private readonly http: HttpClient) {}

  list(key: string): Observable<ParishForm[]> {
    return this.http.get<ParishForm[]>('/api/admin/parishes', { headers: this.headers(key) });
  }

  create(key: string, parish: ParishForm): Observable<{ id: string }> {
    return this.http.post<{ id: string }>('/api/admin/parishes', parish, { headers: this.headers(key) });
  }

  update(key: string, parish: ParishForm): Observable<void> {
    return this.http.put<void>(`/api/admin/parishes/${parish.id}`, parish, { headers: this.headers(key) });
  }

  delete(key: string, id: string): Observable<void> {
    return this.http.delete<void>(`/api/admin/parishes/${id}`, { headers: this.headers(key) });
  }

  private headers(key: string): HttpHeaders {
    return new HttpHeaders({ 'X-Admin-Key': key });
  }
}
