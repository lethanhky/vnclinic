using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace VNCLNIC.Common.Tools
{
    public class DataConverter
    { /// <summary>
      /// Hàm trả về chuỗi ngày tháng năm 2 ký tự
      /// </summary>
      /// <param name="dateTime">Ngày giờ từ SQL Server hoặc hệ thống</param>
      /// <returns></returns>
        public static string DateTime2UI_Lite(DateTime dateTime)
        {
            if (dateTime == null || dateTime == DateTime.MinValue)
                return "";
            return String.Format("{0:dd/MM/yy}", dateTime);
        }
        public static string ToMonthYear(DateTime? dateTime)
        {
            if (dateTime == null || dateTime == DateTime.MinValue)
                return "";
            DateTime dtnew = dateTime ?? DateTime.MinValue;
            string month = dtnew.Month.ToString();
            if (month.Length < 2)
                month = "0" + month;

            return month + "/" + dtnew.Year.ToString();
        }
        /// <summary>
        /// Hàm trả về chuỗi ngày tháng năm 4 ký tự
        /// </summary>
        /// <param name="dateTime">Ngày giờ từ SQL Server hoặc hệ thống</param>
        /// <returns></returns>
        public static string DateTime2UI_Full(DateTime dateTime)
        {
            if (dateTime == null)
                return "";
            return String.Format("{0:dd/MM/yyyy}", dateTime);
        }

        public static string DateTimeFull(DateTime dateTime)
        {
            if (dateTime == null)
                return "";
            return String.Format("{0:h:mm tt dd/MM/yyyy}", dateTime);
        }

        /// <summary>
        /// Hàm lấy một datetime từ chuỗi hiển thị
        /// </summary>
        /// <param name="uiText">Chuỗi hiển thị</param>
        /// <returns></returns>
        public static DateTime UI2DateTime(string uiText)
        {
            if (uiText != null && uiText.Length > 0)
            {
                string[] s = uiText.Split('/');
                int year = int.Parse(s[2]);
                if (year < 1000)
                    year += 2000;
                return new DateTime(year, int.Parse(s[1].ToString()), int.Parse(s[0].ToString()));
            }
            return DateTime.MinValue;
        }

        public static DateTime UI2DateTimeRange(string uiText, bool toMin)
        {
            if (uiText != null && uiText.Length > 0)
            {
                string[] s = uiText.Split('/');
                int year = int.Parse(s[2]);
                if (year < 1000)
                    year += 2000;
                if (toMin)
                    return new DateTime(year, int.Parse(s[1].ToString()), int.Parse(s[0].ToString()), 0, 0, 0);
                else
                    return new DateTime(year, int.Parse(s[1].ToString()), int.Parse(s[0].ToString()), 23, 59, 59);
            }
            return DateTime.MinValue;
        }

        public static DateTime UI2DateTimeMinMax(DateTime date, bool toMin)
        {
            if (toMin)
                return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            else
                return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            return DateTime.MinValue;
        }
        public static DateTime UI2_GetMinMax(DateTime date, bool toMin)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
            if (toMin)
            {
                return new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            }
            else
            {
                return new DateTime(date.Year, date.Month, lastDayOfMonth, 23, 59, 59);
            }
        }

        public static string ConvertJsTime(string input)
        {
            if (input == null)
                return "";
            string[] s = input.Split(' ');
            if (s.Length > 1)
            {
                string[] timepart = s[0].Split(':');
                int hour = int.Parse(timepart[0]);
                if (s[1].Equals("PM"))
                    hour += 12;
                return "," + hour.ToString() + "," + timepart[1];
            }
            return input;
        }
        public static DateTime? UI2DateTimeOrNull(string uiText)
        {
            if (uiText != null && uiText.Length > 0)
            {
                try
                {
                    string[] s = uiText.Split('/');
                    int year = int.Parse(s[2]);
                    if (year < 1000)
                        year += 2000;
                    return new DateTime(year, int.Parse(s[1].ToString()), int.Parse(s[0].ToString()));
                }
                catch
                {
                    return DateTime.MinValue;
                }

            }
            return null;
        }

        public static float parseFloat(string input, float outError)
        {
            try
            {
                return float.Parse(input);
            }
            catch { return outError; }
        }
        public static int parseInt(string input, int outError)
        {
            try
            {
                return int.Parse(input);
            }
            catch { return outError; }
        }

        public static string MinuteTo12H(int minutes)
        {
            DateTime dt = DateTime.Today;
            var timespan = TimeSpan.FromMinutes(minutes);
            var hours = timespan.TotalHours;
            return dt.Subtract(-TimeSpan.FromHours(hours)).ToShortTimeString();

        }
        /// <summary>
        /// Convert giờ theo khung 12 (AM/PM) sang phút 
        /// </summary>
        public static int HoursToMinute(string hours, int outError)
        {
            try
            {
                return DateTime.Parse("2/2/2020 " + hours).Hour * 60 + DateTime.Parse("2/2/2020 " + hours).Minute;
            }
            catch
            {
                return outError;
            }
        }

        public static string GetTimeFromFullDateTime(string datetime)
        {
            try
            {
                return DateTime.ParseExact(datetime, "dd/MM/yy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None).ToString("hh:mm");
            }
            catch
            {
                return "";
            }
        }


        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        /// <summary>
        /// Trả về startDate và out value timeStart, timeEnd, endDate
        /// </summary>
        /// <param name="rangeDate"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static DateTime DateRangeToDates(string rangeDate, out DateTime endDateOut)
        {
            try
            {
                var start = rangeDate.Substring(0, rangeDate.IndexOf('-')).Trim();
                var end = rangeDate.Substring(rangeDate.IndexOf('-') + 1).Trim();
                var startDate = UI2DateTime(start);
                var endDate = UI2DateTime(end);
                endDateOut = endDate;
                return startDate;
            }
            catch
            {
                endDateOut = DateTime.Now;
                return DateTime.Now;
            }
        }

        public static string[] GetListPhotos(string photos)
        {
            return photos.Split(',').ToArray();
        }
        public static string Date2Dsiplay(DateTime? input)
        {
            if (input == null || input == DateTime.MinValue)
            {
                return "-";
            }
            else
            {
                return DateTime2UI_Full(input ?? DateTime.MinValue);
            }
        }

        public static string Date2DayMonth(DateTime date)
        {
            return date.ToString("dd-MMMM");
        }
        public static double AverageA(List<double> list)
        {
            double total = 0;
            int count = list.Count;
            foreach (double x in list)
            {
                total += x;
                if (x <= 0)
                    count--;
            }
            return total / count;
        }

        public static List<WeekOfMonth> GetWeeksOfMonth(int month, int year)
        {
            var dates = Enumerable.Range(1, DateTime.DaysInMonth(year, month)).Select(n => new DateTime(year, month, n));
            DateTime tempdate = new DateTime(year, month, 1);
            DateTime maxDate = DataConverter.UI2_GetMinMax(tempdate, false);
            List<WeekOfMonth> list = new List<WeekOfMonth>();
            int current = 1;
            foreach (DateTime _d in dates)
            {
                int weekNum = _d.DayOfYear / 7;
                if (current != weekNum)
                {
                    current = weekNum;
                    DateTime max = _d.AddDays(7);
                    if (max > maxDate)
                        max = maxDate;
                    list.Add(new WeekOfMonth() { WeekName = weekNum, FromDate = _d, ToDate = max });
                }
            }
            return list;
        }

        public static DataTable SumDataTable(DataTable table)
        {
            DataTable temp = table.Copy();
            DataRow nrow = temp.NewRow();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                int total = 0;
                foreach (DataRow row in table.Rows)
                {
                    total += int.Parse(row[i].ToString());
                }
                nrow[i] = total;
            }
            temp.Rows.Add(nrow);
            temp.AcceptChanges();
            return temp;
        }
        public static string GetMonthYear(DateTime date)
        {
            int month = date.Month;
            int year = date.Year;
            string _month = month.ToString();
            string _year = year.ToString().Substring(2);
            if (_month.Length < 2) _month = "0" + _month;
            return _month + "/" + _year;
        }
        public static DateTime LastDateOfMonth(int month, int year)
        {
            try
            {
                try
                {
                    DateTime dt = new DateTime(year, month, 31);
                    return UI2DateTimeMinMax(dt, false);
                }
                catch
                {
                    try
                    {
                        DateTime dt = new DateTime(year, month, 30);
                        return UI2DateTimeMinMax(dt, false);
                    }
                    catch
                    {
                        try
                        {
                            DateTime dt = new DateTime(year, month, 29);
                            return UI2DateTimeMinMax(dt, false);
                        }
                        catch
                        {
                            try
                            {
                                DateTime dt = new DateTime(year, month, 28);
                                return UI2DateTimeMinMax(dt, false);
                            }
                            catch
                            {

                            }
                        }
                    }
                }

            }
            catch
            {
                return DateTime.MinValue;
            }
            return DateTime.MinValue;
        }

    }
    public class WeekOfMonth
    {
        public int WeekName
        {
            get; set;
        }
        public DateTime FromDate
        {
            get; set;
        }
        public DateTime ToDate
        {
            get; set;
        }
    }
}