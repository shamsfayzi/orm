namespace Operational_Risk_Management.Models.Extensions
{
    public static class DoubleExtensions
    {

        public static string CalculatePercentage(this double part, double total)
        {
            if (total == 0)
            {
                return "0";
            }

            double percentage = (part / total) * 100;
            return percentage % 1 == 0 ? percentage.ToString("0") : percentage.ToString("0.00");
        }



        public static double ToDouble(this int value)
        {
            return Convert.ToDouble(value);
        }
    }
}
