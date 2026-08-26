import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { ParishDetailsComponent } from './app/parish-details.component';
const path = location.pathname.replace(/\/+$/, '') || '/';
const root = path.startsWith('/paroquias/') ? ParishDetailsComponent : AppComponent;
bootstrapApplication(root, { providers: [provideHttpClient()] }).catch(console.error);
