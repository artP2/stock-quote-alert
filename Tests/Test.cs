using Moq;

public class StockQuoteAlertTests {
	private readonly Mock<IStockGetService> _mockStockGetService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Config _config;

    public StockQuoteAlertTests() {
        _mockStockGetService = new Mock<IStockGetService>();
        _mockEmailService = new Mock<IEmailService>();
        _config = new Config {
            TargetEmail = "teste@email.com",
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            SmtpUser = "user",
            SmtpPassword = "password"
        };
    }
	
    [Fact]
    public async Task MonitorStockAsync_ShouldSendEmail_WhenActionIsSell(){
        var stock = new Stock("PETR4", SellPrice: 40, BuyPrice: 20);

        _mockStockGetService
            .Setup(s => s.GetPriceAsync("PETR4"))
            .ReturnsAsync(42);

		// cancel the loop
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

		try {
	        await StockQuoteAlert.MonitorStockAsync(
	            stock,
	            _mockStockGetService.Object,
	            _mockEmailService.Object,
	            _config,
	            cts.Token
	        );
		} catch (TaskCanceledException) {
			// ignore task cancel
		}

        // assert
        _mockEmailService.Verify(es =>
            es.SendAsync(
                "teste@email.com",
                It.Is<string>(s => s.Contains("vender")),
                It.Is<string>(b => b.Contains("PETR4"))
            ),
            Times.Once
        );
    }
}
