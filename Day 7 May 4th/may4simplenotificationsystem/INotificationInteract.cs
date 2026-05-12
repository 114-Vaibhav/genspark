using System.Reflection.Metadata;

namespace simplenotification
{
    internal interface INotificationInteract
    {
                public User CreateUser();
                public void SendNotification();
    }
}
