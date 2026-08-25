import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { ParishDetailsComponent } from './app/parish-details.component';
const root = location.pathname.startsWith('/paroquias/') ? ParishDetailsComponent : AppComponent;
bootstrapApplication(root, { providers: [provideHttpClient()] }).catch(console.error);
