using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models
{
    public class DoctorSchedule
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MorningWork { get; set; }
        public string NoonWork { get; set; }
        public string EveningWork { get; set; }
        public virtual User User { get; set; }
    }
}