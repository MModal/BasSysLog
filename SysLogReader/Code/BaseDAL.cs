using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysLogReader.Code
{
    public class BaseDAL
    {

        protected int ParseInt(object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return 0;
            }
            else if (source is int)
            {
                return (int)source;
            }
            else
            {
                return System.Convert.ToInt32(source);
            }
        }
        protected DateTime? ParseDT(object source)
        {
            if (source == null || source == DBNull.Value)
            {
                return null;
            }
            else if (source is DateTime)
            {
                return (DateTime?)source;
            }
            else
            {
                return DateTime.Parse(source.ToString());
            }
        }

    }
}