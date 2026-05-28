using System;
using System.ServiceModel;
using System.Windows;
using CurrencyService;

namespace CurrencyWpfClient
{
    public partial class MainWindow : Window
    {
        private ICurrencyService GetClient()
        {
            try
            {
                EndpointAddress address = new EndpointAddress("http://localhost:8080/CurrencyService");
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = 10000000;
                ChannelFactory<ICurrencyService> factory = new ChannelFactory<ICurrencyService>(binding, address);
                return factory.CreateChannel();
            }
            catch (Exception ex)
            {
                ResultBox.Text += "Connection error: " + ex.Message + "\n";
                return null;
            }
        }

        private string GetUsername() => UsernameInput.Text.Trim();
        private string GetPassword() => PasswordInput.Password.Trim();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += client.RegisterUser(GetUsername(), GetPassword()) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void TopUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                if (double.TryParse(TopUpInput.Text, out double amount))
                    ResultBox.Text += client.TopUpBalance(GetUsername(), amount) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void Balance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += "--- Balance ---\n" + client.GetBalance(GetUsername()) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void GetRate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += client.GetExchangeRate(CurrencyInput.Text.Trim()) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void USD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += client.GetExchangeRate("USD") + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void EUR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += client.GetExchangeRate("EUR") + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void GBP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += client.GetExchangeRate("GBP") + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                if (double.TryParse(TradeAmountInput.Text, out double amount))
                    ResultBox.Text += client.BuyCurrency(GetUsername(), TradeCodeInput.Text.Trim(), amount) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                if (double.TryParse(TradeAmountInput.Text, out double amount))
                    ResultBox.Text += client.SellCurrency(GetUsername(), TradeCodeInput.Text.Trim(), amount) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                ResultBox.Text += "--- History ---\n" + client.GetTransactionHistory(GetUsername()) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void Historical_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = GetClient();
                if (client == null) return;
                string code = CurrencyInput.Text.Trim();
                string date = DateInput.Text.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    ResultBox.Text += "Please enter a currency code!\n";
                    return;
                }
                ResultBox.Text += client.GetHistoricalRate(code, date) + "\n";
            }
            catch (Exception ex) { ResultBox.Text += "Error: " + ex.Message + "\n"; }
        }

        private void CurrencyInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}