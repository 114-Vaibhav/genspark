namespace librarymanagementsystem.BusinessLib
{
    public interface IFineService
    {
        public void ViewPendingFineAmount(int userId);

        void PayFine(int userId);

        bool HasPendingFineAboveLimit(int userId);
        void ViewFineHistory(int userId);
    }
}