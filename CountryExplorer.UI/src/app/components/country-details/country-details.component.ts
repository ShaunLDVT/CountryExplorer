import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CountryService } from '../../services/country.service';
import { Country } from '../../models/country';
import { CommonModule } from '@angular/common';
import { MatProgressSpinner } from '@angular/material/progress-spinner';


@Component({
  selector: 'app-country-details',
  templateUrl: './country-details.component.html',
  styleUrls: ['./country-details.component.scss'],
  imports: [CommonModule, MatProgressSpinner]
})
export class CountryDetailsComponent implements OnInit {
  country: Country | null = null;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private countryService: CountryService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const countryName = params.get('name');
      if (countryName) {
        this.loadCountryDetails(countryName);
      } else {
        this.error = 'Country name not provided';
        this.loading = false;
      }
    });
  }

  loadCountryDetails(name: string): void {
    this.loading = true;
    this.countryService.getCountryByName(name)
      .subscribe({
        next: (data) => {
          this.country = data;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load country details. Please try again later.';
          this.loading = false;
          console.error('Error loading country details:', err);
        }
      });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  getLanguages(): string {
    if (!this.country || !this.country.languages || Object.keys(this.country.languages).length === 0) {
      return 'N/A';
    }

    return Object.values(this.country.languages).join(', ');
  }

  getCurrencies(): string {
    if (!this.country || !this.country.currencies || Object.keys(this.country.currencies).length === 0) {
      return 'N/A';
    }

    return Object.values(this.country.currencies)
      .map(c => `${c.name} (${c.symbol})`)
      .join(', ');
  }
}
