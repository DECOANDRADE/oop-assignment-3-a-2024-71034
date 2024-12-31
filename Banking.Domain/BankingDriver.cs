using System.Transactions;
using Banking.Domain.controller;

namespace Banking.Domain
{
    public static class BankingDriver
    {
        private static readonly Bank Bank = new Bank();
        private static Dictionary<string, Account> Accounts = new Dictionary<string, Account>();
        
        private static readonly CostumerController _costumerController = new CostumerController();
        private static readonly AccountController _accountController = new AccountController();

        
        public static void Run()
        {
            Console.WriteLine("Banking application has started");
            Login();
        }

        public static void Login()
        {
            Console.WriteLine("Login Menu");
            Console.WriteLine("1. Bank Employee");
            Console.WriteLine("2. Customer");
            Console.Write("Choose your option: ");

            string option = Console.ReadLine() ?? string.Empty;

            switch (option)
            {
                case "1":
                    BankMenu();
                    break;
                case "2":
                    CustomerMenu();
                    break;
                default:
                    Console.WriteLine("Invalid option. Exiting...");
                    break;
            }
        }

        public static void BankMenu()
        {
            Console.WriteLine("Bank Menu");
            Console.WriteLine("1. Create Customer");
            Console.WriteLine("2. Delete Customer");
            Console.WriteLine("3. View All Customers");
            Console.WriteLine("4. Perform Transactions");
            Console.WriteLine("0. Logout");

            string option = Console.ReadLine() ?? string.Empty;

            switch (option)
            {
                case "1":
                    Console.WriteLine("Create Customer Functionality");
                    Console.Write("Enter First Name: ");
                    string firstName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Last Name: ");
                    string lastName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Enter Email: ");
                    string email = Console.ReadLine() ?? string.Empty;
                    
                    _costumerController.CreateCostumer(firstName, lastName, email);
                    break;
                case "2":
                    Console.Write("Enter Account Number to delete: ");
                    string accountNumber = Console.ReadLine() ?? string.Empty;
                    
                    _costumerController.DeleteCustomer(accountNumber);
                    break;
                case "3":
                    Console.WriteLine("View All Customers Functionality");
                    
                    _costumerController.ListCustomers();
                    break;
                case "4":
                    Console.WriteLine("Perform Transactions Functionality");
                    Console.Write("Enter Account Number: ");
                    string accNumber = Console.ReadLine() ?? string.Empty;

                    Console.Write("Enter Transaction Type (Deposit/Withdraw): ");
                    string transactionType = Console.ReadLine() ?? string.Empty;
                    
                    Console.Write("Enter Account Type (Current/Savings): ");
                    string accountType = Console.ReadLine() ?? string.Empty;

                    Console.Write("Enter Amount: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                    {
                        Console.WriteLine("Invalid amount.");
                        break;
                    }

                    string? targetAccountNumber = null;
                    if (transactionType.Equals("Transfer", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Enter Target Account Number: ");
                        targetAccountNumber = Console.ReadLine();
                    }

                    _accountController.PerformTransaction(accountType, accNumber, amount, transactionType, targetAccountNumber);
                    
                    break;
                case "0":
                    Console.WriteLine("Logging out...");
                    Logout();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
            Logout();
        }

        public static void CustomerMenu()
        {
            Console.WriteLine("Customer Menu");
            Console.WriteLine("1. View Transactions");
            Console.WriteLine("2. Perform Transactions");
            Console.WriteLine("0. Logout");

            string option = Console.ReadLine() ?? string.Empty;

            switch (option)
            {
                case "1":
                    Console.WriteLine("View Transactions Functionality");
                    ViewTransactionMenu();
                    break;
                case "2":
                    Console.WriteLine("Perform Transactions Functionality");
                    PerformTransactionMenu();
                    break;
                case "0":
                    Console.WriteLine("Logging out...");
                    Logout();
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
            Logout();
        }
        
        private static void ViewTransactionMenu()
        {
            Console.WriteLine("View Transaction History:");
            Console.WriteLine("1. Savings Account");
            Console.WriteLine("2. Current Account");
            Console.WriteLine("Choose an option:");

            var choice = Console.ReadLine();
            string accountType = choice switch
            {
                "1" => "savings",
                "2" => "current",
                _ => null
            };

            if (accountType == null)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                ViewTransactionMenu();
                return;
            }

            Console.WriteLine("Enter your account number:");
            string accountNumber = Console.ReadLine();

            Bank.ViewTransactions(accountNumber, accountType);
        }
        
        public static void PerformTransactionMenu()
        {
            Console.WriteLine("\nPerform Transaction Menu:");
            Console.WriteLine("1. Deposit");
            Console.WriteLine("2. Withdraw");
            Console.WriteLine("Choose an option:");

            var actionToPerformChoice = Console.ReadLine();
            string actionToPerform = actionToPerformChoice switch
            {
                "1" => "deposit",
                "2" => "withdraw",
                _ => null
            };

            if (actionToPerform == null)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                PerformTransactionMenu();
                return;
            }

            Console.WriteLine("\nEnter your account number:");
            string accountNumber = Console.ReadLine();
            
            Console.WriteLine("\nChoose account type:");
            Console.WriteLine("1. Current");
            Console.WriteLine("2. Savings");
            Console.WriteLine("Choose an option:");
            var accuntTypeChoice = Console.ReadLine();
            string actionAccuntType = accuntTypeChoice switch
            {
                "1" => "current",
                "2" => "savings",
                _ => null
            };

            if (actionAccuntType == null)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                PerformTransactionMenu();
                return;
            }
            
            Console.WriteLine("Enter the value you would like to " + actionToPerform + ": ");
            string input = Console.ReadLine();
            if (decimal.TryParse(input, out decimal result))
            {
                if (actionToPerform.Equals("deposit"))
                {
                    _accountController.Deposit(accountNumber, actionAccuntType, result);
                }
                else
                {
                    _accountController.Withdraw(accountNumber, actionAccuntType, result);
                }
            }

            CustomerMenu();
        }
        
        public static void Logout()
        {
            Console.WriteLine("Logging out...");
            Console.WriteLine("-------------------------------------------------------------------");
            Run(); 
        }
    }
}
