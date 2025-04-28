import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { CountryListComponent } from './country-list.component';
import { CountryService } from '../../services/country.service';
import { CountrySummary } from '../../models/country-summary';

describe('CountryListComponent', () => {
  let component: CountryListComponent;
  let fixture: ComponentFixture<CountryListComponent>;
  let countryServiceSpy: jasmine.SpyObj<CountryService>;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockCountries: CountrySummary[] = [
    { name: 'United States', alpha3Code: 'USA', flagUrl: 'https://example.com/usa.png' },
    { name: 'Canada', alpha3Code: 'CAN', flagUrl: 'https://example.com/canada.png' }
  ];

  beforeEach(async () => {
    const countryService = jasmine.createSpyObj('CountryService', ['getAllCountries']);
    const router = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, CountryListComponent], // Import instead of declare
      providers: [
        { provide: CountryService, useValue: countryService },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();

    countryServiceSpy = TestBed.inject(CountryService) as jasmine.SpyObj<CountryService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountryListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load countries on init', fakeAsync(() => {
    countryServiceSpy.getAllCountries.and.returnValue(of(mockCountries));

    component.ngOnInit();
    tick();

    expect(component.countries).toEqual(mockCountries);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
  }));

  it('should handle error when loading countries fails', fakeAsync(() => {
    countryServiceSpy.getAllCountries.and.returnValue(
      throwError(() => new Error('Network Error'))
    );

    component.ngOnInit();
    tick();

    expect(component.countries).toEqual([]);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('Failed to load countries. Please try again later.');
  }));

  it('should navigate to country details when viewCountryDetails is called', () => {
    component.viewCountryDetails('Canada');

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/country', 'Canada']);
  });
});
