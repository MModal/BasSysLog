using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysLogReader
{
    public class SessionHelper
    {
        public static string SelectedDatabase
        {
            get
            {
                if (HttpContext.Current.Session["SelectedDatabase"] == null)
                {
                    return "SBTEST";
                }

                return HttpContext.Current.Session["SelectedDatabase"].ToString();
            }
        }
    }
}