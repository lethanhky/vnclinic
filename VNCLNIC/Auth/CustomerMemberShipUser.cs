using System;
using System.Collections.Generic;
using VNCLNIC.Models;
using System.Web.Security;

namespace VNCLNIC.Auth
{
    public class CustomerMemberShipUser : MembershipUser
    {
        public int Id { get; set; }
        public ICollection<Role> Roles { get; set; }

        public CustomerMemberShipUser(User user) : base("CustomMembership", user.Username, user.Id, user.Username, string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            Id = user.Id;
            Roles = user.Roles;
        }
    }
}