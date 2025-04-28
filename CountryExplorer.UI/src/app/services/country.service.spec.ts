import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CountryService } from './country.service';
import { CountrySummary } from '../models/country-summary';
import { Country, Currency } from '../models/country';
import { environment } from '../../environments/environment';

describe('CountryService', () => {
  let service: CountryService;
  let httpMock: HttpTestingController;
  const apiUrl = environment.apiUrl + '/api/countries';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CountryService]
    });

    service = TestBed.inject(CountryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllCountries', () => {
    it('should return an Observable<CountrySummary[]>', () => {
      const mockCountries: CountrySummary[] = [
        { name: 'United States', alpha3Code: 'USA', flagUrl: 'https://example.com/usa.png' },
        { name: 'Canada', alpha3Code: 'CAN', flagUrl: 'https://example.com/canada.png' }
      ];

      service.getAllCountries().subscribe(countries => {
        expect(countries.length).toBe(2);
        expect(countries).toEqual(mockCountries);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockCountries);
    });

    it('should handle errors when the API returns an error', () => {
      service.getAllCountries().subscribe({
        next: () => fail('Expected an error, not countries'),
        error: error => expect(error.status).toBe(500)
      });

      const req = httpMock.expectOne(apiUrl);
      req.flush('Internal server error', {
        status: 500,
        statusText: 'Server Error'
      });
    });
  });

  describe('getCountryByName', () => {
    it('should return a country by name', () => {
      const countryName = 'Germany';
      const mockCurrency: Currency = { name: 'Euro', symbol: '€' };
      const mockCurrencies: { [key: string]: Currency } = { 'EUR': mockCurrency };
      const mockLanguages: { [key: string]: string } = { 'deu': 'German' };

      const mockCountry: Country = {
        name: 'Germany',
        commonName: 'Germany',
        officialName: 'Federal Republic of Germany',
        alpha2Code: 'DE',
        alpha3Code: 'DEU',
        population: 83000000,
        capital: 'Berlin',
        region: 'Europe',
        subregion: 'Western Europe',
        flagUrl: 'https://example.com/germany.png',
        languages: mockLanguages,
        currencies: mockCurrencies
      };

      service.getCountryByName(countryName).subscribe(country => {
        expect(country).toEqual(mockCountry);
      });

      const req = httpMock.expectOne(`${apiUrl}/${countryName}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockCountry);
    });

    it('should handle errors when getting country details fails', () => {
      const countryName = 'NonExistentCountry';

      service.getCountryByName(countryName).subscribe({
        next: () => fail('Expected an error, not a country'),
        error: error => expect(error.status).toBe(404)
      });

      const req = httpMock.expectOne(`${apiUrl}/${countryName}`);
      req.flush('Country not found', {
        status: 404,
        statusText: 'Not Found'
      });
    });
  });
});
