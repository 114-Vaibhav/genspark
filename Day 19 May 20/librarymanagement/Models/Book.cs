namespace librarymanagement.ModelLib
{
    public class Book
    {
        public  int BookId { get; set; } = 0;
        public  string Title { get; set; } = string.Empty;
        public  string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; } = 0;
        public int AvailableCopies { get; set; } = 0;
    }
}