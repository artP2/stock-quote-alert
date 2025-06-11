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
		Console.WriteLine($"[Monitorando {stock.StockName}]");
		Console.WriteLine($"Venda em: {stock.SellPrice}\nCompra em: {stock.BuyPrice}\n");
		while (true){
			try {
				StockAction action = await stock.GetStockActionAsync();

				if (action == StockAction.DoNothing){
					Console.WriteLine($"[{stock.StockName}] Nenhuma açao necessaria. Proxima verificaçao em 1 minuto.");
				} else {
					string actionText = action == StockAction.Buy ? "comprar" : "vender";
					Console.WriteLine($"[{stock.StockName}] AVISO! Açao recomendada: {actionText}!");

					var subject = $"Alerta de Preço para a Açao {stock.StockName}";
					var body = $"Este e um alerta para {actionText} a açao {stock.StockName}.\n";
					emailService.Send(targetEmail, subject, body);
					Console.WriteLine($"[{stock.StockName}] Email de alerta enviado com sucesso!");
				}
			} catch (StockNotFoundException e) {
				Console.WriteLine($"[{stock.StockName}] ERRO: " + e.Message);
				Console.WriteLine($"[{stock.StockName}] ENCERRANDO O MONITORAMENTO...");
				return;
			} catch (Exception e){
				Console.WriteLine($"[{stock.StockName}] ERRO: " + e.Message);
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

			Console.WriteLine($"Email de destino: {config.TargetEmail}\n");
			var emailService = new Email(config);

			// processes the command-line arguments in chunks of 3
			// for each chunk, it creates a Stock object and starts a monitoring task
			List<Task> monitorTasks = args.Chunk(3)
				.Select(stockArgs => Stock.Parse(stockArgs))
				.Select(stock => MonitorStockAsync(stock, emailService, config.TargetEmail))
				.ToList();

			// awaits the completion of all monitoring tasks
			await Task.WhenAll(monitorTasks);

		} catch (Exception e) {
			Console.WriteLine("ERRO: " + e.Message);
		}
	}
} 
