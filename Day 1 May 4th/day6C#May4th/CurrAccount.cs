namespace UnderstandingOOPs
{
    internal class CurrAccount : Account
    {
         public CurrAccount()
        {
            AccountType = AccType.CurrentAccount;
            Balance = 0.0f;
        }
    }
}