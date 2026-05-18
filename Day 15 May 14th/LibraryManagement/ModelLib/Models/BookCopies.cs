namespace librarymanagementsystem.ModelLib
{
    public enum BookCopyStatus
    {
        Available,
        Borrowed,
        Reserved,
        Lost,
        Damaged,
        DamagedAvailable
    }

    public class BookCopies
    {
        public int BookCopiesId { get; set; }

        public int BookId { get; set; }

        public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;

        public int DamagePercentage { get; set; } = 0;

        public string? DamageDescription { get; set; }

        // Navigation
        public Book Book { get; set; }


        public override string ToString()
        {   
            
            return $"BookCopyId: {BookCopiesId}, BookId: {BookId},  BookCopyStatus: {Status}, DamagePercentage: {DamagePercentage}, DamageDescription: {DamageDescription}";
        }

    }
}