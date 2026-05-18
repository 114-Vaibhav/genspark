using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib
{
    public interface IUserRepository
    {
        public bool AddNewUserInDB(User user);
        public List<User> getAllUsersFromDB();
        public User getUserByIdFromDB(int userId);

        public User getUserByContactFromDB(string contact, int contactType);
        public bool updateMembershipStatusInDB(int userId, int toMembershipId);
        public bool deactivateUserInDB(int userId);
        public bool deleteUserFromDB(int userId);
        public User getPersonalDetailsFromDB(int userId);
        public MembershipTypes getMembershipStatusFromDB(int userId);
        public List<MembershipTypes> getAllMembershipTypesFromDB();


    }
}