namespace booksite.Helpers
{
    public static class CurrencyHelper
    {
        public static string FormatCurrency(decimal amount)
        {
            return $"{amount:N0} ₽";
        }

        public static string FormatCurrency(double amount)
        {
            return $"{amount:N0} ₽";
        }

        public static string FormatCurrency(int amount)
        {
            return $"{amount:N0} ₽";
        }
    }
}