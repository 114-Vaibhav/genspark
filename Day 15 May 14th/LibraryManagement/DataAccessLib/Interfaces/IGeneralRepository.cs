using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IGeneralRepository
    {
        List<Rules>  getAllRules();
        User UserLogin(string email, string password);
        bool AdminLogin(string email, string password);
    }
}