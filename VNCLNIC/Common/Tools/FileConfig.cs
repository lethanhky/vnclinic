using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace VNCLNIC.Common.Tools
{
    public class FileConfig
    {
        /// <summary>
        /// Đường dẫn chứa các file upload
        /// </summary>
        public static string UploadPath = ConfigurationManager.AppSettings["uploadPath"];

        public static string ImageToBase64(string filePath)
        {
            string path = filePath;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    string content = Convert.ToBase64String(imageBytes);
                    if (content != null)
                        return content;
                }
            }
            return null;
        }

        public VCFileInfo GetFinalName(string uploadingPath, string fileExtension)
        {
            DateTime now = DateTime.Now;
            string name = "";
            name += (now.Day < 10) ? "0" + now.Day.ToString() : now.Day.ToString();
            name += (now.Month < 10) ? "0" + now.Month.ToString() : now.Month.ToString();
            name += now.Year.ToString();
            string relativePath = uploadingPath + name;
            if (!Directory.Exists(relativePath))
            {
                Directory.CreateDirectory(relativePath);
            }
            string uniqueName = UniqueName();
            while (File.Exists(relativePath + uniqueName))
            {
                uniqueName = UniqueName();
            }
            return new VCFileInfo() { FileName = uniqueName + fileExtension, DirectoryContain = name };
        }

        public string UniqueName()
        {
            string hashed = MD5Hash(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
            if (hashed.Length > 20)
                return hashed.Substring(0, 20);
            return hashed;
        }

        public string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }
    public class VCFileInfo
    {
        /// <summary>
        /// Tên thư mục chứa file
        /// </summary>
        public string DirectoryContain
        {
            get; set;
        }
        /// <summary>
        /// Tên file
        /// </summary>
        public string FileName
        {
            get; set;
        }
    }
}