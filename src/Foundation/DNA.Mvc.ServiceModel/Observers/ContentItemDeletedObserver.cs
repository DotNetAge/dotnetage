//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Events;

namespace DNA.Web.ServiceModel.Observers
{
    [BindTo(EventNames.ContentDataItemDeleted)]
    public class ContentItemDeletedObserver : HttpObserverBase<ContentDataItemDeletedEventArgs>
    {
        public override void Process(object sender, ContentDataItemDeletedEventArgs e)
        {
            var views = e.List.Views;
            var listPath = AppContext.NetDrive.MapPath(e.List.DefaultListPath);

            foreach (var view in views)
            {
                var viewTable = ContentViewDataHelper.GetViewTable(view);
                if (viewTable != null)
                {
                    var row = viewTable.Rows.Find(e.ItemID);
                    if (row != null)
                    {
                        //ContentViewDataHelper.Bind(view, row, e.DataItem);
                        viewTable.Rows.Remove(row);
                        viewTable.AcceptChanges();
                        ContentViewDataHelper.SaveViewTable(view, viewTable);
                    }
                }
            }
        }
    }
}
