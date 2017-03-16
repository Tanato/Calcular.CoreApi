using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Common
{
    public static class Utils
    {
        /// <summary>
        /// Today and tomorrow by default + days, if friday or saturday show until monday.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetNextDates(int days = 2)
        {
            var today = DateTime.Now.DayOfWeek;
            return Enumerable.Range(0, today == DayOfWeek.Friday ? days + 2 : today == DayOfWeek.Saturday ? days + 1 : days).Select(x => DateTime.Now.AddDays(x).Date);
        }
    }
}
