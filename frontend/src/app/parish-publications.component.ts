import { Component, Input, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
interface PublishedContent { id:string;category:string;title:string;eventStartsAt?:string;dayOfWeek?:string;time?:string;location?:string;imageUrl?:string;description?:string; }
@Component({
  selector:'app-parish-publications',standalone:true,imports:[CommonModule],
  styles:[`:host{display:block;margin-top:36px}h2{font:28px Georgia,serif;color:#174c3c}.cards{display:grid;grid-template-columns:repeat(auto-fit,minmax(260px,1fr));gap:18px}article{background:white;border:1px solid #dbe6df;border-radius:14px;overflow:hidden}img{width:100%;max-height:300px;object-fit:contain;background:#eef3ed}.body{padding:22px}h3{font:24px Georgia,serif;color:#174c3c;margin:0 0 16px}.description{white-space:pre-wrap;overflow-wrap:anywhere}.meta{color:#52665b}.status{padding:18px;background:white;border-radius:8px}section{margin-bottom:32px}`],
  template:`
    <p class="status" *ngIf="loading">Carregando publicações...</p>
    <p class="status" *ngIf="error">{{error}} <button (click)="load()">Tentar novamente</button></p>
    <p class="status" *ngIf="!loading&&!error&&!contents.length">Esta paróquia ainda não tem eventos ou grupos publicados.</p>
    <ng-container *ngFor="let category of categories">
      <section *ngIf="byCategory(category.value).length">
        <h2>{{category.label}}</h2><div class="cards"><article *ngFor="let item of byCategory(category.value)">
          <img *ngIf="safeImage(item.imageUrl)" [src]="item.imageUrl" [alt]="item.title" loading="lazy">
          <div class="body"><h3>{{item.title}}</h3>
            <p class="meta" *ngIf="item.eventStartsAt"><b>Quando:</b> {{eventDate(item.eventStartsAt)}}</p>
            <p class="meta" *ngIf="item.dayOfWeek||item.time"><b>Encontros:</b> {{item.dayOfWeek}} {{item.time}}</p>
            <p class="meta" *ngIf="item.location"><b>Local:</b> {{item.location}}</p>
            <p class="description">{{item.description}}</p>
          </div></article></div>
      </section>
    </ng-container>`
})
export class ParishPublicationsComponent implements OnChanges {
  @Input() parishId=''; contents:PublishedContent[]=[]; loading=false; error='';
  readonly categories=[{value:'Event',label:'Eventos'},{value:'PrayerGroup',label:'Grupos de oração'},{value:'YouthGroup',label:'Grupos de jovens'},{value:'Notice',label:'Outros eventos e avisos'}];
  constructor(private http:HttpClient){}
  ngOnChanges(){this.load();}
  load(){if(!this.parishId)return;this.loading=true;this.error='';this.contents=[];
    this.http.get<PublishedContent[]>('/api/parishes/'+this.parishId+'/contents').subscribe({
      next:x=>{this.contents=x;this.loading=false;},error:()=>{this.loading=false;this.error='Não foi possível carregar as publicações.';}});
  }
  byCategory(value:string){return this.contents.filter(x=>x.category===value);}
  safeImage(value?:string){return !!value&&(/^https?:\/\//i.test(value)||/^\/(?!\/)/.test(value));}
  eventDate(value:string){return new Intl.DateTimeFormat('pt-BR',{dateStyle:'short',timeStyle:'short',timeZone:'America/Sao_Paulo'}).format(new Date(value));}
}
