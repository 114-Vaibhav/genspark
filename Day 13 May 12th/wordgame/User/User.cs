namespace wordgame
{

    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string name { get; set; }

        public User(string name,string email,string username,string password)
        {
            this.name = name;
            this.email = email;
            this.username = username;
            this.password = password;
        }
    }
}