namespace Banking.Domain
{
    public class Bank
    {
        
        public void ViewTransactions(string accountName, string accountType)
        {
            string fileName = $"{accountName}-{accountType}.txt";

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Account file {fileName} not found.");
                return;
            }

            var transactions = File.ReadAllLines(fileName);
            if (transactions.Length == 0)
            {
                Console.WriteLine("No transactions found.");
                return;
            }

            Console.WriteLine($"Transactions for {accountName} ({accountType} account):");
            foreach (var transaction in transactions)
            {
                Console.WriteLine(transaction);
            }
        }
        
    }
}