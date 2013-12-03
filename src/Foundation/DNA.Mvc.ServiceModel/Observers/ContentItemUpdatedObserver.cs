//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Events;

namespace DNA.Web.ServiceModel.Observers
{
    /// <summary>
    /// Represents an observer class use to update the content list view item when item is updated.
    /// </summary>
    [BindTo(EventNames.ContentDataItemUpdated)]
    public class ContentItemUpdatedObserver : HttpObserverBase<ContentDataItemEventArgs>
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
                    var row = viewTable.Rows.Find(e.DataItem.ID);
                    if (row != null)
                    {
                        ContentViewDataHelper.Bind(view, row, e.DataItem);
                        viewTable.AcceptChanges();
                        ContentViewDataHelper.SaveViewTable(view, viewTable);
                    }
                }
            }
        }
    }
}
