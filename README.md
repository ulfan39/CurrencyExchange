# Currency Exchange Office System

**Course:** Network Application Development
**Author:** Ulfan Ibrahimob
**Student ID:** \[64287]

## Project Description

A network-based currency exchange office system built with **Windows Communication Foundation (WCF)** on the .NET Framework. The system retrieves real exchange rates from the **National Bank of Poland (NBP) API** and allows users to register accounts, manage their balance, and buy or sell currencies through a WPF desktop client.

Main functionality:

* **Live exchange rates** — fetches current rates (USD, EUR, GBP, etc.) from the NBP public API
* **Historical exchange rates** — query rates for any past date
* **User accounts** — registration and account management
* **Virtual wallet** — top up PLN balance (simulated transfer)
* **Currency trading** — buy and sell foreign currencies at live rates
* **Transaction history** — full log of all buy/sell operations
* **Data persistence** — all user data, balances, and transactions stored in a JSON database

## Architecture

The solution follows a Service-Oriented Architecture (SOA) and consists of four projects:

|Project|Type|Role|
|-|-|-|
|`CurrencyService`|Class Library|WCF service contract, business logic, and data layer|
|`CurrencyHost`|Console App|Hosts the WCF service on `http://localhost:8080/CurrencyService`|
|`CurrencyWpfClient`|WPF App|Graphical user interface for end users|
|`CurrencyClient`|Console App|Simple console client used for testing|

**Communication:** Clients connect to the service over HTTP/SOAP using `BasicHttpBinding`.

## Technologies

* .NET Framework 4.7.2
* Windows Communication Foundation (WCF)
* Windows Presentation Foundation (WPF)
* Newtonsoft.Json (JSON serialization \& API parsing)
* NBP REST API (exchange rate data)

## Service Operations

|Method|Description|
|-|-|
|`GetExchangeRate(code)`|Returns the current rate for a currency|
|`GetHistoricalRate(code, date)`|Returns the rate for a specific past date|
|`RegisterUser(username, password)`|Creates a new user account|
|`TopUpBalance(username, amount)`|Adds PLN funds to an account|
|`GetBalance(username)`|Returns PLN and currency balances|
|`BuyCurrency(username, code, amount)`|Buys foreign currency using PLN|
|`SellCurrency(username, code, amount)`|Sells foreign currency for PLN|
|`GetTransactionHistory(username)`|Returns the user's transaction log|

## How to Run

> \*\*Note:\*\* The service host must run with administrator privileges to bind to port 8080.

1. Open the solution in **Visual Studio (run as Administrator)**.
2. Build the solution: **Build → Rebuild Solution**.
3. Start the service host. Either run it from Visual Studio, or from an **administrator command prompt**:

```
   cd /d "...\\CurrencyExchange\\CurrencyHost\\bin\\x64\\Debug"
   CurrencyHost.exe
   ```

   Wait for the message `Service is running at: http://localhost:8080/CurrencyService`.

4. Run the **CurrencyWpfClient** project (Ctrl+F5).
5. In the client window:

   * Enter a username and password, then click **Register**.
   * Use **Top Up** to add PLN funds to your account.
   * Click **USD / EUR / GBP** or enter a code and click **Get Rate** to view exchange rates.
   * Use **Buy** / **Sell** to trade currencies.
   * Click **History** to view your past transactions.

   > An active internet connection is required for live exchange rate functionality.

   ## Database

   Data is persisted to a JSON file (`ExchangeDb.json`) in the application directory. Stored entities:

* **User** — Id, Username, Password, PlnBalance
* **CurrencyBalance** — CurrencyCode, Amount (per user)
* **Transaction** — CurrencyCode, Type, Amount, Rate, Date (per user)

