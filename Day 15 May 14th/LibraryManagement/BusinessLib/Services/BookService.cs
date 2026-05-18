using librarymanagementsystem.ModelLib;
using librarymanagementsystem.DataAccessLib;
namespace librarymanagementsystem.BusinessLib
{
    public class BookService : IBookService
    {
        private IBookRepository bookRepository;
        public BookService()
        {
            bookRepository = new BookRepository();
        }
        public void AddNewBook(Book book)
        {
            try
            {
                if(bookRepository.AddNewBookInDB(book))
                {
                    Console.WriteLine("Book added successfully.");
                }
                else
                {
                    throw new Exception("Failed to add new book. ");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while adding new book: {ex.Message}");
            }
            
        }

        public void ViewAllBooks()
        {
            try
            {
                var books = bookRepository.GetAllBooksFromDB();
                if(books == null || books.Count == 0){
                    Console.WriteLine("No books found in the database.");
                    
                }else if(books.Count > 0){
                    Console.WriteLine("-----------------Books in the library--------------------");
                    foreach(var book in books)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("------------------------End---------------------------------");
                }else {
                    throw new Exception("Unexpected error occurred while retrieving books.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while retrieving all books: {ex.Message}");
                
            }
        }

        public void ViewAllAvailableBooks()
        {
            try
            {
                var books = bookRepository.GetAllAvailableBooksFromDB();
                if(books == null || books.Count == 0){
                    Console.WriteLine("No available books found in the database.");
                    
                }else if(books.Count > 0){
                    Console.WriteLine("-----------------Available Books in the library--------------------");
                    foreach(var book in books)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("------------------------End---------------------------------");
                }else {
                    throw new Exception("Unexpected error occurred while retrieving available books.");
                }
                
            }catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while retrieving available books: {ex.Message}");
                
            }
        }

        public void ViewAllDamagedBooks()
        {
            try
            {
                var books = bookRepository.GetAllDamagedBooksFromDB();
                if(books == null || books.Count == 0){
                    Console.WriteLine("No damaged books found in the database.");
                   
                }else if(books.Count > 0){
                    Console.WriteLine("-----------------Damaged Books in the library--------------------");
                    foreach(var book in books)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("------------------------End---------------------------------");
                }else {
                    throw new Exception("Unexpected error occurred while retrieving damaged books.");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while retrieving damaged books: {ex.Message}");
                
            }
        }   
        public void ViewAllDamagedAvailableBooks()
        {
            try
            {
                var books = bookRepository.GetAllDamagedAvailableBooksFromDB();
                if(books == null || books.Count == 0){
                    Console.WriteLine("No damaged available books found in the database.");
                }else if(books.Count > 0){
                    Console.WriteLine("-----------------Damaged Available Books in the library--------------------");
                    foreach(var book in books)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("------------------------End---------------------------------");
                }else {
                    throw new Exception("Unexpected error occurred while retrieving damaged available books.");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while retrieving damaged available books: {ex.Message}");
                
            }
        }

        public void FindBook(string searchText, int searchType)
        {
            try
            {
                var book = bookRepository.FindBookFromDB(searchText, searchType);
                if(book == null)
                {
                    Console.WriteLine("No book found matching the search criteria.");
                    
                }else {
                    Console.WriteLine(book);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while finding book: {ex.Message}");
                
            }
        }

        public void AddCopiesOfBook(int bookId, int numberOfCopies)
        {
            for(int i = 0; i < numberOfCopies; i++)
            {
                if(!bookRepository.AddCopyOfBookInDB(bookId))
                {
                    Console.WriteLine("Error occurred while adding book copy.");
                    return;
                }
            }
            Console.WriteLine($"{numberOfCopies} copies of the book have been added successfully.");
        }

        public void UpdateBookCondition(int bookCopyId)
        {
                Console.WriteLine("Enter damage percentage (0-100): ");
                int damagePercentage;
                while(!int.TryParse(Console.ReadLine(), out damagePercentage) || damagePercentage < 0 || damagePercentage > 100)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 0 and 100 for damage percentage.");
                }
                Console.WriteLine("Enter damage description: ");
                string damageDescription = Console.ReadLine();
                try
                {
                    if(bookRepository.UpdateBookConditionInDB(bookCopyId, damagePercentage, damageDescription))
                    {
                        Console.WriteLine("Book condition updated successfully.");
                        
                    }else {
                        throw new Exception("Failed to update book condition.");
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine($"Error in BookService while updating book condition: {ex.Message}");
                    
                }
        }

        public void DeleteBook(int bookId)
        {
            try
            {
                if(bookRepository.DeleteBookFromDB(bookId))
                {
                    Console.WriteLine("Book deleted successfully.");
                }
                else
                {
                    throw new Exception("Failed to delete book.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while deleting book: {ex.Message}");
            }
        }
    }
}