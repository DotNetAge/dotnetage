//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web;
using DNA.Web.Scheduling;
using DNA.Web.ServiceModel;
using DNA.Web.ServiceModel.Tasks;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DNA.Modules.TaskManager.Controllers
{
    public class SchedulerController : Controller
    {
        [HostDashboard(Text = "Task Scheduler", Sequence = 6, Icon = "d-icon-clock")]
        public ActionResult Index()
        {
            var workers = Scheduler.LoadWorkers();
            return View(workers);
        }

        [HostDashboard]
        public ActionResult List(TaskStates state)
        {
            var workers = Scheduler.LoadWorkers().Where(w => w.State.Equals(state));
            ViewBag.TaskState = state;
            return PartialView(workers);
        }

        [HostDashboard]
        public ActionResult Create()
        {
            return PartialView();
        }

        [HostOnly, HttpPost]
        public ActionResult Create(TaskDescriptor task, FormCollection forms)
        {
            var recurringDaysOfMonth = forms["RecurringDaysOfMonth"];
            var recurringDaysOfWeek = forms["RecurringDaysOfWeek"];
            var recurringMonths = forms["RecurringMonths"];
            var jobType = forms["JobType"];

            if (task.Frequency == Frequencies.Weekly)
            {
                if (recurringDaysOfWeek != null && recurringDaysOfWeek.Length > 0)
                    task.RecurringDaysOfWeek = recurringDaysOfWeek.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
            }

            if (task.Frequency == Frequencies.Monthly)
            {
                if (recurringDaysOfMonth != null && recurringDaysOfMonth.Length > 0)
                    task.RecurringDaysOfMonth = recurringDaysOfMonth.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

                if (recurringMonths != null && recurringMonths.Length > 0)
                    task.RecurringMonths = recurringMonths.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
            }

            if (!string.IsNullOrEmpty(jobType))
            {
                task.JobType = Type.GetType(jobType);
                var props = task.JobType.GetProperties();
                task.CommandData = new System.Collections.Generic.Dictionary<string, object>();

                foreach (var pro in props)
                {
                    var key = task.JobType.Name + "." + pro.Name;
                    if (forms[key] != null)
                        task.CommandData.Add(pro.Name, Convert.ChangeType(forms[key], pro.PropertyType));
                }
            }

            task.Name = DNA.Utility.TextUtility.Slug(task.JobType.FullName + "." + System.IO.Path.GetRandomFileName().Replace(".", ""));
            Scheduler.AddTask(task);
            return PartialView();
        }

        [HostDashboard]
        public ActionResult Edit(string id)
        {
            var workItem = Scheduler.GetWorker(id);
            return PartialView(workItem);
        }

        [HostOnly, HttpPost]
        public ActionResult Edit(string id, string title, Frequencies frequency, int recurs, DateTime startAt, FormCollection forms)
        {
            var workItem = Scheduler.GetWorker(id);
            var recurringDaysOfMonth = forms["RecurringDaysOfMonth"];
            var recurringDaysOfWeek = forms["RecurringDaysOfWeek"];
            var recurringMonths = forms["RecurringMonths"];

            if (workItem.Frequency == Frequencies.Weekly)
            {
                if (recurringDaysOfWeek != null && recurringDaysOfWeek.Length > 0)
                    workItem.RecurringDaysOfWeek = recurringDaysOfWeek.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
            }

            if (workItem.Frequency == Frequencies.Monthly)
            {
                if (recurringDaysOfMonth != null && recurringDaysOfMonth.Length > 0)
                    workItem.RecurringDaysOfMonth = recurringDaysOfMonth.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

                if (recurringMonths != null && recurringMonths.Length > 0)
                    workItem.RecurringMonths = recurringMonths.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
            }

            var props = workItem.JobType.GetProperties();
            workItem.CommandData = new System.Collections.Generic.Dictionary<string, object>();

            foreach (var pro in props)
            {
                var key = workItem.JobType.Name + "." + pro.Name;
                if (forms[key] != null)
                    workItem.CommandData.Add(pro.Name, Convert.ChangeType(forms[key], pro.PropertyType));
            }

            workItem.Recurs = recurs;
            workItem.StartAt = startAt;
            workItem.NextStart = workItem.GetNextStart(startAt);

            Scheduler.Update(workItem);
            return PartialView();
        }

        [HostOnly, HttpPost]
        public void Start()
        {
            if (!Scheduler.IsRunning)
                Scheduler.Start();
        }

        [HostOnly, HttpPost]
        public void Stop()
        {
            if (Scheduler.IsRunning)
                Scheduler.Stop();
        }

        //[HostOnly, HttpPost]
        //public ActionResult Stop(string id)
        //{
        //    var workItem = Scheduler.GetWorker(id);
        //    if (workItem.Cancellation != null && workItem.Cancellation.Token != null && workItem.Cancellation.Token.CanBeCanceled)
        //        workItem.Cancellation.Cancel();
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        [HostOnly, HttpPost]
        public ActionResult Delete(string id)
        {
            Scheduler.Remove(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Configure(string type, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var works = Scheduler.LoadWorkers();
                var workItem = works.FirstOrDefault(w => w.Name.Equals(name));
                if (workItem != null)
                {
                    var job = (Job)workItem.GetJob();
                    if (!string.IsNullOrEmpty(job.ConfigureView))
                        return PartialView(job.ConfigureView, job);
                }
            }
            else
            {
                var deCodeType = Server.UrlDecode(type);
                var jt = Type.GetType(deCodeType);
                var jobs = Scheduler.GetAllJobs();
                var job = jobs.FirstOrDefault(j => j.GetType().Equals(jt));
                if (!string.IsNullOrEmpty(job.ConfigureView))
                    return PartialView(job.ConfigureView);
            }
            return Content("");
        }

    }
}
