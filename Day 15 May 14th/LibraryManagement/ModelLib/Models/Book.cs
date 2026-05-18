namespace librarymanagementsystem.ModelLib
{
    public class Book
    {
        public  int BookId { get; set; }
        public  string Title { get; set; }
        public  string Author { get; set; }
        public int PublicationYear { get; set; }
        public  string Category { get; set; }

        public ICollection<BookCopies> BookCopies { get; set; } = new List<BookCopies>();
        public Stock? Stock { get; set; }

        public Book() { }
        public Book(string title, string author, string category, string year)
        {
            Title = title;
            Author = author;
            Category = category;
            PublicationYear = Convert.ToInt32(year);
        }


        public override string ToString()
        {
            return $"BookId: {BookId}, Title: {Title}, Author: {Author}, Category: {Category}, Year: {PublicationYear}";
        }

    }
}