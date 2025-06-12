using System.Text.Json;

public class LeDevStockGetService : IStockGetService {
	private readonly HttpClient _httpClient;

	/// <summary>
    /// Inicializa uma nova instância da classe <see cref="LeDevStockGetService"/>.
    /// </summary>
    /// <param name="httpClient">O cliente HTTP para fazer requisições à API.</param>
    public LeDevStockGetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Asynchronously fetches the current stock price from the API.
    /// </summary>
    /// <param name="stockName">The stock to get the price.</param>
    /// <returns>The current stock price as a decimal.</returns>
    public async Task<decimal> GetPriceAsync(string stockName) {
		try {
			using HttpResponseMessage response  = await _httpClient.GetAsync($"{stockName}");

			// check the get status
			switch (response.StatusCode) {
				case System.Net.HttpStatusCode.NotFound : throw new StockNotFoundException(stockName);
				case System.Net.HttpStatusCode.OK: break;
				default: throw new Exception();
			}
    
		    var jsonResponse = await response.Content.ReadAsStringAsync();

			var jsonData = JsonDocument.Parse(jsonResponse);
			var priceString = jsonData.RootElement.GetProperty("price").GetString();

			if (priceString == null) {
				throw new Exception();
			}
	        var price = Decimal.Parse(priceString, System.Globalization.CultureInfo.InvariantCulture);
			return price;
		} catch (StockNotFoundException) {
			throw;
		} catch {
			throw new Exception($"Nao foi possivel consultar o preço de [{stockName}]");
		}
	}

}
