import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CountrySummary } from '../models/country-summary';
import { Country } from '../models/country';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  private apiUrl = environment.apiUrl + '/api/countries';

  constructor(private http: HttpClient) { }

  getAllCountries(): Observable<CountrySummary[]> {
    return this.http.get<CountrySummary[]>(this.apiUrl);
  }

  getCountryByName(name: string): Observable<Country> {
    return this.http.get<Country>(`${this.apiUrl}/${name}`);
  }
}