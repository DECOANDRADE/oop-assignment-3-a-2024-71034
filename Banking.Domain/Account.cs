namespace Banking.Domain
{
    public abstract class Account
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; protected set; }

        public Account(string firstName, string lastName)
        {
            AccountName = GenerateAccountName(firstName, lastName);
            Balance = 0;
        }

        public string GenerateAccountName(string firstName, string lastName)
        {
            int totalNameLength = firstName.Length + lastName.Length;
            int firstInitialPos = char.ToUpper(firstName[0]) - 'A' + 1;
            int secondInitialPos = char.ToUpper(lastName[0]) - 'A' + 1;

            return $"{firstName[0]}{lastName[0]}-{totalNameLength}-{firstInitialPos}-{secondInitialPos}";
        }
    }
}