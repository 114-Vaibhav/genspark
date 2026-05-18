using librarymanagementsystem.ModelLib;
// using System;.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
using librarymanagementsystem.DataAccessLib.Contexts;
namespace librarymanagementsystem.DataAccessLib
{
    public class BookRepository: IBookRepository
    {
        private  LibraryContext db;
        public BookRepository()
        {
            db = new LibraryContext();
        }
         public bool AddNewBookInDB(Book book)
        {
            try
            {
                db.Books.Add(book);
                db.SaveChanges();
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

        public List<Book> GetAllAvailableBooksFromDB()
        {
            try
            {
                var availableBooks = db.Books.Where(b => b.Stock.AvailableCopies>0).ToList();
                if(availableBooks == null || availableBooks.Count == 0)
                {
                    Console.WriteLine("No available books found in the database.");
                    return new List<Book>();
                }else if(availableBooks.Count > 0)
                {
                    return availableBooks;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving available books.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving available books: {ex.Message}");
                return new List<Book>();
            }
        }

        public List<Book> GetAllDamagedBooksFromDB()
        {
            try
            {
                var damagedBooks = db.BookCopies.Where(bc => bc.Status == BookCopyStatus.Damaged).Select(bc => bc.Book).ToList();
                if(damagedBooks == null || damagedBooks.Count == 0)
                {
                    Console.WriteLine("No damaged books found in the database.");
                    return new List<Book>();
                }else if(damagedBooks.Count > 0)
                {
                    return damagedBooks;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving damaged books.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving damaged books: {ex.Message}");
                return new List<Book>();
            }
        }

        public List<Book> GetAllDamagedAvailableBooksFromDB()
        {
            try
            {
                var damagedAvailableBooks = db.BookCopies.Where(bc => bc.Status == BookCopyStatus.DamagedAvailable).Select(bc => bc.Book).ToList();
                if(damagedAvailableBooks == null || damagedAvailableBooks.Count == 0)
                {
                    Console.WriteLine("No damaged available books found in the database.");
                    return new List<Book>();
                }else if(damagedAvailableBooks.Count > 0)
                {
                    return damagedAvailableBooks;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving damaged available books.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving damaged available books: {ex.Message}");
                return new List<Book>();
            }
        }

        public Book? FindBookFromDB(string searchText, int searchType)
        {
            try
            {
                Book? book = null;
                switch(searchType)
                {
                    case 1:
                        book = db.Books.FirstOrDefault(b => b.Title.ToLower() == searchText.ToLower());
                        break;
                    case 2:
                        book = db.Books.FirstOrDefault(b => b.Author.ToLower() == searchText.ToLower());
                        break;
                    case 3:
                        book = db.Books.FirstOrDefault(b => b.Category.ToLower() == searchText.ToLower());
                        break;
                    default:
                        Console.WriteLine("Invalid search type. Please use 1 for Title, 2 for Author, or 3 for Category.");
                        return null;
                }
                if(book == null)
                {
                    Console.WriteLine("No book found matching the search criteria.");
                    return null;
                }else {
                    return book;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error finding book: {ex.Message}");
                return null;
            }
        }

        public bool AddCopyOfBookInDB(int bookId)
        {
            try
            {
               db.BookCopies.Add(new BookCopies { BookId = bookId });
               db.SaveChanges();
               return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding copy of book: {ex.Message}");
                return false;
            }

        }

        public bool UpdateBookConditionInDB(int bookCopyId, int damagePercentage, string damageDescription)
        {
            try
            {
                var bookCopy = db.BookCopies.FirstOrDefault(bc => bc.BookCopiesId == bookCopyId);
                if(bookCopy == null)
                {
                    Console.WriteLine("Book copy not found in the database.");
                    return false;
                }
                else
                {
                    bookCopy.DamagePercentage = damagePercentage;
                    bookCopy.DamageDescription = damageDescription;
                   
                    if(bookCopy.DamagePercentage >= 50)
                    {
                        bookCopy.Status = BookCopyStatus.Damaged;
                    }
                    else if(bookCopy.DamagePercentage > 0 && bookCopy.DamagePercentage < 50)
                    {
                        bookCopy.Status = BookCopyStatus.DamagedAvailable;
                    }
                    else
                    {
                        bookCopy.Status = BookCopyStatus.Available;
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error updating book condition: {ex.Message}");
                return false;
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
    }
}