namespace simplenotification
{
    public enum NotiType
    {
        SMSNotification =1,
        EmailNotification=2,
        WhatsappNotification=3
    }
    internal class  Notification
    {
        public string message {get; set;}
        public DateTime sentdate{get; set;}
        public NotiType NotificationType{get; set;}
      
    }
}