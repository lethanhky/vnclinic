using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VNCLNIC.Common.Tools;

namespace VNCLNIC.Controllers
{
    public class ResourceController : Controller
    {

        public byte[] ImageToByteArray(string path)
        {
            try
            {
                return System.IO.File.ReadAllBytes(path);
            }
            catch
            {
                return System.IO.File.ReadAllBytes(Server.MapPath("~/Content/image/person-512.png"));
            }
        }

        public ActionResult VCIMG(string appx)
        {
            string filename = appx;
            string filepath = FileConfig.UploadPath + filename;
            return this.File(ImageToByteArray(filepath), "image/png", "image.png");
        }
    }
}