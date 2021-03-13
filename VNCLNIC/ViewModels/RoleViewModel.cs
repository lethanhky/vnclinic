using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Tên của vai trò
        /// </summary>
        public string RoleName { get; set; }
        //[Column("Role")]
        //public UserRole IntRole { get; set; }
        /// <summary>
        /// Trạng thái kích hoạt của vai trò
        /// </summary>
        public bool IsActive
        {
            get; set;
        }
        /// <summary>
        /// Xem role này có đặc quyền cao nhất không (Nếu là Yes thì không cần phải xét quyền)
        /// </summary>
        public bool IsRoot
        {
            get; set;
        }

    }
}