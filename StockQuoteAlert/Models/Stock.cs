/// <summary>
/// Exception thrown when arguments are invalid.
/// </summary>
public class ArgsParseException : Exception {
    public ArgsParseException(string message)
        : base($"{message}\n\nComo usar: stock-quote-alert [ativo] [preco_de_venda] [preco_de_compra]")
    {}
}

/// <summary>
/// Defines a base class to represent the possible monitoring actions.
/// Used as a discriminated union.
/// </summary>
public abstract record StockAction;

/// <summary>
/// Represents a Sell recommendation at a given price.
/// </summary>
public sealed record Sell(decimal Price) : StockAction;

/// <summary>
/// Represents a Buy recommendation at a given price.
/// </summary>
public sealed record Buy(decimal Price) : StockAction;

/// <summary>
/// Represents the absence of a recommendation.
/// </summary>
public sealed record DoNothing : StockAction;

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

	/// <summary>
	/// Determines the action to be taken based on the current price.
	/// </summary>
	/// <param name="price">The current stock price.</param>
	/// <returns>A StockAction object (Sell, Buy, or DoNothing).</returns>
	StockAction StockActionFromPrice(decimal price) =>
		price > SellPrice ? new Sell(price) :
		price < BuyPrice ? new Buy(price) :
		new DoNothing();

	/// <summary>
	/// Orchestrates fetching the price and determining the recommended action.
	/// </summary>
	/// <param name="getService">The service to get stock price.</param>
	/// <returns>A StockAction object representing the action to be taken.</returns>
	public async Task<StockAction> GetStockActionAsync(IStockGetService getService){
		decimal price = await getService.GetPriceAsync(StockName);
		return StockActionFromPrice(price);
	}
};
