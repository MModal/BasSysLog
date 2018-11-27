using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SysLogReader.Code;

namespace SysLogReader.Models
{
    public class SysLogModel
    {
        public List<Code.dtoLogEntry> Entries { get; set; }
        //public SysLogDAL.MessageType SelectedMsgType;

        //public SelectList MessageTypeList 
        //{
        //    get
        //    {
        //        var statuses = from SysLogDAL.MessageType s in Enum.GetValues(typeof(SysLogDAL.MessageType))
        //                        select new { ID = s, Name = s.ToString() };
        //        return new SelectList(statuses, "Id", "Name");
        //    }
        //}

        public List<SysLogDAL.MsgType> MsgTypes { get; set; }
        public SysLogDAL.MsgType SelectedMsgType { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        public int RecordCount { get; set; }

        public SysLogModel ()
        {
            MsgTypes = SysLogDAL.MsgTypes;
            MsgTypes.Insert(0,new SysLogDAL.MsgType(){Name="All",Value=null});
        }
    }

    public class BatchJobType
    {
        public string JobTypeName { get; set; }
        public object data { get; set; }
    }
    public class BatchJobModel
    {
        public List<BatchJobType> JobTypes;
    }

    public class xMonth
    {
        public xMonth(DateTime d)
        {
            Date = d;
        }
        public DateTime Date { get; set; }

        public string Display
        {
            get { return Date.ToString("MMM yyyy"); }
        }
    }
    public class DailyJobModel
    {
        public object data { get; set; }
        public List<xMonth> MonthList { get; set; }

        public xMonth SelectedMonth { get; set; }
    }

    public class SyncModel
    {
        public object data { get; set; }
        public object data2 { get; set; }
        public int SelectedDayCount { get; set; }
        public List<int> DayCountOptions { get; set; }
        public string BatchJobType { get; set; }
        public List<string> BatchJobTypes { get; set; }
    }

    public class OverallSyncTimeModel
    {
        public List<object> data { get; set; }
        public List<string> names { get; set; }
    }

    public class DbCnn
    {
        public string DbName { get; set; }
        public string Connection { get; set; }
    }

    public class DbSelectModel
    {
        public DbSelectModel()
        {
            
        }
        public DbSelectModel(object selectedDatabaseValue)
        {
            if (selectedDatabaseValue != null)
            {
                foreach (var database in Databases)
                {
                    if (database.Connection == selectedDatabaseValue.ToString())
                    {
                        this.SelectedDatabase = database;
                        this.Selected = database.DbName;
                    }
                }
            }
            else
            {
                this.SelectedDatabase = Databases[1];
                this.Selected = Databases[1].DbName;
            }
        }

        public List<DbCnn> Databases = new List<DbCnn>()
                {
                    new DbCnn(){DbName = "SBTEST", Connection = "SBTEST"},
                    new DbCnn(){DbName = "SBPROD", Connection = "SBPROD"},
                };

        public DbCnn SelectedDatabase { get; set; }
        public string Selected { get; set; }
    }
}