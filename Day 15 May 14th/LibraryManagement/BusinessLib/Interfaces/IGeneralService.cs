using librarymanagementsystem.DataAccessLib;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public interface IGeneralService
    {
        void LibraryRules();
        User UserLogin(string email, string password);
        bool AdminLogin(string email, string password);
    }
}