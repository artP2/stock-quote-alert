/// <summary>
/// Represents the structure of the application's configuration data.
/// </summary>
/// <param name="TargetEmail">The email that will receive the alerts.</param>
/// <param name="SmtpServer">The SMTP server address.</param>
/// <param name="SmtpPort">The SMTP server port.</param>
/// <param name="SmtpUser">The username for SMTP authentication.</param>
/// <param name="SmtpPassword">The password for SMTP authentication.</param>
public class Config {
    public string TargetEmail { get; set; } = default!;
    public string SmtpServer { get; set; } = default!;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = default!;
    public string SmtpPassword { get; set; } = default!;
}
