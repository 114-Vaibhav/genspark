namespace simplenotification
{
    public enum NotiType
    {
        SMSNotification =1,
        EmailNotification=2,
        WhatsappNotification=3
    }

    public class Notification
    {
        public int NotificationId { get; set; }
        public string message { get; set; } = string.Empty;
        public DateTime sentdate { get; set; }
        public NotiType NotificationType { get; set; }
        public string SentToName { get; set; } = string.Empty;
        public string SentToContact { get; set; } = string.Empty;
      
        public override string ToString()
        {
            return $"Type : {NotificationType}\nUser : {SentToName}\nContact : {SentToContact}\nMessage : {message}\nSent Date : {sentdate}\n";
        }
    }
}
