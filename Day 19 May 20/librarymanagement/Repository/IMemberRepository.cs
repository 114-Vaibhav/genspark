using librarymanagement.ModelLib;

namespace librarymanagement.DataAccessLib
{
    public interface IMemberRepository
    {
        public bool AddNewUserInDB(Member user);
        public List<Member> getAllUsersFromDB();
        public Member getUserByIdFromDB(int userId);
        public Member getUserByContactFromDB(string contact, int contactType);

    }
}