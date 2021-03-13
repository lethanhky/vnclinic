using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNCLNIC.Auth;
using VNCLNIC.Common.Tools;
using VNCLNIC.Common.Enums;
using VNCLNIC.Data;
using VNCLNIC.Models;
using VNCLNIC.ViewModels;

namespace VNCLNIC.Controllers
{
    public class UserController : BaseController
    {
        private readonly DataContext db;
        public UserController(DataContext db)
        {
            this.db = db;
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult Detail(int? id)
        {
            ViewBag.Roles = db.Roles.ToList();
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();

            return PartialView("_UserDetail", user);
        }

        [HttpGet]
        public PartialViewResult UpdateProfile(int? userId)
        {
            var user = db.Users.Where(x => x.Id == userId).FirstOrDefault();
            return PartialView("_UpdateProfile", user);
        }
        [HttpGet]
        public PartialViewResult Add()
        {
            ViewBag.Roles = db.Roles.ToList();
            var user = new User();

            return PartialView("_AddUser", user);
        }

        //chưa thêm ảnh
        [HttpPost]
        public JsonResult AddUser(string username, string password, string fullname, bool isActive, bool isDoctor, string email, string phone, string address, int usertype, List< int > roles)
        {
            try
            {
                if (roles == null)
                {
                    return Json(new { success = false, message = "Vui lòng chọn quyền !" }, JsonRequestBehavior.AllowGet);
                }
                var usercheck = db.Users.Where(x => x.Username == username).FirstOrDefault();
                if (usercheck == null)
                {
                    var user = new User();
                    user.Username = username;
                    user.IsActive = isActive;
                    user.IsDoctor = isDoctor;
                    user.Password = Utilities.Encrypt(password);
                    user.FullName = fullname;
                    user.Email = email;
                    user.PhoneNumber = phone;
                    user.Address = address;

                    var lsrole = new List<Role>();
                    foreach (var item in roles)
                    {
                        var role = db.Roles.Where(x => x.Id == item).FirstOrDefault();
                        lsrole.Add(role);
                    }
                    user.Roles = lsrole;

                    //Field IsRemember chưa được xét
                    user.IsRemember = false;

                    if (usertype == 0)
                    {
                        user.UserType = UserType.CUSTOMER;
                    }
                    else if (usertype == 1)
                    {
                        user.UserType = UserType.CLINIC;
                    }
                    else if (usertype == 2)
                    {
                        user.UserType = UserType.SYSTEM;
                    }

                    db.Users.AddOrUpdate(user);
                }
                else
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã tồn tại !" }, JsonRequestBehavior.AllowGet);
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Lưu dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public PartialViewResult Filter()
        {
            int items = 25;
            int page = 1;
            int state = 0;

            string searchString = "";
            string sortProperty = "FullName";
            string sortOrder = "desc";

            if (Request.Form.Get("numItems") != null)
            {
                items = int.Parse(Request.Form.Get("numItems").ToString());
            }
            if (Request.Form.Get("selPage") != null)
            {
                page = int.Parse(Request.Form.Get("selPage").ToString());
            }
            if (Request.Form.Get("state") != null)
            {
                state = int.Parse(Request.Form.Get("state").ToString());
            }
            if (Request.Form.Get("searchString") != null)
            {
                searchString = Request.Form.Get("searchString").ToString().ToUpper();
            }
            if (Request.Form.Get("sortType") != null)
            {
                sortOrder = Request.Form.Get("sortType").ToString();
            }
            if (Request.Form.Get("sortBy") != null)
            {
                sortProperty = Request.Form.Get("sortBy").ToString();
            }
            if (sortProperty == "")
            {
                sortProperty = "FullName";
            }
            //lọc các participants theo project
            IQueryable<User> users = null;
            #region  xây dựng header cho lưới
            string header = "";
            //các cột cần hiển thị
            List<GridColumn> columns = new List<GridColumn>();
            //Lấy tất cả thuộc tính 
            var properties = typeof(User).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name.Equals("FullName"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Tên Người Dùng", Order = 2, AllowSorting = true });
                }
                if (item.Name.Equals("UserType"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Loại người dùng thuộc", Order = 4, AllowSorting = true });
                }
                if (item.Name.Equals("IsActive"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Kích hoạt?", Order = 3, AllowSorting = true });
                }
            }
            //sắp xếp lại các cột theo thứ tự
            columns = columns.OrderBy(c => c.Order).ToList();

            foreach (var col in columns)
            {
                if (col.AllowSorting)
                {
                    if (sortOrder.Equals("desc") && sortProperty.Equals(col.Name))
                    {
                        header += "<th class='sorting_desc'><a style='display: block;' href='#' onclick='findcolName(\"" + col.Name + "\",\"asc\")'>" + col.Text + "</a></th>";
                    }
                    else if (sortOrder.Equals("asc") && sortProperty.Equals(col.Name))
                    {
                        header += "<th class='sorting_asc'><a style='display: block;' href='#' onclick='findcolName(\"" + col.Name + "\",\"desc\")'>" + col.Text + "</a></th>";
                    }
                    else
                    {
                        header += "<th class='sorting_asc'><a style='display: block;' href='#' onclick='findcolName(\"" + col.Name + "\",\"asc\")'>" + col.Text + "</a></th>";
                    }
                }
                else
                    header += "<th>" + col.Text + "</th>";
            }
            #endregion


            #region Phần sắp xếp			
            //Lấy dataset rỗng
            if (sortOrder.Equals("desc"))
            {
                if (sortProperty.Equals("FullName"))
                    users = from c in db.Users orderby c.FullName descending select c;
                else if (sortProperty.Equals("IsActive"))
                    users = from c in db.Users orderby c.IsActive descending select c;
                else
                    users = from c in db.Users orderby c.UserType descending select c;
            }
            else
            {

                if (sortProperty.Equals("FullName"))
                    users = from c in db.Users orderby c.FullName ascending select c;
                else if (sortProperty.Equals("IsActive"))
                    users = from c in db.Users orderby c.IsActive ascending select c;
                else
                    users = from c in db.Users orderby c.UserType ascending select c;
            }
            #endregion

            if (searchString.Length > 0)
            {
                users = users.Where(c => c.FullName.ToString().ToLower().Contains(searchString.ToLower()));
            }

            if (state > 0)
            {
                if (state == 1)
                {
                    users = users.Where(c => c.IsActive == true);
                }
                else
                {
                    users = users.Where(c => c.IsActive == false);
                }
            }

            //IQueryable<RoleViewModel> roleViewModels = null;
            //if (users != null)
            //{
            //    roleViewModels = from a in users
            //                     select new RoleViewModel()
            //                     {
            //                         Id = a.Id,
            //                         RoleName = a.RoleName,
            //                         IsActive = a.IsActive,
            //                         IsRoot = a.IsRoot,
            //                     };
            //}

            #region Phần phân trang		
            int count = users.Count();
            Paging paging = new Paging(page, items, count, "updatePage", "");
            ViewBag.Paging = paging.PageModel.HTML;
            ViewBag.Label = paging.PageModel.Label;

            var lsUser = users.ToList();
            if (paging.PageModel.StartItem >= 1 && count > 0)
            {
                lsUser = lsUser.Skip(paging.PageModel.StartItem - 1).Take(paging.PageModel.StopItem - paging.PageModel.StartItem + 1).ToList();
            }
            #endregion
            ViewBag.GridHeader = header;
            return PartialView("_ListUser", lsUser);
        }

        [HttpPost]
        public JsonResult UpdateProfile(User user, HttpPostedFileBase Avatar)
        {
            try
            {
                if (Avatar != null && Avatar.ContentLength > 0)
                {
                    if (Avatar.ContentLength > 4194304) // 4mb
                    {
                        return Json(new { success = false, message = "Hình ảnh quá lớn !" }, JsonRequestBehavior.AllowGet);
                    }

                }
                if (Avatar != null && !Avatar.ContentType.Contains("image"))
                {
                    return Json(new { success = false, message = "Định dạng không được hỗ trợ !" }, JsonRequestBehavior.AllowGet);
                }

                var oldUser = db.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                if (oldUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng !" }, JsonRequestBehavior.AllowGet);
                }

                oldUser.FullName = user.FullName;
                oldUser.Email = user.Email;
                oldUser.PhoneNumber = user.PhoneNumber;
                oldUser.Address = user.Address;

                if (Avatar != null)
                {
                    FileConfig ezfile = new FileConfig();
                    VCFileInfo fileInfo = ezfile.GetFinalName(FileConfig.UploadPath, Path.GetExtension(Avatar.FileName));
                    string relativePath = fileInfo.DirectoryContain + "//" + fileInfo.FileName;
                    Avatar.SaveAs(FileConfig.UploadPath + "//" + relativePath);
                    oldUser.Image = relativePath;
                }

                db.Users.AddOrUpdate(oldUser);
                db.SaveChanges();

                return Json(new { success = true, message = "Lưu dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}