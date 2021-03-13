using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VNCLNIC.Auth
{
    public class PermissionBasedAuthorize : AuthorizeAttribute
    {
        private List<string> screen { get; set; }

        public PermissionBasedAuthorize(string ScreenNames)
        {
            if (!string.IsNullOrEmpty(ScreenNames))
                screen = ScreenNames.Split(',').ToList();
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var requestingUser = (CustomPrincipal)HttpContext.Current.User;

            foreach (var item in screen)
            {
                if (!requestingUser.IsInPermission(item))
                {
                    filterContext.Result = new RedirectToRouteResult(
                                              new RouteValueDictionary {
                                                { "action", "Index" },
                                                { "controller", "Unpermissioned" } });
                }
            }


        }
    }
}