using System;
using System.Net;
using System.ServiceModel;
using Newtonsoft.Json;

namespace CurrencyService
{
    [ServiceContract]
    public interface ICurrencyService
    {
        [OperationContract]
        string GetExchangeRate(string currencyCode);
        [OperationContract]
        string RegisterUser(string username, string password);
        [OperationContract]
        string TopUpBalance(string username, double amount);
        [OperationContract]
        string GetBalance(string username);
        [OperationContract]
        string BuyCurrency(string username, string currencyCode, double amount);
        [OperationContract]
        string SellCurrency(string username, string currencyCode, double amount);
        [OperationContract]
        string GetTransactionHistory(string username);
        [OperationContract]
        string GetHistoricalRate(string currencyCode, string date);
    }

    public class CurrencyServiceImpl : ICurrencyService
    {
        static CurrencyServiceImpl()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Database.Initialize();
        }

        private double GetRateFromNBP(string currencyCode)
        {
            string url = "http://api.nbp.pl/api/exchangerates/rates/a/" + currencyCode.ToLower() + "/";
            using (var wc = new WebClient())
            {
                wc.Headers.Add("Accept", "application/json");
                string response = wc.DownloadString(url);
                dynamic data = JsonConvert.DeserializeObject(response);
                return (double)data.rates[0].mid;
            }
        }

        public string GetExchangeRate(string currencyCode)
        {
            try
            {
                double rate = GetRateFromNBP(currencyCode);
                return currencyCode.ToUpper() + " rate: " + rate + " PLN";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string GetHistoricalRate(string currencyCode, string date)
        {
            try
            {
                string url = "http://api.nbp.pl/api/exchangerates/rates/a/"
                             + currencyCode.ToLower() + "/" + date + "/";
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("Accept", "application/json");
                    string response = wc.DownloadString(url);
                    dynamic data = JsonConvert.DeserializeObject(response);
                    double rate = (double)data.rates[0].mid;
                    return currencyCode.ToUpper() + " rate on " + date + ": " + rate + " PLN";
                }
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string RegisterUser(string username, string password)
        {
            try
            {
                if (Database.GetUser(username) != null)
                    return "Error: Username already exists!";
                Database.RegisterUser(username, password);
                return "User registered successfully!";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string TopUpBalance(string username, double amount)
        {
            try
            {
                var user = Database.GetUser(username);
                if (user == null) return "Error: User not found!";
                Database.UpdateBalance(user.Id, user.PlnBalance + amount);
                return "Balance topped up! New balance: " + (user.PlnBalance + amount) + " PLN";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string GetBalance(string username)
        {
            try
            {
                var user = Database.GetUser(username);
                if (user == null) return "Error: User not found!";
                string result = "PLN: " + user.PlnBalance + "\n";
                foreach (var bal in Database.GetAllBalances(user.Id))
                    result += bal.CurrencyCode + ": " + bal.Amount + "\n";
                return result;
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string BuyCurrency(string username, string currencyCode, double amount)
        {
            try
            {
                double rate = GetRateFromNBP(currencyCode);
                double cost = amount * rate;
                var user = Database.GetUser(username);
                if (user == null) return "Error: User not found!";
                if (user.PlnBalance < cost) return "Error: Insufficient PLN balance!";
                Database.UpdateBalance(user.Id, user.PlnBalance - cost);
                double current = Database.GetCurrencyBalance(user.Id, currencyCode.ToUpper());
                Database.SetCurrencyBalance(user.Id, currencyCode.ToUpper(), current + amount);
                Database.AddTransaction(user.Id, currencyCode.ToUpper(), "BUY", amount, rate);
                return "Bought " + amount + " " + currencyCode.ToUpper() + " at " + rate + " PLN. Cost: " + cost + " PLN";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string SellCurrency(string username, string currencyCode, double amount)
        {
            try
            {
                double rate = GetRateFromNBP(currencyCode);
                double earned = amount * rate;
                var user = Database.GetUser(username);
                if (user == null) return "Error: User not found!";
                double current = Database.GetCurrencyBalance(user.Id, currencyCode.ToUpper());
                if (current < amount) return "Error: Insufficient currency balance!";
                Database.SetCurrencyBalance(user.Id, currencyCode.ToUpper(), current - amount);
                Database.UpdateBalance(user.Id, user.PlnBalance + earned);
                Database.AddTransaction(user.Id, currencyCode.ToUpper(), "SELL", amount, rate);
                return "Sold " + amount + " " + currencyCode.ToUpper() + " at " + rate + " PLN. Earned: " + earned + " PLN";
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }

        public string GetTransactionHistory(string username)
        {
            try
            {
                var user = Database.GetUser(username);
                if (user == null) return "Error: User not found!";
                string result = "";
                foreach (var t in Database.GetTransactions(user.Id))
                    result += t.Date.ToString("yyyy-MM-dd HH:mm") + " | " + t.Type + " | " + t.Amount + " " + t.CurrencyCode + " @ " + t.Rate + " PLN\n";
                return string.IsNullOrEmpty(result) ? "No transactions yet." : result;
            }
            catch (Exception ex) { return "Error: " + ex.Message; }
        }
    }
}