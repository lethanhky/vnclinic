using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Auth
{
    public class CustomSerializeModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public List<string> RoleName { get; set; }

        /// <summary>
        /// Danh sách quyền chứa các mã quyền
        /// </summary>
        public List<string> PermissionList
        {
            get; set;
        }
        /// <summary>
        /// Nếu có quyền tối cao là bằng 1, mặc định là 0
        /// </summary>
        public int IsRoot
        {
            get; set;
        }
    }
}