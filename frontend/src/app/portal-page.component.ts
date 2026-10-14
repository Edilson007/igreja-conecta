import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Parish } from './core/models/parish';
type Content={id:string;category:string;status:string;title:string;eventStartsAt?:string;dayOfWeek?:string;time?:string;location?:string;imageUrl?:string;description?:string};
@Component({selector:'app-root',standalone:true,imports:[CommonModule,FormsModule],
templateUrl:'./portal-page.component.html',styleUrls:['./portal-page.component.css','./portal-links-footer.css']})
export class PortalPageComponent {
  parishes:Parish[]=[]; parishId=''; parishName=''; parishCity=''; isPremium=false;
  email=''; password=''; token=''; name=''; role=''; message=''; busy=false;
  contents:Content[]=[]; editingId=''; form=this.emptyForm();
  readonly categories=[{value:'Event',label:'Eventos'},{value:'PrayerGroup',label:'Grupos de oração'},{value:'YouthGroup',label:'Grupos de jovens'},{value:'Notice',label:'Outros eventos e avisos'}];
  constructor(private http:HttpClient){http.get<Parish[]>('/api/parishes').subscribe({next:x=>this.parishes=x,error:e=>this.fail(e)});}
  emptyForm(){return {category:'Event',title:'',eventStartsAt:'',dayOfWeek:'',time:'',location:'',imageUrl:'',description:''};}
  get canPublish(){return this.role==='Administrator'||this.role==='Publisher';}
  get roleLabel(){return this.role==='Administrator'?'Administrador paroquial':this.role==='Publisher'?'Publicador':'Editor';}
  get publicUrl(){return '/paroquias/'+this.slugify(`${this.parishName}-${this.parishCity}`);}
  private slugify(value:string){return value.normalize('NFD').replace(/[\u0300-\u036f]/g,'').toLowerCase().replace(/[^a-z0-9]+/g,'-').replace(/^-|-$/g,'');}
  categoryLabel(v:string){return this.categories.find(x=>x.value===v)?.label||v;}
  statusLabel(v:string){return ({Draft:'Rascunho',PendingReview:'Aguardando aprovação',Published:'Publicado',ChangesRequested:'Ajustes solicitados'} as Record<string,string>)[v]||v;}
  headers(){return new HttpHeaders({'X-Portal-Token':this.token});}
  login(){
    if(!this.parishId||!this.email||!this.password){this.message='Selecione a paróquia e informe e-mail e senha.';return;}
    this.busy=true;this.message='';
    this.http.post<{token:string;name:string;role:string;parishId:string;parishName:string;parishCity:string;isPremium:boolean}>('/api/portal/login',{parishId:this.parishId,email:this.email,password:this.password}).subscribe({
      next:x=>{this.token=x.token;this.name=x.name;this.role=x.role;this.parishId=x.parishId;this.parishName=x.parishName;this.parishCity=x.parishCity;this.isPremium=x.isPremium;this.password='';this.busy=false;this.load();},
      error:e=>this.fail(e)});
  }
  logout(){this.token='';this.contents=[];this.message='';this.cancelEdit();}
  load(){this.http.get<Content[]>('/api/portal/contents',{headers:this.headers()}).subscribe({next:x=>this.contents=x,error:e=>this.fail(e)});}
  edit(item:Content){
    this.editingId=item.id;let localDate='';
    if(item.eventStartsAt){const d=new Date(item.eventStartsAt);localDate=new Date(d.getTime()-d.getTimezoneOffset()*60000).toISOString().slice(0,16);}
    this.form={category:item.category,title:item.title,eventStartsAt:localDate,dayOfWeek:item.dayOfWeek||'',time:item.time||'',location:item.location||'',imageUrl:item.imageUrl||'',description:item.description||''};this.message='';
  }
  cancelEdit(){this.editingId='';this.form=this.emptyForm();}
  save(){
    if(!this.form.title.trim()){this.message='Informe o título.';return;}
    const d=this.form.eventStartsAt?new Date(this.form.eventStartsAt):null;
    if(d&&Number.isNaN(d.getTime())){this.message='Informe uma data válida.';return;}
    const payload={...this.form,eventStartsAt:d?.toISOString()||null};
    this.busy=true;this.message='';
    const request=this.editingId?this.http.put('/api/portal/contents/'+this.editingId,payload,{headers:this.headers()}):this.http.post('/api/portal/contents',payload,{headers:this.headers()});
    request.subscribe({next:()=>{this.busy=false;this.cancelEdit();this.message='Rascunho salvo. Envie para aprovação quando estiver pronto.';this.load();},error:e=>this.fail(e)});
  }
  action(id:string,action:string){
    this.busy=true;this.message='';
    this.http.post('/api/portal/contents/'+id+'/'+action,{},{headers:this.headers()}).subscribe({
      next:()=>{this.busy=false;this.message=action==='publish'?'Publicado. Abra a página da paróquia para conferir.':action==='submit'?'Enviado para aprovação.':'Devolvido para edição e removido da página pública.';this.load();},error:e=>this.fail(e)});
  }
  private fail(e:HttpErrorResponse){this.busy=false;if(e.status===401){this.token='';this.message='Acesso inválido ou sessão expirada. Entre novamente.';}else if(e.status===403)this.message='Seu perfil não tem permissão para esta ação.';else this.message=typeof e.error==='string'?e.error:'Não foi possível concluir. Verifique se a API está rodando e tente novamente.';}
}
