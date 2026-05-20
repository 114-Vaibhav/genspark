using librarymanagement.ModelLib;
using librarymanagement.DataAccessLib;
using librarymanagement.DataAccessLib.Contexts;
namespace librarymanagement.BusinessLib
{
    public class BookService : IBookService
    {
        private readonly BookRepository bookRepository;

        public BookService(LibraryDbContext context)
        {
            bookRepository = new BookRepository(context);
        }
        public void AddNewBook(Book book)
        {
            try
            {
                if (validateBook(book))
                {
                    if(bookRepository.AddNewBookInDB(book))
                    {
                        Console.WriteLine("Book added successfully.");
                    }
                    else
                    {
                        throw new Exception("Failed to add new book. ");
                    }
                }
                else
                {
                    return;
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while adding new book: {ex.Message}");
            }
            
        }
        private bool validateBook(Book book)
        {
            bool valid=true;
            if (book.Title.Length < 3)
            {
                Console.WriteLine("Title should be at least 3 characters long.");
                // return false;
                valid=false;
            }
            if (book.Author.Length < 3)
            {
                Console.WriteLine("Author name should be at least 3 characters long.");
                valid=false;
                // return false;
            }
            if (book.PublicationYear < 1800 || book.PublicationYear > DateTime.UtcNow.Year)
            {
                Console.WriteLine("Invalid publication year.");
                valid=false;
                // return false;
            }
            return valid;
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

        public void FindBook(string searchText, int searchType)
        {
            try
            {
                var books = bookRepository.FindBooksFromDB(searchText, searchType);
                if(books.Count == null)
                {
                    Console.WriteLine("No book found matching the search criteria.");
                    
                }else {
                    Console.WriteLine("-----------------Found Books in the library--------------------");
                    foreach(var book in books)
                    {
                        Console.WriteLine(book);
                    }
                    Console.WriteLine("------------------------End---------------------------------");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in BookService while finding book: {ex.Message}");
                
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