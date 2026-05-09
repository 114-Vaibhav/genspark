
namespace simplenotification
{
    internal class NotificationRepo{
        private List<Notification> sentnotification;
        public NotificationRepo()
        {
            sentnotification = new List<Notification>();
        }
        public void saveNotification(Notification note)
        {
            sentnotification.Add(note);
        }
        public List<Notification> getAllSentNotification()
        {
            
            return sentnotification;
        }
    }
}