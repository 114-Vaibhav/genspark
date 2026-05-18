using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IGeneralRepository
    {
        List<Rules>  getAllRules();
    }
}