namespace UnderstandingOOPs
{
    public enum AccType
    {
        SavingAccount=1,
        CurrentAccount=2
    }
    internal class Account
    {
        // public required string  AccountNo{get; set;}
        public string AccountNo{get; set;} = string.Empty;
        // public string accountno
        // {
        //     get
        //     {
        //         var accno = accountno.Substring(4,7);
        //         return "**********"+accno;
        //     }
        //     set
        //     {
        //         accountno = value;
        //     }
        // }
        public string NameOnAccount {get; set;} = string.Empty;
        public DateTime DateOfBirth {get;set;}
        public string Phone {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public float Balance {get; set;}
        public AccType AccountType { get; set; }

        public Account()
        {
            
        }
        public Account(string accno,string name,DateTime dob,
        string email,string phoneno,float bal)
        {
            AccountNo=accno;
            NameOnAccount=name;
            DateOfBirth = dob;
            Email=email;
            Phone = phoneno;
            Balance = bal;


        }
          public override string ToString()
        {
            return $"Account Number : {AccountNo}\nAccount Holder Name : {NameOnAccount}\nPhone Number : {Phone}\n" +
                $"Email : {Email}\nBalance : ${Balance}";
        }
    }
}