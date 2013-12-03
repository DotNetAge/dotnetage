//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Events;

namespace DNA.Web.ServiceModel.Observers
{
    [BindTo(EventNames.ContentDataItemCreated)]
    public class ContentItemInsertedObserver : HttpObserverBase<ContentDataItemEventArgs>
    {
        public override void Process(object sender, ContentDataItemEventArgs e)
        {
            var views = e.List.Views;
            var listPath = AppContext.NetDrive.MapPath(e.List.DefaultListPath);

            foreach (var view in views)
            {
                var viewTable = ContentViewDataHelper.GetViewTable(view);
                if (viewTable != null)
                {
                    ContentViewDataHelper.CreateDataRow(viewTable, view, e.DataItem);
                    viewTable.AcceptChanges();
                    ContentViewDataHelper.SaveViewTable(view, viewTable);
                }
            }

        }
    }
}
