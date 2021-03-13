using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace VNCLNIC.Auth
{
    public static class Utilities
    {
        private const string KEY_HASH = "e$Z6^HR";

        public static string Encrypt(string text)
        {
            return MD5Hash(text);
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string Decrypt(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(KEY_HASH));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// Truyền vào listdata, các propoty cần search và value để trả về giá trị
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">list data</param>
        /// <param name="property">các property</param>
        /// <param name="value">giá trị search</param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> source, string[] property, object value)
        {
            var properties = new List<PropertyInfo>();
            List<T> returnList = new List<T>();
            foreach (var prop in property)
            {
                var pro = typeof(T).GetProperty(prop, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                if (pro != null)
                    properties.Add(pro);
            }
            foreach (var item in properties)
            {
                if (source.Any(w => ((string)item.GetValue(w)) != null))
                {
                    var listWithProperty = source.Where(d => ((string)item.GetValue(d)).ToLower().Trim().Contains(value.ToString().ToLower().Trim())).ToList();
                    if (listWithProperty.Count > 0)
                        returnList.AddRange(listWithProperty);
                }
            }
            return returnList.Distinct().AsQueryable();
        }

        public static int GetWeekOfYear(DateTime dateTime)
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("vi-VN");
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            return myCal.GetWeekOfYear(dateTime, myCWR, myFirstDOW);
        }
        /// <summary>
        /// Kiểm tra date truyền vào phải có thể nhỏ hơn 5 ngày và lớn hơn 10 năm
        /// </summary>
        public static bool CheckRangeDate(DateTime dateTime)
        {
            return DateTime.Now.AddDays(-5) < dateTime && DateTime.Now.AddYears(10) > dateTime;
        }

        /// <summary>
        /// Kiểm tra xem date truyền vào phải lớn hơn hoặc bằng ngày hiện tại
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool CheckCurrentDate(DateTime dateTime)
        {
            return DateTime.Now.Date <= dateTime.Date;
        }

        public static DateTime? ConvertDate(string datetime)
        {
            DateTime? date = null;
            if (DateTime.TryParseExact(datetime, "dd/MM/yyyy", CultureInfo.InstalledUICulture, DateTimeStyles.None, out DateTime outDate))
            {
                date = outDate;
            }
            return date;
        }

        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}