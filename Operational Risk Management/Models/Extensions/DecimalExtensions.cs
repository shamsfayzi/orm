namespace Operational_Risk_Management.Models.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToCurrencyString(this decimal currency)
        {
            return currency.ToString("C0").Replace("$", string.Empty);
        }
    }
}
