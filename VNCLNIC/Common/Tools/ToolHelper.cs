using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Common.Tools
{
    public class ToolHelper
    {
        /// <summary>
        /// Hỗ trợ lấy IP hiện tại
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            try
            {

                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip))
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }
            catch { return "0.0.0.0"; }
        }
    }
}