using System;
using System.Collections.Generic;

namespace ATM_Management_System
{
    public interface IBankSystem
    {
        BankAccount CurrentUser { get; set; }
        void DisplayWelcomeScreen();
        void DisplayTransaction();
        void Next();
        void Exit();
    }

    public abstract class BankSystem : IBankSystem
    {
        public BankAccount CurrentUser { get; set; } 

        protected void Clear(Action displayOptions)
        {
            Console.Clear();
            displayOptions();
        }

        protected int IntInput(string message)
        {
            while (true)
            {
                try
                {
                    Console.Write(message);
                    return int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                }
            }
        }

        protected double DoubleInput(string message)
        {
            while (true)
            {
                try
                {
                    Console.Write(message);
                    return double.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a valid double.");
                }
            }
        }

        public abstract void DisplayWelcomeScreen();
        public abstract void DisplayTransaction();
        public abstract void Next();
        public abstract void Exit();
    }

    public class EnglishATM : BankSystem
    {
        private ATM atm;

        public EnglishATM(ATM a)
        {
            atm = a;
        }

        public override void Exit()
        {
            Environment.Exit(0);
        }

        public override void Next()
        {
            Console.WriteLine("\nEnter For Next....");
            Console.ReadLine();
        }

        public override void DisplayTransaction()
        {
            DisplayOptions();
        }

        private void DisplayOptions()
        {
            Console.WriteLine("\tTransaction Options:");
            Console.WriteLine("1. Balance Inquiry");
            Console.WriteLine("2. Mini Statement");
            Console.WriteLine("3. Cheque Book Request");
            Console.WriteLine("4. Fast Cash");
            Console.WriteLine("5. Withdraw Cash");
            Console.WriteLine("6. Pin Change");
            Console.WriteLine("7. Fund Transfer");
            Console.WriteLine("8. Exit");

            int option = IntInput("Select an option: ");
            Process(option);
        }

        public void Process(int option)
        {
            switch (option)
            {
                case 1:
                    Clear(BalanceInquiry);
                    break;
                case 2:
                    Clear(MiniStatement);
                    break;
                case 3:
                    Clear(ChequeBookRequest);
                    break;
                case 4:
                    Clear(FastCash);
                    break;
                case 5:
                    Clear(WithdrawCash);
                    break;
                case 6:
                    Clear(PinChange);
                    break;
                case 7:
                    Clear(FundTransfer);
                    break;
                case 8:
                    Exit();
                    break;
                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }

        private void BalanceInquiry()
        {
            Console.WriteLine($"\n \tBalance: ${CurrentUser.Balance}");
            Next();
            DisplayTransaction();
        }

        private void MiniStatement()
        {
            Console.WriteLine($"Mini Statement for {CurrentUser.UserName}:");

            foreach (var record in CurrentUser.MiniStatement)
            {
                Console.WriteLine(record);
            }

            Next();
            DisplayTransaction();

        }

        private void ChequeBookRequest()
        {
            Console.WriteLine("\tCheque Book Request:");
            Console.WriteLine("Your cheque book request has been received.");
            Console.WriteLine("It will be delivered to your registered address soon.");
            Console.ReadLine();

            DisplayTransaction();
        }

        private void FastCash()
        {
            double[] fastCashOptions = { 20, 40, 60, 100 };

            Console.WriteLine("\tFast Cash Options:");
            for (int i = 0; i < fastCashOptions.Length; i++)
            {
                Console.WriteLine($" {i + 1}.${fastCashOptions[i]}");
            }

            int selection = IntInput("Select a Fast Cash option: ");

            if (selection >= 1 && selection <= fastCashOptions.Length)
            {
                double Amount = fastCashOptions[selection - 1];
                FastCashWithdraw(Amount);
            }
            else
            {
                Console.WriteLine("Invalid option selected...");
                Clear(DisplayTransaction);
            }
            Console.Write("\nDo you want to perform another transaction? (Y/N): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            if (choice == 'Y')
            {
                Console.ReadLine();
                Clear(FastCash);
            }
        }

        private void FastCashWithdraw(double Amount)
        {
            if (Amount > 0 && Amount <= CurrentUser.Balance)
            {
                CurrentUser.Balance -= Amount;
                Console.WriteLine($"Dispensing Cash: ${Amount}");
                Console.WriteLine($"Remaining Balance: ${CurrentUser.Balance}");

                CurrentUser.AddToMiniStatement($"Withdrawal Fast Cash: ${Amount}, Remaining Balance: ${CurrentUser.Balance}");
            }
            else
            {
                Console.WriteLine("Invalid amount or insufficient balance.");
            }

            Console.Write("\nDo you want to perform another transaction? (Y/N): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);

            if (choice == 'Y')
            {
                Console.ReadLine();
                Clear(FastCash);
            }

            Next();
            DisplayTransaction();
        }

        private void WithdrawCash()
        {
            double amount = DoubleInput("Enter Withdrawal amount: ");
            CashWithdrawal(amount);
        }

        private void CashWithdrawal(double amount)
        {
            if (amount > 0 && amount <= CurrentUser.Balance)
            {
                CurrentUser.Balance -= amount;
                Console.WriteLine($"Dispensing Cash: ${amount}");
                Console.WriteLine($"Remaining Balance: ${CurrentUser.Balance}");

                CurrentUser.AddToMiniStatement($"Withdrawal: ${amount}, Remaining Balance: ${CurrentUser.Balance}");
            }
            else
            {
                Console.WriteLine("Invalid amount or insufficient balance.");
            }

            Console.Write("\nDo you want to perform another transaction? (Y/N): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            if (choice == 'Y')
            {
                Console.ReadLine();
                Clear(WithdrawCash);
            }

            Next();
            DisplayTransaction();
        }

        private void PinChange()
        {
            Console.Write("Enter new PIN: ");
            string newPin = Console.ReadLine();

            CurrentUser.ChangePin(newPin);
            Console.WriteLine("Pin Change: Your PIN has been changed successfully.");

            Next();
            Clear(DisplayTransaction);
        }



        private void FundTransfer()
        {
            Console.WriteLine("Fund Transfer Options:");
            Console.WriteLine("1. Within the Bank");
            Console.WriteLine("2. Other Bank");
            Console.WriteLine("3. Exit");

            int fundTransfer = IntInput("Select Fund Transfer option: ");

            switch (fundTransfer)
            {
                case 1:
                    Clear(WithinBank);
                    break;
                case 2:
                    Clear(OtherBank);
                    break;
                case 3:
                    Exit();
                    break;
                default:
                    Console.WriteLine("Invalid option selected for Fund Transfer.");
                    break;
            }

            CurrentUser.AddToMiniStatement("Fund Transfer");
        }

        private void WithinBank()
        {
            Console.WriteLine("\n\tEnter recipient's account number within the bank: ");
            int recipientAccNum = IntInput("Recipient's Account Number: ");

            BankAccount recipientAccount = atm.GetUserByAccountNumber(recipientAccNum);

            if (recipientAccount != null)
            {
                double transferAmount = DoubleInput("Enter transfer amount: ");

                if (transferAmount > 0 && transferAmount <= CurrentUser.Balance)
                {
                    CurrentUser.Balance -= transferAmount;
                    recipientAccount.Balance += transferAmount;

                    Console.WriteLine($"Fund Transfer within the bank successful. Remaining Balance: ${CurrentUser.Balance}");

                    CurrentUser.AddToMiniStatement($"Within-Bank Fund Transfer: ${transferAmount}, Remaining Balance: ${CurrentUser.Balance}");
                }
                else
                {
                    Console.WriteLine("Invalid amount or insufficient balance.");
                }
            }
            else
            {
                Console.WriteLine("Recipient account not found within the bank.");
            }

            Console.Write("\nDo you want to perform another transaction? (Y/N): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);

            if (choice == 'Y')
            {
                Console.ReadLine();
                Clear(WithinBank);
            }

            Next();
            DisplayTransaction();
        }

        private void OtherBank()
        {
            Console.Write("Enter recipient's bank name: ");
            string bankName = Console.ReadLine();

            int accountNumber = IntInput("Enter recipient's account number in the other bank: ");
            double amount = DoubleInput("Enter the Amount for Transfer: ");

            if (amount > 0 && amount <= CurrentUser.Balance)
            {
                CurrentUser.Balance -= amount;

                Console.WriteLine($"Fund Transfer other bank successful. Remaining Balance: ${CurrentUser.Balance}");

                CurrentUser.AddToMiniStatement($"Other-Bank Fund Transfer: ${amount}, Remaining Balance: ${CurrentUser.Balance}");

                Console.WriteLine($"\n\nFund Transfer to {bankName} account Number {accountNumber}. Transfer amount is ${amount} ");
            }
            else
            {
                Console.WriteLine("Invalid amount or insufficient balance.");
            }


            Console.Write("\nDo you want to perform another transaction? (Y/N): ");
            char choice = char.ToUpper(Console.ReadKey().KeyChar);
            if (choice == 'Y')
            {
                Console.ReadLine();
                Clear(OtherBank);
            }

            Next();
            DisplayTransaction();
        }

        public override void DisplayWelcomeScreen()
        {
            Console.WriteLine("\tWelcome!");
            Console.Write("Enter your Account Number:");
            string userInput = Console.ReadLine();

            BankAccount user = atm.GetUserByInput(userInput, atm.GetUserAccounts());

            if (user != null)
            {
                CurrentUser = user;
                Console.WriteLine("Account Number matched!");
                DisplayPinEntry();
            }
            else
            {
                Console.WriteLine("Your account number is not matched. Try again...");
                Console.ReadLine();
                Clear(DisplayWelcomeScreen);
            }
        }

        private void DisplayPinEntry()
        {
            Console.Write("Enter your PIN: ");
            string enteredPIN = Console.ReadLine();

            if (ValidatePIN(enteredPIN))
            {
                Console.WriteLine("PIN is correct!");

                Next();
                Clear(DisplayTransaction);
            }
            else
            {

                Console.WriteLine("Incorrect PIN. Try again...");
                Console.ReadLine();
                DisplayPinEntry();
            }
        }

        private bool ValidatePIN(string enteredPIN)
        {
            return enteredPIN == CurrentUser?.Pin;
        }
    }

    public enum TransactionType
    {
        BalanceInquiry,
        MiniStatement,
        ChequeBookRequest,
        FastCash,
        WithdrawCash,
        PinChange,
        FundTransfer,
        Exit
    }

    public class Transaction
    {
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
    }

    public class ATM
    {
        private List<BankAccount> userAccounts;
        private BankSystem currentATM;

        public ATM()
        {
            userAccounts = new List<BankAccount>
            {
                new BankAccount("Taimoor", 12345678, "1234", 43000.00),
                new BankAccount("Tahir", 23456789, "5678", 9000.00),
                new BankAccount("Tanveer", 34567890, "9012", 20400.00)
            };

            currentATM = new EnglishATM(this);
        }

        public void RunATM()
        {
            DisplayWelcomeScreen();
            DisplayTransactionOptions();
        }

        public List<BankAccount> GetUserAccounts()
        {
            return userAccounts;
        }

        public BankAccount GetUserByInput(string input, List<BankAccount> userAccounts)
        {
            return userAccounts.Find(account =>  account.AccountNumber.ToString() == input);
        }

        public BankAccount GetUserByAccountNumber(int accountNumber)
        {
            return userAccounts.Find(account => account.AccountNumber == accountNumber);
        }

        private void DisplayWelcomeScreen()
        {
            currentATM.DisplayWelcomeScreen();
        }

        private void DisplayTransactionOptions()
        {
            currentATM.DisplayTransaction();
        }

        public void Exit()
        {
            Environment.Exit(0);
        }
    }

    public class BankAccount
    {
        public string UserName { get; set; }
        public int AccountNumber { get; set; }
        public string Pin { get; set; }
        public double Balance { get; set; }

        private string filePath;
        private string pinFilePath;

        public List<string> MiniStatement = new List<string>();

        public BankAccount(string name, int accNumber, string pin, double balance)
        {
            UserName = name;
            AccountNumber = accNumber;
            Pin = pin;
            Balance = balance;
            filePath = @$"E:\MiniStatement_{UserName}.txt";
            pinFilePath = @$"E:\Pin_{UserName}.txt";

            LoadMiniStatementFromFile();
            LoadPinFromFile(); 
        }

        public void AddToMiniStatement(string transactionRecord)
        {
            MiniStatement.Add(transactionRecord);
            SaveMiniStatementToFile();
        }

        public void ChangePin(string newPin)
        {
            Pin = newPin;
            SavePinToFile();
        }

        private void LoadMiniStatementFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            MiniStatement.Add(streamReader.ReadLine());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mini-statement from file: {ex.Message}");
            }
        }

        private void SaveMiniStatementToFile()
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var record in MiniStatement)
                    {
                        streamWriter.WriteLine(record);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving mini-statement to file: {ex.Message}");
            }
        }

        private void LoadPinFromFile()
        {
            try
            {
                if (File.Exists(pinFilePath))
                {
                    using (StreamReader streamReader = new StreamReader(pinFilePath))
                    {
                        Pin = streamReader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading PIN from file: {ex.Message}");
            }
        }

        private void SavePinToFile()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(pinFilePath))
                {
                    streamWriter.WriteLine(Pin);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving PIN to file: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            ATM atm = new ATM();
            atm.RunATM();
        }
    }
}

