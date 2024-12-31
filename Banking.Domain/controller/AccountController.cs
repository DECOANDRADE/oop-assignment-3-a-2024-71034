using Banking.Domain.repository;

namespace Banking.Domain.controller;

public class AccountController
{
    private static readonly CostumerRepository _repository = new CostumerRepository();

    public bool Deposit(string accountNumber, string accountType, decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Deposit amount must be positive.");
            return false;
        }

        // Determinar o caminho do arquivo com base no tipo de conta
        string filePath = accountType == "savings"
            ? $"{accountNumber}-savings.txt"
            : $"{accountNumber}-current.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The specified {accountType} account does not exist.");
            return false;
        }

        decimal currentBalance = GetAccountBalance(filePath);

        // Adicionar o valor do depósito ao saldo atual
        decimal newBalance = currentBalance + amount;

        // Registrar a transação no arquivo
        bool isDepositSuccess = RecordTransaction(filePath, "Deposit", amount, newBalance);

        if (isDepositSuccess)
        {
            Console.WriteLine($"Deposit of {amount} to {accountType} account successful. New balance: {newBalance}");
            return true;
        }

        return false;
    }
    
    public void Withdraw(string accountNumber, string accountType, decimal amount)
    {
        // Validar tipo de conta
        if (accountType.ToLower() != "savings" && accountType.ToLower() != "current")
        {
            Console.WriteLine("Invalid account type. Use 'savings' or 'current'.");
            return;
        }

        // Construir o nome do arquivo
        string fileName = $"{accountNumber}-{accountType.ToLower()}.txt";

        if (!File.Exists(fileName))
        {
            Console.WriteLine("Account not found.");
            return;
        }

        // Ler o saldo e validar
        string[] transactions = File.ReadAllLines(fileName);
        if (transactions.Length == 0)
        {
            Console.WriteLine("No transactions found for this account.");
            return;
        }

        // Encontrar o saldo atual (última linha)
        string lastTransaction = transactions.Last();
        string[] transactionParts = lastTransaction.Split('\t');

        if (!decimal.TryParse(transactionParts[3], out decimal currentBalance))
        {
            Console.WriteLine("Error reading account balance.");
            return;
        }

        // Verificar saldo suficiente
        if (currentBalance < amount)
        {
            Console.WriteLine("Insufficient funds.");
            return;
        }

        // Atualizar o saldo
        decimal newBalance = currentBalance - amount;

        // Adicionar a transação ao arquivo
        string transactionRecord = $"{DateTime.Now:dd-MM-yyyy}\tWithdrawal\t-{amount}\t{newBalance}";
        File.AppendAllText(fileName, transactionRecord + Environment.NewLine);

        Console.WriteLine($"Withdrawal of {amount:C} successful. New balance: {newBalance:C}");
    }

    public void PerformTransaction(string accountType, string accNumber, decimal amount, string transactionType, string? targetAccountNumber)
    {
        if (transactionType != null && transactionType.ToLower() != "deposit")
        {
            this.Deposit(accNumber, accountType, amount);
        }else if (transactionType != null && transactionType.ToLower() != "withdraw")
        {
            this.Withdraw(accNumber, accountType, amount);
        }
    }

    private string GetAccountFilePath(string accountNumber, string savingsFilePath, string currentFilePath)
    {
        // Verificar se o arquivo de poupança existe
        if (File.Exists(savingsFilePath))
        {
            return savingsFilePath;
        }

        // Verificar se o arquivo corrente existe
        if (File.Exists(currentFilePath))
        {
            return currentFilePath;
        }

        return null;
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

    private bool RecordTransaction(string filePath, string action, decimal amount, decimal newBalance)
    {
        try
        {
            // Obter a data atual
            string date = DateTime.Now.ToString("dd-MM-yyyy");

            // Criar a linha de transação a ser gravada
            string transactionRecord = $"{date}\t{action}\t{amount}\t{newBalance}";

            // Gravar a transação no arquivo
            File.AppendAllLines(filePath, new[] { transactionRecord });

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error recording transaction: {ex.Message}");
            return false;
        }
    }


}