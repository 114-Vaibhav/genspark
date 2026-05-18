using librarymanagementsystem.BusinessLib;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.FronendApplication
{
    internal class Library
    {
        private readonly IBookService bookService = new BookService();
        private readonly IUserManagementService userService = new UserManagementService();
        private readonly IBorrowService borrowService = new BorrowService();
        private readonly IFineService fineService = new FineService();
        private readonly IReportsService reportService = new ReportsService();
        private InputHandler inputHandler = new InputHandler();
        private IGeneralService generalService = new GeneralService();

        public void AdminMenu()
        {
            
            Console.WriteLine("Enter Admin Email: ");
            string email = Console.ReadLine().ToLower();
            Console.WriteLine("Enter Admin Password: ");
            string password = inputHandler.ReadPassword();
            
            if(!generalService.AdminLogin(email, password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid credentials.");
                Console.ResetColor();
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login successful.");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
            int choice;

            do
            {
                Console.WriteLine("============== ADMIN MENU ==============");
                Console.WriteLine("1. User Management");
                Console.WriteLine("2. Book Management");
                Console.WriteLine("3. Report Management");
                Console.WriteLine("4. Logout");

                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                
                
                switch (choice)
                {
                    case 1:
                        UserManagement();
                        break;

                    case 2:
                        BookManagement(true, 0);
                        break;

                    case 3:
                        ReportManagement();
                        break;

                    case 4:
                        Console.WriteLine("Logging out...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                // Console.ReadKey();

            } while (choice != 4);
            Console.ResetColor();
        }
        public void UserMenu()
        {
            Console.WriteLine("Enter User Email: ");
            string email = Console.ReadLine().ToLower();
            Console.WriteLine("Enter User Password: ");
            string password = inputHandler.ReadPassword();
            User user =generalService.UserLogin(email, password);
            if(user==null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid credentials.");
                Console.ResetColor();
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Login successful.");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            int userId = user.UserId;
            int choice;

            do
            {
                Console.WriteLine("============== USER MENU ==============");
                Console.WriteLine("1. Book Management");
                Console.WriteLine("2. Borrow Management");
                Console.WriteLine("3. Fine Management");
                Console.WriteLine("4. My Profile");
                Console.WriteLine("5. Logout");

                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        BookManagement(false, userId);
                        break;

                    case 2:
                        BorrowManagement(userId);
                        break;

                    case 3:
                        FineManagement(userId);
                        break;

                    case 4:
                        userService.ViewPersonalDetails(userId);
                        break;

                    case 5:
                        Console.WriteLine("Logging out...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
                Console.ResetColor();
                

            } while (choice != 5);
        }
        public void UserManagement()
        {
            int choice;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                Console.WriteLine("========= USER MANAGEMENT =========");
                Console.WriteLine("1. Add User");
                Console.WriteLine("2. View All Users");
                Console.WriteLine("3. Find User By Id");
                Console.WriteLine("4. Find User By Contact(Email/Phone Number)");
                Console.WriteLine("5. Update Membership");
                Console.WriteLine("6. Deactivate User");
                Console.WriteLine("7. Delete User");
                Console.WriteLine("8. Back");

                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        inputHandler.userCreation();
                        break;

                    case 2:
                        userService.ViewAllUsers();
                        break;

                    case 3:
                        Console.Write("Enter User Id: ");
                        userService.FindUserById(
                            Convert.ToInt32(Console.ReadLine()));
                        break;

                    case 4:
                        Console.Write("Enter your choice 1.Email  2.Phone : ");
                        int type = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Enter Contact: ");
                        string contact = Console.ReadLine();


                        userService.FindUserByContact(contact, type);
                        break;

                    case 5:
                        Console.Write("Enter User Id: ");
                        userService.UpdateMembershipStatusByAdmin(
                            Convert.ToInt32(Console.ReadLine()));
                        break;

                    case 6:
                        Console.Write("Enter User Id: ");
                        userService.DeactivateUser(
                            Convert.ToInt32(Console.ReadLine()));
                        break;

                    case 7:
                        Console.Write("Enter User Id: ");
                        userService.DeleteUser(
                            Convert.ToInt32(Console.ReadLine()));
                        break;
                }
                Console.ResetColor();
                

            } while (choice != 8);
        }
        public void BookManagement(bool isAdmin, int userId)
        {
            int choice;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                Console.WriteLine("========= BOOK MANAGEMENT =========");

                if (isAdmin)
                {
                    Console.WriteLine("1. Add Book");
                    Console.WriteLine("2. View All Books");
                    Console.WriteLine("3. View Available Books");
                    Console.WriteLine("4. View Damaged Books");
                    Console.WriteLine("5. View Damaged Available Books");
                    Console.WriteLine("6. Find Book");
                    Console.WriteLine("7. Add Copies");
                    Console.WriteLine("8. Update Book Condition");
                    Console.WriteLine("9. Delete Book");
                    Console.WriteLine("10. Back");
                }
                else
                {
                    Console.WriteLine("1. View All Books");
                    Console.WriteLine("2. View Available Books");
                    Console.WriteLine("3. View Damaged Available Books");
                    Console.WriteLine("4. Find Book");
                    Console.WriteLine("5. Back");
                }
                
                Console.WriteLine("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                if (isAdmin)
                {
                    switch (choice)
                    {
                        case 1:
                            inputHandler.bookCreation();
                            break;
                        case 2:
                            bookService.ViewAllBooks();
                            break;

                        case 3:
                            bookService.ViewAllAvailableBooks();
                            break;

                        case 4:
                            bookService.ViewAllDamagedBooks();
                            break;

                        case 5:
                            bookService.ViewAllDamagedAvailableBooks();
                            break;

                        case 6:
                            Console.WriteLine("Enter your choice 1.Title  2.Author  3.Category  4.Year : ");
                            int searchType = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Enter Search Text: ");
                            string searchText = Console.ReadLine();
                            bookService.FindBook(searchText,searchType);
                            break;

                        case 7:
                            Console.WriteLine("Enter Book Id: ");
                            int bookId=Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter Number of Copies: ");
                            int noOfCopies=Convert.ToInt32(Console.ReadLine());
                            bookService.AddCopiesOfBook(bookId,noOfCopies);
                            break;

                        case 8:
                            Console.WriteLine("Enter Book Id: ");
                            bookId=Convert.ToInt32(Console.ReadLine());
                            bookService.UpdateBookCondition(bookId);
                            break;

                        case 9:
                            Console.WriteLine("Enter Book Id: ");
                            bookId=Convert.ToInt32(Console.ReadLine());
                            bookService.DeleteBook(bookId);
                            break;
                        case 10:
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case 1:
                            bookService.ViewAllBooks();
                            break;

                        case 2:
                            bookService.ViewAllAvailableBooks();
                            break;

                        case 3:
                            bookService.ViewAllDamagedAvailableBooks();
                            break;
                        case 4:
                            Console.WriteLine("Enter your choice 1.Title  2.Author  3.Category  4.Year : ");
                            int searchType = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Enter Search Text: ");
                            string searchText = Console.ReadLine();
                            bookService.FindBook(searchText,searchType);
                            break;
                        case 5:
                            break;
                    }
                }

                Console.ResetColor();

            } while ((isAdmin && choice != 10)
                    || (!isAdmin && choice != 5));
        }
        public void BorrowManagement(int userId)
        {
            int choice;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                Console.WriteLine("========= BORROW MANAGEMENT =========");
                Console.WriteLine("1. Borrow Book");
                Console.WriteLine("2. Return Book");
                Console.WriteLine("3. View Borrowed Books");
                Console.WriteLine("4. View Overdue Books");
                Console.WriteLine("5. View Borrowing History");
                Console.WriteLine("6. Back");

                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Book Id: ");
                        borrowService.BorrowBook(
                            userId,
                            Convert.ToInt32(Console.ReadLine()));
                        break;

                    case 2:
                        Console.Write("Enter Book Id: ");
                        borrowService.ReturnBook(
                            userId,
                            Convert.ToInt32(Console.ReadLine()));
                        break;

                    case 3:
                        borrowService.ViewBorrowedBooks(userId);
                        break;

                    case 4:
                        borrowService.ViewOverdueBooks(userId);
                        break;

                    case 5:
                        borrowService.ViewBorrowingHistory(userId);
                        break;
                }

                Console.ResetColor();

            } while (choice != 6);
        }

        public void FineManagement(int userId)
        {
            int choice;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                Console.WriteLine("========= FINE MANAGEMENT =========");
                Console.WriteLine("1. View Pending Fine");
                Console.WriteLine("2. Pay Fine");
                Console.WriteLine("3. View Fine History");
                Console.WriteLine("4. Back");
                
                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        fineService.ViewPendingFineAmount(userId);
                        break;

                    case 2:
                        fineService.PayFine(userId);
                        break;

                    case 3:
                        fineService.ViewFineHistory(userId);
                        break;
                }

                Console.ResetColor();

            } while (choice != 4);
        }
        public void ReportManagement()
        {
            int choice;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.BackgroundColor = ConsoleColor.Black;
            do
            {
                Console.WriteLine("========= REPORT MANAGEMENT =========");
                Console.WriteLine("1. Overdue Books");
                Console.WriteLine("2. Fine Collection");
                Console.WriteLine("3. Currently Borrowing");
                Console.WriteLine("4. Book Condition");
                Console.WriteLine("5. Pending Fine Members");
                Console.WriteLine("6. Most Borrowed Books");
                Console.WriteLine("7. Available Books By Category");
                Console.WriteLine("8. Back");
                
                Console.Write("Enter choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }

                switch (choice)
                {
                    case 1:
                        reportService.generateOverdueBooksReport();
                        break;

                    case 2:
                        reportService.generateFineCollectionReport();
                        break;

                    case 3:
                        reportService.generateCurrentlyBorrowingReport();
                        break;

                    case 4:
                        reportService.generateBookConditionReport();
                        break;

                    case 5:
                        reportService.generateMemberWithPendingFinesReport();
                        break;

                    case 6:
                        reportService.generateMostBorrowedBooksReport();
                        break;

                    case 7:
                        reportService.generateAvailableBooksByCategoryReport();
                        break;
                }

                Console.ResetColor();

            } while (choice != 8);
        }
    }
}