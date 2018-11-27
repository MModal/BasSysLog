using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SysLogReader.Code;
using SysLogReader.Models;

namespace SysLogReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Connection = SessionHelper.SelectedDatabase;

            var model = new Models.SysLogModel();
            model.RecordCount = 100;
            var x = new Code.SysLogDAL(SessionHelper.SelectedDatabase);

            try
            {
                model.Entries = x.GetLog((SysLogDAL.MsgType)null, model.RecordCount);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SysLogModel model)
        {
            //Don't know why the fuck we have to do this
            foreach (var msgType in model.MsgTypes)
            {
                if (msgType.Value == Request.Form["SelectedMsgType"])
                {
                    model.SelectedMsgType = msgType;
                }
            }

            ViewBag.Connection = SessionHelper.SelectedDatabase;

            var x = new Code.SysLogDAL(SessionHelper.SelectedDatabase);

            if (!string.IsNullOrEmpty(model.ID))
            {
                model.Entries = x.GetLogRecord(model.ID);
            }
            else if (!string.IsNullOrEmpty(model.Description))
            {
                model.Entries = x.GetLogByDesc(model.Description, model.RecordCount);
            }
            else
            {
                model.Entries = x.GetLog(model.SelectedMsgType, model.RecordCount);
            }
            
            return View(model);
        }

        public ActionResult Entry(string id)
        {
            Guid gid = new Guid(id);

            Code.dtoLogEntry model;
            var da = new Code.SysLogDAL(SessionHelper.SelectedDatabase);
            model = da.GetEntry(gid);

            return View(model);
        }

        public JsonResult Processed(string id)
        {
            Guid gid = new Guid(id);

            Code.dtoLogEntry model;
            var da = new Code.SysLogDAL(SessionHelper.SelectedDatabase);
            da.MarkProcessed(gid);

            return Json(new { success = true, msg = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt") });
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult JobTimes()
        {
            ViewBag.Connection = SessionHelper.SelectedDatabase;

            var model = new Models.BatchJobModel();
            var dal = new Code.BatchJobDAL(SessionHelper.SelectedDatabase);

            var jobTypes = dal.GetJobTypes();

            model.JobTypes = new List<BatchJobType>();
            foreach (var jobtype in jobTypes)
            {
                var jobs = dal.GetLog(jobtype.ExtSys, jobtype.JobType);
                if (jobs.Max(j => j.Time) > 1)
                {
                    var jobType = new BatchJobType();
                    jobType.JobTypeName = jobtype.ExtSys + "-" + jobtype.JobType;

                    object[,] a = new object[jobs.Count + 1,2];
                    a[0, 0] = "Day";
                    a[0, 1] = "Run Time";

                    int cnt = 1;
                    foreach (var job in jobs)
                    {
                        a[cnt, 0] = job.Day.Value.ToString("M/d");
                        a[cnt, 1] = job.Time;
                        cnt++;
                    }
                    jobType.data = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                    ViewBag.CommCountData = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                    model.JobTypes.Add(jobType);
                }
            }

            return View(model);
        }

        public ActionResult DailyJobCounts(string SelectedMonth)
        {
            ViewBag.Connection = SessionHelper.SelectedDatabase;
            var model = new Models.DailyJobModel();

            DateTime d = DateTime.Parse(DateTime.Today.ToString("M/1/yyyy"));

            model.MonthList = new List<xMonth>();
            model.MonthList.Add(new xMonth(d));
            model.SelectedMonth = model.MonthList[0];
            for (int x = -1; x > -6; x--)
            {
                model.MonthList.Add(new xMonth(d.AddMonths(x)));
            }

            if (SelectedMonth != null)
            {
                model.SelectedMonth = model.MonthList.Find(m => m.Date == DateTime.Parse(SelectedMonth));
            }

            var dal = new Code.WorkUnitDAL(SessionHelper.SelectedDatabase);

            var start = model.SelectedMonth.Date;
            var end = start.AddMonths(1);
            var Entries = dal.GetJobCounts(start, end);

            var extSysList = new List<string>();
            foreach (var source in Entries.Select(j => j.ExtSys).Distinct().ToList())
            {
                extSysList.Add(source);
            }

            int dayCount = 0;

            if (Entries.Count > 0) dayCount = Entries.Select(j => j.Day).Max().Value.Day;

            object[,] a = new object[dayCount + 1, extSysList.Count + 1];
            a[0, 0] = "Day";

            for (int x = 0; x < extSysList.Count; x++)
            {
                a[0, x + 1] = extSysList[x];
            }

            foreach (var job in Entries)
            {
                a[job.Day.Value.Day, 0] = job.Day.Value.ToString("M/d");
                a[job.Day.Value.Day, extSysList.IndexOf(job.ExtSys) + 1] = job.Count;
            }

            model.data = Newtonsoft.Json.JsonConvert.SerializeObject(a);

            return View(model);
        }

        public ActionResult SyncTimes(int? selectedDayCount, string batchJobType)
        {
            ViewBag.Connection = SessionHelper.SelectedDatabase;
            var model = new Models.SyncModel();
            model.DayCountOptions = new List<int>();
            model.DayCountOptions.Add(0);
            model.DayCountOptions.Add(1);
            model.DayCountOptions.Add(3);
            model.BatchJobTypes = new List<string>();
            model.BatchJobTypes.Add("DEPU");
            model.BatchJobTypes.Add("TCFO");
            model.BatchJobTypes.Add("INTC");

            if (selectedDayCount != null)
            {
                model.SelectedDayCount = selectedDayCount.Value;
            }
            if (batchJobType == null)
            {
                batchJobType = "DEPU";
            }
            model.BatchJobType = batchJobType;

            var dal = new Code.BatchJobDAL(SessionHelper.SelectedDatabase);

            var entries = dal.GetSyncTimes(batchJobType);
            var list = entries.ToList();

            if (model.SelectedDayCount > 0)
            {
                list = entries.Where(e => e.DaysRun == model.SelectedDayCount).ToList();
            }

            object[,] a = new object[entries.Count + 1, 2];
            a[0, 0] = "Day";
            a[0, 1] = "Jobs Per Minute";
            object[,] b = new object[entries.Count + 1, 2];
            b[0, 0] = "Day";
            b[0, 1] = "Run time (in min)";
            int cnt = 1;
            foreach (var time in list)
            {
                a[cnt, 0] = time.Day.Value.ToString("M/d");
                a[cnt, 1] = time.JobsPerMinute;
                b[cnt, 0] = time.Day.Value.ToString("M/d");
                b[cnt, 1] = time.RunMinutes <0 ? 0 :time.RunMinutes;
                cnt++;
            }

            model.data = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            model.data2 = Newtonsoft.Json.JsonConvert.SerializeObject(b);

            return View(model);
        }

        public ActionResult OverallSyncTimes(int? days)
        {
            if (days == null || days == 0) days = 7;

            ViewBag.Connection = SessionHelper.SelectedDatabase;
            ViewBag.NoData = "";
            var model = new Models.OverallSyncTimeModel();

            var dal = new Code.BatchJobDAL(SessionHelper.SelectedDatabase);

            var jobTypes = dal.GetJobTypes();

            model.data = new List<object>();
            model.names = new List<string>();
            foreach (var jobtype in jobTypes)
            {
                var entries = dal.GetSyncTimes2(jobtype.JobType, jobtype.ExtSys, days.Value);

                if (entries.Count > 0)
                {
                    if (entries.Max(e => e.RunMinutes) > 5)
                    {
                        var list = entries.ToList();

                        object[,] a = new object[entries.Count + 1, 2];
                        a[0, 0] = "Day";
                        a[0, 1] = "Run time (in min) [" + jobtype.ExtSys + "," + jobtype.JobType + "]";

                        int cnt = 1;
                        foreach (var time in list)
                        {
                            a[cnt, 0] = time.Day.Value.ToString("M/d");
                            a[cnt, 1] = time.RunMinutes < 0 ? 0 : time.RunMinutes;
                            cnt++;
                        }

                        model.data.Add(Newtonsoft.Json.JsonConvert.SerializeObject(a));
                        model.names.Add(jobtype.Description + "[" + jobtype.ExtSys + "," + jobtype.JobType + "]");
                    }
                }
                else
                {
                    ViewBag.NoData += jobtype.JobType + "," + jobtype.ExtSys + "  ||  ";
                }
            }

            return View(model);
        }
    
        [HttpPost]
        public ActionResult DbSelect(DbSelectModel model)
        {
            Session["SelectedDatabase"] = Request.Form["Selected"];

            return Redirect("~/");
        }

    }
}
