using System.Reflection.Metadata;
using System.Net.Mail;
using System.Linq;
namespace simplenotification
{
    internal class NotificationService : INotificationInteract
    {   
        List<User> users = new List<User>();
         public User CreateUser()
        {
            
            System.Console.WriteLine("Enter the user's name: ");
            string name = Console.ReadLine();
            System.Console.WriteLine("Enter the email id: ");
            
            string email;
            while (true)
            {
                email = Console.ReadLine();
                try
                {
                    var addr = new MailAddress(email);
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid, try again");
                }
            }
           System.Console.WriteLine("Enter 10 digit of phone no: ");
            string phone;
            while (true)
            {
                phone = Console.ReadLine();

                if (phone.Length == 10 && phone.All(char.IsDigit))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid phone number, enter exactly 10 digits.");
                }
            }
            User user = new User(name,email,phone);

            users.Add(user);
            PrintUserDetails(user);
            return user;
        } 
        public void  SendNotification()
        {
            Notification noti = createNotification();
            if (noti.NotificationType == NotiType.SMSNotification)
            {
                foreach(var user in users)
                {
                    System.Console.WriteLine($"Notification sent to user {user.name} on {user.phone}");
                }
            }else 
            if (noti.NotificationType == NotiType.EmailNotification)
            {
                foreach(var user in users)
                {
                    System.Console.WriteLine($"Notification sent to user {user.name} on {user.email}");
                }
            }

        }
        private Notification createNotification()
        {
            Notification noti;
            System.Console.WriteLine("select notification type 1 for SMS, 2 for EMail");
            int typeChoice;
            while(!Int32.TryParse(Console.ReadLine(), out typeChoice) && typeChoice>0 && typeChoice<3)
                System.Console.WriteLine("Try again");
            if(typeChoice==1) noti = new SMSNotification();
            else noti = new EmailNotification();
            System.Console.WriteLine("Type your message: ");
            string mes = Console.ReadLine();
            noti.message=mes;
            noti.sentdate = DateTime.Now;
            return noti;
        }
        private void PrintUserDetails(User user)
        {
            System.Console.WriteLine("---------User Details-----------------");
            System.Console.WriteLine(user);
            System.Console.WriteLine("---------End-----------------");
        }
    }
}