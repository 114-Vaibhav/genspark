using System.Net.Mail;
namespace simplenotification
{
    public class NotificationService
    {   
        private Dictionary<int,User> users = new Dictionary<int, User>();
        private Dictionary<string,int> idmapping = new Dictionary<string, int>();
        private NotificationRepository notificationRepository = new NotificationRepository();

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
            int id=0;
            if (users.Count > 0)
            {
                id = users.Keys.Max();
            }

            User user = new User(id+1,name,email,phone);
            users[id+1]=user;
            idmapping[email]=id+1;
            idmapping[phone]=id+1;
            PrintUserDetails(user);
            return user;
        } 

        public void  SendNotification()
        {
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
                    sentdate = DateTime.Now,
                    SentToName = user.name,
                    SentToContact = noti.NotificationType == NotiType.SMSNotification ? user.phone : user.email
                };

                sender.Send(user, sentNotification);
                notificationRepository.SaveNotification(sentNotification);
            }
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

        public void FindUserByEmail(string email)
        {
            if (!idmapping.ContainsKey(email))
            {
                System.Console.WriteLine("Email not found");
                return;
            }
            System.Console.WriteLine("-----------------------"); 
            System.Console.WriteLine(users[idmapping[email]]);
        }
        public void FindUserByPhone(string phone)
        {
            if (!idmapping.ContainsKey(phone))
            {
                System.Console.WriteLine("Phone not found");
                return;
            }
            System.Console.WriteLine("-----------------------");
            System.Console.WriteLine(users[idmapping[phone]]);
        }

        public void UpdateUserByPhone(string phone)
        {
            if (!idmapping.ContainsKey(phone))
            {
                System.Console.WriteLine("Phone not found");
                return;
            }
            int id = idmapping[phone];
            idmapping.Remove(phone);
            idmapping.Remove(users[id].email);
            updateDetails(id);
            System.Console.WriteLine("User Details Updated");

        }
        public void UpdateUserByEmail(string email)
        {
            if (!idmapping.ContainsKey(email))
            {
                System.Console.WriteLine("Email not found");
                return;
            }
            int id = idmapping[email];
            idmapping.Remove(email);
            idmapping.Remove(users[id].phone);
            updateDetails(id);
            System.Console.WriteLine("User Details Updated");
        }
        private void updateDetails(int id)
        {
            string newname;
            string newemail;
            string newphone;
            System.Console.WriteLine("Do you want to update name: type yes or no");
            string inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing name: {users[id].name}");
                System.Console.WriteLine("Type your new name: ");
                newname = Console.ReadLine() ?? string.Empty;
            }else
            {
                newname=users[id].name;
            }
            System.Console.WriteLine("Do you want to update phone: type yes or no");
            inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing phone: {users[id].phone}");
                System.Console.WriteLine("Type your new phone: ");
                newphone = Console.ReadLine() ?? string.Empty;
            }else
            {
                newphone=users[id].phone;
            }
            System.Console.WriteLine("Do you want to update email: type yes or no");
            inpt = Console.ReadLine() ?? string.Empty;
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing email: {users[id].name}");
                System.Console.WriteLine("Type your new email: ");
                newemail = Console.ReadLine() ?? string.Empty;
            }else
            {
                newemail=users[id].email;
            }
            User user = new User(id,newname,newemail,newphone);
            idmapping[newemail]=id;
            idmapping[newphone]=id;
            users[id]=user;
        }
        public void DeleteUserByPhone(string phone)
        {
            if (!idmapping.ContainsKey(phone))
            {
                System.Console.WriteLine("Phone not found");
                return;
            }
            int id = idmapping[phone];
            idmapping.Remove(phone);
            idmapping.Remove(users[id].email);
            users.Remove(id);
            System.Console.WriteLine("User Deleted");
        }
        public void DeleteUserByEmail(string email)
        {
            if (!idmapping.ContainsKey(email))
            {
                System.Console.WriteLine("Email not found");
                return;
            }
            int id = idmapping[email];
            idmapping.Remove(email);
            idmapping.Remove(users[id].phone);
            users.Remove(id);
            System.Console.WriteLine("User Deleted");
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

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
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

        private void PrintUserDetails(User user)
        {
            System.Console.WriteLine("---------User Details-----------------");
            System.Console.WriteLine(user);
            System.Console.WriteLine("---------End-----------------");
        }
    }
}
