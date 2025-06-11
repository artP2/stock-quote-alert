# Stock Quote Alert

> **Note:** This project was developed as part of a selective process.

A .NET command-line application to monitor stock prices and send email alerts when predefined buy or sell values are met.

## Setup

1.  Clone the repository.
2.  In the root directory, create a file named `config.json`.
3.  Add your SMTP server details and the target email address to the file, following this structure:

    ```json
    {
      "TargetEmail": "your-email@example.com",
      "SmtpServer": "smtp.example.com",
      "SmtpPort": 587,
      "SmtpUser": "your-smtp-user@example.com",
      "SmtpPassword": "your-smtp-password"
    }
    ```

## Usage

Run the application using the `dotnet run` command, followed by one or more sets of stock arguments.

Each set of arguments must be in the format: `[stock_name] [sell_price] [buy_price]`

**Example (monitoring one stock):**
```bash
dotnet run PETR4 35.50 31.00
```

**Example (monitoring multiple stocks):**
```bash
dotnet run PETR4 35.50 31.00 MGLU3 2.50 1.80
```

To stop the application, press `Ctrl+C`.

## API Credits

This project uses a public API provided by Ledev for stock price data.

- **API Base URL**: `https://ledev.com.br/api/cotacoes/`
