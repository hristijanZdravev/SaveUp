namespace SaveUp.DTOs.Helper
{
    public class ExchangeRates
    {

        public static readonly Dictionary<string, double> _exchangeRates = new()
        {
            { "MKD", 61.5 },
            { "USD", 1.1 },
            { "EUR", 1.0 }
        };

        public static double ConvertToEUR(double amount, string currency)
        {
            if (!_exchangeRates.ContainsKey(currency))
                throw new ArgumentException($"Unsupported currency: {currency}");

            return amount / _exchangeRates[currency];
        }

        public static double ConvertFromEUR(double amountInEUR, string targetCurrency)
        {
            if (!_exchangeRates.ContainsKey(targetCurrency))
                throw new ArgumentException($"Unsupported currency: {targetCurrency}");

            return amountInEUR * _exchangeRates[targetCurrency];
        }
    }
}
