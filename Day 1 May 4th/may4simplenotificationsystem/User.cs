namespace simplenotification
{
    public class User
    {
        public string name {get; set;}
        public string email{get; set;}
        public string phone {get; set;}
        public User(string username,string useremail,string userphone)
        {
            name=username;
            email=useremail;
            phone=userphone;
        }
      public override string ToString()
        {
            return $"Name : {name}\nPhone Number : {phone}\n" +
                $"Email : {email}\n";
        }
    }
}