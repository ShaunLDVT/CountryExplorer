import { Component, OnInit } from '@angular/core';
import { CountryService } from '../../services/country.service';
import { CountrySummary } from '../../models/country-summary';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-country-list',
  templateUrl: './country-list.component.html',
  styleUrls: ['./country-list.component.scss'],
  imports: [CommonModule]
})
export class CountryListComponent implements OnInit {
  countries: CountrySummary[] = [];
  loading = true;
  error = '';

  constructor(
    private countryService: CountryService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadCountries();
  }

  loadCountries(): void {
    this.loading = true;
    this.countryService.getAllCountries()
      .subscribe({
        next: (data) => {
          this.countries = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load countries. Please try again later.';
          this.loading = false;
          console.error('Error loading countries:', err);
        }
      });
  }

  viewCountryDetails(name: string): void {
    this.router.navigate(['/country', name]);
  }
}