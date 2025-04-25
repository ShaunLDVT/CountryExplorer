import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CountryDetailsComponent } from './components/country-details/country-details.component';
import { CountryListComponent } from './components/country-list/country-list.component';

const routes: Routes = [
  { path: '', component: CountryListComponent },
  { path: 'country/:name', component: CountryDetailsComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
