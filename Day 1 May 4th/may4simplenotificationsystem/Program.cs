

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
            while (choice != 3)
            {   
                System.Console.WriteLine("1. Add User ");
                System.Console.WriteLine("2. Send Notification ");
                System.Console.WriteLine("3. Exit");
                System.Console.WriteLine("Enter your choice: ");
                choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        var user = notiInteract.CreateUser();
                        System.Console.WriteLine(user);
                        break;
                    case 2:
                       notiInteract.SendNotification();
                        break;
                    case 3:
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