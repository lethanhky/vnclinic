using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models
{
    public class Department
    {
        public int Id { get; set; }
        /// <summary>   
        /// Tên chuyên khoa
        /// </summary>
        public string Name { get; set; }
        /// <summary>   
        /// Thời lượng buổi khám (Phút)
        /// </summary>
        public int Duration { get; set; }
        public virtual ICollection<Clinic> Clinics { get; set; }
    }
}