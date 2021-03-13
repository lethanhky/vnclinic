using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models
{
    public class Permission
    {
        public int Id { get; set; }
        /// <summary>
        /// Tên của permission
        /// </summary>
        public string PermissionName { get; set; }
        /// <summary>
        /// Mã permission để so quyền
        /// </summary>
        [Index(IsUnique = true)]
        [StringLength(20)]
        public string PermissionCode
        {
            get; set;
        }
        /// <summary>
        /// Trạng thái của permission
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Các role mà permission này có liên hệ
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
    }
}