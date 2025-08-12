# Exchange CLI

A simple command-line tool for converting currencies using FX exchange rates fetched from [https://www.lb.lt](https://www.lb.lt).  
The tool uses an auto-expiring in-memory cache of currency rates and includes hardcoded fallback exchange values in case the external client is unavailable.
The tool supports conversions between the following currencies:

- EUR
- USD
- GBP
- SEK
- NOK
- CHF
- JPY
- DKK

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) — if running locally
- [Docker](https://www.docker.com/) — if running inside a container

---

## Usage (Docker)

From the **repository root**:

### 1. Build the image

```bash
docker build -t exchange-cli -f src/Exchange.Cli/Dockerfile .
```

### 2. Run the CLI tool

```bash
docker run -it --rm exchange-cli
```


## Usage (Locally with .NET)

From the **repository root**, run the CLI app:

```bash
cd src/Exchange.Cli
dotnet run
```

---

## Running Tests

To run unit tests:

```bash
dotnet test
```

Make sure you're in the solution root directory when running this command.

---

## Usage (CLI)

```
Usage: Exchange <currency pair> <amount>
Example: Exchange EUR/DKK 1

Available commands:
  list   - Show supported currencies
  help   - Show this help message
  exit   - Quit the application
```

---
## Author

[Erikas Silobritas]
