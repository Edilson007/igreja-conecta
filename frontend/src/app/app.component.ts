import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Parish } from './core/models/parish';
import { ParishService } from './core/services/parish.service';

@Component({
  selector: 'app-root', standalone: true, imports: [CommonModule, FormsModule],
  template: `
    <header class="site-header"><a class="brand" href="#inicio"><span class="brand-mark">✦</span><span><strong>Igreja</strong> Conecta<small>Diocese de Guaxupé</small></span></a><nav><a href="#missas">Missas</a><a href="#paroquias">Paróquias</a><a href="#sobre">Sobre</a></nav><a class="header-action" href="#paroquias">Encontrar igreja <span>→</span></a></header>
    <main id="inicio">
      <section class="hero"><div class="hero-copy"><p class="eyebrow"><span></span> Diocese de Guaxupé</p><h1>Sua comunidade<br><em>mais perto de você.</em></h1><p class="hero-description">Encontre horários de missas, atividades e informações das paróquias da diocese em um só lugar.</p><div class="hero-trust"><b>✓</b> Horários atualizados pelas próprias paróquias</div></div><div class="hero-art" aria-hidden="true"><i class="sun"></i><i class="cross">✦</i><i class="hill"></i><i class="chapel">✝</i></div></section>
      <section id="missas" class="finder"><div class="finder-heading"><p class="eyebrow dark"><span></span> Encontre uma celebração</p><h2>Onde você quer celebrar?</h2></div><form class="search-panel" (ngSubmit)="load()"><label><span>⌖</span><input [(ngModel)]="city" name="city" placeholder="Em qual cidade?"></label><label><span>⌕</span><input [(ngModel)]="query" name="query" placeholder="Paróquia ou bairro"></label><label><span>◫</span><select [(ngModel)]="sector" name="sector"><option value="">Todos os setores</option><option *ngFor="let item of sectors" [value]="item">{{ item }}</option></select></label><button type="submit">Buscar <b>→</b></button></form><button *ngIf="hasFilters" type="button" class="clear-filters" (click)="clearFilters()">Limpar filtros</button></section>
      <section class="quick-links"><a href="#paroquias"><span class="quick-icon">⌂</span><span><b>Paróquias</b><small>Encontre a sua comunidade</small></span><i>→</i></a><a href="#paroquias"><span class="quick-icon">□</span><span><b>Próximas atividades</b><small>Participe da vida da Igreja</small></span><i>→</i></a><a href="#sobre"><span class="quick-icon">♡</span><span><b>Precisa de ajuda?</b><small>Fale com uma paróquia</small></span><i>→</i></a></section>
      <section id="paroquias" class="parishes"><div class="section-heading"><div><p class="eyebrow dark"><span></span> Comunidades</p><h2>Paróquias para você</h2><p>{{ resultDescription }}</p></div><div class="result-count" *ngIf="!loading"><strong>{{ parishes.length }}</strong> encontradas</div></div><div *ngIf="loading" class="state-message"><span class="spinner"></span> Buscando paróquias...</div><div *ngIf="error" class="state-message error"><b>Não foi possível carregar.</b> {{ error }} <button (click)="load()">Tentar novamente</button></div><div *ngIf="!loading && !error && parishes.length === 0" class="empty-state"><span>⌕</span><h3>Nenhuma paróquia encontrada</h3><p>Experimente buscar por outra cidade ou limpe os filtros.</p><button (click)="clearFilters()">Ver todas as paróquias</button></div><div class="parish-grid" *ngIf="!loading && parishes.length"><article class="parish-card" *ngFor="let parish of parishes; trackBy: trackById"><div class="card-top"><span class="sector-tag">{{ shortSector(parish.sector) }}</span><span class="premium" *ngIf="parish.isPremium">Comunidade parceira</span></div><h3>{{ parish.name }}</h3><p class="location">⌖ {{ parish.city }}, {{ parish.state }}</p><div class="next-mass" *ngIf="parish.massSchedules.length; else noMasses"><span class="mass-icon">✦</span><div><small>MISSAS</small><p><b>{{ dayLabel(parish.massSchedules[0].day) }}</b> · {{ parish.massSchedules[0].time }}</p><em *ngIf="parish.massSchedules[0].description">{{ parish.massSchedules[0].description }}</em></div></div><ng-template #noMasses><div class="next-mass"><span class="mass-icon">–</span><div><small>MISSAS</small><p>Horários em atualização</p></div></div></ng-template><div class="activities" *ngIf="parish.activities.length"><span>Próxima atividade</span><b>{{ parish.activities[0].title }}</b></div><a class="card-link" [href]="parish.phone ? 'tel:' + parish.phone : '#paroquias'">{{ parish.phone ? 'Falar com a paróquia' : 'Ver informações' }} <span>→</span></a></article></div></section>
      <section id="sobre" class="about-band"><div><p class="eyebrow"><span></span> Igreja Conecta</p><h2>Feito para aproximar pessoas e comunidades.</h2></div><p>Esta é uma demonstração com dados de teste. Confirme os horários diretamente com a paróquia antes de se deslocar.</p></section>
    </main><footer><span>© {{ currentYear }} Igreja Conecta</span><span>Um projeto em teste para a Diocese de Guaxupé</span></footer>
  `, styles: []
})
export class AppComponent implements OnInit {
  city = ''; sector = ''; query = ''; parishes: Parish[] = []; loading = false; error = '';
  readonly currentYear = new Date().getFullYear(); readonly sectors = ['Setor Guaxupé', 'Setor Poços de Caldas', 'Setor Passos', 'Setor Cássia'];
  constructor(private readonly parishService: ParishService) {}
  get hasFilters() { return Boolean(this.city || this.sector || this.query); }
  get resultDescription() { return this.hasFilters ? 'Resultados para sua busca na Diocese de Guaxupé.' : 'Conheça as comunidades disponíveis na Diocese de Guaxupé.'; }
  ngOnInit() { this.load(); }
  load() { this.loading = true; this.error = ''; this.parishService.search(this.city, this.sector, this.query).subscribe({ next: data => { this.parishes = data; this.loading = false; }, error: () => { this.error = 'Verifique sua conexão e tente novamente.'; this.loading = false; } }); }
  clearFilters() { this.city = ''; this.sector = ''; this.query = ''; this.load(); }
  trackById(_: number, parish: Parish) { return parish.id; }
  shortSector(sector: string) { return sector.replace('Setor ', ''); }
  dayLabel(day: string) { return ({ Sunday: 'Domingo', Monday: 'Segunda-feira', Tuesday: 'Terça-feira', Wednesday: 'Quarta-feira', Thursday: 'Quinta-feira', Friday: 'Sexta-feira', Saturday: 'Sábado' } as Record<string, string>)[day] ?? day; }
}
