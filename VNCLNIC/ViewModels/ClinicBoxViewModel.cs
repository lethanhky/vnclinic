using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.ViewModels
{
    public class ClinicBoxViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Tên phòng khám, bệnh viện
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Hotline
        /// </summary>
        public string HotLine { get; set; }
        /// <summary>   
        /// Logo
        /// </summary>
        public string Logo { get; set; }
    }
}