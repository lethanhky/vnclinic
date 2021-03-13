using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNCLNIC.Common.Tools;
using VNCLNIC.Data;
using VNCLNIC.Models;
using VNCLNIC.Models.Data;
using VNCLNIC.ViewModels;

namespace VNCLNIC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DataContext db;
        public HomeController(DataContext db)
        {
            this.db = db;
        }
        // GET: Home
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.City = db.Citys.ToList();
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Booking()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult BookingStep()
        {
            ViewBag.City = db.Citys.ToList();
            return PartialView("_3SelectDoctor");
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

        /// <summary>
        /// Tìm kiếm phòng khám
        /// </summary>
        /// <param name="searchHome">Key search</param>
        /// <param name="CityHome">City Id</param>
        /// <param name="DistrictHome">District Id</param>
        /// <returns>Danh sách phòng khám</returns>
        [HttpGet]
        [AllowAnonymous]
        public PartialViewResult Search(string searchHome, int? CityHome, int? DistrictHome, string ClinicTypeHome, int? selPage, int? type)
        {
            int items = 10;
            int page = 1;
            
            if (selPage != null)
            {
                page = int.Parse(selPage.ToString());
            }

            var vnClinic = new List<Clinic>();
            IEnumerable<Clinic> lsClinic = null;
            if (CityHome != null && CityHome > 0)
            {
                lsClinic = db.Clinics.Where(x => x.City.Id == CityHome);
            }
            if (DistrictHome != null && DistrictHome > 0)
            {
                if (lsClinic != null)
                {
                    lsClinic = lsClinic.Where(x => x.District.Id == DistrictHome);
                }
                else
                {
                    lsClinic = db.Clinics.Where(x => x.District.Id == DistrictHome);
                }
            }
            if (searchHome != null && searchHome.Trim() != "")
            {
                if (lsClinic != null)
                {
                    lsClinic = lsClinic.Where(x => x.Name.ToLower().Contains(searchHome.ToLower()) || x.Address.ToLower().Contains(searchHome.ToLower()));
                }
                else
                {
                    lsClinic = db.Clinics.Where(x => x.Name.ToLower().Contains(searchHome.ToLower()) || x.Address.ToLower().Contains(searchHome.ToLower()));
                }
            }

            var ListClinicTypeChecked = new List<int>();

            if (ClinicTypeHome != null && ClinicTypeHome != "")
            {
                var ListClinicType = ClinicTypeHome.Split(',').Where(x => int.Parse(x) > 0).ToArray();

                foreach (var item in ListClinicType)
                {
                    ListClinicTypeChecked.Add(int.Parse(item));
                }
            }

            var LsClinicType = new List<ClinicType>();
            var lsClinicTypeBox = new List<ClinicTypeBoxViewModel>();

            if (lsClinic != null)
            {
                LsClinicType = lsClinic.Select(x => x.ClinicType).Distinct().ToList();
            }

            if (ListClinicTypeChecked.Count() > 0)
            {
                if (lsClinic != null)
                {
                    lsClinic = lsClinic.Where(x => ListClinicTypeChecked.Any(a => a == x.ClinicType.Id));
                }
                else
                {
                    lsClinic = db.Clinics.Where(x => ListClinicTypeChecked.Any(a => a == x.ClinicType.Id));
                }

            }
            int count = 0;
            if (lsClinic != null)
            {
                count = lsClinic.Count();
                vnClinic = lsClinic.ToList();
            }

            Paging paging = new Paging(page, items, count, "updatePageCustom", "");
            ViewBag.Paging = paging.PageModel.HTML;
            ViewBag.Label = paging.PageModel.Label;


            foreach (var item in LsClinicType)
            {
                var clinicTypeBox = new ClinicTypeBoxViewModel();
                clinicTypeBox.Id = item.Id;
                clinicTypeBox.Name = item.Name;
                clinicTypeBox.IsChecked = false;
                if (ListClinicTypeChecked.Count() > 0)
                {
                    if (ListClinicTypeChecked.Any(x => x == item.Id))
                    {
                        clinicTypeBox.IsChecked = true;
                    }
                }
                lsClinicTypeBox.Add(clinicTypeBox);
            }

            ViewBag.ClincType = lsClinicTypeBox;
            ViewBag.Type = type;
            if (paging.PageModel.StartItem >= 1 && count > 0)
            {
                vnClinic = vnClinic.Skip(paging.PageModel.StartItem - 1).Take(paging.PageModel.StopItem - paging.PageModel.StartItem + 1).ToList();
            }

            return PartialView("_ListClinic", vnClinic);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SearchMap( float? Lat, float Lng)
        {
            ViewBag.Lat = Lat;
            ViewBag.Lng = Lng;
            return PartialView("_Map");
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult Cities()
        {
            var cities = db.Citys.ToList();
            //lấy distinct district của các store
            var json = (from f in cities
                        select new
                        {
                            id = f.Id,
                            text = f.Name
                        }).AsEnumerable().OrderBy(x => x.text).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult Departments()
        {
            var departments = db.Departments.ToList();
            var json = (from f in departments
                        select new
                        {
                            id = f.Id,
                            text = f.Name
                        }).AsEnumerable().OrderBy(x => x.text).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}