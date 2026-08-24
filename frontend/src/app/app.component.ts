import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Parish } from './core/models/parish';
import { ParishService } from './core/services/parish.service';

@Component({
  selector: 'app-root', standalone: true, imports: [CommonModule, FormsModule],
  template: `
    <header><div class="brand">✦ Igreja Conecta</div><span>Diocese de Guaxupé · piloto</span></header>
    <main>
      <section class="hero"><h1>Horários e atividades na Diocese de Guaxupé</h1><p>Protótipo para encontrar missas e comunidades. Os horários mostrados são dados de teste e devem ser confirmados com cada paróquia.</p>
        <div class="search"><input [(ngModel)]="city" placeholder="Cidade"><select [(ngModel)]="sector"><option value="">Todos os setores</option><option *ngFor="let item of sectors" [value]="item">{{ item }}</option></select><input [(ngModel)]="query" placeholder="Paróquia ou endereço"><button (click)="load()">Buscar</button></div>
      </section>
      <p *ngIf="loading">Buscando paróquias...</p><p *ngIf="error" class="error">{{ error }}</p>
      <section class="results"><article *ngFor="let parish of parishes"><div class="tag">{{ parish.sector }}</div><h2>{{ parish.name }}</h2><p>{{ parish.address }} · {{ parish.city }}/{{ parish.state }}</p><p class="confirmed">Horários confirmados em: {{ parish.lastScheduleConfirmation | date:'dd/MM/yyyy' }}</p><h3>Horários de missa</h3><ul><li *ngFor="let mass of parish.massSchedules">{{ dayLabel(mass.day) }} às {{ mass.time }} <span>{{ mass.description }}</span></li></ul><div *ngIf="parish.chapels.length"><h3>Capelas e comunidades</h3><ul><li *ngFor="let chapel of parish.chapels">{{ chapel.name }}</li></ul></div><a *ngIf="parish.phone" [href]="'tel:' + parish.phone">Contato: {{ parish.phone }}</a></article></section>
    </main>`,
  styles: [`header{display:flex;justify-content:space-between;align-items:center;padding:1.2rem 8%;background:#173c5a;color:#fff}.brand{font-size:1.25rem;font-weight:700}main{max-width:1040px;margin:auto;padding:2rem}.hero{padding:3rem 0}.hero h1{font-size:2.2rem;margin:0}.search{display:flex;gap:.7rem;margin-top:1.5rem}.search input{flex:1;padding:.8rem;border:1px solid #cbd5e1;border-radius:.4rem}.search button{background:#c79535;color:#fff;border:0;border-radius:.4rem;padding:0 1.2rem;font-weight:700}.results{display:grid;grid-template-columns:repeat(auto-fit,minmax(280px,1fr));gap:1rem}article{position:relative;border:1px solid #dce3e9;border-radius:.6rem;padding:1.25rem;background:#fff;box-shadow:0 2px 7px #0000000a}h2{color:#173c5a;margin-top:0}.tag{position:absolute;right:1rem;top:1rem;font-size:.75rem;color:#805b19;background:#fff3d8;padding:.25rem .5rem;border-radius:1rem}li{margin:.5rem 0}li span{color:#64748b}.error{color:#b91c1c}@media(max-width:640px){header{display:block}.search{flex-direction:column}.search button{padding:.8rem}}`]
})
export class AppComponent implements OnInit {
  city = ''; sector = ''; query = ''; parishes: Parish[] = []; loading = false; error = '';
  readonly sectors = ['Setor Guaxupé', 'Setor Poços de Caldas', 'Setor Passos', 'Setor Cássia'];
  constructor(private readonly parishService: ParishService) {}
  ngOnInit() { this.load(); }
  load() { this.loading = true; this.error = ''; this.parishService.search(this.city, this.sector, this.query).subscribe({ next: data => { this.parishes = data; this.loading = false; }, error: () => { this.error = 'Não foi possível carregar as paróquias. Verifique se a API está em execução.'; this.loading = false; } }); }
  dayLabel(day: string) { return ({ Sunday: 'Domingo', Monday: 'Segunda', Tuesday: 'Terça', Wednesday: 'Quarta', Thursday: 'Quinta', Friday: 'Sexta', Saturday: 'Sábado' } as Record<string, string>)[day] ?? day; }
}
