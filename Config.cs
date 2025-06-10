using System.Text.Json;

/// <summary>
/// Represents the structure of the application's configuration data.
/// </summary>
/// <param name="TargetEmail">The email that will receive the alerts.</param>
/// <param name="SmtpServer">The SMTP server address.</param>
/// <param name="SmtpPort">The SMTP server port.</param>
/// <param name="SmtpUser">The username for SMTP authentication.</param>
/// <param name="SmtpPassword">The password for SMTP authentication.</param>
public record Config (
	string TargetEmail,
	string SmtpServer,
	int SmtpPort,
	string SmtpUser,
	string SmtpPassword
) {
	/// <summary>
	/// Loads the settings from the 'config.json' file.
	/// </summary>
	/// <returns>A Config instance with data from the file.</returns>
	static public Config Load(){
		try {
			var json = File.ReadAllText("config.json");
			return JsonSerializer.Deserialize<Config>(json)
				?? throw new JsonException();
		} catch (FileNotFoundException) {
			throw new Exception("Arquivo de configuraçao 'config.json' nao encontrado.");
		} catch (JsonException) {
			throw new Exception("Configuraçao invalida.");
		}
	}
}
