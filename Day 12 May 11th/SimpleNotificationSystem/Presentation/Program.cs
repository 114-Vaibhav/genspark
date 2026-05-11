namespace simplenotification
{

    internal class Program{
    NotificationService notiInteract;
        public Program(){
            notiInteract = new NotificationService();
        }
        void Services()
        {
            int choice =0;
            while (choice != 10)
            {   
                Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("1. Add User ");
                System.Console.WriteLine("2. Find User By Email");
                System.Console.WriteLine("3. Find User By Phone");
                System.Console.WriteLine("4. Update User By Email");
                System.Console.WriteLine("5. Update User By Phone");
                System.Console.WriteLine("6. Delete User By Email");
                System.Console.WriteLine("7. Delete User By Phone");
                System.Console.WriteLine("8. Send Notification ");
                System.Console.WriteLine("9. Display Sent Notifications");
                System.Console.WriteLine("10. Exit");
                System.Console.WriteLine("Enter your choice: ");
                if (!Int32.TryParse(Console.ReadLine(), out choice))
                {
                    System.Console.WriteLine("Invalid choice");
                    continue;
                }
                Console.ForegroundColor = ConsoleColor.Cyan;

                switch (choice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        var user = notiInteract.CreateUser();
                        System.Console.WriteLine(user);
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        System.Console.WriteLine("Enter user's email: ");
                        string email =Console.ReadLine();
                        notiInteract.FindUserByEmail(email);
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone =Console.ReadLine();
                        notiInteract.FindUserByPhone(phone);
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        System.Console.WriteLine("Enter user's email: ");
                        string email1 =Console.ReadLine();
                        notiInteract.UpdateUserByEmail(email1);
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone1 =Console.ReadLine();
                        notiInteract.UpdateUserByPhone(phone1);
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.White;
                        System.Console.WriteLine("Enter user's email: ");
                        string email2 =Console.ReadLine();
                        notiInteract.DeleteUserByEmail(email2);
                        break;
                    case 7:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        System.Console.WriteLine("Enter user's phone: ");
                        string phone2 =Console.ReadLine();
                        notiInteract.DeleteUserByPhone(phone2);
                        break;
                    case 8:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        notiInteract.SendNotification();
                        break;
                    case 9:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        notiInteract.DisplaySentNotifications();
                        break;
                    case 10:
                        break;
                    default:
                        System.Console.WriteLine("Invalid choice");
                        break;
                }
               Console.ResetColor();
            }
           
           
        }
        static void Main(string[] args)
        {
            
            
            Console.ResetColor();
            new Program().Services();
        }
    }
    
}
