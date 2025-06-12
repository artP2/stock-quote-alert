/// <summary>
/// Exception thrown when an error occurs while trying to send an email.
/// </summary>
public class SendEmailException : Exception {
	public SendEmailException(string? message) : base(message) { }
};

public interface IEmailService {
	/// <summary>
	/// Sends an email.
	/// </summary>
	/// <param name="targetEmail">The recipient's email address.</param>
	/// <param name="subject">The email's subject.</param>
	/// <param name="body">The email's body.</param>
    void Send(string targetEmail, string subject, string body);
}
