using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNCLNIC.Common.Tools;
using VNCLNIC.Data;
using VNCLNIC.Models.Data;

namespace VNCLNIC.Controllers
{
    public class ClinicTypeController : BaseController
    {
        private readonly DataContext db;
        public ClinicTypeController(DataContext db)
        {
            this.db = db;
        }
        // GET: ClinicType
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult Filter()
        {
            int items = 25;
            int page = 1;
            int state = 0;

            string searchString = "";
            string sortProperty = "Name";
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
                sortProperty = "Name";
            }
            //lọc các participants theo project
            IQueryable<ClinicType> clinicTypes = null;
            #region  xây dựng header cho lưới
            string header = "";
            //các cột cần hiển thị
            List<GridColumn> columns = new List<GridColumn>();
            //Lấy tất cả thuộc tính 
            var properties = typeof(ClinicType).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name.Equals("Name"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Loại phòng khám", Order = 2, AllowSorting = true });
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
                if (sortProperty.Equals("Name"))
                    clinicTypes = from c in db.ClinicTypes orderby c.Name descending select c;
            }
            else
            {

                if (sortProperty.Equals("Name"))
                    clinicTypes = from c in db.ClinicTypes orderby c.Name ascending select c;

            }
            #endregion

            if (searchString.Length > 0)
            {
                clinicTypes = clinicTypes.Where(c => c.Name.ToString().ToLower().Contains(searchString.ToLower()));
            }

            #region Phần phân trang		
            int count = clinicTypes.Count();
            Paging paging = new Paging(page, items, count, "updatePage", "");
            ViewBag.Paging = paging.PageModel.HTML;
            ViewBag.Label = paging.PageModel.Label;

            var lsclinictype = clinicTypes.ToList();
            if (paging.PageModel.StartItem >= 1 && count > 0)
            {
                lsclinictype = lsclinictype.Skip(paging.PageModel.StartItem - 1).Take(paging.PageModel.StopItem - paging.PageModel.StartItem + 1).ToList();
            }
            #endregion
            ViewBag.GridHeader = header;
            return PartialView("_ListClinicType", lsclinictype);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewBag.Permissions = db.Permissions.ToList();
            var clinicType = new ClinicType();

            return PartialView("_AddClinicType", clinicType);
        }

        [HttpGet]
        public PartialViewResult Update(int? id)
        {
            var clinicType = db.ClinicTypes.Where(x => x.Id == id).FirstOrDefault();
            return PartialView("_AddClinicType", clinicType);
        }

        [HttpPost]
        public JsonResult AddOrUpdate(int Id, string name)
        {
            try
            {
                if (name == null)
                {
                    return Json(new { success = false, message = "Vui lòng nhập tên loại phòng khám !" }, JsonRequestBehavior.AllowGet);
                }
                if (Id <= 0)
                {
                    var clinicType = new ClinicType();
                    clinicType.Id = Id;
                    clinicType.Name = name;

                    db.ClinicTypes.AddOrUpdate(clinicType);
                }
                else
                {
                    var oldClinicType = db.ClinicTypes.Where(x => x.Id == Id).FirstOrDefault();
                    if (oldClinicType != null)
                    {
                        oldClinicType.Name = name;
                        db.ClinicTypes.AddOrUpdate(oldClinicType);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Không tìm thấy loại phòng khám !" }, JsonRequestBehavior.AllowGet);
                    }
                }
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