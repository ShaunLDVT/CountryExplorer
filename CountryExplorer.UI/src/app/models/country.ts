export interface Country {
    name: string;
    commonName: string;
    officialName: string;
    alpha2Code: string;
    alpha3Code: string;
    population: number;
    capital: string;
    region: string;
    subregion: string;
    flagUrl: string;
    languages: { [key: string]: string };
    currencies: { [key: string]: Currency };
  }
  
  export interface Currency {
    name: string;
    symbol: string;
  }