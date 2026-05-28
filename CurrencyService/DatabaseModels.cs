using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CurrencyService
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double PlnBalance { get; set; }
        public List<CurrencyBalance> CurrencyBalances { get; set; } = new List<CurrencyBalance>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class CurrencyBalance
    {
        public string CurrencyCode { get; set; }
        public double Amount { get; set; }
    }

    public class Transaction
    {
        public string CurrencyCode { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public double Rate { get; set; }
        public DateTime Date { get; set; }
    }

    public static class Database
    {
        private static string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "ExchangeDb.json");

        private static List<User> Load()
        {
            if (!File.Exists(DbPath))
                return new List<User>();
            return JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(DbPath))
                   ?? new List<User>();
        }

        private static void Save(List<User> users)
        {
            File.WriteAllText(DbPath, JsonConvert.SerializeObject(users, Formatting.Indented));
        }

        public static void Initialize()
        {
            if (!File.Exists(DbPath))
                Save(new List<User>());
        }

        public static User GetUser(string username)
        {
            return Load().Find(u => u.Username == username);
        }

        public static bool RegisterUser(string username, string password)
        {
            var users = Load();
            if (users.Find(u => u.Username == username) != null)
                return false;
            int newId = users.Count > 0 ? users[users.Count - 1].Id + 1 : 1;
            users.Add(new User { Id = newId, Username = username, Password = password, PlnBalance = 0 });
            Save(users);
            return true;
        }

        public static void UpdateBalance(int userId, double newBalance)
        {
            var users = Load();
            var user = users.Find(u => u.Id == userId);
            if (user != null) user.PlnBalance = newBalance;
            Save(users);
        }

        public static double GetCurrencyBalance(int userId, string code)
        {
            var user = Load().Find(u => u.Id == userId);
            if (user == null) return 0;
            var bal = user.CurrencyBalances.Find(b => b.CurrencyCode == code);
            return bal != null ? bal.Amount : 0;
        }

        public static void SetCurrencyBalance(int userId, string code, double amount)
        {
            var users = Load();
            var user = users.Find(u => u.Id == userId);
            if (user == null) return;
            var bal = user.CurrencyBalances.Find(b => b.CurrencyCode == code);
            if (bal == null)
                user.CurrencyBalances.Add(new CurrencyBalance { CurrencyCode = code, Amount = amount });
            else
                bal.Amount = amount;
            Save(users);
        }

        public static void AddTransaction(int userId, string code, string type, double amount, double rate)
        {
            var users = Load();
            var user = users.Find(u => u.Id == userId);
            if (user == null) return;
            user.Transactions.Add(new Transaction
            {
                CurrencyCode = code,
                Type = type,
                Amount = amount,
                Rate = rate,
                Date = DateTime.Now
            });
            Save(users);
        }

        public static List<CurrencyBalance> GetAllBalances(int userId)
        {
            var user = Load().Find(u => u.Id == userId);
            return user != null ? user.CurrencyBalances : new List<CurrencyBalance>();
        }

        public static List<Transaction> GetTransactions(int userId)
        {
            var user = Load().Find(u => u.Id == userId);
            if (user == null) return new List<Transaction>();
            var list = new List<Transaction>(user.Transactions);
            list.Sort((a, b) => b.Date.CompareTo(a.Date));
            return list;
        }
    }
}