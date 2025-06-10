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
};

/// <summary>
/// Main class that orchestrates stock monitoring.
/// </summary>
public class StockQuoteAlert {
	/// <summary>
	/// Main entry method of the program.
	/// </summary>
	/// <param name="args">Command-line arguments: [stock_name] [sell_price] [buy_price]</param>
	public static void Main(string[] args) {
		try {
			Stock stock = Stock.Parse(args);
			Console.WriteLine(
				$"Ativo: {stock.StockName}\nVenda em: {stock.SellPrice}\nCompra em: {stock.BuyPrice}"
			);
		} catch (Exception e) {
			Console.WriteLine(e.Message);
		}
	}
} 
