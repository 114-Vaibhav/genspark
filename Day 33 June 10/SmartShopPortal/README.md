# SmartShop Portal

SmartShop Portal is an Angular application created for practicing routing, login flow, API integration, shared user state, and route protection.

## Features

- Login page using DummyJSON authentication API
- Protected dashboard area after login
- Product listing from DummyJSON products API
- Product details page using route parameter
- Profile page showing logged-in user information
- Header/home screen with personalized greeting
- Logout support using session storage

## Main Pages

- `/` - Login page
- `/dashboard/header` - Welcome page
- `/dashboard/products` - Product list
- `/dashboard/product-details/:id` - Product details
- `/dashboard/profile` - User profile

## APIs Used

- `POST https://dummyjson.com/auth/login`
- `GET https://dummyjson.com/products`
- `GET https://dummyjson.com/products/{id}`

## Project Structure

```text
src/app/
├── components/
│   ├── dashboard/
│   ├── header/
│   ├── login/
│   ├── product-details/
│   ├── products/
│   └── profile/
├── guards/
├── models/
├── rxjs/
└── services/
```

## Run Project

Install dependencies:

```bash
npm install 

```

Start development server:

```bash
npm start
or ng serve 
```

Open the app at:

```text
http://localhost:4200/
```

## Build

```bash
npm run build
```

## Notes

The app uses Angular standalone components, Angular Router, HttpClient services, and DummyJSON data for authentication and products.
