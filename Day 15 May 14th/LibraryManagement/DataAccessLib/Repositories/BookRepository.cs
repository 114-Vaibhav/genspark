using librarymanagementsystem.ModelLib;
using Microsoft.EntityFrameworkCore;
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
            var transaction = db.Database.BeginTransaction();
            try
            {
                db.Books.Add(book);
                db.Stocks.Add(new Stock { BookId = book.BookId });
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
                    case 1: // Title
                        query = query.Where(b =>
                            EF.Functions.ILike(b.Title, $"%{searchText}%"));
                        break;

                    case 2: // Author
                        query = query.Where(b =>
                            EF.Functions.ILike(b.Author, $"%{searchText}%"));
                        break;

                    case 3: // Category
                        query = query.Where(b =>
                            EF.Functions.ILike(b.Category, $"%{searchText}%"));
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
        public bool AddCopyOfBookInDB(int bookId)
        {
            var transaction = db.Database.BeginTransaction();
            try
            {
               db.BookCopies.Add(new BookCopies { BookId = bookId });
               db.Stocks.Where(s => s.BookId == bookId).FirstOrDefault().AvailableCopies += 1;
               db.SaveChanges();
               transaction.Commit();
               return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding copy of book: {ex.Message}");
                transaction.Rollback();
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
        public List<BookCopies> GetAllCopiesOfBookFromDB(int bookId)
        {
            try
            {
                var bookCopies = db.BookCopies.Where(bc => bc.BookId == bookId).ToList();
                if(bookCopies == null || bookCopies.Count == 0)
                {
                    Console.WriteLine("No book copies found in the database.");
                    return new List<BookCopies>();
                }else if(bookCopies.Count > 0)
                {
                    return bookCopies;
                }else {
                    throw new Exception("Unexpected error occurred while retrieving book copies.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error retrieving book copies: {ex.Message}");
                return new List<BookCopies>();
            }
        }
    }
}