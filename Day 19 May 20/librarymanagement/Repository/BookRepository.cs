using librarymanagement.ModelLib;
using Microsoft.EntityFrameworkCore;
using librarymanagement.DataAccessLib;
using librarymanagement.DataAccessLib.Contexts;

namespace librarymanagement.DataAccessLib
{
    public class BookRepository: IBookRepository
    {
         private readonly LibraryDbContext db;

        public BookRepository(LibraryDbContext context)
        {
            db = context;
        }
         public bool AddNewBookInDB(Book book)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Books.Add(book);
                db.SaveChanges();
                transaction.Commit();
                if(book.BookId > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Failed to add book to the database.");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }

        public List<Book> GetAllBooksFromDB()
        {
            try
            {
                var books = db.Books.ToList();
                if(books == null || books.Count == 0)
                {
                    Console.WriteLine("No books found in the database.");
                    return new List<Book>();
                }else if(books.Count > 0)
                {
                    return books;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving books.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving books: {ex.Message}");
                return new List<Book>();
            }
            
        }

        public List<Book> FindBooksFromDB(string searchText, int searchType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                    return new List<Book>();

                searchText = searchText.Trim();

                IQueryable<Book> query = db.Books;

                switch (searchType)
                {
                    case 1: 
                        query = query.Where(b =>
                            EF.Functions.ILike(b.Title, $"%{searchText}%"));
                        break;

                    case 2: 
                        query = query.Where(b =>
                            EF.Functions.ILike(b.Author, $"%{searchText}%"));
                        break;

                    case 3: 
                        query = query.Where(b =>
                            EF.Functions.ILike(b.PublicationYear.ToString(), $"%{searchText}%"));
                        break;

                    default:
                        Console.WriteLine("Invalid search type.");
                        return new List<Book>();
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding books: {ex.Message}");
                return new List<Book>();
            }
        }

        public bool DeleteBookFromDB(int bookId)
        {
            try
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                if(book == null)
                {
                    Console.WriteLine("Book not found in the database.");
                    return false;
                }
                else
                {
                    db.Books.Remove(book);
                    db.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }
        public Book FindBookById(int bookId)
        {
            try
            {
                var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
                return book;
            }catch(Exception ex)
            {
                Console.WriteLine($"Error finding book by ID: {ex.Message}");
                return null;
            }
        }
    }
}