using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNCLNIC.Auth;
using VNCLNIC.Common.Tools;
using VNCLNIC.Data;
using VNCLNIC.Models;
using VNCLNIC.ViewModels;

namespace VNCLNIC.Controllers
{
    public class RoleController : BaseController
    {
        private readonly DataContext db;
        public RoleController( DataContext db)
        {
            this.db = db;
        }
        // GET: Role
        public ActionResult Index()
        {
            ViewBag.Permissions = db.Permissions.ToList();
            return View();
        }

        [HttpPost]
        public PartialViewResult Filter()
        {
            int items = 25;
            int page = 1;
            int state = 0;

            string searchString = "";
            string sortProperty = "RoleName";
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
                sortProperty = "RoleName";
            }
            //lọc các participants theo project
            IQueryable<Role> roles = null;
            #region  xây dựng header cho lưới
            string header = "";
            //các cột cần hiển thị
            List<GridColumn> columns = new List<GridColumn>();
            //Lấy tất cả thuộc tính 
            var properties = typeof(Role).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name.Equals("RoleName"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Tên quyền", Order = 2, AllowSorting = true });
                }
                if (item.Name.Equals("IsActive"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Kích hoạt?", Order = 3, AllowSorting = true });
                }
                if (item.Name.Equals("IsRoot"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Quyền cao nhất?", Order = 4, AllowSorting = true });
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
                if (sortProperty.Equals("RoleName"))
                    roles = from c in db.Roles orderby c.RoleName descending select c;
                else if (sortProperty.Equals("IsActive"))
                    roles = from c in db.Roles orderby c.IsActive descending select c;
                else
                    roles = from c in db.Roles orderby c.IsRoot descending select c;
            }
            else
            {

                if (sortProperty.Equals("RoleName"))
                    roles = from c in db.Roles orderby c.RoleName ascending select c;
                else if (sortProperty.Equals("IsActive"))
                    roles = from c in db.Roles orderby c.IsActive ascending select c;
                else
                    roles = from c in db.Roles orderby c.IsRoot ascending select c;
            }
            #endregion

            if (searchString.Length > 0)
            {
                roles = roles.Where(c => c.RoleName.ToString().ToLower().Contains(searchString.ToLower()));
            }

            if(state > 0)
            {
                if(state == 1)
                {
                    roles = roles.Where(c => c.IsActive == true);
                }
                else
                {
                    roles = roles.Where(c => c.IsActive == false);
                }
            }

            IQueryable<RoleViewModel> roleViewModels = null;
            if (roles != null)
            {
                roleViewModels = from a in roles
                                 select new RoleViewModel()
                                {
                                    Id = a.Id,
                                    RoleName = a.RoleName,
                                    IsActive = a.IsActive,
                                    IsRoot = a.IsRoot,
                                };
            }

            #region Phần phân trang		
            int count = roles.Count();
            Paging paging = new Paging(page, items, count, "updatePage", "");
            ViewBag.Paging = paging.PageModel.HTML;
            ViewBag.Label = paging.PageModel.Label;

            var lsRoleViewModel = roleViewModels.ToList();
            if (paging.PageModel.StartItem >= 1 && count > 0)
            {
                lsRoleViewModel = lsRoleViewModel.Skip(paging.PageModel.StartItem - 1).Take(paging.PageModel.StopItem - paging.PageModel.StartItem + 1).ToList();
            }
            #endregion
            ViewBag.GridHeader = header;
            return PartialView("_ListRole", lsRoleViewModel);
        }

        [HttpPost]
        public JsonResult AddOrUpdate(int Id, string name, bool isactive,bool isroot, List<int> permissions)
        {
            try
            {
                if (isroot == false && permissions == null)
                {
                    return Json(new { success = false, message = "Vui lòng chọn quyền !" }, JsonRequestBehavior.AllowGet);
                }
                if (Id <= 0)
                {
                    var role = new Role();
                    role.RoleName = name;
                    role.IsActive = isactive;
                    role.IsRoot = isroot;
                    var lspermission = new List<Permission>();
                    if(isroot == true)
                    {
                        lspermission = db.Permissions.ToList();
                    }
                    else
                    {
                        foreach (var item in permissions)
                        {
                            var permission = db.Permissions.Where(x => x.Id == item).FirstOrDefault();
                            lspermission.Add(permission);
                        }
                    }
                    role.Permissions = lspermission;
                    db.Roles.AddOrUpdate(role);
                }
                else
                {
                    var oldrole = db.Roles.Where(x => x.Id == Id).FirstOrDefault();
                    if (oldrole != null)
                    {

                        var lspermission = new List<Permission>();
                        if (isroot == true)
                        {
                            lspermission = db.Permissions.ToList();
                        }
                        else
                        {
                            foreach (var item in permissions)
                            {
                                var permission = db.Permissions.Where(x => x.Id == item).FirstOrDefault();
                                lspermission.Add(permission);
                            }
                        }

                        UpdateRolePermission(lspermission, oldrole);
                        oldrole.RoleName = name;
                        oldrole.IsActive = isactive;
                        oldrole.IsRoot = isroot;

                        db.Roles.AddOrUpdate(oldrole);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy nhóm quyền !" }, JsonRequestBehavior.AllowGet);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Lưu dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }

            void UpdateRolePermission(List<Permission> selected_permissions, Role role)
            {
                //chưa check addmin
                var currentPermissions = role.Permissions;
                foreach (var permission in db.Permissions.ToList())
                {
                    if (selected_permissions.Contains(permission))
                    {
                        if (!role.Permissions.Contains(permission))
                        {
                            role.Permissions.Add(permission);
                        }
                    }
                    else
                    {
                        if (role.Permissions.Contains(permission))
                        {
                            role.Permissions.Remove(permission);
                        }
                    }
                }

            }
        }


        //[HttpGet]
        //public JsonResult Update(int? id)
        //{
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return Json(new { success = false, message = "Không tìm thấy nhóm quyền !" }, JsonRequestBehavior.AllowGet);
        //        }
        //        var role = db.Roles.Where(x => x.Id == id).FirstOrDefault();

        //        return Json(new { success = true, role = role }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { success = false, message = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewBag.Permissions = db.Permissions.ToList();
            var role = new Role();

            return PartialView("_AddRole", role);
        }

        [HttpGet]
        public PartialViewResult Update(int? id)
        {
            ViewBag.Permissions = db.Permissions.ToList();
            var role = db.Roles.Where(x => x.Id == id).FirstOrDefault();

            return PartialView("_AddRole", role);
        }

        [HttpGet]
        public PartialViewResult Detail(int? id)
        {
            ViewBag.Permissions = db.Permissions.ToList();
            var role = db.Roles.Where(x => x.Id == id).FirstOrDefault();

            return PartialView("_RoleDetail", role);
        }
    }
}