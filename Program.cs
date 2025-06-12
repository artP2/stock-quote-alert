using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;

/// <summary>
/// Main class that orchestrates stock monitoring.
/// </summary>
public class StockQuoteAlert {
	/// <summary>
	/// Asynchronously monitors the price of a stock, sending email alerts when buy or sell prices are reached.
	/// </summary>
	/// <param name="stock">The stock object to be monitored.</param>
	/// <param name="stockGetService">The service for getting stock data.</param>
	/// <param name="emailService">The service for sending emails.</param>
	/// <param name="config">The applcation configuration data.</param>
	public static async Task MonitorStockAsync(
		Stock stock,
		IStockGetService stockGetService,
		IEmailService emailService,
		Config config
	){
		Console.WriteLine($"[Monitorando {stock.StockName}]");
		Console.WriteLine($"Venda em: {stock.SellPrice}\nCompra em: {stock.BuyPrice}\n");
		while (true){
			try {
				Console.WriteLine($"[{stock.StockName}] Verificando preço...");
				StockAction action = await stock.GetStockActionAsync(stockGetService);

				if (action is Buy or Sell){
					var (actionText, currPrice) = action switch {
						Buy b => ("comprar", b.Price),
						Sell s => ("vender", s.Price),
						_ => throw new InvalidOperationException()
					};
					Console.WriteLine($"[{stock.StockName}] AVISO! Açao recomendada: {actionText}!");

					var subject = $"Alerta de Preço para {actionText} a Açao {stock.StockName}";
					var body =
						$"Este e um alerta para {actionText} a açao {stock.StockName}.\n\n" +
						$"Preço atual: {currPrice};\n\n" +
						$"Preço de venda: {stock.SellPrice};\n" +
						$"Preço de compra: {stock.BuyPrice};";
					await emailService.SendAsync(config.TargetEmail, subject, body);
					Console.WriteLine($"[{stock.StockName}] Email de alerta enviado com sucesso!");
				} else {
					Console.WriteLine($"[{stock.StockName}] Nenhuma açao necessaria.");
				}
			} catch (StockNotFoundException e) {
				Console.WriteLine($"[{stock.StockName}] ERRO: " + e.Message);
				Console.WriteLine($"[{stock.StockName}] ENCERRANDO O MONITORAMENTO...");
				return;
			} catch (Exception e){
				Console.WriteLine($"[{stock.StockName}] ERRO: " + e.Message);
			} finally {
				await Task.Delay(TimeSpan.FromMinutes(config.MonitorIntervalMinutes));
			}
		}
	}

	/// <summary>
	/// Main entry method of the program.
	/// </summary>
	/// <param name="args">Command-line arguments: [stock_name] [sell_price] [buy_price]</param>
	public static async Task Main(string[] args) {
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) => {
				services.Configure<Config>(context.Configuration.GetSection("Config"));
                services.AddSingleton<IEmailService, EmailService>(provider => {
                    var config = provider.GetRequiredService<IOptions<Config>>().Value;
                    return new EmailService(config);
                });
				services.AddSingleton<IStockGetService, LeDevStockGetService>();
				services.AddHttpClient<IStockGetService, LeDevStockGetService>(client=>{
					client.BaseAddress = new Uri($"https://ledev.com.br/api/cotacoes/");	
					client.Timeout = TimeSpan.FromSeconds(10);
				});
		}).Build();
		try {
	        var config = host.Services.GetRequiredService<IOptions<Config>>().Value;
	        var emailService = host.Services.GetRequiredService<IEmailService>();
			var stockGetService = host.Services.GetRequiredService<IStockGetService>();

			Console.WriteLine($"Email de destino: {config.TargetEmail}\n");

			// processes the command-line arguments in chunks of 3
			// for each chunk, it creates a Stock object and starts a monitoring task
			List<Task> monitorTasks = args
				.Where(arg => !arg.StartsWith("--")) // ignore config args
				.Chunk(3)
				.Select(stockArgs => Stock.Parse(stockArgs))
				.Select(stock => MonitorStockAsync(stock, stockGetService, emailService, config))
				.ToList();

			// awaits the completion of all monitoring tasks
			await Task.WhenAll(monitorTasks);

		} catch (Exception e) {
			Console.WriteLine("ERRO: " + e.Message);
		}
	}
} 
