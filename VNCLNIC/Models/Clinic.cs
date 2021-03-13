using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VNCLNIC.Common.Enums;
using VNCLNIC.Models.Data;

namespace VNCLNIC.Models
{
    public class Clinic
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
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Hotline
        /// </summary>
        public string HotLine { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>   
        /// Logo
        /// </summary>
        public string Logo { get; set; }
        /// <summary>   
        /// Kinh độ
        /// </summary>
        public string Lat { get; set; }
        /// <summary>   
        /// Vỹ độ
        /// </summary>
        public string Lng { get; set; }
        /// <summary>   
        /// Tình trạng hoạt động
        /// </summary>
        public ClinicStatus Status { get; set; }
        /// <summary>   
        /// Làm việc buổi sáng
        /// </summary>
        public bool IsWorkMorning { get; set; }
        /// <summary>   
        /// Giờ làm buổi sáng
        /// </summary>
        public TimeSpan StartMorning { get; set; }
        /// <summary>   
        /// Giờ nghỉ buổi sáng
        /// </summary>
        public TimeSpan StopMorning { get; set; }
        /// <summary>   
        /// Làm việc buổi trưa
        /// </summary>
        public bool IsWorAfternoon { get; set; }
        /// <summary>   
        /// Giờ làm buổi chiều
        /// </summary>
        public TimeSpan StartAfternoon { get; set; }
        /// <summary>   
        /// Giờ nghỉ buổi chiều
        /// </summary>
        public TimeSpan StopAfternoon { get; set; }
        /// <summary>   
        /// Làm việc buổi tối
        /// </summary>
        public bool IsWorEvening { get; set; }
        /// <summary>   
        /// Giờ làm buổi chiều
        /// </summary>
        public TimeSpan StartEvening { get; set; }
        /// <summary>   
        /// Giờ nghỉ buổi chiều
        /// </summary>
        public TimeSpan StopEvening { get; set; }
        /// <summary>   
        /// Loại phòng khám
        /// </summary>
        public virtual ClinicType ClinicType { get; set; }
        /// <summary>
        /// 
        /// Tỉnh thành
        /// </summary>
        public virtual City City { get; set; }
        /// <summary>
        /// Tỉnh thành
        /// </summary>
        public virtual District District { get; set; }
        /// <summary>
        /// Các chuyên khoa trong phòng khám
        /// </summary>
        public virtual ICollection<Department> Departments { get; set; }
    }
}