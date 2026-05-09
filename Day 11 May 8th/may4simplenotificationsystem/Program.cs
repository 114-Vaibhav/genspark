

using System.ComponentModel;

namespace simplenotification
{

    internal class Program{
    INotificationInteract notiInteract;
        public Program(){
            notiInteract = new NotificationService();
        }
        void Services()
        {
            int choice =0;
            while (choice != 9)
            {   
                System.Console.WriteLine("1. Add User ");
                System.Console.WriteLine("2. Find User By Email");
                System.Console.WriteLine("3. Find User By Phone");
                System.Console.WriteLine("4. Update User By Email");
                System.Console.WriteLine("5. Update User By Phone");
                System.Console.WriteLine("6. Delete User By Email");
                System.Console.WriteLine("7. Delete User By Phone");
                System.Console.WriteLine("8. Send Notification ");
                System.Console.WriteLine("9. Exit");
                System.Console.WriteLine("Enter your choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        var user = notiInteract.CreateUser();
                        System.Console.WriteLine(user);
                        break;
                    case 2:
                        System.Console.WriteLine("Enter user's email: ");
                        string email =Console.ReadLine();
                        notiInteract.FindUserByEmail(email);
                        break;
                    case 3:
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone =Console.ReadLine();
                        notiInteract.FindUserByPhone(phone);
                        break;
                    case 4:
                        System.Console.WriteLine("Enter user's email: ");
                        string email1 =Console.ReadLine();
                        notiInteract.UpdateUserByEmail(email1);
                        break;
                    case 5:
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone1 =Console.ReadLine();
                        notiInteract.UpdateUserByPhone(phone1);
                        break;
                    case 6:
                        System.Console.WriteLine("Enter user's email: ");
                        string email2 =Console.ReadLine();
                        notiInteract.DeleteUserByEmail(email2);
                        break;
                    case 7:
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone2 =Console.ReadLine();
                        notiInteract.DeleteUserByPhone(phone2);
                        break;
                    case 8:
                       notiInteract.SendNotification();
                        break;
                    default:
                        System.Console.WriteLine("Invalid choice");
                        break;
                }
               
            }
           
           
        }
        static void Main(string[] args)
        {
            new Program().Services();
        }
    }
    
}