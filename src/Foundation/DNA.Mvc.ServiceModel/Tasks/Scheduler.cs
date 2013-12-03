//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Scheduling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DNA.Web.ServiceModel.Tasks
{
    /// <summary>
    /// Represent a task scheduler that use to execute the wok item task in background.
    /// </summary>
    public static class Scheduler
    {
        private static List<WorkItem> _workers;
        private static CancellationTokenSource cancellationTokenSource;

        private static WorkItem _AddTask(TaskDescriptor task)
        {
            var taskFile = HttpRuntime.AppDomainAppPath + "app_data\\tasks\\" + task.Name + ".xml";
            if (!File.Exists(taskFile))
            {
                var workItem = new WorkItem()
                {
                    Title = task.Title,
                    Name = task.Name,
                    Description = task.Description,
                    File = taskFile,
                    JobType = task.JobType,
                    Frequency = task.Frequency,
                    StartAt = task.StartAt,
                    NextStart = task.GetNextStart(task.StartAt),
                    Recurs = task.Recurs,
                    RecurringMonths = task.RecurringMonths,
                    RecurringDaysOfMonth = task.RecurringDaysOfMonth,
                    RecurringDaysOfWeek = task.RecurringDaysOfWeek
                };

                if (task.CommandData != null && task.CommandData.Count > 0)
                    workItem.CommandData = task.CommandData;

                workItem.Save();
                return workItem;
            }
            return null;
        }

        /// <summary>
        /// Get all job object
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Job> GetAllJobs()
        {
            var jobs = new List<Job>();
            var types = TypeSearcher.Instance().SearchTypesByBaseType(typeof(Job));
            foreach (var t in types)
            {
                try
                {
                    jobs.Add((Job)Activator.CreateInstance(t));
                }
                catch
                {
                    continue;
                }
            }
            return jobs;
        }

        /// <summary>
        /// Gets all loaded workers.
        /// </summary>
        public static List<WorkItem> Workers
        {
            get
            {
                if (_workers == null)
                    _workers = LoadWorkers().ToList();
                return _workers;
            }
        }

        /// <summary>
        /// Get all register work items.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<WorkItem> LoadWorkers(string path = "app_data\\tasks")
        {
            var basePath = HttpRuntime.AppDomainAppPath + path;

            if (!System.IO.Directory.Exists(basePath))
                System.IO.Directory.CreateDirectory(basePath);

            var tasks = System.IO.Directory.GetFiles(basePath, "*.xml");
            var workers = new List<WorkItem>();
            foreach (var task in tasks)
            {
                var worker = new WorkItem(task);
                workers.Add(worker);
            }
            return workers;
        }

        /// <summary>
        /// Get workitem by specified config file.
        /// </summary>
        /// <param name="file">The workitem name</param>
        /// <returns></returns>
        public static WorkItem GetWorker(string name)
        {
            return Workers.FirstOrDefault(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void Update(WorkItem workItem)
        {
            Remove(workItem.Name);
            workItem.Save();
            Workers.Add(workItem);
        }

        /// <summary>
        /// Add a new task to scheduler
        /// </summary>
        /// <param name="task"></param>
        public static void AddTask(TaskDescriptor task)
        {
            var t = _AddTask(task);
            if (t != null)
                Workers.Add(t);
        }

        /// <summary>
        /// Add tasks to scheduler
        /// </summary>
        /// <param name="tasks"></param>
        public static void AddTasks(TaskCollection tasks)
        {
            if (IsRunning)
                throw new Exception("Scheduler is running! You must stop the Scheduler fast.");

            foreach (var task in tasks)
                _AddTask(task);
        }

        /// <summary>
        /// Start the all tasks
        /// </summary>
        public static void Start()
        {
            if (IsRunning)
                return;

            _workers = null;

            cancellationTokenSource = new CancellationTokenSource();
            var interval = App.Settings.SchedulerFrequency;

            var mainTask = TaskRepeater.Interval(
                    TimeSpan.FromSeconds(interval),
                    () => CheckWorksToDo(),
                    cancellationTokenSource.Token);

            IsRunning = true;
        }

        private static void CheckWorksToDo()
        {
            var workers = Workers.Where(w => w.State.Equals(TaskStates.Ready) && w.StartAt <= DateTime.Now);

            foreach (var worker in workers)
            {
                var tokenSrc = new CancellationTokenSource();
                worker.Cancellation = tokenSrc;
                var task = Task.Factory.StartNew((state) =>
                 {
                     var workItem = state as WorkItem;
                     workItem.Run();
                 }, worker, tokenSrc.Token);
            }
        }

        /// <summary>
        /// Gets the scheduler is running.
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// Cancel all running task.
        /// </summary>
        public static void Stop()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();
            IsRunning = false;
        }

        /// <summary>
        /// Remove task from scheduler.
        /// </summary>
        /// <param name="name">The loaded work item name.</param>
        public static void Remove(string name)
        {
            var item = Workers.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                if (item.Cancellation != null && item.Cancellation.Token != null && item.Cancellation.Token.CanBeCanceled)
                    item.Cancellation.Cancel();

                Workers.Remove(item);
            }

            if (File.Exists(item.File))
                File.Delete(item.File);
        }

    }
}
