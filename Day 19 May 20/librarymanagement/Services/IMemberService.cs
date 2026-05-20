using librarymanagement.ModelLib;

namespace librarymanagement.BusinessLib
{
    public interface IMemberService
    {
        bool AddNewUser(Member user); 
        void ViewAllUsers();
        void FindUserById(int userId);
        void FindUserByContact(string contact, int contactType);

    }
}