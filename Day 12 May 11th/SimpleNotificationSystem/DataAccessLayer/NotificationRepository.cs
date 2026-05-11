using Npgsql;
using System.Net.NetworkInformation;

namespace simplenotification
{
    public class NotificationRepository
    {
        // private List<Notification> sentnotification;
        string connectionString =
            "Host=localhost;Port=5432;Database=simplenotificationsystem;Username=vaibhavgupta;Password=8098";
        NpgsqlConnection connection;
        public NotificationRepository()
        {
            connection = new NpgsqlConnection(connectionString);
           
        }

        public void SaveNotification(Notification note)
        {
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS notifications (
                    message TEXT,
                    sentdate TIMESTAMP,
                    notificationtype TEXT,
                    senttoname TEXT,
                    senttocontact TEXT
                );
            ";

            string insertQuery = @"
                INSERT INTO notifications 
                (message, sentdate, notificationtype, senttoname, senttocontact)
                VALUES 
                (@message, @sentdate, @type, @name, @contact);
            ";

            try
            {
                connection.Open();
                using (var createCommand = new NpgsqlCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                }
                using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@message", note.message);
                    insertCommand.Parameters.AddWithValue("@sentdate", note.sentdate);
                    insertCommand.Parameters.AddWithValue("@type", note.NotificationType.ToString());
                    insertCommand.Parameters.AddWithValue("@name", note.SentToName);
                    insertCommand.Parameters.AddWithValue("@contact", note.SentToContact);

                    insertCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving notification: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        
        public List<Notification> GetAllSentNotification()
        {
            string query = "select * from notifications";
            var command = new NpgsqlCommand(query, connection);
            List<Notification> notifications = new List<Notification>();
            try
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())                {
                    Notification noti = new Notification
                    {
                        message = reader.GetString(0),
                        sentdate = reader.GetDateTime(1),
                        NotificationType = reader.GetString(2) == "SMSNotification" ? NotiType.SMSNotification : NotiType.EmailNotification,
                        SentToName = reader.GetString(3),
                        SentToContact = reader.GetString(4)
                    };
                    notifications.Add(noti);

                }
            }
            finally
            {
                connection.Close();
            }
            return notifications;
        }
    }
}
