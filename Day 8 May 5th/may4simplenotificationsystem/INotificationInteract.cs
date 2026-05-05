using System.Reflection.Metadata;

namespace simplenotification
{
    internal interface INotificationInteract
    {
                public User CreateUser();
                public void FindUserByPhone(string phone);
                public void FindUserByEmail(string email);
                public void UpdateUserByPhone(string phone);
                public void UpdateUserByEmail(string email);
                public void DeleteUserByPhone(string phone);
                public void DeleteUserByEmail(string email);

                public void SendNotification();
    }
}
