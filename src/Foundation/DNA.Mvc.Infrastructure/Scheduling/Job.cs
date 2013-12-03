//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Patterns.Commands;
using System;
using System.Collections.Generic;

namespace DNA.Web.Scheduling
{
    /// <summary>
    /// Represent the base class of a job 
    /// </summary>
    public abstract class Job : ICommand
    {
        /// <summary>
        /// Gets the job title
        /// </summary>
        public virtual string Title { get { return this.GetType().Name; } }

        /// <summary>
        /// Gets the job description
        /// </summary>
        public virtual string Descritpion { get { return ""; } }

        /// <summary>
        /// Gets the configuration view file name.
        /// </summary>
        public virtual string ConfigureView { get { return ""; } }

        /// <summary>
        /// Gets/Sets the contextal data of the job
        /// </summary>
        public IDictionary<string, object> Data { get; set; }

        /// <summary>
        /// Do this job
        /// </summary>
        protected abstract void OnExecute();
        
        /// <summary>
        /// Handling before the task start.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Handling on task execute complete.
        /// </summary>
        protected virtual void OnCompleted() { }

        public void Execute()
        {
            OnStart();
            OnExecute();
            OnCompleted();
        }

        /// <summary>
        /// Handling the error during the job runtime.
        /// </summary>
        /// <param name="exception"></param>
        public virtual void OnError(Exception exception) { }
    }
}
