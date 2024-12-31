namespace Banking.Domain.repository;

public class CostumerRepository
{
    public void UpdateCustomerFile(string accountNumber)
    {
        const string filePath = "customers.txt";
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Customer file not found. Nothing to update.");
            return;
        }

        var allLines = File.ReadAllLines(filePath).ToList();
        var updatedLines = allLines.Where(line => !line.StartsWith(accountNumber + "\t")).ToList();

        File.WriteAllLines(filePath, updatedLines);
    }

    public void DeleteCustomerFiles(Customer customer)
    {
        string savingsFile = $"{customer.AccountNumber}-savings.txt";
        string currentFile = $"{customer.AccountNumber}-current.txt";

        if (File.Exists(savingsFile))
            File.Delete(savingsFile);

        if (File.Exists(currentFile))
            File.Delete(currentFile);
    }
    
}