using DNA.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNA.Modules.TaskManager
{
    public class TaskManagerRegistration : SolutionModule
    {
        public override string Name
        {
            get { return "TaskManager"; }
        }

        //public override void RegisterTasks(Web.Scheduling.TaskCollection tasks)
        //{
        //    tasks.AddOnce<SimpleJob>("simplejob", "Write job name", DateTime.Now,
        //        jobData: new { JobName = "DotNetAge" });
        //}
    }

    //public class SimpleJob : Web.Scheduling.Job
    //{
    //    public string JobName { get; set; }

    //    protected override void OnExecute()
    //    {
    //        Logger.Info("This text is from:" + JobName);
    //    }
    //}
}