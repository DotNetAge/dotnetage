using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.Scheduling
{
    /// <summary>
    /// Represents the task collection class use to register task in module.
    /// </summary>
    public class TaskCollection : IEnumerable<TaskDescriptor>
    {
        private List<TaskDescriptor> InnerList { get; set; }

        public TaskCollection() { InnerList = new List<TaskDescriptor>(); }

        /// <summary>
        /// Add OneTime Task
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddOnce<TJob>(string name, string title, DateTime startAt, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.OneTime, desc, jobData: jobData);
        }

        /// <summary>
        ///  Add daily task
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="startAt"></param>
        /// <param name="recurs"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddDaily<TJob>(string name, string title, DateTime startAt, int recurs = 1, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.Daily, desc, recurs, jobData: jobData);
        }

        /// <summary>
        /// Add hourly job
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="startAt"></param>
        /// <param name="recurs"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddHourly<TJob>(string name, string title, DateTime startAt, int recurs = 1, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.Hourly, desc, recurs, jobData: jobData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="startAt"></param>
        /// <param name="recurs"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddMinutely<TJob>(string name, string title, DateTime startAt, int recurs = 1, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.Minutely, desc, recurs, jobData: jobData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="startAt"></param>
        /// <param name="recurs"></param>
        /// <param name="recurringDaysOfWeek"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddWeekly<TJob>(string name, string title, DateTime startAt, int recurs = 1, int[] recurringDaysOfWeek = null, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.Weekly, desc, recurs, recurringDaysOfWeek, jobData: jobData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="startAt"></param>
        /// <param name="recurringMonths"></param>
        /// <param name="recurringDaysOfMonth"></param>
        /// <param name="desc"></param>
        /// <param name="jobData"></param>
        /// <returns></returns>
        public TaskDescriptor AddMonthly<TJob>(string name, string title, DateTime startAt, int[] recurringMonths, int[] recurringDaysOfMonth = null, string desc = "", object jobData = null)
            where TJob : Job
        {
            return Add<TJob>(name, title, startAt, Frequencies.Monthly, desc, recurringDaysOfMonth: recurringDaysOfMonth, recurringMonths: recurringMonths, jobData: jobData);
        }

        private TaskDescriptor Add<TJob>(string name, string title, DateTime startAt,
            Frequencies frequency = Frequencies.OneTime,
            string desc = "",
            int recurs = 1,
            int[] recurringDaysOfWeek = null,
            int[] recurringMonths = null,
            int[] recurringDaysOfMonth = null,
            object jobData = null) where TJob : Job
        {
            var internalName = Utility.TextUtility.Slug(typeof(TJob).FullName + "." + name);

            if (InnerList.Count(l => l.Name.Equals(internalName, StringComparison.OrdinalIgnoreCase)) > 0)
                throw new Exception(string.Format("The task named '{0}' is register", name));

            var task = new TaskDescriptor()
            {
                Title = title,
                Description = desc,
                Name = internalName,
                Recurs = recurs,
                RecurringDaysOfWeek = recurringDaysOfWeek,
                RecurringMonths = recurringMonths,
                RecurringDaysOfMonth = recurringDaysOfMonth,
                JobType = typeof(TJob),
                StartAt = startAt,
                Frequency = frequency
            };

            if (jobData != null)
            {
                var dict = jobData.ToDictionary();
                task.CommandData = new Dictionary<string, object>();
                foreach (var key in dict.Keys)
                    task.CommandData.Add(key, dict[key]);
            }

            this.InnerList.Add(task);
            return task;
        }

        IEnumerator<TaskDescriptor> IEnumerable<TaskDescriptor>.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }
    }
}
