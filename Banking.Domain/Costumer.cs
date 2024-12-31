namespace Banking.Domain
{
    public class Customer
    {
        public AccountType Type { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; private set; }
        public string Pin { get; private set; }
        public SavingsAccount SavingsAccount { get; private set; }
        public CurrentAccount CurrentAccount { get; private set; }

        public Customer(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            AccountNumber = GenerateAccountNumber(firstName, lastName);
            Pin = GeneratePin(firstName, lastName);

            SavingsAccount = new SavingsAccount(firstName, lastName);
            CurrentAccount = new CurrentAccount(firstName, lastName);
        }
        
        public Customer(string firstName, string lastName, string accountNumber, string pin)
        {
            FirstName = firstName;
            LastName = lastName;
            AccountNumber = accountNumber;
            Pin = pin;
        }

        private string GenerateAccountNumber(string firstName, string lastName)
        {
            return $"{firstName[0]}{lastName[0]}-{firstName.Length + lastName.Length}-{char.ToUpper(firstName[0]) - 'A' + 1}-{char.ToUpper(lastName[0]) - 'A' + 1}";
        }

        private string GeneratePin(string firstName, string lastName)
        {
            return $"{char.ToUpper(firstName[0]) - 'A' + 1}{char.ToUpper(lastName[0]) - 'A' + 1}";
        }
    }
    public enum AccountType
    {
        Savings,
        Current
    }
}