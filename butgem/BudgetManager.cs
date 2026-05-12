using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace butgem
{
    public class BudgetManager
    {
        public List<Transaction> Transactions { get; private set; }
        private static string filePath = "transactions.txt";

        public decimal TotalBudget
        {
            get { return Transactions.Sum(t => t.Type == TransactionType.Доход ? t.Amount : -t.Amount); }
        }

        public BudgetManager()
        {
            Transactions = new List<Transaction>();
            LoadTransactions();
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            SaveTransactions();
        }

        public void RemoveTransaction(Transaction transaction)
        {
            Transactions.Remove(transaction);
            SaveTransactions();
        }

        public void UpdateTransaction(Transaction transaction, string newDescription, decimal newAmount, TransactionType newType)
        {
            transaction.Description = newDescription;
            transaction.Amount = newAmount;
            transaction.Type = newType;
            SaveTransactions();
        }

        private void SaveTransactions()
        {
            var lines = Transactions.Select(t =>
                $"{t.Description}|{t.Amount}|{(int)t.Type}|{t.Date:yyyy-MM-dd HH:mm:ss}");
            File.WriteAllLines(filePath, lines);
        }

        private void LoadTransactions()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4 &&
                        decimal.TryParse(parts[1], out decimal amount) &&
                        int.TryParse(parts[2], out int typeInt) &&
                        DateTime.TryParse(parts[3], out DateTime date))
                    {
                        Transactions.Add(new Transaction(parts[0], amount, (TransactionType)typeInt, date));
                    }
                }
            }
        }
    }
}