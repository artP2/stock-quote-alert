/// <summary>
/// Represents the structure of the application's configuration data.
/// </summary>
public class Config {
    /// <summary>
    /// The email that will receive the alerts.
    /// </summary>
    public string TargetEmail { get; set; } = default!;
    /// <summary>
    /// The SMTP server address.
    /// </summary>
    public string SmtpServer { get; set; } = default!;
    /// <summary>
    /// The SMTP server port.
    /// </summary>
    public int SmtpPort { get; set; }
    /// <summary>
    /// The username for SMTP authentication.
    /// </summary>
    public string SmtpUser { get; set; } = default!;
    /// <summary>
    /// The password for SMTP authentication.
    /// </summary>
    public string SmtpPassword { get; set; } = default!;
    /// <summary>
    /// The interval, in minutes, between price checks. Defauts to 1 minute.
    /// </summary>
    public int MonitorIntervalMinutes { get; set; } = 1;
}
