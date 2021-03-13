using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VNCLNIC.Auth;
using VNCLNIC.Common.Tools;
using VNCLNIC.Data;
using VNCLNIC.Models;

namespace VNCLNIC.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private string keyCookie = ConfigurationManager.AppSettings["cookie"];

        private readonly CustomMembership membership;
        private readonly DataContext db;

        public AccountController(CustomMembership membership, DataContext db)
        {
            this.membership = membership;
            this.db = db;
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        


        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel, string ReturnUrl = "")
        {
            var userip = ToolHelper.GetIp();
            if (ModelState.IsValid)
            {
                bool isActive = false;
                if (membership.ValidateUser(loginViewModel.Username, loginViewModel.Password, out isActive))
                {
                    if (isActive)
                    {
                        var user = membership.GetUser(loginViewModel.Username, false);
                        if (user.IsActive)
                        {
                            if (user != null)
                            {
                                AurthenticateSupport authenSupport = new AurthenticateSupport(user);
                                CustomSerializeModel userModel = new CustomSerializeModel()
                                {
                                    FullName = user.FullName,
                                    Image = user.Image,
                                    UserId = user.Id,
                                    Email = user.Username,
                                    RoleName = user.Roles.Select(r => r.RoleName).ToList(),
                                    IsRoot = authenSupport.IsRoot,
                                    PermissionList = authenSupport.PermissionList.Select(p => p.PermissionCode).ToList()
                                };

                                string userData = JsonConvert.SerializeObject(userModel);
                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                                (
                                  1, loginViewModel.Username, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
                                );
                                string enTicket = FormsAuthentication.Encrypt(authTicket);
                                if (loginViewModel.IsRemember)
                                {

                                    HttpCookie faCookie = new HttpCookie(keyCookie, enTicket);
                                    FormsAuthentication.SetAuthCookie(loginViewModel.Username, loginViewModel.IsRemember);
                                    Response.Cookies.Add(faCookie);
                                }
                                else
                                {
                                    HttpCookie cookie = Request.Cookies[keyCookie];
                                    if (cookie != null)
                                    {
                                        cookie.Expires = DateTime.Now.AddHours(48);
                                    }
                                    else
                                    {
                                        cookie = new HttpCookie(keyCookie, enTicket);
                                        cookie.Expires = DateTime.Now.AddHours(48);
                                    }
                                    Response.Cookies.Add(cookie);
                                }

                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Tài khoản của bạn đang bị khóa !");
                            return View(loginViewModel);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tài khoản của bạn chưa được cấp quyền !");
                        return View(loginViewModel);
                    }

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Thông tin đăng nhập bị sai !");
            return View(loginViewModel);
        }



        [HttpPost]
        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie(keyCookie, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

        public ActionResult ForgotPassword()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Username)
        {
            MailHelper mailer = new MailHelper();
            string message = "";
            var user = db.Users.Where(s => s.Username == Username).FirstOrDefault();
            if (user != null)
            {
                if (user.IsActive)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    var resetUrl = "/Account/ResetPassword/" + resetCode;
                    var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, resetUrl);
                    /// Config mail với host của Gmail
                    //var mailConfig = unitWork.MailConfig.Get(c => c.Default == true).FirstOrDefault();
                    string subject = "Thiết lập lại mật khẩu";
                    string content = "Xin chào, <br /><br />Chúng tôi vừa nhận yêu cầu thiết lập lại mật khẩu cho bạn. Vui lòng nhấn vào đường link bên dưới để đặt lại mật khẩu."
                                + "<br /><br /><a href=" + link + ">Đường dẫn thiết lập mật khẩu</a>";
                    //MailHelper.SendAsyncEmail(mailConfig.ServerAddress, mailConfig.Port, 0, mailConfig.UseSSL, mailConfig.EmailSend, Username, subject, content, mailConfig.Username, mailConfig.Password, mailConfig.EmailCC, null);
                    user.ResetPasswordCode = resetCode;
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    message = "Đường dẫn thiết lập lại mật khẩu đã được gởi đến email của bạn.";
                }
                else
                {
                    message = "Tài khoản của bạn đang bị khóa. Vui lòng liên hệ quản trị viên.";
                }
            }
            else
            {
                message = "Không tìm thấy người dùng.";
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.Users.Where(u => u.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordViewModel resetPasswordVM = new ResetPasswordViewModel();
                resetPasswordVM.Code = id;
                return View(resetPasswordVM);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(s => s.ResetPasswordCode == model.Code).FirstOrDefault();
                if (user != null)
                {
                    user.Password = Utilities.Encrypt(model.Password);
                    user.ResetPasswordCode = "";
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    message = "New password updated successfully";
                }
                else
                {
                    message = "Something invalid";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ConfirmPassword(string id)
        {
            var user = db.Users.Where(u => u.ConfirmPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordViewModel resetPasswordVM = new ResetPasswordViewModel();
                resetPasswordVM.Code = id;
                return View(resetPasswordVM);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPassword(ResetPasswordViewModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(s => s.ConfirmPasswordCode == model.Code).FirstOrDefault();
                if (user != null)
                {
                    user.Password = Utilities.Encrypt(model.Password);
                    user.ConfirmPasswordCode = "";
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    message = "New password updated successfully";
                }
                else
                {
                    message = "Something invalid";
                }
            }
            ViewBag.Message = message;
            return View();
        }



        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = db.Users.Where(x => x.Id == id.Value).FirstOrDefault();

            if (user != null)
            {
                if (user.Id == User.UserId)
                {
                    return PartialView("_ChangePassword",new ChangePasswordViewModel { Id = user.Id });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var user = db.Users.Where(x => x.Id == model.Id).FirstOrDefault();
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng !" }, JsonRequestBehavior.AllowGet);
                }
                if (Utilities.Encrypt(model.OldPassword) != user.Password)
                {
                    return Json(new { success = false, message = "Sai mật khẩu, vui lòng nhập lại !" }, JsonRequestBehavior.AllowGet);
                }
                user.Password = Utilities.Encrypt(model.NewPassword);
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
                return Json(new { success = true, message = "Thay đổi mật khẩu thành công !" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã có lỗi xảy ra" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}