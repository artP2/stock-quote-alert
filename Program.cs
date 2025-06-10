/// <summary>
/// Main class that orchestrates stock monitoring.
/// </summary>
public class StockQuoteAlert {
	/// <summary>
	/// Asynchronously monitors the price of a stock, sending email alerts when buy or sell prices are reached.
	/// </summary>
	/// <param name="stock">The stock object to be monitored.</param>
	/// <param name="emailService">The service for sending emails.</param>
	/// <param name="targetEmail">The target email for the alerts.</param>
	public static async Task MonitorStockAsync(Stock stock, Email emailService, string targetEmail){
		Console.WriteLine($"\n[Monitorando {stock.StockName}]");
		Console.WriteLine($"Venda em: {stock.SellPrice}\nCompra em: {stock.BuyPrice}");
		while (true){
			try {
				StockAction action = await stock.GetStockActionAsync();

				if (action == StockAction.DoNothing){
					Console.WriteLine("Nenhuma açao necessaria. Proxima verificaçao em 1 minuto.");
				} else {
					string actionText = action == StockAction.Buy ? "comprar" : "vender";
					Console.WriteLine($"AVISO! Açao recomendada: {actionText}!");

					var subject = $"Alerta de Preço para a Açao {stock.StockName}";
					var body = $"Este e um alerta para {actionText} a açao {stock.StockName}.\n";
					emailService.Send(targetEmail, subject, body);
					Console.WriteLine("Email de alerta enviado com sucesso!");
				}
			}
			catch (Exception e){
				throw e;
			} finally {
				await Task.Delay(TimeSpan.FromMinutes(1));
			}
		}
	}

	/// <summary>
	/// Main entry method of the program.
	/// </summary>
	/// <param name="args">Command-line arguments: [stock_name] [sell_price] [buy_price]</param>
	public static async Task Main(string[] args) {
		try {
			Console.WriteLine("Carregando configuraçao...");
			Config config = Config.Load();
			Console.WriteLine("Configuraçao carregada!");

			Console.WriteLine($"Email de destino: {config.TargetEmail}");
			var emailService = new Email(config);

			Stock stock = Stock.Parse(args);
			await MonitorStockAsync(stock, emailService, config.TargetEmail);

		} catch (SendEmailException e) {
			Console.WriteLine(e.Message);
		} catch (Exception e) {
			Console.WriteLine(e.Message);
			Environment.Exit(1);
		}
	}
} 
