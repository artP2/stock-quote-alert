using System.Net.Mail;
using System.Net;

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
