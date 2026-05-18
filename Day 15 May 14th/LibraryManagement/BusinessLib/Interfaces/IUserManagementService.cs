using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.BusinessLib
{
    public interface IUserManagementService
    {
        bool AddNewUser(User user); 
        void ViewAllUsers();

        void FindUserById(int userId);

        void FindUserByContact(string contact, int contactType);

        void UpdateMembershipStatusByAdmin(int userId);

        void DeactivateUser(int userId);

        void ViewPersonalDetails(int userId);

        void ViewMembershipStatus(int userId);
        void DeleteUser(int userId);
        void printAllMembershipTypes();

    }
}