# Library Management System

A .NET console application for managing a community library. The project uses a layered architecture with separate model, data access, business logic, and frontend console projects. Data is stored in PostgreSQL using Entity Framework Core migrations.

## Project Structure

- `FrontendApplication` - console menus, login flow, and user input handling
- `BusinessLib` - business rules and services for books, users, borrowing, fines, and reports
- `DataAccessLib` - EF Core `DbContext`, repositories, and migrations
- `ModelLib` - entity models and enums

## Main Features

### Admin

- Admin login
- Add, view, search, update, deactivate, and delete users
- Assign and update membership type
- Add new books
- Add multiple book copies
- View all books, available books, damaged books, and damaged-but-available books
- Search books by title, author, or category
- Update book copy condition and damage details
- Delete books
- Generate reports:
  - Overdue books
  - Fine collection
  - Currently borrowed books
  - Book condition count
  - Members with pending fines
  - Most borrowed books
  - Available books by category

### User

- User login
- View all books and available books
- Search books by title, author, or category
- Borrow books according to membership rules
- Return borrowed books
- View borrowed books, overdue books, and borrowing history
- View pending fines
- Pay fines
- View fine history
- View personal profile details

## Business Rules

- Membership limits are stored in `MembershipTypes`.
- Borrowing is restricted by active borrowing count and pending fine limit.
- A user cannot borrow the same book again while it is actively borrowed.
- Book copies are marked as borrowed when issued and available/damaged on return.
- Late returns and damaged returns create fine transactions.
- Fine-related values such as late fine, missing page fine, hard cover fine, and maximum pending fine are stored in `Rules`.

## Technologies Used

- C#
- .NET 10
- Entity Framework Core
- PostgreSQL
- Npgsql EF Core provider

## Database Tables

- `Books`
- `BookCopies`
- `Stocks`
- `Users`
- `UserStats`
- `MembershipTypes`
- `BorrowTransactions`
- `BookReturnTransactions`
- `FineTransactions`
- `Rules`

## How to Run

1. Create a PostgreSQL database named `librarymanagementsystem`.
2. Update the PostgreSQL connection string in `DataAccessLib/Contexts/LibraryContext.cs` if needed.
3. Apply EF Core migrations.
4. Run the console app from `FrontendApplication`.

```bash
dotnet run --project FrontendApplication
```

