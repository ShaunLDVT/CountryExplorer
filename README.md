# CountryExplorer

CountryExplorer is a web application that allows users to explore information about countries, including their names, population, region, and more. The project consists of a .NET 9 backend API and an Angular frontend.

---

## Table of Contents
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Running the Application](#running-the-application)
  - [Running the Backend](#running-the-backend)
  - [Running the Frontend](#running-the-frontend)
---

## Prerequisites

Before setting up the application, ensure you have the following installed:

1. **.NET 9 SDK**  
   Download and install from [Microsoft's .NET website](https://dotnet.microsoft.com/).

2. **Node.js (v20.x or later)**  
   Download and install from [Node.js website](https://nodejs.org/).

3. **Angular CLI**  
   Install globally using npm:
   ```

   npm install -g @angular/cli

   ```
5. **Redis (Optional)**  
   Redis is used for distributed caching in the backend. Install Redis locally or use a cloud-hosted Redis instance.
   ```

   docker run --name redis -p 6379:6379 -d redis

   ```

---

## Setup Instructions

### Backend Setup

1. Navigate to the backend project directory:
```

   cd CountryExplorer.API

```
2. Restore dependencies:
```

   dotnet restore

```
3. Update the `appsettings.json` file with the required configuration:
```
{ 
      "Urls": { "CountryApiBaseUrl": "https://restcountries.com/v3.1" }, 
      "Cache": { "RedisConnection": "localhost:6379" }, 
      "Cors": { "AllowedOrigins": [ "http://localhost:4200" ] } 
}
```

4. Ensure Redis is running locally or update the `RedisConnection` string to point to your Redis instance.

---

### Frontend Setup

1. Navigate to the frontend project directory:
```

   cd CountryExplorer.UI

```
   
2. Install dependencies:
```

   npm install

```

3. Update the `environment.ts` file with the backend API URL:
```

   export const environment = { production: false, apiUrl: 'https://localhost:7181' };

```
---

## Running the Application

### Running the Backend

1. Navigate to the backend project directory:
```

   cd CountryExplorer.API

```

2. Run the backend API:
```

   dotnet run

```

3. The API will be available at `https://localhost:7181`.

---

### Running the Frontend

1. Navigate to the frontend project directory:
```

   cd CountryExplorer.UI

```

2. Start the Angular development server:
```

   ng serve

```
3. The frontend will be available at `http://localhost:4200`.
