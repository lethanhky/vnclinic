using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace VNCLNIC.Auth
{
    public class CustomPrincipal : IPrincipal
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string[] Roles { get; set; }

        public string[] Permissions
        {
            get; set;
        }
        public int IsRoot
        {
            get; set;
        }
        public IIdentity Identity { get; private set; }

        public bool IsInPermission(string permission)
        {
            if (Permissions.Any(p => p.Contains(permission)))
            {
                return true;
            }
            return false;
        }

        public bool IsInRole(string role)
        {
            if (Roles.Any(r => r.Contains(role)))
            {
                return true;
            }
            return false;
        }
        public bool HavingRight(string permissionCode)
        {
            if (IsRoot.Equals(1))
                return true;
            if (Permissions.Any(p => p.Contains(permissionCode)))
            {
                return true;
            }
            return false;
        }
        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }
    }
}