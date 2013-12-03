//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Patterns.Commands;
using DNA.Web.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace DNA.Web.ServiceModel.Tasks
{
    /// <summary>
    /// Represetnts a workitem properties and use to execute command.
    /// </summary>
    public class WorkItem : TaskDescriptor
    {
        private Job job = null;

        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Gets/ Sets the config file name
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets/Sets the next start time when the workitem is repeatable.
        /// </summary>
        public DateTime NextStart { get; set; }

        /// <summary>
        /// Gets/Sets the work last run time.
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// Gets Job instance.
        /// </summary>
        /// <returns></returns>
        public Job GetJob()
        {
            //var type = Type.GetType(CommandType);
            var _job = (Job)Activator.CreateInstance(JobType);
            _job.Data = CommandData;

            if (CommandData != null && CommandData.Count > 0)
            {
                var props = JobType.GetProperties().Where(p => p.CanRead && p.CanWrite);
                foreach (var key in CommandData.Keys)
                {
                    if (CommandData[key] == null)
                        continue;

                    var prop = props.FirstOrDefault(p => p.Name.Equals(key));

                    if (prop != null)
                    {
                        try
                        {
                            var val = Convert.ChangeType(CommandData[key], prop.PropertyType);
                            prop.SetValue(_job, val, null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                }
            }

            return _job;
        }

        /// <summary>
        /// Gets the work item state.
        /// </summary>
        public TaskStates State { get; private set; }

        /// <summary>
        /// Put the work item to work.
        /// </summary>
        public void Run()
        {
            this.State = TaskStates.Running;
            if (job == null)
                job = GetJob();

            try
            {
                job.Execute();
                this.LastRunTime = DateTime.Now;
            }
            catch (Exception e)
            {
                this.State = TaskStates.Stop;

                //Fail and retry ? Should we need to handle the retry ?
                job.OnError(e);
                Logger.Error(e);
            }

            //Counter++;
            if (Frequency == Frequencies.OneTime)
                this.State = TaskStates.Completed;
            else
            {
                StartAt = NextStart;
                this.State = TaskStates.Ready;
                NextStart = GetNextStart(StartAt);
            }
            this.Save();
        }

        /// <summary>
        /// Initializes a new instance of the WorkItem class.
        /// </summary>
        public WorkItem()
        {
            DateTime startAt = DateTime.MinValue;
            DateTime nextStart = DateTime.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the WorkItem class with workitem definition xml file.
        /// </summary>
        /// <param name="url">The definition xml file name.</param>
        public WorkItem(string url)
        {
            this.Read(url);
        }

        /// <summary>
        /// Read properties from specified url.
        /// </summary>
        /// <param name="url"></param>
        public void Read(string url)
        {
            var xmlDoc = XDocument.Load(url);
            var ns = xmlDoc.Root.GetDefaultNamespace();

            DateTime startAt = DateTime.MinValue;
            DateTime nextStart = DateTime.MinValue;
            var root = xmlDoc.Root;
            CommandData = new Dictionary<string, object>();
            DateTime.TryParse(root.Element(ns + "startAt").Value, out startAt);
            DateTime.TryParse(root.Element(ns + "nextStart").Value, out nextStart);

            var fstr = root.Element(ns + "recurs").StrAttr("frequency");

            if (string.IsNullOrEmpty(this.Name))
                this.Name= System.IO.Path.GetFileNameWithoutExtension(url);
            
            this.Frequency = !string.IsNullOrEmpty(fstr) ? (Frequencies)Enum.Parse(typeof(Frequencies), fstr) : Frequencies.Minutely;
            this.JobType = Type.GetType(root.Element(ns + "command").StrAttr("type"));
            this.Recurs = root.Element(ns + "recurs").IntAttr("value");

            if (root.Element(ns + "title") != null)
                this.Title = root.Element(ns + "title").Value;

            if (root.Element(ns + "desc") != null)
                this.Description = root.Element(ns + "desc").Value;

            if (root.Element(ns + "state") != null)
            {
                var stateStr = root.Element(ns + "state").Value;
                if (!string.IsNullOrEmpty(stateStr))
                {
                    this.State = (TaskStates)Enum.Parse(typeof(TaskStates), stateStr);
                }
            }

            var recurringDaysOfWeek = root.Element(ns + "recurs").StrAttr("daysOfWeek");
            var recurringDaysOfMonth = root.Element(ns + "recurs").StrAttr("daysOfMonth");
            var recurringMonths = root.Element(ns + "recurs").StrAttr("months");

            if (!string.IsNullOrEmpty(recurringDaysOfWeek))
                this.RecurringDaysOfWeek = recurringDaysOfWeek.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            if (!string.IsNullOrEmpty(recurringDaysOfMonth))
                this.RecurringDaysOfWeek = recurringDaysOfMonth.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            if (!string.IsNullOrEmpty(recurringMonths))
                this.RecurringMonths = recurringMonths.Split(',').Select(a => Convert.ToInt32(a)).ToArray();

            foreach (var param in root.Element(ns + "command").Elements())
                CommandData.Add(param.StrAttr("name"), param.Value);

            this.StartAt = startAt;
            this.NextStart = nextStart;
            this.File = url;
        }

        /// <summary>
        /// Save the properties to data file.
        /// </summary>
        public void Save()
        {
            XNamespace ns = "http://www.dotnetage.com/XML/Schema/task";

            var element = new XElement(ns + "task",
                new XAttribute("xmlns", ns),
                new XElement(ns + "startAt", StartAt),
                new XElement(ns + "nextStart", NextStart));

            var recurringEl = new XElement(ns + "recurs",
                new XAttribute("value", Recurs),
                new XAttribute("frequency", Frequency.ToString()));

            if (Frequency == Frequencies.Weekly)
            {
                if (RecurringDaysOfWeek != null && RecurringDaysOfWeek.Length > 0)
                    recurringEl.Add(new XAttribute("daysOfWeek", string.Join(",", RecurringDaysOfWeek)));
            }

            if (Frequency == Frequencies.Monthly)
            {
                if (RecurringMonths != null && RecurringMonths.Length > 0)
                    recurringEl.Add(new XAttribute("months", string.Join(",", RecurringMonths)));

                if (RecurringDaysOfMonth != null && RecurringDaysOfMonth.Length > 0)
                    recurringEl.Add(new XAttribute("daysOfMonths", string.Join(",", RecurringDaysOfMonth)));
            }

            element.Add(recurringEl);

            if (!string.IsNullOrEmpty(Title))
                element.Add(new XElement(ns + "title", Title));

            if (State != TaskStates.Ready)
                element.Add(new XElement(ns + "state", State.ToString()));

            if (!string.IsNullOrEmpty(Description))
                element.Add(new XElement(ns + "desc", Description));

            var cmd = new XElement(ns + "command", new XAttribute("type", this.JobType));
            if (CommandData != null && CommandData.Count > 0)
            {
                foreach (var key in CommandData.Keys)
                    cmd.Add(new XElement(ns + "param", new XAttribute("name", key), CommandData[key].ToString()));
            }
            element.Add(cmd);
            element.Save(File);
        }
    }
}
