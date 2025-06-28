using System.Reflection;
using System.Runtime.CompilerServices;
namespace Operational_Risk_Management.Models.Extensions
{
    public static class StringExtionsions
    {

        public static string Limit(this string text, int size)
        {
            return text.Length <= size ? text : text.Substring(0, size) + " ...";

        }
        public static string GetAbbr(this string text, int size)
        {
            return text.Trim().Length <= size ? text.Trim() : text.Trim().Substring(0, size);

        }
        public static bool IsEmptyOrNull(this string text)
        {
            return string.IsNullOrEmpty(text);

        }
        public static string ToText(this string? text)
        {
            return text is not null ? text : "";

        }
        public static long ToInt(this string? text)
        {
            return text is not null ? long.Parse(text) : 0;

        }
        public static Guid ToGuid(this string textGuid)
        {
            if (Guid.TryParse(textGuid, out var guid))
            {
                return guid;
            }

            return Guid.Empty;

        }
        public static string AddDoubleQuotes(this string value)
        {
            return "\"" + value.ToString() + "\"";
        }
        public static string GetFullNameProfile(this string value)
        {
            return value.Split(" ").ElementAt(0).ElementAt(0).ToString().ToUpper() + " " + value.Split(" ").ElementAt(1).ElementAt(0).ToString().ToUpper();
        }
        public static string ToUpperFirstLatter(this string text)
        {
            var result= text.ElementAt(0).ToString().ToUpper() + text.Substring(1, text.Length-1);
            return result;
        }
        public static DateTime ToDatetime(this string datetimeString)
        {
            return DateTime.Parse(datetimeString);
        }

       
    }
}
