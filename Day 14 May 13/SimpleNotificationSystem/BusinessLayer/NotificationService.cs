using System.Net.Mail;
namespace simplenotification
{
    public class NotificationService
    {   
        private NotificationRepository notificationRepository ;
        private UserRepository userRepository ;

        public NotificationService()
        {
            notificationRepository = new NotificationRepository();
            userRepository = new UserRepository();
        }

        private Notification createNotification()
        {
            System.Console.WriteLine("select notification type 1 for SMS, 2 for EMail");
            int typeChoice;
            while(!Int32.TryParse(Console.ReadLine(), out typeChoice) || typeChoice < 1 || typeChoice > 2)
            {
                System.Console.WriteLine("Try again");
            }

            NotiType notificationType = typeChoice == 1 ? NotiType.SMSNotification : NotiType.EmailNotification;
            string message = ReadValidMessage(notificationType);

            return new Notification
            {
                message = message,
                NotificationType = notificationType,
                sentdate = DateTime.Now
            };
        }
        public void  SendNotification()

        {
            var users = new UserRepository().GetAllUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("Add at least one user before sending notification.");
                return;
            }

            Notification noti = createNotification();
            INotificationSender sender = GetNotificationSender(noti.NotificationType);

            foreach(var user in users.Values)
            {
                if (!CanSendToUser(user, noti))
                {
                    continue;
                }

                var sentNotification = new Notification
                {
                    message = noti.message,
                    NotificationType = noti.NotificationType,
                    sentdate = DateTime.UtcNow,
                    SentToName = user.name,
                    SentToContact = noti.NotificationType == NotiType.SMSNotification ? user.phone : user.email
                };

                sender.Send(user, sentNotification);
                notificationRepository.SaveNotification(sentNotification);
            }
        }
        private bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
    
        private bool IsValidPhone(string phone)
            {
                return phone.Length == 10 && phone.All(char.IsDigit);
            }

        private string ReadValidMessage(NotiType notificationType)
        {
            while (true)
            {
                System.Console.WriteLine("Type your message: ");
                string message = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("Message should not be empty.");
                    continue;
                }

                if (message.Length < 5)
                {
                    Console.WriteLine("Message length should be at least 5 characters.");
                    continue;
                }

                if (notificationType == NotiType.SMSNotification && message.Length > 160)
                {
                    Console.WriteLine("SMS message should not exceed 160 characters.");
                    continue;
                }

                return message;
            }
        }

        public User CreateUser()
        {
            System.Console.WriteLine("Enter the user's name: ");
            string name = Console.ReadLine() ?? string.Empty;
            System.Console.WriteLine("Enter the email id: ");
            
            string email;
            while (true)
            {
                email = Console.ReadLine() ?? string.Empty;
                if (IsValidEmail(email))
                {
                    break;
                }

                Console.WriteLine("Invalid, try again");
            }
           System.Console.WriteLine("Enter 10 digit of phone no: ");
            string phone;
            while (true)
            {
                phone = Console.ReadLine() ?? string.Empty;

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
            userRepository.addUserIntoDatabase(user);
            // users[id+1]=user;
            // idmapping[email]=id+1;
            // idmapping[phone]=id+1;
            // PrintUserDetails(user);
            return user;
        } 

        public User FindUserByEmail(string email)
        {
            User user = userRepository.FindUserByEmail(email);
            if (user == null)
            {
                System.Console.WriteLine("Email not found");
                return null;
            }
            // PrintUserDetails(user);
            return user;
        }
        public User FindUserByPhone(string phone)
        {
            User user = userRepository.FindUserByPhone(phone);
            if (user == null)
            {
                System.Console.WriteLine("Phone not found");
                return null;
            }
            // PrintUserDetails(user);
            return user;
        }
        public void UpdateUserByPhone(string phone)
        {
            User user = FindUserByPhone(phone);
            if (user == null)            {
                System.Console.WriteLine("Phone not found");
                return;
            }  
            userRepository.updateDetails(user);
            System.Console.WriteLine("User Details Updated");

        }
        public void UpdateUserByEmail(string email)
        {
            User user = FindUserByEmail(email);
            if (user == null)
            {
                System.Console.WriteLine("Email not found");
                return;
            }
            userRepository.updateDetails(user);
            System.Console.WriteLine("User Details Updated");
        }
      
        public void DeleteUserByEmail(string email)
        {
            User user = FindUserByEmail(email);
            if (user == null)
            {
                System.Console.WriteLine("Email not found");
                return;
            }
            userRepository.DeleteUserByEmail(email);
            System.Console.WriteLine("User Deleted");
        }
        public void DeleteUserByPhone(string phone)
        {
            User user = FindUserByPhone(phone);
            if (user == null)
            {
                System.Console.WriteLine("Phone not found");
                return;
            }
            userRepository.DeleteUserByPhone(phone);
            System.Console.WriteLine("User Deleted");
        }
        private INotificationSender GetNotificationSender(NotiType notificationType)
        {
            if (notificationType == NotiType.SMSNotification)
            {
                return new SMSNotificationSender();
            }

            return new EmailNotificationSender();
        }

        private bool CanSendToUser(User user, Notification notification)
        {
            if (notification.NotificationType == NotiType.EmailNotification && !IsValidEmail(user.email))
            {
                Console.WriteLine($"Email notification skipped for {user.name}: invalid email.");
                return false;
            }

            if (notification.NotificationType == NotiType.SMSNotification && !IsValidPhone(user.phone))
            {
                Console.WriteLine($"SMS notification skipped for {user.name}: invalid phone number.");
                return false;
            }

            return true;
        }
         public void DisplaySentNotifications()
        {
            List<Notification> notifications = notificationRepository.GetAllSentNotification();
            if (notifications.Count == 0)
            {
                Console.WriteLine("No notification sent yet.");
                return;
            }

            Console.WriteLine("---------Sent Notifications-----------------");
            foreach (var notification in notifications)
            {
                Console.WriteLine(notification);
            }
            Console.WriteLine("---------End-----------------");
        }

        public void PrintUserDetails(User user)
        {
            System.Console.WriteLine("---------User Details-----------------");
            System.Console.WriteLine(user);
            System.Console.WriteLine("---------End-----------------");
        }
    }
}
