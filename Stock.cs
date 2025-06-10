using System.Text.Json;

/// <summary>
/// Exception thrown when arguments are invalid.
/// </summary>
public class ArgsParseException : Exception {
    public ArgsParseException(string message)
        : base($"ERRO: {message}\n\nComo usar: stock-quote-alert [ativo] [preco_de_venda] [preco_de_compra]")
    {}
}

public enum StockAction {
	Sell,
	DoNothing,
	Buy
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

	/// <summary>
	/// Orchestrates fetching the price and determining the recommended action.
	/// </summary>
	/// <returns>A StockAction enum representing the action to be taken.</returns>
	public async Task<StockAction> GetStockActionAsync(){
		decimal price = await GetPriceAsync();

		if (price > SellPrice) {
			return StockAction.Sell;
		}
		if (price < BuyPrice) {
			return StockAction.Buy;
		}
		return StockAction.DoNothing;
	}
};
