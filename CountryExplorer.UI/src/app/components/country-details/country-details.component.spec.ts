import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CountryDetailsComponent } from './country-details.component';
import { CountryService } from '../../services/country.service';
import { Country, Currency } from '../../models/country';

describe('CountryDetailsComponent', () => {
  let component: CountryDetailsComponent;
  let fixture: ComponentFixture<CountryDetailsComponent>;
  let countryServiceSpy: jasmine.SpyObj<CountryService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let activatedRouteMock: any;

  const mockCurrency: Currency = { name: 'Euro', symbol: '€' };
  const mockCurrencies: { [key: string]: Currency } = { 'EUR': mockCurrency };
  const mockLanguages: { [key: string]: string } = { 'deu': 'German', 'eng': 'English' };

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

  beforeEach(async () => {
    const countryService = jasmine.createSpyObj('CountryService', ['getCountryByName']);
    const router = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CountryDetailsComponent], // Import instead of declare for standalone components
      providers: [
        { provide: CountryService, useValue: countryService },
        { provide: Router, useValue: router },
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ name: 'Germany' }))
          }
        }
      ]
    }).compileComponents();

    countryServiceSpy = TestBed.inject(CountryService) as jasmine.SpyObj<CountryService>;
    routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    activatedRouteMock = TestBed.inject(ActivatedRoute);
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountryDetailsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load country details on init', fakeAsync(() => {
    countryServiceSpy.getCountryByName.and.returnValue(of(mockCountry));

    component.ngOnInit();
    tick();

    expect(component.country).toEqual(mockCountry);
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('');
  }));

  it('should handle error when loading country details fails', fakeAsync(() => {
    countryServiceSpy.getCountryByName.and.returnValue(
      throwError(() => new Error('Network Error'))
    );

    component.ngOnInit();
    tick();

    expect(component.country).toBeNull();
    expect(component.loading).toBeFalse();
    expect(component.error).toBe('Failed to load country details. Please try again later.');
  }));

  it('should handle case when country name is not provided', fakeAsync(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [CountryDetailsComponent],
      providers: [
        { provide: CountryService, useValue: countryServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({})) // Empty params
          }
        }
      ]
    });

    fixture = TestBed.createComponent(CountryDetailsComponent);
    component = fixture.componentInstance;

    component.ngOnInit();
    tick();

    expect(component.error).toBe('Country name not provided');
    expect(component.loading).toBeFalse();
  }));

  it('should navigate back when goBack is called', () => {
    component.goBack();

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should format languages correctly', () => {
    component.country = mockCountry;

    expect(component.getLanguages()).toBe('German, English');
  });

  it('should format currencies correctly', () => {
    component.country = mockCountry;

    expect(component.getCurrencies()).toBe('Euro (€)');
  });

  it('should return N/A when languages are not available', () => {
    component.country = { ...mockCountry, languages: {} };

    expect(component.getLanguages()).toBe('N/A');
  });

  it('should return N/A when currencies are not available', () => {
    component.country = { ...mockCountry, currencies: {} };

    expect(component.getCurrencies()).toBe('N/A');
  });

  it('should return N/A when country is null', () => {
    component.country = null;

    expect(component.getLanguages()).toBe('N/A');
    expect(component.getCurrencies()).toBe('N/A');
  });
});
