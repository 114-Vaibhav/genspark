namespace simplenotification
{
    public class User
    {
        public int UserId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }

        public User(string name,string email,string phone)
        {
            this.name=name;
            this.email=email;
            this.phone=phone;
        }
      public override string ToString()
        {
            return $"-------------\n\nName : {name}\nPhone Number : {phone}\n" +
                $"Email : {email}\n";
        }
    }
}
