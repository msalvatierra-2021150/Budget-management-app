namespace ManagementWebApp.Models
{
    public class IndexAccountsViewModel
    {
        public string AccountType{ get; set; }
        public IEnumerable<Accounts> Accounts { get; set; }
        public decimal Balance => Accounts.Sum(account => account.Balance);
    }
}
