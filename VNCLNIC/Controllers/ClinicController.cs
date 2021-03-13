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
    public class ClinicController : BaseController
    {
        private readonly DataContext db;
        public ClinicController(DataContext db)
        {
            this.db = db;
        }
        // GET: Clinic
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public PartialViewResult Filter()
        {
            int items = 25;
            int page = 1;
            int status = 0;

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
            if (Request.Form.Get("status") != null)
            {
                status = int.Parse(Request.Form.Get("status").ToString());
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
            IQueryable<Clinic> clinics = null;
            #region  xây dựng header cho lưới
            string header = "";
            //các cột cần hiển thị
            List<GridColumn> columns = new List<GridColumn>();
            //Lấy tất cả thuộc tính 
            var properties = typeof(Clinic).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name.Equals("Name"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Tên phòng khám", Order = 2, AllowSorting = true });
                }
                if (item.Name.Equals("Address"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Địa chỉ", Order = 3, AllowSorting = true });
                }
                if (item.Name.Equals("Status"))
                {
                    columns.Add(new GridColumn() { Name = item.Name, Text = "Hoạt động ?", Order = 4, AllowSorting = true });
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
                    clinics = from c in db.Clinics orderby c.Name descending select c;
                else if (sortProperty.Equals("Status"))
                    clinics = from c in db.Clinics orderby c.Status descending select c;
                else
                    clinics = from c in db.Clinics orderby c.Address descending select c;
            }
            else
            {

                if (sortProperty.Equals("Name"))
                    clinics = from c in db.Clinics orderby c.Name ascending select c;
                else if (sortProperty.Equals("Status"))
                    clinics = from c in db.Clinics orderby c.Status ascending select c;
                else
                    clinics = from c in db.Clinics orderby c.Address ascending select c;
            }
            #endregion

            if (searchString.Length > 0)
            {
                clinics = clinics.Where(c => c.Name.ToString().ToLower().Contains(searchString.ToLower()));
            }

            if (status > 0)
            {
                if (status == 1)
                {
                    clinics = clinics.Where(c => c.Status == Common.Enums.ClinicStatus.ACTIVE);
                }
                else
                {
                    clinics = clinics.Where(c => c.Status == Common.Enums.ClinicStatus.INACTIVE);
                }
            }

            #region Phần phân trang		
            int count = clinics.Count();
            Paging paging = new Paging(page, items, count, "updatePage", "");
            ViewBag.Paging = paging.PageModel.HTML;
            ViewBag.Label = paging.PageModel.Label;

            var lsClinic = clinics.ToList();
            if (paging.PageModel.StartItem >= 1 && count > 0)
            {
                lsClinic = lsClinic.Skip(paging.PageModel.StartItem - 1).Take(paging.PageModel.StopItem - paging.PageModel.StartItem + 1).ToList();
            }
            #endregion
            ViewBag.GridHeader = header;
            return PartialView("_ListClinic", lsClinic);
        }

        [HttpGet]
        public PartialViewResult Detail(int? id)
        {
            //ViewBag.Permissions = db.Permissions.ToList();
            var clinic = db.Clinics.Where(x => x.Id == id).FirstOrDefault();

            return PartialView("_ClinicDetail", clinic);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewBag.City = db.Citys.ToList();
            var clinic = new Clinic();

            return PartialView("_AddClinic", clinic);
        }

        /// <summary>
        /// Lấy danh sách Tỉnh/Thành
        /// </summary>
        /// <returns>Danh sách tỉnh thành phố (id, text)</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult City()
        {
            var citys = db.Citys.ToList();
            //lấy distinct city của các store
            var json = (from f in citys
                        select new
                        {
                            id = f.Id,
                            text = f.Name
                        }).AsEnumerable().OrderBy(x => x.text).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách nhân viên phòng khám
        /// </summary>
        /// <returns>Danh sách nhân viên (id, text)</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ClinicUser()
        {
            var users = db.Users.Where(x => x.UserType == UserType.CLINIC).ToList();
            //lấy distinct user của các store
            var json = (from f in users
                        select new
                        {
                            id = f.Id,
                            text = f.FullName
                        }).AsEnumerable().OrderBy(x => x.text).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách Quận/Huyện theo Tỉnh/Thành
        /// </summary>
        /// <param name="CityId">City Id</param>
        /// <returns>Danh sách quận huyên (id, text)</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult District(int? CityId)
        {
            if (CityId != null)
            {
                var districts = db.Districts.Where(x => x.City.Id == CityId).ToList();
                //lấy distinct district của các store
                var json = (from f in districts
                            select new
                            {
                                id = f.Id,
                                text = f.Name
                            }).AsEnumerable().OrderBy(x => x.text).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var json = new
                {
                    id = "",
                    text = ""
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddClinic(string name, string address, string hotline, string phone, string email, int idCity, int idDistrict, bool status, bool isMorn, bool isAfter, bool isEven, string startMorn, string startAfter, string startEven, string stopMorn, string stopAfter, string stopEven, List<int> users)
        {
            try
            {
                var city = db.Citys.Where(x => x.Id == idCity).FirstOrDefault();
                var district = db.Districts.Where(x => x.Id == idDistrict).FirstOrDefault();
                string fulladdress = address + ", " + district.Name + ", " + city.Name;
                if (isMorn || isAfter || isEven)
                {
                    var newclinic = new Clinic
                    {
                        Name = name,
                        Address = fulladdress,
                        HotLine = hotline,
                        PhoneNumber = phone,
                        Email = email,
                        City = city,
                        District = district,
                        IsWorkMorning = isMorn,
                        IsWorAfternoon = isAfter,
                        IsWorEvening = isEven
                    };

                    if (status)
                    {
                        newclinic.Status = ClinicStatus.ACTIVE;
                    }
                    else
                    {
                        newclinic.Status = ClinicStatus.INACTIVE;
                    }

                    if (!isMorn)
                    {
                        newclinic.StartMorning = TimeSpan.Parse("00:00");
                        newclinic.StopMorning = TimeSpan.Parse("00:00");
                        newclinic.StartAfternoon = TimeSpan.Parse(startAfter);
                        newclinic.StopAfternoon = TimeSpan.Parse(stopAfter);
                        newclinic.StartEvening = TimeSpan.Parse(startEven);
                        newclinic.StopEvening = TimeSpan.Parse(stopEven);
                    }
                    else if (!isAfter)
                    {
                        newclinic.StartMorning = TimeSpan.Parse(startMorn);
                        newclinic.StopMorning = TimeSpan.Parse(stopMorn);
                        newclinic.StartAfternoon = TimeSpan.Parse("00:00");
                        newclinic.StopAfternoon = TimeSpan.Parse("00:00");
                        newclinic.StartEvening = TimeSpan.Parse(startEven);
                        newclinic.StopEvening = TimeSpan.Parse(stopEven);
                    }
                    else if (!isEven)
                    {
                        newclinic.StartMorning = TimeSpan.Parse(startMorn);
                        newclinic.StopMorning = TimeSpan.Parse(stopMorn);
                        newclinic.StartAfternoon = TimeSpan.Parse(startAfter);
                        newclinic.StopAfternoon = TimeSpan.Parse(stopAfter);
                        newclinic.StartEvening = TimeSpan.Parse("00:00");
                        newclinic.StopEvening = TimeSpan.Parse("00:00");
                    }
                    //if (users != null)
                    //{
                    //    foreach (var item in users)
                    //    {
                    //        var user = db.Users.Where(x => x.Id == item).FirstOrDefault();
                    //        foreach (var role in user.Roles)
                    //        {
                    //            var clinicRole = new ClinicRole
                    //            {
                    //                User = user,
                    //                Role = role,
                    //                Clinic = newclinic
                    //            };
                    //            db.ClinicRoles.AddOrUpdate(clinicRole);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    db.Clinics.AddOrUpdate(newclinic);
                    //}
                    db.Clinics.AddOrUpdate(newclinic);
                }
                else
                {
                    return Json(new { success = false, message = "Vui lòng chọn thời gian hoạt động" }, JsonRequestBehavior.AllowGet);
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