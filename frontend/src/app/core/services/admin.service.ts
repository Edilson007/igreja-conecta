import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

export interface ParishForm { name: string; sector: string; city: string; state: string; address: string; phone: string; imageUrl: string; isPremium: boolean; massDay: string; massTime: string; massDescription: string; }

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private readonly http: HttpClient) {}
  create(key: string, form: ParishForm) {
    return this.http.post('/api/admin/parishes', {
      name: form.name, sector: form.sector, city: form.city, state: form.state, address: form.address, phone: form.phone, imageUrl: form.imageUrl, isPremium: form.isPremium,
      massSchedules: form.massDay && form.massTime ? [{ day: form.massDay, time: form.massTime, description: form.massDescription }] : []
    }, { headers: new HttpHeaders({ 'X-Admin-Key': key }) });
  }
}
