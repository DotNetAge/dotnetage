using DNA.Web.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web.ServiceModel.Jobs
{
    public class BackupToDisk:Job
    {
        public override string Title
        {
            get
            {
                return "Backup to file";
            }
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }
    }
}
