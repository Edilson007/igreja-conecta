import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { ParishDetailsComponent } from './app/parish-details.component';
import { AdminPageComponent } from './app/admin-page.component';
import { AdminAccessPageComponent } from './app/admin-access-page.component';
import { PortalPageComponent } from './app/portal-page.component';
// The local static server does not rewrite deep links to index.html. Accepting
// hash routes keeps the same screens accessible in development and production.
const requestedPath = location.hash.startsWith('#/') ? location.hash.slice(1) : location.pathname;
const path = requestedPath.replace(/\/+$/, '') || '/';
const options = { providers: [provideHttpClient()] };
if (path === '/portal') bootstrapApplication(PortalPageComponent, options).catch(console.error);
else if (path === '/admin/acessos') bootstrapApplication(AdminAccessPageComponent, options).catch(console.error);
else if (path === '/admin') bootstrapApplication(AdminPageComponent, options).catch(console.error);
else if (path.startsWith('/paroquias/')) bootstrapApplication(ParishDetailsComponent, options).catch(console.error);
else bootstrapApplication(AppComponent, options).catch(console.error);
