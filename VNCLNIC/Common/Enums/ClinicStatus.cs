using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VNCLNIC.Common.Enums
{
    public enum ClinicStatus
    {
        [Display(Name = "Hoạt động")]
        ACTIVE = 0,
        [Display(Name = "Ngưng hoạt động")]
        INACTIVE = 1
    }
}