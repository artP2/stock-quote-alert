using Moq;

public class StockQuoteAlertTests {
    [Fact]
    public async Task MonitorStockAsync_ShouldSendEmail_WhenActionIsSell(){
        var stock = new Stock("PETR4", SellPrice: 40, BuyPrice: 20);

        var mockStockGetService = new Mock<IStockGetService>();
        mockStockGetService
            .Setup(s => s.GetPriceAsync("PETR4"))
            .ReturnsAsync(42);

        var mockEmailService = new Mock<IEmailService>();
        var config = new Config {
            TargetEmail = "teste@email.com",
            SmtpServer = "smtp.example.com",
            SmtpPort = 587,
            SmtpUser = "user",
            SmtpPassword = "password"
        };

		// cancel the loop
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

		try {
	        await StockQuoteAlert.MonitorStockAsync(
	            stock,
	            mockStockGetService.Object,
	            mockEmailService.Object,
	            config,
	            cts.Token
	        );
		} catch (TaskCanceledException) {
			// ignore task cancel
		}

        // assert
        mockEmailService.Verify(es =>
            es.SendAsync(
                "teste@email.com",
                It.Is<string>(s => s.Contains("vender")),
                It.Is<string>(b => b.Contains("PETR4"))
            ),
            Times.Once
        );
    }
}
