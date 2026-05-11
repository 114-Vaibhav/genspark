namespace simplenotification
{
    public class SMSNotificationSender : INotificationSender
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine($"SMS notification sent to {user.name} on {user.phone}");
        }
    }
}
