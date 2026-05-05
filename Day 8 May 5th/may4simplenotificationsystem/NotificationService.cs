using System.Reflection.Metadata;
using System.Net.Mail;
using System.Linq;
namespace simplenotification
{
    internal class NotificationService : INotificationInteract
    {   
        // List<User> users = new List<User>();
        Dictionary<int,User> users = new Dictionary<int, User>();
        Dictionary<string,int> idmapping = new Dictionary<string, int>();

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
            int id=0;
            if (users.Count > 0)
            {
                id = users.Keys.Max();
            }

            User user = new User(id+1,name,email,phone);
            users[id+1]=user;
            idmapping[email]=id+1;
            idmapping[phone]=id+1;
            // users.Add(user);
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
                    System.Console.WriteLine($"Notification sent to user {user.Value.name} on {user.Value.phone}");
                }
            }else 
            if (noti.NotificationType == NotiType.EmailNotification)
            {
                foreach(var user in users)
                {
                    System.Console.WriteLine($"Notification sent to user {user.Value.name} on {user.Value.email}");
                }
            }

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
            string inpt = Console.ReadLine();
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing name: {users[id].name}");
                System.Console.WriteLine("Type your new name: ");
                newname = Console.ReadLine();
            }else
            {
                newname=users[id].name;
            }
            System.Console.WriteLine("Do you want to update phone: type yes or no");
            inpt = Console.ReadLine();
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing phone: {users[id].phone}");
                System.Console.WriteLine("Type your new phone: ");
                newphone = Console.ReadLine();
            }else
            {
                newphone=users[id].phone;
            }
            System.Console.WriteLine("Do you want to update email: type yes or no");
            inpt = Console.ReadLine();
            if (inpt == "yes")
            {
                System.Console.WriteLine($"Your existing email: {users[id].name}");
                System.Console.WriteLine("Type your new email: ");
                newemail = Console.ReadLine();
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