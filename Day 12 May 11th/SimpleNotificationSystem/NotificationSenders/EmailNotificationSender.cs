namespace simplenotification
{
    public class EmailNotificationSender : INotificationSender
    {
        public void Send(User user, Notification notification)
        {
            Console.WriteLine($"Email notification sent to {user.name} on {user.email}");
        }
    }
}
