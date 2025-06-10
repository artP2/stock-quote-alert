using System.Text.Json;
using System.Net.Mail;
using System.Net;

/// <summary>
/// Exception thrown when arguments are invalid.
/// </summary>
public class ArgsParseException : Exception {
    public ArgsParseException(string message)
        : base($"ERRO: {message}\n\nComo usar: stock-quote-alert [ativo] [preco_de_venda] [preco_de_compra]")
    {}
}

/// <summary>
/// Represents a stock with its monitoring parameters.
/// </summary>
public record Stock (
	string StockName,
	decimal SellPrice,
	decimal BuyPrice
) {
	/// <summary>
	/// Parses the arguments and creates a Stock instance.
	/// </summary>
	/// <param name="args">A string array with the name, sell price, and buy price.</param>
	/// <returns>A new Stock instance.</returns>
	static public Stock Parse(string[] args) {
		if (args.Length != 3) {
			throw new ArgsParseException("O numero de argumentos esta incorreto.");
		}
		try {
			var stock = new Stock(args[0], Decimal.Parse(args[1]), Decimal.Parse(args[2]));
			return stock;
		} catch {
			throw new ArgsParseException("Os preços devem ser numeros.");
		}
	}

	// static http client for reuse
	private static HttpClient client = new() {
		BaseAddress = new Uri($"https://ledev.com.br/api/cotacoes/"),	
		Timeout = TimeSpan.FromSeconds(10),
	};

	/// <summary>
	/// Asynchronously fetches the current stock price from the API.
	/// </summary>
	/// <returns>The current stock price as a decimal.</returns>
	public async Task<decimal> GetPriceAsync() {
		try {
			using HttpResponseMessage response  = await client.GetAsync($"{StockName}");

			response.EnsureSuccessStatusCode();
    
		    var jsonResponse = await response.Content.ReadAsStringAsync();
		    Console.WriteLine($"{jsonResponse}\n");

			var jsonData = JsonDocument.Parse(jsonResponse);
			var priceString = jsonData.RootElement.GetProperty("price").GetString();

			if (priceString == null) {
				throw new Exception("null");
			}
	        var price = Decimal.Parse(priceString, System.Globalization.CultureInfo.InvariantCulture);
			return price;
		} catch (Exception e) {
			throw e;
		}
	}
};

/// <summary>
/// Exception thrown when an error occurs while trying to send an email.
/// </summary>
public class SendEmailException : Exception {
	public SendEmailException(string? message) : base(message) { }
};

/// <summary>
/// Main class that orchestrates stock monitoring.
/// </summary>
public class StockQuoteAlert {
	/// <summary>
	/// Sends an email.
	/// </summary>
	/// <param name="config">The config with email service data.</param>
	/// <param name="subject">The email's subject.</param>
	/// <param name="body">The email's body.</param>
	void SendEmail(Config config, string subject, string body){
		try {
			var client = new SmtpClient(config.SmtpServer, config.SmtpPort){
				Credentials = new NetworkCredential(config.SmtpUser, config.SmtpPassword)	
			};
			var message = new MailMessage(config.SmtpUser, config.TargetEmail, subject, body);

			client.Send(message);
		} catch {
			throw new SendEmailException("ERRO: Nao foi possivel enviar o email.");
		}
	}

	/// <summary>
	/// Main entry method of the program.
	/// </summary>
	/// <param name="args">Command-line arguments: [stock_name] [sell_price] [buy_price]</param>
	public static void Main(string[] args) {
		try {
			Console.WriteLine("Carregando configuraçao...");
			Config config = Config.Load();
			Console.WriteLine("Configuraçao carregada!");
			Console.WriteLine($"Email de destino: {config.TargetEmail}");
			Stock stock = Stock.Parse(args);
			Console.WriteLine(
				$"Ativo: {stock.StockName}\nVenda em: {stock.SellPrice}\nCompra em: {stock.BuyPrice}"
			);
		} catch (SendEmailException e) {
			Console.WriteLine(e.Message);
		} catch (Exception e) {
			Console.WriteLine(e.Message);
			Environment.Exit(1);
		}
	}
} 
