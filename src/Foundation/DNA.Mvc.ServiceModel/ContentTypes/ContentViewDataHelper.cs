//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    internal class ContentViewDataHelper
    {
        internal static DataRow CreateDataRow(DataTable dt, ContentViewDecorator view, ContentDataItemDecorator item)
        {
            var vfs = view.FieldRefs.Count == 0 ? view.Parent.Fields.Select(f => f).ToList() : view.FieldRefs.Select(f => f.Field).ToList();
            return CreateDataRow(dt, vfs, item);
        }

        internal static DataRow CreateDataRow(DataTable dt, List<ContentField> fields, ContentDataItemDecorator item)
        {
            var row = dt.NewRow();
            Bind(fields, row, item);
            dt.Rows.Add(row);
            return row;
        }

        internal static void Bind(List<ContentField> fields, DataRow row, ContentDataItemDecorator item)
        {
            int[] cats = null;
            if (item.Model.Categories != null)
                cats = item.Model.Categories.Select(c => c.ID).ToArray();

            row[DataNames.ID] = item.ID;
            row[DataNames.ParentID] = item.ParentItemID;
            row[DataNames.Privacy] = item.Privacy;
            row[DataNames.Created] = item.Created;
            row[DataNames.Pos] = item.Pos;
            if (item.Modified.HasValue)
                row[DataNames.Modified] = item.Modified;

            if (item.Published.HasValue)
                row[DataNames.Published] = item.Published;

            row[DataNames.IsPublished] = item.IsPublished;
            row[DataNames.Modifier] = item.Modifier;
            row[DataNames.State] = item.ModerateState;
            row[DataNames.Tags] = item.Tags;
            row[DataNames.Slug] = item.Slug;
            row[DataNames.Path] = item.Path;
            row[DataNames.Categories] = cats != null ? (string.Join(",", item.Context.Where<Category>(c => cats.Contains(c.ID)).Select(c => c.Name).ToArray())) : "";
            row[DataNames.EnableComments] = item.EnableComments;
            row[DataNames.Owner] = item.Owner;
            row[DataNames.Ratings] = item.Ratings;
            row[DataNames.Reads] = item.Reads;
            row[DataNames.TotalAttachs] = item.TotalAttachments;
            row[DataNames.TotalVotes] = item.TotalVotes;
            row[DataNames.TotalComms] = item.TotalComments;
            row[DataNames.TotalShares] = item.Parent.AllowResharing ? item.Reshares().Count() : 0;
            row[DataNames.Version] = item.Version;
            row[DataNames.HasChildren] = item.Parent.IsHierarchy ? item.Children().Count() > 0 : false;

            foreach (var field in fields)
            {
                var raw = item.Value(field.Name).Raw;
                if (raw == null)
                    row[field.Name] = DBNull.Value;
                else
                    row[field.Name] = raw;
            }
        }

        internal static void Bind(ContentViewDecorator view, DataRow row, ContentDataItemDecorator item)
        {
            var vfs = view.FieldRefs.Count == 0 ? view.Parent.Fields.Select(f => f).ToList() : view.FieldRefs.Select(f => f.Field).ToList();
            Bind(vfs, row, item);
        }

        internal static void SaveViewTable(ContentViewDecorator view,DataTable table)
        {
            var listPath = App.Get().NetDrive.MapPath(view.Parent.DefaultListPath);
            var viewFile = Path.Combine(listPath, "cache", "view_" + view.Name + ".xml");
            table.WriteXml(viewFile);
        }

        internal static DataTable GetViewTable(ContentViewDecorator view)
        {
            var listPath = App.Get().NetDrive.MapPath(view.Parent.DefaultListPath);
            var viewFile = Path.Combine(listPath, "cache", "view_" + view.Name + ".xml");
            var schemaFile = Path.Combine(listPath, "cache", "schema_" + view.Name + ".xml");

            if (File.Exists(viewFile) && File.Exists(schemaFile))
            {
                var dt = new DataTable();
                dt.ReadXmlSchema(schemaFile);
                dt.ReadXml(viewFile);
                return dt;
            }

            return null;
        }
    }
}
