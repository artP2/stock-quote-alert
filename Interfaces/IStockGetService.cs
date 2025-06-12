/// <summary>
/// Exception thrown when a stock is not found in the API.
/// </summary>
public class StockNotFoundException : Exception {
	public StockNotFoundException(string stockName) : base($"{stockName} nao foi encontrado na API.") {}
}

public interface IStockGetService {
    /// <summary>
    /// Asynchronously fetches the current stock price from the API.
    /// </summary>
    /// <param name="stockName">The stock to get the price.</param>
    /// <returns>The current stock price as a decimal.</returns>
    Task<decimal> GetPriceAsync(string stockName);
}
