# 🚌 Bus Booking System

A comprehensive full-stack bus booking and management platform built with modern technologies. This system provides complete functionality for users to book bus tickets, operators to manage buses and trips, and admins to oversee the entire platform.

---

## 📋 Table of Contents

- [Features](#features)
- [System Architecture](#system-architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Installation & Setup](#installation--setup)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Role-Based Access](#role-based-access)
- [Key Implementations](#key-implementations)
- [Known Configurations](#known-configurations)
- [Future Enhancements](#future-enhancements)

---

## ✨ Features

### 🧑‍💼 **User (Traveler) Features**

- **Authentication**: Email/password login and SSO registration
- **Trip Search**: Fuzzy logic search by source, destination, and date
- **Seat Selection**: Interactive 2D seat layout with real-time availability
- **Single Lady Feature**: Pink-highlighted female seats; female-only bookings with automatic gender enforcement
- **Concurrent Booking**: 5-minute seat lock with automatic expiry to prevent overbooking
- **Traveler Details**: Capture name, age, gender for each passenger
- **Payment Processing**: Dummy payment gateway integration with confirmation
- **Ticket Management**:
  - Download tickets as text files
  - Email confirmation with ticket details
  - Pickup and drop-off addresses included
- **Booking History**: View past, present, and future bookings with status tracking
- **Cancellation**: Cancel bookings with dynamic refund rules:
  - **48+ hours before journey**: 75% refund
  - **24-48 hours**: 50% refund
  - **6-24 hours**: 25% refund
  - **< 6 hours**: No refund
- **User Profile**: View and edit personal information (name, mobile, gender, DOB)

### 🚌 **Operator Features**

- **Registration**: Apply as operator (requires admin approval)
- **Bus Management**:
  - Register buses with unique bus numbers
  - Auto-generate seat layouts (2+2, 2+3, 1+2 configurations)
  - Choose bus type (Seater or Sleeper)
  - Mark buses as active/inactive
  - Send email notifications to affected passengers on bus removal
- **Trip Management**:
  - Create trips by assigning buses to routes
  - Set departure/arrival times
  - Define dynamic pricing
  - Specify pickup and drop-off addresses
  - View all bookings for their buses
- **Booking Visibility**:
  - See all bookings across their buses
  - View trip-wise passenger details
- **Dashboard**: View total revenue per bus/trip

### 👨‍💼 **Admin Features**

- **Operator Management**:
  - Approve/reject pending operator registrations
  - Enable/disable operators
  - View operator revenue breakdowns
- **Route Management**:
  - Add new routes (source, destination, distance)
  - Enable/disable routes
  - Edit route information
- **Platform Configuration**:
  - Set platform/convenience fee percentage
  - Configure email settings
  - Manage system settings
- **User Notifications**:
  - Automatic email notifications on booking confirmations
  - Cancellation notifications with refund amounts
  - Bus removal notifications to all affected passengers

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT LAYER                             │
│  Angular SPA (Frontend) - Responsive Web Application        │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ↓ HTTP/HTTPS
┌─────────────────────────────────────────────────────────────┐
│                    API LAYER                                │
│  .NET Core 8.0 REST API with JWT Authentication            │
│  - Auth Controller (Login, Register)                        │
│  - User Controller (Profile Management)                     │
│  - Booking Controller (Seat Locking, Booking Creation)      │
│  - Trip Controller (Search, Details)                        │
│  - Bus Controller (Management, Toggle)                      │
│  - Operator Controller (Booking Views)                      │
│  - Admin Controller (Revenue, Approvals)                    │
│  - Payment Controller (Dummy Gateway)                       │
│  - Cancellation Controller (Refund Processing)              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ↓ Raw SQL + Connection Pooling
┌─────────────────────────────────────────────────────────────┐
│                  DATABASE LAYER                             │
│  PostgreSQL Database                                        │
│  - Entity Relationships with Foreign Keys                   │
│  - Transactional Integrity                                  │
│  - Automated Migrations                                     │
└─────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Tech Stack

### **Frontend**

- **Framework**: Angular 19+ (Standalone Components)
- **Language**: TypeScript
- **Styling**: Tailwind CSS 4
- **State Management**: Local/Session Storage
- **HTTP Client**: Angular HttpClient with Interceptors
- **Forms**: Reactive Forms with Validators

### **Backend**

- **Framework**: .NET Core 8.0
- **Language**: C#
- **Database**: PostgreSQL 14+
- **ORM**: Raw SQL with Npgsql
- **Authentication**: JWT Bearer Tokens
- **Email Service**: MailKit (SMTP)
- **Password Hashing**: BCrypt.Net

### **DevOps & Deployment**

- **Version Control**: Git
- **Database Migrations**: Entity Framework Migrations
- **CORS**: Enabled for frontend development

---

## 📁 Project Structure

```
Bus Booking System/
├── backend/
│   ├── controllers/
│   │   ├── AuthController.cs           # Login & Registration
│   │   ├── UserController.cs           # Profile Management
│   │   ├── BookingController.cs        # Booking & Seat Locking
│   │   ├── TripController.cs           # Trip Search & Details
│   │   ├── BusController.cs            # Bus Management
│   │   ├── OperatorController.cs       # Operator Bookings
│   │   ├── AdminController.cs          # Admin Functions
│   │   ├── PaymentController.cs        # Payment Processing
│   │   ├── CancellationController.cs   # Refund Management
│   │   ├── RouteController.cs          # Route Management
│   │   └── Other Controllers...
│   ├── Models/
│   │   ├── User.cs, Operator.cs, Bus.cs, Trip.cs, Booking.cs
│   │   ├── Traveler.cs, Seat.cs, SeatLayout.cs, SeatLock.cs
│   │   ├── Route.cs, Payment.cs, Cancellation.cs, Setting.cs
│   ├── DTOs/
│   │   ├── BookingRequest.cs, LockSeatRequest.cs
│   │   ├── TicketResponse.cs, TripSearchResponse.cs
│   │   └── Other DTOs...
│   ├── Data/
│   │   ├── AppDbContext.cs            # EF Core Context
│   │   └── PostgresSqlRunner.cs       # SQL Execution Helper
│   ├── Services/
│   │   ├── IEmailService.cs           # Email Interface
│   │   └── SmtpEmailService.cs        # SMTP Implementation
│   ├── Security/
│   │   ├── JwtSettings.cs             # JWT Configuration
│   │   └── TokenGenerator.cs          # Token Creation
│   ├── Migrations/
│   │   └── [Database Migrations]
│   ├── Program.cs                      # DI Configuration
│   ├── backend.csproj                  # Project File
│   └── appsettings.json                # Configuration
│
├── frontend/
│   ├── src/
│   │   ├── app/
│   │   │   ├── pages/
│   │   │   │   ├── home/              # Trip Search
│   │   │   │   ├── auth/              # Login/Register
│   │   │   │   ├── booking/           # Seat Selection & Payment
│   │   │   │   ├── history/           # Booking History
│   │   │   │   ├── profile/           # User Profile
│   │   │   │   ├── operator/          # Operator Dashboard
│   │   │   │   └── admin/             # Admin Dashboard
│   │   │   ├── core/
│   │   │   │   ├── services/
│   │   │   │   │   ├── api.service.ts # API Calls
│   │   │   │   │   ├── auth.service.ts# Auth Logic
│   │   │   │   │   └── Other Services
│   │   │   │   ├── guards/
│   │   │   │   │   ├── auth.guard.ts  # Auth Check
│   │   │   │   │   └── role.guard.ts  # Role Verification
│   │   │   │   ├── interceptors/      # HTTP Interceptors
│   │   │   │   └── models.ts          # TypeScript Interfaces
│   │   │   ├── app.routes.ts          # Route Configuration
│   │   │   ├── app.ts                 # Root Component
│   │   │   └── app.config.ts          # App Configuration
│   │   ├── main.ts                    # Entry Point
│   │   ├── index.html                 # HTML Shell
│   │   └── styles.css                 # Global Styles
│   ├── package.json                   # Dependencies
│   ├── angular.json                   # Angular Config
│   ├── tsconfig.json                  # TypeScript Config
│   └── public/                        # Static Assets
│
├── auth-debug/                         # Testing/Debugging
├── CODEBASE_ANALYSIS.md               # Architecture Analysis
├── Bus Booking System.sln             # Solution File
└── README.md                          # This File
```

---

## 🚀 Installation & Setup

### **Prerequisites**

- Node.js 18+ and npm
- .NET SDK 8.0+
- PostgreSQL 14+
- Visual Studio Code or Visual Studio 2022

### **Backend Setup**

1. **Navigate to backend directory**

   ```bash
   cd backend
   ```

2. **Update Database Connection**
   Edit `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=bus_ticket_booking;Username=postgres;Password=8098"
     },
     "Email": {
       "Host": "smtp.ethereal.email",
       "Port": 587,
       "User": "test@ethereal.email",
       "Pass": "testpass",
       "From": "noreply@busbooking.com"
     }
   }
   ```

3. **Apply Database Migrations**

   ```bash
   dotnet ef database update
   ```

4. **Run Backend**
   ```bash
   dotnet run
   ```
   Backend runs on `http://localhost:5299`

### **Frontend Setup**

1. **Navigate to frontend directory**

   ```bash
   cd frontend
   ```

2. **Install Dependencies**

   ```bash
   npm install
   ```

3. **Run Frontend**

   ```bash
   npm start
   ```

   Frontend runs on `http://localhost:4200`

4. **Build for Production**
   ```bash
   npm run build
   ```

---

## 📡 API Endpoints

### **Authentication**

| Method | Endpoint         | Auth | Description                |
| ------ | ---------------- | ---- | -------------------------- |
| POST   | `/auth/register` | ❌   | Register new user/operator |
| POST   | `/auth/login`    | ❌   | Login with credentials     |

### **User Management**

| Method | Endpoint        | Auth | Description         |
| ------ | --------------- | ---- | ------------------- |
| GET    | `/user/profile` | ✅   | Get user profile    |
| PUT    | `/user/profile` | ✅   | Update user profile |

### **Trip Search**

| Method | Endpoint         | Auth | Description                  |
| ------ | ---------------- | ---- | ---------------------------- |
| GET    | `/trip-search`   | ❌   | Search trips by route & date |
| GET    | `/trip/{tripId}` | ❌   | Get trip details             |

### **Booking**

| Method | Endpoint             | Auth | Description         |
| ------ | -------------------- | ---- | ------------------- |
| POST   | `/booking/lock-seat` | ✅   | Lock seats (5 min)  |
| POST   | `/booking/create`    | ✅   | Create booking      |
| GET    | `/history`           | ✅   | Get booking history |

### **Payment & Tickets**

| Method | Endpoint              | Auth | Description        |
| ------ | --------------------- | ---- | ------------------ |
| POST   | `/payment`            | ✅   | Process payment    |
| GET    | `/ticket/{bookingId}` | ✅   | Get ticket details |

### **Cancellation**

| Method | Endpoint              | Auth | Description                  |
| ------ | --------------------- | ---- | ---------------------------- |
| POST   | `/cancel/{bookingId}` | ✅   | Cancel booking (with refund) |

### **Bus Management (Operator)**

| Method | Endpoint              | Auth        | Description              |
| ------ | --------------------- | ----------- | ------------------------ |
| POST   | `/bus`                | ✅ Operator | Add bus                  |
| GET    | `/bus`                | ❌          | Get all buses            |
| GET    | `/bus/my-buses`       | ✅ Operator | Get operator's buses     |
| PUT    | `/bus/{busId}/toggle` | ✅ Operator | Toggle bus active status |

### **Trip Management (Operator)**

| Method | Endpoint                            | Auth        | Description         |
| ------ | ----------------------------------- | ----------- | ------------------- |
| POST   | `/trip`                             | ✅ Operator | Create trip         |
| GET    | `/operator/bookings`                | ✅ Operator | Get all bookings    |
| GET    | `/operator/trips/{tripId}/bookings` | ✅ Operator | Get trip passengers |

### **Admin Functions**

| Method | Endpoint                         | Auth     | Description            |
| ------ | -------------------------------- | -------- | ---------------------- |
| GET    | `/admin/operators/pending`       | ✅ Admin | List pending operators |
| POST   | `/admin/approve-operator/{opId}` | ✅ Admin | Approve operator       |
| POST   | `/admin/reject-operator/{opId}`  | ✅ Admin | Reject operator        |
| GET    | `/admin/revenue`                 | ✅ Admin | Get operator revenues  |
| GET    | `/admin/settings/platform-fee`   | ✅ Admin | Get platform fee %     |
| PUT    | `/admin/settings/platform-fee`   | ✅ Admin | Set platform fee %     |
| PUT    | `/admin/operator/{opId}/toggle`  | ✅ Admin | Toggle operator active |
| PUT    | `/admin/routes/{routeId}/toggle` | ✅ Admin | Toggle route active    |

### **Routes**

| Method | Endpoint  | Auth     | Description     |
| ------ | --------- | -------- | --------------- |
| GET    | `/routes` | ❌       | List all routes |
| POST   | `/routes` | ✅ Admin | Add route       |

### **Seat Layouts**

| Method | Endpoint             | Auth        | Description      |
| ------ | -------------------- | ----------- | ---------------- |
| POST   | `/layout`            | ✅ Op/Admin | Create layout    |
| GET    | `/layout/{layoutId}` | ❌          | Get layout seats |

---

## 🗄️ Database Schema

### Key Tables

#### **Users**

```sql
id (PK) | name | email | mobileNo | gender | dob | passwordHash | role | createdAt
```

#### **Operators**

```sql
id (PK) | userId (FK) | address | approved | isActive | createdAt
```

#### **Buses**

```sql
id (PK) | operatorId (FK) | busNumber | totalSeats | layoutId (FK) | isActive
```

#### **Trips**

```sql
id (PK) | busId (FK) | routeId (FK) | journeyDate | departureTime | arrivalTime
price | pickupAddress | dropAddress
```

#### **Bookings**

```sql
id (PK) | userId (FK) | tripId (FK) | totalAmount | platformFee | status
refundAmount | createdAt
```

#### **Travelers**

```sql
id (PK) | bookingId (FK) | name | age | gender | seatNumber
```

#### **SeatLocks**

```sql
id (PK) | tripId (FK) | seatNumber | lockedByUserId (FK) | expiryTime
```

#### **Cancellations**

```sql
id (PK) | bookingId (FK) | refundAmount | cancelledAt
```

---

## 🔐 Role-Based Access

| Feature                   | User                   | Operator | Admin |
| ------------------------- | ---------------------- | -------- | ----- |
| Search & Book Trips       | ✅                     | ❌       | ❌    |
| View Booking History      | ✅                     | ❌       | ❌    |
| Cancel Bookings           | ✅                     | ❌       | ❌    |
| Register as Operator      | ✅ (Requires Approval) | ❌       | ❌    |
| Add Buses                 | ❌                     | ✅       | ❌    |
| Create Trips              | ❌                     | ✅       | ❌    |
| View Bookings (Own Buses) | ❌                     | ✅       | ❌    |
| Approve Operators         | ❌                     | ❌       | ✅    |
| Add Routes                | ❌                     | ❌       | ✅    |
| Set Platform Fees         | ❌                     | ❌       | ✅    |
| View System Revenue       | ❌                     | ❌       | ✅    |

---

## 🔧 Key Implementations

### **Concurrent Booking Protection**

- **Mechanism**: SeatLocks table with 5-minute expiry
- **Flow**:
  1. User selects seats → Lock initiated
  2. System stores lock with expiry timestamp
  3. Lock automatically expires after 5 minutes
  4. Prevents simultaneous bookings on same seat

### **Gender-Based Seat Rules**

- **Single Lady Feature**: Female users can opt for female-only section
- **Adjacent Seat Constraint**: Males and females cannot book adjacent seats
- **Pink Highlighting**: Female-booked seats shown in pink
- **Enforcement**: Backend validates gender compatibility

### **Dynamic Refund Calculation**

```
if (hoursUntilJourney > 48) → 75% refund
else if (hoursUntilJourney > 24) → 50% refund
else if (hoursUntilJourney > 6) → 25% refund
else → 0% refund
```

### **Auto-Layout Generation**

- **Configurations**: 2+2, 2+3, 1+2
- **Bus Types**: Seater, Sleeper
- **Row/Column Mapping**: Automatic based on seat count
- **Aisle Spacing**: 1 column gap between sections

### **Platform Fee Calculation**

- **Admin Configuration**: Percentage-based (default 10%)
- **Application**: `Fee = TicketPrice × PassengerCount × FeePercentage / 100`
- **Included in Total**: Total = (TicketPrice × PassengerCount) + Fee

### **Email Notifications**

- **On Booking Confirmation**: Ticket details via email
- **On Cancellation**: Refund amount and cancellation confirmation
- **On Bus Removal**: Notification to all affected passengers
- **Service**: MailKit with SMTP Configuration

---

## ⚙️ Known Configurations

### **Email Configuration** (appsettings.json)

```json
"Email": {
  "Host": "smtp.ethereal.email",  // Change for production
  "Port": 587,
  "User": "test@ethereal.email",  // Change for production
  "Pass": "testpass",             // Change for production
  "From": "noreply@busbooking.com"
}
```

### **JWT Configuration** (JwtSettings.cs)

- **Secret Key**: Auto-generated per deployment
- **Expiry**: 2 hours
- **Claims**: UserId, Email, Role

### **Database Connection**

```
Host=localhost;Port=5432;Database=bus_ticket_booking;Username=postgres;Password=8098
```

### **Default Platform Fee**: 10%

---

## 📦 What to Push to GitHub

### **DO PUSH** ✅

```
├── backend/
│   ├── controllers/
│   ├── Models/
│   ├── DTOs/
│   ├── Data/
│   ├── Services/
│   ├── Security/
│   ├── Migrations/
│   ├── Program.cs
│   ├── backend.csproj
│   ├── .gitignore
│   └── appsettings.json (sanitized - remove credentials)
├── frontend/
│   ├── src/
│   ├── public/
│   ├── package.json
│   ├── angular.json
│   ├── tsconfig.json
│   ├── .gitignore
│   └── README.md
├── CODEBASE_ANALYSIS.md
├── README.md
└── .gitignore
```

### **DO NOT PUSH** ❌

```
├── backend/
│   ├── bin/              # Compiled binaries
│   ├── obj/              # Build artifacts
│   └── *.log            # Log files
├── frontend/
│   ├── node_modules/    # Dependencies
│   ├── dist/            # Build output
│   └── .angular/        # Build cache
├── .dotnet/             # SDK cache
├── auth-debug/          # Debug folder
├── backend-build-out/   # Build output
├── *-stderr.log         # Log files
├── *-stdout.log         # Log files
├── .env                 # Local credentials
└── *.pdb               # Debug symbols
```

### **.gitignore Template**

```
# Backend
backend/bin/
backend/obj/
backend/*.log
backend/appsettings.*.json

# Frontend
frontend/node_modules/
frontend/dist/
frontend/.angular/

# IDE
.vscode/
.idea/
*.user
*.suo

# OS
.DS_Store
Thumbs.db

# Environment
.env
.env.local

# Logs
*.log
*.pdb

# Build artifacts
backend-build-out/
backend-stderr.log
backend-stdout.log
```

---

## 🚀 Deployment Checklist

### **Before Production**

- [ ] Update `appsettings.json` with production email credentials
- [ ] Update database connection string
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Set up production JWT secret key
- [ ] Configure CORS for frontend domain only
- [ ] Test all email notifications
- [ ] Enable database backups
- [ ] Set up monitoring and logging
- [ ] Run security audit
- [ ] Load testing with expected user volume

### **Deployment Steps**

1. Clone repository
2. Update configuration files
3. Run database migrations
4. Build backend: `dotnet publish -c Release`
5. Build frontend: `npm run build`
6. Deploy to hosting platform (Azure, AWS, DigitalOcean)
7. Configure DNS and SSL
8. Monitor logs and performance

---

## 🔮 Future Enhancements

- [ ] **Real Payment Gateway** (Razorpay/PayPal integration)
- [ ] **Multi-language Support** (i18n)
- [ ] **Mobile App** (React Native/Flutter)
- [ ] **Rating & Reviews** (5-star system)
- [ ] **Loyalty Program** (Discount coupons)
- [ ] **Dynamic Pricing** (Surge pricing, discounts)
- [ ] **Real-time Tracking** (WebSocket bus tracking)
- [ ] **Multiple Seat Classes** (Premium, Standard, Economy)
- [ ] **Insurance Options** (Cancellation, Travel insurance)
- [ ] **Groups Booking** (Bulk discounts)
- [ ] **Return Journeys** (Round-trip bookings)
- [ ] **Advanced Analytics** (Dashboard, Reports)
- [ ] **Return Journeys** (Round-trip bookings)
- [ ] **Automated Testing** (Unit, Integration, E2E)
- [ ] **API Documentation** (Swagger/OpenAPI)

---

## 📞 Support & Documentation

- **Backend API Docs**: See [CODEBASE_ANALYSIS.md](./CODEBASE_ANALYSIS.md)
- **Code Comments**: Extensive inline documentation
- **Error Codes**: HTTP status codes per endpoint
- **Logs**: Check application logs for debugging

---

## 📄 License

This project is proprietary and confidential. Unauthorized copying or distribution is strictly prohibited.

---

## 👥 Team & Contribution

Developed as part of Presidio Training - Genspark Program.

**Last Updated**: April 25, 2026
**Status**: ✅ Production Ready

---

**Happy Coding! 🚀**
