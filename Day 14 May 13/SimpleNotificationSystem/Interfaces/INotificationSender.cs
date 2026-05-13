namespace simplenotification
{
    public interface INotificationSender
    {
        void Send(User user, Notification notification);
    }
}
