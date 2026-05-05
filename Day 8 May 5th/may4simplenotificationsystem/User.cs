namespace simplenotification
{
    public class User
    {
        public int id {get;set;}
        public string name {get; set;}
        public string email{get; set;}
        public string phone {get; set;}
        public User(int userid,string username,string useremail,string userphone)
        {
            id=userid;
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