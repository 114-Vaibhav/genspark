using librarymanagementsystem.DataAccessLib.Contexts;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public class GeneralRepository : IGeneralRepository
    {
        LibraryContext db;
        public GeneralRepository( )
        {
            db = new LibraryContext();
        }
        public List<Rules> getAllRules()
        {
            var rules = db.Rules.ToList();
            return rules;
        }
    }
}