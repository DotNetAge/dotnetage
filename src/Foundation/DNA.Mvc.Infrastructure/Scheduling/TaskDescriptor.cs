//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.Scheduling
{
    /// <summary>
    /// Represents a Task to create a work item for scheduler.
    /// </summary>
    public class TaskDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the TaskDescriptor class.
        /// </summary>
        public TaskDescriptor() { Frequency = Frequencies.Minutely; }

        /// <summary>
        /// Gets/Sets the task title display text.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/Sets the task description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/Sets the task name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets/Sets the work item start time
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Ges/Sets the frequency of the interval.
        /// </summary>
        public Frequencies Frequency { get; set; }

        /// <summary>
        /// Gets/Sets the recur times
        /// </summary>
        public int Recurs { get; set; }

        /// <summary>
        /// Gets/Sets the recurring days of a week
        /// </summary>
        public int[] RecurringDaysOfWeek { get; set; }

        /// <summary>
        /// Gets/Sets the recurring days of a month
        /// </summary>
        public int[] RecurringDaysOfMonth { get; set; }

        /// <summary>
        /// Gets/Sets the recurring months
        /// </summary>
        public int[] RecurringMonths { get; set; }

        /// <summary>
        /// Gets/Sets the job type.
        /// </summary>
        public Type JobType { get; set; }

        /// <summary>
        /// Gets/Sets the json string for command param data .
        /// </summary>
        public Dictionary<string, object> CommandData { get; set; }

        /// <summary>
        /// Get the next started date.
        /// </summary>
        /// <param name="started"></param>
        /// <returns></returns>
        public DateTime GetNextStart(DateTime started)
        {
            var nextStart = DateTime.MinValue;
            switch (Frequency)
            {
                case Frequencies.Minutely:
                    nextStart = started.AddMinutes(Recurs);
                    break;
                case Frequencies.Hourly:
                    nextStart = started.AddHours(Recurs);
                    break;
                case Frequencies.Daily:
                    nextStart = started.AddDays(Recurs);
                    break;
                case Frequencies.Weekly:
                    var offset = 7 * Recurs;
                    if (RecurringDaysOfWeek != null && RecurringDaysOfWeek.Length > 0)
                    {
                        var days = RecurringDaysOfWeek.OrderBy(a => a).ToArray();
                        foreach (var d in days)
                        {
                            var infer = started.AddDays(d);
                            if (infer > DateTime.Now)
                            {
                                nextStart = infer;
                                return nextStart;
                            }
                        }

                        //Search in next recurs
                        foreach (var d in days)
                        {
                            var infer = started.AddDays(d * Recurs);
                            if (infer > DateTime.Now)
                            {
                                nextStart = infer;
                                return nextStart;
                            }
                        }
                        //not found in this week
                    }
                    nextStart = started.AddDays(7 * Recurs);
                    break;
                case Frequencies.Monthly:
                    var month = started.Month;
                    var day = started.Day;

                    var _months = RecurringMonths.Where(m => m >= month).OrderBy(a => a).ToArray();
                    var _days = RecurringDaysOfMonth.OrderBy(a => a).ToArray();

                    //The roles: RecurringDaysOfMonth and RecurringMonths are required.
                    //Get the month first then locate the day.

                    //Get the day in current month.
                    if (_months.Contains(month))
                    {
                        if (_days.Count(d => d > day) > 0)
                            return started.AddDays(_days.First(a => a > day) - day);
                    }

                    //Get the next month value in the array
                    var nextMonth = _months.FirstOrDefault(a => a > month);
                    var nextDay = _days.First();

                    if (nextMonth == 0)
                    {
                        //Go to next year
                        nextMonth = RecurringMonths.OrderBy(a => a).First();
                        nextStart = new DateTime(started.Year + 1, nextMonth, nextDay, started.Hour, started.Minute, started.Second);
                    }
                    else
                        nextStart = new DateTime(started.Year, nextMonth, nextDay, started.Hour, started.Minute, started.Second);

                    break;
            }
            return nextStart;
        }

    }
}
