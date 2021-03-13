using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        /// <summary>
        /// Tên lịch hẹn
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ngày đặt lịch khám
        /// </summary>
        public DateTime BookingDate { get; set; }
        /// <summary>
        /// Ngày khám
        /// </summary>
        public DateTime ScheduleDate { get; set; }
        /// <summary>
        /// Giờ khám
        /// </summary>
        public string ScheduleTime { get; set; }
        /// <summary>
        /// Tên bác sĩ khám
        /// </summary>
        public string DoctorName { get; set; }
        /// <summary>
        /// Hồ sơ bệnh nhân
        /// </summary>
        public virtual PatientProfile PatientProfile { get; set; }
        /// <summary>
        /// Bác sĩ khám
        /// </summary>
        public virtual User Doctor { get; set; }
    }
}