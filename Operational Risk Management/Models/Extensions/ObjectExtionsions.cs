using System.Reflection;
using System.Runtime.CompilerServices;
//using Operational_Risk_Management.Data.Interfaces;
using Operational_Risk_Management.Models.Entities;
using Operational_Risk_Management.Models.Enums;
//using Operational_Risk_Management.Models.Statics;

namespace Operational_Risk_Management.Models.Extensions
{
    public static class ObjectExtionsions
    {


        public static string ToYesNo(this bool value)
        {

            return value == true ? "Yes" : "No";
        }
        public static string ToYesNo(this bool? value)
        {

            return value != null ? value == true ? "Yes" : "No" : "";
        }
        //public static void AddPolicy(this IVulnerability va)
        //{

        //    switch (va.Risk)
        //    {
        //        case Risks.Critical:
        //            va.DueDate = DateTime.Now.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(7);
        //            break;
        //        case Risks.High:
        //            va.DueDate = DateTime.Now.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(14);
        //            break;
        //        case Risks.Medium:
        //            va.DueDate = DateTime.Now.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(30);
        //            break;
        //        case Risks.Low:
        //            va.DueDate = DateTime.Now.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(30);
        //            break;
        //    }
        //    if (va.DueDate is null)// if status is not defined so it is not assigned
        //    {
        //        va.Status = Status.NotAssigned;
        //    }
        //    else if (va.DueDate?.Date >= DateTime.Now.Date)//if expected date is bigger than today its WIP
        //    {
        //        va.Status = Status.WIP;
        //    }
        //    else if (va.DueDate?.Date < DateTime.Now.Date)
        //    {
        //        va.Status = Status.WIP;
        //        if (va.Risk != Risks.Low)
        //        {
        //            va.IsOverDue = true;
        //        }
        //    }

        //}
        //public static void UpdatePolicy(this IVulnerability va)
        //{
        //    if (va.DueDate is null)
        //    {
        //        switch (va.Risk)
        //        {
        //            case Risks.Critical:
        //                va.DueDate = va.CreateDate.Value.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(7);
        //                break;
        //            case Risks.High:
        //                va.DueDate = va.CreateDate.Value.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(14);
        //                break;
        //            case Risks.Medium:
        //                va.DueDate = va.CreateDate.Value.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(30);
        //                break;
        //            case Risks.Low:
        //                va.DueDate = va.CreateDate.Value.AddDays(-(va.CreateDate.Value.Date.Day + 1)).AddDays(30);
        //                break;
        //        }
        //    }
        //    if (va.DueDate?.Date >= DateTime.Now.Date)//if expected date is bigger than today its WIP
        //    {
        //        va.Status = Status.WIP;
        //    }
        //    else if (va.DueDate?.Date < DateTime.Now.Date)
        //    {
        //        va.Status = Status.WIP;
        //        if (va.Risk != Risks.Low)
        //        {
        //            va.IsOverDue = true;
        //        }
        //    }

        //}
        //public static IEnumerable<Vulnerability> GetMonthly(this IEnumerable<Vulnerability> vaList, int month)
        //{
        //    return vaList.Where(a => (a.CreateDate.Value.AddMonths(-1).Month) == month).ToList();
        //}

        //public static DateTime GetExactMonth(this DateTime date)
        //{
        //    return date.AddMonths(-1);
        //}

        //public static IQueryable<Notification> FilterForMe(this IQueryable<Notification> notifics, string fullName)
        //{
        //    return notifics.Where(a => a.Reciver == fullName);
        //}

        //public static bool IsDR(this Vulnerability va)
        //{
        //    return Static_IPRanges.DR_Range.Any(a => va.Host.StartsWith(a));
        //}

        //public static bool IsGR(this Vulnerability va)
        //{
        //    return Static_IPRanges.GR_Range.Any(a => va.Host.StartsWith(a));
        //}

        //public static bool IsHO(this Vulnerability va)
        //{
        //    return Static_IPRanges.HO_Range.Any(a => va.Host.StartsWith(a));
        //}
    }
}
