namespace librarymanagementsystem.ModelLib
{
    public class Stock
    {
        public int StockId { get; set; }
        public int BookId { get; set; }

        public int TotalCopies { get; set; } = 0;

        public int AvailableCopies { get; set; } = 0;

        public int BorrowedCopies { get; set; } = 0;

        public int DamagedCopies { get; set; } = 0;

        // Navigation
        public Book Book { get; set; }

        override public string ToString()
        {
            return $"StockId: {StockId}, BookId: {BookId}, TotalCopies: {TotalCopies}, AvailableCopies: {AvailableCopies}, BorrowedCopies: {BorrowedCopies}, DamagedCopies: {DamagedCopies}";
        }
    }
}