
namespace simplenotification
{
    public class NotificationRepository
    {
        private List<Notification> sentnotification;

        public NotificationRepository()
        {
            sentnotification = new List<Notification>();
        }

        public void SaveNotification(Notification note)
        {
            sentnotification.Add(note);
        }

        public List<Notification> GetAllSentNotification()
        {
            return sentnotification;
        }
    }
}
