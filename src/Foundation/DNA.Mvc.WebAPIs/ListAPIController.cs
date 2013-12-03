//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Web.Contents;
using DNA.Web.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class ListAPIController : Controller
    {
        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public void Remove(int id)
        {
            App.Get().CurrentWeb.Lists.Remove(id);
        }

        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public void Save(int id)
        {
            var list = App.Get().CurrentWeb.Lists[id];

            if (TryUpdateModel(list))
                list.Save();
        }

        #region field api

        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public ActionResult Add_Field(int id, FormCollection form)
        {
            return Content(_UpdateField(id, form), "application/json", Encoding.UTF8);
        }

        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public ActionResult Update_Field(int id, FormCollection form)
        {
            return Content(_UpdateField(id, form, true), "application/json", Encoding.UTF8);
        }

        [HttpPost, Loc, PermissionRequired("Edit", ControllerTypeName = "DNA.Web.Controllers.FormController,DNA.Web")]
        public void Delete_Field(int id, string name)
        {
            var list = App.Get().CurrentWeb.Lists[id];
            var field = list.Fields[name];
            if (field != null)
                list.Fields.Remove(field);
            list.SaveSchema();
        }

        #endregion

        #region internal methods

        private string SerializeField(ContentField field)
        {
            ContentFieldTypes t = (ContentFieldTypes)field.FieldType;
            return JsonConvert.SerializeObject(
                new
                {
                    name = field.Name,
                    title = field.Title,
                    required = field.IsRequired,
                    type = t.ToString()
                });
        }

        private string _UpdateField(int id, FormCollection form, bool updated = false)
        {
            var list = App.Get().CurrentWeb.Lists[id];

            var fieldType = (ContentFieldTypes)Enum.Parse(typeof(ContentFieldTypes), form["FieldType"]);
            var factory = new ContentFieldFactory();
            var newField = factory.Create(form["FieldType"]);
            var properties = form.AllKeys.Where(k => !k.Equals("FieldType", StringComparison.OrdinalIgnoreCase) && !k.Equals("id", StringComparison.OrdinalIgnoreCase)).ToArray();
            var flag = false;
            #region get field type
            switch (fieldType)
            {
                case ContentFieldTypes.Boolean:
                    flag = TryUpdateModel<BooleanField>((BooleanField)newField, properties);
                    break;
                case ContentFieldTypes.Choice:
                    flag = TryUpdateModel<ChoiceField>((ChoiceField)newField, properties);
                    if (!string.IsNullOrEmpty(form["Choices"]))
                    {
                        ((ChoiceField)newField).Choices = ((ChoiceField)newField).Choices.Replace("\r\n", ",");
                    }
                    break;
                case ContentFieldTypes.Currency:
                    flag = TryUpdateModel<CurrencyField>((CurrencyField)newField, properties);
                    break;
                case ContentFieldTypes.DateTime:
                    flag = TryUpdateModel<DateField>((DateField)newField, properties);
                    break;
                case ContentFieldTypes.Image:
                    flag=TryUpdateModel<ImageField>((ImageField)newField, properties);
                    break;
                case ContentFieldTypes.Integer:
                    flag = TryUpdateModel<IntegerField>((IntegerField)newField, properties);
                    break;
                case ContentFieldTypes.Number:
                    flag = TryUpdateModel<NumberField>((NumberField)newField, properties);
                    break;
                case ContentFieldTypes.Note:
                    flag = TryUpdateModel<NoteField>((NoteField)newField, properties);
                    break;
                case ContentFieldTypes.User:
                    flag = TryUpdateModel<UserField>((UserField)newField, properties);
                    break;
                case ContentFieldTypes.Video:
                    flag = TryUpdateModel<VideoField>((VideoField)newField, properties);
                    break;
                case ContentFieldTypes.Lookup:
                    flag = TryUpdateModel<LookupField>((LookupField)newField, properties);
                    break;
                default:
                    flag = TryUpdateModel<TextField>((TextField)newField, properties);
                    break;
            }
            #endregion

            if (!flag)
            {
                var errors = new StringBuilder();
                foreach (var err in ModelState)
                {
                    if (err.Value.Errors.Count > 1)
                    {
                        foreach (var e in err.Value.Errors)
                        {
                            errors.AppendLine(err.Key + ":" + e.ErrorMessage);
                        }
                    }
                }
                throw new Exception("Update field fail!" + errors.ToString());
            }

            if (updated)
            {
                var orgField = list.Fields[form["Name"]];
                var index = list.Fields.IndexOf(orgField);
                list.Fields.Remove(orgField);
                list.Fields.Insert(index, newField);
            }
            else
            {
                list.Fields.Add(newField);
            }

            list.SaveSchema();

            if (!updated)
            {
                // Add field to the forms
                foreach (var f in list.Forms)
                {
                    if (f.Fields[newField.Name] == null)
                        f.Fields.Add(newField.Name);
                }
            }

            return SerializeField(newField);
        }

        #endregion

        //#region data api
        //[HttpPost, PermissionRequired(typeof(DNA.Web.Controllers.FormController), "Edit")]
        //public void Import(string url = "", string title = "", string name = "", string type = "csv")
        //{
        //    //csv
        //    url = Server.MapPath("~/app_data/files/public/data/test.csv");
        //    using (var reader = new System.IO.StreamReader(url))
        //    {
        //        //Prepare the list
        //        var baseEleUrl = Server.MapPath("~/content/types/base/config.xml");
        //        var baseElement = System.Xml.Linq.XDocument.Load(baseEleUrl);
        //        var list = AppModel.Get().CurrentWeb.CreateList(baseElement.Root, "csvlist", "csvlist");
        //        var line = reader.ReadLine();

        //        #region Create columns
        //        var columns = line.Split(',');
        //        for (var i = 0; i < columns.Length; i++)
        //        {
        //            var col = columns[i];
        //            var colName = DNA.Utility.TextUtility.Slug(col);
        //            var field = new TextField()
        //            {
        //                Title = col,
        //                Name = colName,
        //                IsLinkToItem = i == 0
        //            };
        //            list.Fields.Add(field);
        //        }

        //        list.SaveSchema();
        //        //list.Save();

        //        #endregion

        //        while (!reader.EndOfStream)
        //        {
        //            var rowline = reader.ReadLine();
        //            var rowObject=list.CreateItemData(rowline.Split(','));
        //            list.NewItem(rowObject, User.Identity.Name);
        //        }

        //    }
        //}

        //public void Export() { }

        //#endregion
    }
}
