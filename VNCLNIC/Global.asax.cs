using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI.WebControls;
using VNCLNIC.Auth;

namespace VNCLNIC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterComponents();
        }
        protected void Application_Error(object sender_, CommandEventArgs e_)
        {
            Exception exception = Server.GetLastError();
            if (exception is CryptographicException)
            {
                FormsAuthentication.SignOut();
            }
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {

                String keyCookie = ConfigurationManager.AppSettings["cookie"];
                HttpCookie authCookie = Request.Cookies[keyCookie];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

                    CustomPrincipal principal = new CustomPrincipal(authTicket.Name);

                    principal.FullName = serializeModel.FullName;
                    principal.Image = serializeModel.Image;
                    principal.UserId = serializeModel.UserId;
                    principal.Email = serializeModel.Email;
                    principal.Roles = serializeModel.RoleName.ToArray<string>();
                    principal.Permissions = serializeModel.PermissionList.ToArray();
                    principal.IsRoot = serializeModel.IsRoot;

                    HttpContext.Current.User = principal;
                }
            }
            catch (CryptographicException cex)
            {
                FormsAuthentication.SignOut();
            }
        }
    }
}
