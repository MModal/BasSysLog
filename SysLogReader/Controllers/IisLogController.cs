using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SysLogReader.Code;

namespace SysLogReader.Controllers
{
    public class IisLogController : Controller
    {
        //
        // GET: /IisLog/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadLog()
        {
            var reader = new IisLogReader();
            var d = new DirectoryInfo(@"C:\Temp\IIS Log");
            foreach (var file in d.GetFiles())
            {
                reader.ReadLog(file.FullName);
            }
            reader.Close();

            return View();
        }

    }
}
