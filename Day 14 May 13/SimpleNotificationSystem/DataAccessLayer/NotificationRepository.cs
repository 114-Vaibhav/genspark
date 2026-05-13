using Npgsql;
using System.Net.NetworkInformation;
using simplenotification.Contexts;
namespace simplenotification
{
    public class NotificationRepository
    {
        private NotificationDBContext db;
        public NotificationRepository()
        {
            db= new NotificationDBContext();
        }

        public void SaveNotification(Notification note)
        {

            try
            {
                var notificationFromDB = db.Notifications.FirstOrDefault(n => n.NotificationId == note.NotificationId);
                if(notificationFromDB == null)
                {
                    db.Notifications.Add(note);
                    db.SaveChanges();
                }
                else
                {
                    notificationFromDB.message = note.message;
                    notificationFromDB.sentdate = note.sentdate;
                    notificationFromDB.NotificationType = note.NotificationType;
                    notificationFromDB.SentToName = note.SentToName;
                    notificationFromDB.SentToContact = note.SentToContact;

                    db.SaveChanges();
                }
                
            }
            catch (NpgsqlException npgsqlEx)
            {
                Console.WriteLine("Database error: " + npgsqlEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving notification: " + ex.Message);
                Console.WriteLine("Error saving notification: " + ex.InnerException?.Message);
                Console.WriteLine("Error saving notification: " + ex.InnerException?.InnerException?.Message);
            }
        }

        
        public List<Notification> GetAllSentNotification()
        {
            List<Notification> notifications = new List<Notification>();
            try
            {
                return db.Notifications.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving notifications: " + ex.Message);
            }
            return notifications;
        }
    }
}
