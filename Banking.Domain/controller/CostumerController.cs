using Banking.Domain.repository;

namespace Banking.Domain.controller;

public class CostumerController
{
    private static readonly CostumerRepository _repository = new CostumerRepository();

    public void CreateCostumer(string firstName, string lastName, string email)
    {
        string accountName = GenerateAccountName(firstName, lastName);
        string accountPIN = GeneratePIN(firstName, lastName);
        string customerFile = "customers.txt";

        if (File.Exists(customerFile) && File.ReadAllLines(customerFile).Any(line => line.StartsWith(accountName)))
        {
            Console.WriteLine($"Customer {accountName} already exists.");
        }

        using (StreamWriter writer = new StreamWriter(customerFile, append: true))
        {
            writer.WriteLine($"{accountName}\t{firstName} {lastName}\t{accountPIN}");
        }

        File.Create($"{accountName}-savings.txt").Close();
        File.Create($"{accountName}-current.txt").Close();

        Console.WriteLine($"Customer {firstName} {lastName} created successfully with account: {accountName}");
    }

    private string GenerateAccountName(string firstName, string lastName)
    {
        string initials = $"{firstName[0]}{lastName[0]}".ToLower();
        int nameLength = firstName.Length + lastName.Length;
        int firstInitialPosition = char.ToUpper(firstName[0]) - 'A' + 1;
        int secondInitialPosition = char.ToUpper(lastName[0]) - 'A' + 1;

        return $"{initials}-{nameLength}-{firstInitialPosition}-{secondInitialPosition}";
    }

    private string GeneratePIN(string firstName, string lastName)
    {
        int firstInitialPosition = char.ToUpper(firstName[0]) - 'A' + 1;
        int lastInitialPosition = char.ToUpper(lastName[0]) - 'A' + 1;
        return $"{firstInitialPosition:D2}{lastInitialPosition:D2}";
    }

    public void DeleteCustomer(string accountNumber)
    {
        string customersFilePath = "customers.txt";

        // Verificar se o arquivo de clientes existe
        if (!File.Exists(customersFilePath))
        {
            Console.WriteLine("Customer file not found.");
            return;
        }

        // Ler todas as linhas do arquivo de clientes
        var customerLines = File.ReadAllLines(customersFilePath).ToList();

        // Procurar o cliente com o número da conta correspondente
        var customerLine = customerLines.FirstOrDefault(line => line.Split('\t')[0] == accountNumber);

        if (customerLine == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // Dividir a linha para obter os dados do cliente
        var parts = customerLine.Split('\t');
        string customerName = parts[1];
        string pin = parts[2];

        // Criar um objeto Customer a partir dos dados lidos
        var nameParts = customerName.Split(' ');
        string firstName = nameParts[0];
        string lastName = nameParts[1];

        Customer customer = new Customer(firstName, lastName, accountNumber, pin);

        // Verificar se o cliente tem saldo em alguma conta
        string savingsFilePath = $"{accountNumber}-savings.txt";
        string currentFilePath = $"{accountNumber}-current.txt";

        decimal savingsBalance = 0;
        decimal currentBalance = 0;

        if (File.Exists(savingsFilePath))
        {
            savingsBalance = GetAccountBalance(savingsFilePath);
        }

        if (File.Exists(currentFilePath))
        {
            currentBalance = GetAccountBalance(currentFilePath);
        }

        if (savingsBalance > 0 || currentBalance > 0)
        {
            Console.WriteLine("Customer cannot be deleted. Account balances must be zero.");
            return;
        }

        // Remover a linha do arquivo de clientes
        customerLines.Remove(customerLine);

        // Regravar o arquivo de clientes sem o cliente deletado
        File.WriteAllLines(customersFilePath, customerLines);

        // Excluir os arquivos de transações do cliente
        DeleteCustomerFiles(accountNumber);

        Console.WriteLine("Customer deleted successfully.");
    }

    private decimal GetAccountBalance(string filePath)
    {
        decimal balance = 0;

        // Verificar se o arquivo de transações existe
        if (!File.Exists(filePath))
        {
            return balance;
        }

        // Ler as transações do arquivo
        var transactions = File.ReadAllLines(filePath);

        // Se o arquivo tiver transações, pegar o último saldo registrado
        if (transactions.Any())
        {
            var lastTransaction = transactions.Last().Split('\t');
            if (lastTransaction.Length == 4)
            {
                balance = decimal.Parse(lastTransaction[3]); // O saldo final da transação
            }
        }

        return balance;
    }

    private void DeleteCustomerFiles(string accountNumber)
    {
        string savingsFilePath = $"{accountNumber}-savings.txt";
        string currentFilePath = $"{accountNumber}-current.txt";

        // Excluir os arquivos de transações, se existirem
        if (File.Exists(savingsFilePath))
        {
            File.Delete(savingsFilePath);
        }

        if (File.Exists(currentFilePath))
        {
            File.Delete(currentFilePath);
        }
    }


    public void ListCustomers()
    {
        string filePath = "customers.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("No customers found. The file does not exist.");
            return;
        }

        Console.WriteLine("List of Customers:");
        Console.WriteLine("----------------------------------------------------");

        var lines = File.ReadAllLines(filePath);

        if (lines.Length == 0)
        {
            Console.WriteLine("No customers found in the file.");
            return;
        }

        foreach (var line in lines)
        {
            var data = line.Split('\t');

            if (data.Length < 3)
            {
                Console.WriteLine("Invalid customer data found.");
                continue;
            }

            string accountNumber = data[0];
            string customerName = data[1];
            string pin = data[2];

            Console.WriteLine($"Account Number: {accountNumber}");
            Console.WriteLine($"Customer Name : {customerName}");
            Console.WriteLine($"PIN           : {pin}");
            Console.WriteLine("----------------------------------------------------");
        }
    }
}