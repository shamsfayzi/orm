using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class GuidExtensions
    {
        public static string ToStringList(this IEnumerable<Guid> idList)
        {
            var stringList = "";
            var isFirstIndex = true;
            foreach (var id in idList)
            {
                if (isFirstIndex)
                {
                    stringList += id.ToString().AddDoubleQuotes();
                    isFirstIndex = false;
                }
                else
                {
                    stringList += "," + id.ToString().AddDoubleQuotes();
                }
            }
            return stringList;
        }
        public static Guid ToGuid(this Guid? guid)
        {
            return guid.ToString().ToGuid();
        }
    }
}
