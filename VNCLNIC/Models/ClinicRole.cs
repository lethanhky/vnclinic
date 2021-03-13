using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models
{
    public class ClinicRole
    {
        public int Id { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}