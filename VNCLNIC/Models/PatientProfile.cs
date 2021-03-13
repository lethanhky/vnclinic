using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VNCLNIC.Models.Data;

namespace VNCLNIC.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        /// <summary>   
        /// Tên đầy đủ
        /// </summary>
        public string FullName { get; set; }
        /// <summary>   
        /// Mã bệnh nhân
        /// </summary>
        public string Code { get; set; }
        /// <summary>   
        /// Ngày sinh
        /// </summary>
        public DateTime DOB { get; set; }
        /// <summary>   
        /// Sdt
        /// </summary>
        public string Phone { get; set; }
        /// <summary>   
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>   
        /// Công việc
        /// </summary>
        public string Job { get; set; }
        /// <summary>   
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        /// <summary>   
        /// Người nhập
        /// </summary>
        public virtual User User { get; set; }
    }
}