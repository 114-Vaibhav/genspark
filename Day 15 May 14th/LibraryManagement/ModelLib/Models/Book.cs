namespace librarymanagementsystem.ModelLib
{
    public class Book
    {
        public required int BookId { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public int PublicationYear { get; set; }
        public required string Category { get; set; }

        public ICollection<BookCopies> BookCopies { get; set; } = new List<BookCopies>();
        public Stock? Stock { get; set; }
    }
}