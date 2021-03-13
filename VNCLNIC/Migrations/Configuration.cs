using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using VNCLNIC.Auth;
using VNCLNIC.Data;
using VNCLNIC.Models;

namespace VNCLNIC.Migrations
{

    public class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataContext context)
        {
            return;
            List<Permission> permissions = new List<Permission>();
            permissions.Add(new Permission() { PermissionCode = "USER_MNT", PermissionName = "Quản lý nhân viên", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "SYSTEM_CFG", PermissionName = "Cấu hình hệ thống", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "DASHBOARD", PermissionName = "Xem dashboard", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "CAT_MNT", PermissionName = "Quản lý danh mục", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "PROJECT_MNT", PermissionName = "Quản lý dự án tuyển dụng", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "FORM_SCR", PermissionName = "Sàng lọc hồ sơ", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "MAIL_SEND", PermissionName = "Gửi thư mời/ thông báo", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "SCHEDULING", PermissionName = "Xếp lịch phỏng vấn", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "INTERVIEW", PermissionName = "Phỏng vấn", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "ENROL_STAFF", PermissionName = "Nhận nhân viên vào làm việc", IsActive = true });
            permissions.Add(new Permission() { PermissionCode = "REPORT", PermissionName = "Xem báo cáo", IsActive = true });
            context.Permissions.AddOrUpdate(p => p.Id, permissions.ToArray());

            List<Role> roles = new List<Role>();
            roles.Add(new Role() { IsRoot = true, RoleName = "Quản trị hệ thống", IsActive = true, Permissions = permissions.GetRange(0, permissions.Count) });
            roles.Add(new Role() { IsRoot = true, RoleName = "Recruiter", IsActive = true, Permissions = permissions.GetRange(2, permissions.Count - 2) });
            roles.Add(new Role() { IsRoot = true, RoleName = "User", IsActive = true, Permissions = permissions.GetRange(2, 1) });
            roles.Add(new Role() { IsRoot = true, RoleName = "Manager", IsActive = true, Permissions = permissions.GetRange(2, permissions.Count - 2) });

            context.Roles.AddOrUpdate(r => r.Id, roles.ToArray());

            string _defPassword = "123456";
            List<User> users = new List<User>();
            users.Add(new User() { Username = "admin@gmail.com", Password = Utilities.Encrypt(_defPassword), IsRemember = false, IsActive = true, Roles = roles.GetRange(0, 1), FullName = "Administrator" });
            users.Add(new User() { Username = "staff@gmail.com", Password = Utilities.Encrypt(_defPassword), IsRemember = false, IsActive = true, Roles = roles.GetRange(2, 1), FullName = "Staff" });
            users.Add(new User() { Username = "recruiter@gmail.com", Password = Utilities.Encrypt(_defPassword), IsRemember = false, IsActive = true, Roles = roles.GetRange(1, 1), FullName = "Recruiter" });
            users.Add(new User() { Username = "manager@gmail.com", Password = Utilities.Encrypt(_defPassword), IsRemember = false, IsActive = true, Roles = roles.GetRange(3, 1), FullName = "Manager" });
            context.Users.AddOrUpdate(u => u.Id, users.ToArray());

        }
    }
}