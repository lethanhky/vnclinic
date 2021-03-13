using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VNCLNIC.Common.Enums
{
    public enum UserType
    {
        [Display(Name = "Khách hàng")]
        CUSTOMER = 0,
        [Display(Name = "Phòng khám")]
        CLINIC = 1,
        [Display(Name = "Hệ thống")]
        SYSTEM = 2,
    }
}