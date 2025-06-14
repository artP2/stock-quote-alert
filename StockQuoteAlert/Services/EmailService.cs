using System.Net.Mail;
using System.Net;

/// <summary>
/// Represents the email sending service.
/// </summary>
public class EmailService : IEmailService {
	private readonly SmtpClient client;
	private readonly string user;
	/// <summary>
	/// Constructor that initializes the email service from a configuration object.
	/// </summary>
	/// <param name="config">The config with email service settings.</param>
	public EmailService(Config config) {
		client = new SmtpClient(config.SmtpServer, config.SmtpPort){
			Credentials = new NetworkCredential(config.SmtpUser, config.SmtpPassword),	
			EnableSsl = true
		};
		user = config.SmtpUser;
	}

	/// <summary>
	/// Asynchronously sends an email.
	/// </summary>
	/// <param name="targetEmail">The recipient's email address.</param>
	/// <param name="subject">The email's subject.</param>
	/// <param name="body">The email's body.</param>
	public async Task SendAsync(string targetEmail, string subject, string body){
		try {
			var message = new MailMessage(user, targetEmail, subject, body);

			await client.SendMailAsync(message);
		} catch {
			throw new SendEmailException("Nao foi possivel enviar o email.");
		}
	}
}
