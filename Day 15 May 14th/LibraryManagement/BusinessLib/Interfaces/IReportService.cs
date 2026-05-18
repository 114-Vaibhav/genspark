using librarymanagementsystem.ModelLib;
namespace librarymanagementsystem.BusinessLib
{
    public interface IReportsService
    {
        public void generateOverdueBooksReport();
        public void generateFineCollectionReport();
        
        public void generateCurrentlyBorrowingReport();
        public void generateBookConditionReport();
        public void generateMemberWithPendingFinesReport();
        public void generateMostBorrowedBooksReport();
        public void generateAvailableBooksByCategoryReport();
        
    }
}