using System;
using System.Globalization;

namespace Securities
{
    public static class ParsingUtilities
    {
        /// <summary>
        /// tries to parse the date in a number of different formats
        /// </summary>
        public static DateTime GetDate(string s)
        {
            bool conversionSuccess = DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

            if (!conversionSuccess && DateTime.TryParseExact(s, "d-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                conversionSuccess = true;
            }

            if (!conversionSuccess)
            {
                throw new InvalidCastException($"Could not parse the following date: {s}");
            }

            return date;
        }
    }
}
