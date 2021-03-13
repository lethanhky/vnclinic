using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VNCLNIC.Common.Enums;

namespace VNCLNIC.Models
{
    public class Role
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
        } /// <summary>
          /// Loại nhóm quyền (Hệ thống - Phòng khám)
          /// </summary>
        public RoleType RoleType
        {
            get; set;
        }

        /// <summary>
        /// Các user sở hữu role này
        /// </summary>
        public virtual ICollection<User> Users { get; set; }
        /// <summary>
        /// Các permission mà role này sở hữu
        /// </summary>
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}