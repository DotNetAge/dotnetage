//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Utility;
using DNA.Web.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DNA.Web.Controllers
{
    public class WebFilesController : Controller
    {
        private INetDriveService service;
        public WebFilesController(INetDriveService svc) { service = svc; }

        [HttpPut]
        public ActionResult Update(string path, string name)
        {
            var uri = !string.IsNullOrEmpty(name) ? service.Rename(Request.Url, name) : Request.Url;

            if (!string.IsNullOrEmpty(path))
            {
                Uri tmpUri = null;
                Uri.TryCreate(path, UriKind.Absolute, out tmpUri);
                if (tmpUri != null)
                {
                    service.Move(Request.Url, tmpUri);
                    uri = tmpUri;
                }
            }

            UpdateAttributes(uri);

            string jsonStr = JsonConvert.SerializeObject(ToJson(new WebResourceInfo(uri)), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
            return Content(jsonStr, "application/json", Encoding.UTF8);
        }

        private void UpdateAttributes(Uri uri)
        {
            var attrs = service.GetAttributes(uri);
            if (attrs == null)
                attrs = new Dictionary<string, object>();

            foreach (var key in Request.Form.Keys)
            {
                var _key = key.ToString();
                if (_key.Equals("name", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (attrs.ContainsKey(_key))
                    attrs[_key] = Request.Form[_key];
                else
                    attrs.Add(_key, Request.Form[_key]);
            }

            if (attrs.Count > 0)
                service.SetAttributes(uri, attrs);
        }

        [HttpGet, WebFileCache(Duration = 3600), Compress]
        public ActionResult GetPath(string path, int? w, int? h, string match = "*.*", int resize = 0, int index = 0, int size = 0, bool ratio = true, bool enlage = true, string format = "")
        {
            var urlString = Request.Url.ToString();
            if (!string.IsNullOrEmpty(Request.Url.Query))
                urlString = urlString.Replace(Request.Url.Query, "");
            var url = new Uri(urlString);

            if (!service.Exists(url))
                return HttpNotFound();

            var webres = new WebResourceInfo(Request.Url);

            if (match.Equals("self"))
            {
                string result = JsonConvert.SerializeObject(ToJson(new WebResourceInfo(url)), new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat });
                return Content(result, "application/json", Encoding.UTF8);
            }
            //return Json(ToJson(new WebResourceInfo(url)), JsonRequestBehavior.AllowGet);

            if (webres.IsFile)
            {
                //if (IsRequestFromExternalDomain(HttpContext.Request))
                //{
                //    if (!_context.Web.IsTrusted(HttpContext.Request.Url, HttpContext.Request.UrlReferrer))
                //    {
                //        if (!string.IsNullOrEmpty(_context.Web.MasterTools.UrlForUntrustLink))
                //        {
                //            var ulink = Url.Content(_context.Web.MasterTools.UrlForUntrustLink);
                //            return File(ulink, FileUtility.GetContentType(ulink));
                //        }
                //        return HttpNotFound();
                //    }
                //}

                var serverPath = service.MapPath(url);

                if (((w.HasValue && h.HasValue) || resize > 0) && webres.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    //return File(GetThumbnailImage(path, webres.ContentType, w.Value, h.Value, ratio, !enlage), webres.ContentType);
                    return Thumb(serverPath, webres, w, h, resize, ratio, enlage);
                }

                if (!string.IsNullOrEmpty(format) && format == "code")
                    return Content(Text.TextEngine.CodeFile(serverPath).ToHtmlString(), "text/html");

                return File(serverPath, FileUtility.GetContentType(serverPath));
            }
            else
            {
                var paging = index > 0 && size > 0;
                IQueryable<Uri> results = null;
                var decodeType = Server.UrlDecode(string.IsNullOrEmpty(match) ? "*" : match);
                //var url = Request.Url;

                if (resize > 0 || (w.HasValue && h.HasValue)) // Get folder cover
                {
                    var serverPath = service.MapPath(url);
                    var files = Directory.GetFiles(serverPath);
                    FileInfo lastestFileInfo = null;
                    var attrs = service.GetAttributes(url);
                    var coverUrl = "";
                    Uri coverUri = null;
                    int width = resize;
                    int height = resize;
                    if (w.HasValue && h.HasValue)
                    {
                        width = w.Value;
                        height = h.Value;
                    }

                    if (attrs != null && attrs.Count > 0 && attrs.ContainsKey("cover") && !string.IsNullOrEmpty((string)attrs["cover"]))
                    {
                        coverUrl = url.ToString() + (url.ToString().EndsWith("/") ? "" : "/") + (string)attrs["cover"];
                        Uri.TryCreate(coverUrl, UriKind.Absolute, out coverUri);
                        if (coverUri != null)
                        {
                            if (service.Exists(coverUri))
                            {
                                var coverPath = service.MapPath(coverUri);
                                var coverRes = new WebResourceInfo(coverUri);
                                if (coverRes.ContentType.StartsWith("image"))
                                    return File(GetThumbnailImage(coverPath, coverRes.ContentType, width, height, ratio, !enlage), coverRes.ContentType);
                            }
                        }
                    }

                    //If the cover is set in metax

                    foreach (var _f in files)
                    {
                        var fi = new FileInfo(_f);
                        if (fi.Extension.Equals(".metax"))
                            continue;

                        if (lastestFileInfo != null)
                        {
                            if (fi.CreationTime > lastestFileInfo.CreationTime)
                                lastestFileInfo = fi;
                        }
                        else
                            lastestFileInfo = fi;
                    }

                    if (lastestFileInfo != null)
                    {
                        var lastWebResUrl = url.ToString() + (url.ToString().EndsWith("/") ? "" : "/") + lastestFileInfo.Name;
                        var lastWebRes = new WebResourceInfo(lastWebResUrl);
                        if (lastWebRes.ContentType.StartsWith("image"))
                            return File(GetThumbnailImage(lastestFileInfo.FullName, lastWebRes.ContentType, width, height, ratio, !enlage), lastWebRes.ContentType);
                    }

                    return HttpNotFound();
                }

                if (decodeType == "*.*") //Paths and files
                {
                    results = service.GetPaths(url).AsQueryable();
                    results = results.Union(service.GetFiles(url));
                }
                else
                {
                    if (decodeType == "*") //Only paths
                        results = service.GetPaths(url).AsQueryable();

                    if (decodeType[0] == '.' || decodeType.StartsWith("*.")) //only files
                    {
                        results = service.GetFiles(url).AsQueryable();
                        if (decodeType != ".*" && decodeType.Length > 1) // extension filter
                            results = results.Where(r => r.ToString().ToLower().EndsWith(decodeType.StartsWith("*") ? decodeType.Substring(1) : decodeType));
                    }
                }

                if (results == null)
                    return Json(new List<WebResourceInfo>(), JsonRequestBehavior.AllowGet);

                if (!paging)
                {
                    if (results.Count() > 0)
                        return Json(results.ToList().Select(f => ToJson(new WebResourceInfo(f))),
                            JsonRequestBehavior.AllowGet);
                    else
                        return Json(new List<WebResourceInfo>(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var jsonResults = results.Skip((index - 1) * size).Take(size).ToList();
                    return Json(new
                    {
                        Total = results.Count(),
                        Model = jsonResults.Count > 0 ? jsonResults.Select(f => ToJson(new WebResourceInfo(f))) : jsonResults
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [SecurityAction("Delete paths",
             PermssionSet = "Web file system",
             Description = "Allows user to delete the directory on the server.",
             ThrowOnDeny = true,
            TitleResName = "SA_DeletePaths",
            DescResName = "SA_DeletePathsDesc",
            PermssionSetResName = "SA_WebResSystem")]
        [HttpDelete]
        public ActionResult Delete()
        {
            try
            {
                service.Delete(HttpContext.Request.Url);
            }
            catch (Exception e)
            {
                throw new HttpException(e.ToString(), e);
            }
            return new HttpStatusCodeResult(200);
        }

        private bool IsRequestFromExternalDomain(HttpRequestBase request)
        {
            if (request.UrlReferrer != null)
            {
                if ((request.Url.IsAbsoluteUri) && (request.UrlReferrer.IsAbsoluteUri))
                {
                    return !request.Url.Authority.Equals(request.UrlReferrer.Authority);
                }
            }
            return false;
        }

        [NonAction]
        public FileResult Thumb(string path, WebResourceInfo info, int? w, int? h, int size = 64, bool ratio = true, bool enlage = true)
        {
            int width = size;
            int height = size;

            if (w.HasValue && h.HasValue)
            {
                width = w.Value;
                height = h.Value;
            }

            return File(GetThumbnailImage(path, info.ContentType, width, height, ratio, !enlage), info.ContentType);
        }

        private byte[] GetThumbnailImage(string path, string contentType, int width = 64, int height = 64, bool preserveAspectRatio = true, bool preventEnlage = false)
        {
            //int width = size;
            //int height = size;
            //using (var stream = new MemoryStream())
            //{
            //    using (var image = Image.FromFile(path))
            //    {
            //        using (var thumb =ImageHelper.ResizeImage(image, height, width, preserveAspectRatio, preventEnlage))
            //        {
            //            thumb.Save(stream, ImageHelper.GetImageFormat(contentType));
            //            stream.Position = 0;
            //            return stream.ToArray();
            //        }
            //    }
            //}

            var stream = new MemoryStream();
            var image = Image.FromFile(path);
            var thumb = ImageHelper.ResizeImage(image, height, width, preserveAspectRatio, preventEnlage);
            thumb.Save(stream, ImageHelper.GetImageFormat(contentType));
            stream.Position = 0;
            var result = stream.ToArray();
            thumb.Dispose();
            image.Dispose();
            stream.Flush();
            stream.Close();
            stream.Dispose();
            return result;
        }

        [SecurityAction("Upload files",
             PermssionSet = "Web file system",
             Description = "Allows user to upload the file to the server.",
             ThrowOnDeny = true,
            TitleResName = "SA_UploadFiles",
            DescResName = "SA_UploadFilesDesc",
            PermssionSetResName = "SA_WebResSystem")]
        [HttpPost]
        public ActionResult UploadOrCreate(string path, string sub = "")
        {
            if (!string.IsNullOrEmpty(sub))
            {
                var newPath = new Uri(Request.Url.ToString() + (Request.Url.LocalPath.EndsWith("/") ? "" : "/") + sub);
                service.CreatePath(newPath);
                UpdateAttributes(newPath);

                dynamic dirJson = PathToJson(new WebResourceInfo(newPath));
                //return Json(fileJson , JsonRequestBehavior.AllowGet);
                var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                string jsontext = ser.Serialize(dirJson);
                return Content(jsontext, "application/json", System.Text.Encoding.UTF8);
                //return Json(PathToJson(new WebResourceInfo(newPath)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (Request.Files.Count == 0)
                        return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);

                    foreach (string key in Request.Files.Keys)
                    {
                        var file = Request.Files[key];
                        //var fileInfo = new FileInfo(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        //if (!_context.RootWeb.IsAllowUpload(ext))
                        //    return new HttpStatusCodeResult((int)HttpStatusCode.UnsupportedMediaType, string.Format(Resources.language.WebFilesController_Unsupported, "\"" + ext + "\""));

                        //if (file.ContentLength > (_context.RootWeb.MaximumFileSize * 1000000))
                        //    return new HttpStatusCodeResult((int)HttpStatusCode.RequestEntityTooLarge, Resources.language.WebFilesController_Forbidden);

                        var fileName = TextUtility.Slug(Path.GetFileNameWithoutExtension(file.FileName)) + ext;

                        service.SaveFile(FileUtility.ReadStream(file.InputStream), fileName, Request.Url);

                        var requestUrl = Request.Url.ToString();
                        var fileUri = new Uri(requestUrl + (requestUrl.EndsWith("/") ? "" : "/") + fileName);
                        UpdateAttributes(fileUri);
                        dynamic fileJson = FileToJson(new WebResourceInfo(fileUri));
                        //return Json(fileJson , JsonRequestBehavior.AllowGet);
                        var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        string jsontext = ser.Serialize(fileJson);
                        return Content(jsontext, "application/json", System.Text.Encoding.UTF8);
                    }
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = 500;
                    return Content("Method Failure - " + e.Message);
                }
            }

            HttpContext.Response.StatusCode = 201;
            return Content("OK");
        }

        #region UI Actions

        [SiteDashboard(ResKey = "NetDrive",
            Sequence = 4,
            RouteName = "dna_netdrive",
            Icon = "d-icon-hdd")]
        [SecurityAction("NetDrvie",
             PermssionSet = "Web file system",
             Description = "Allows user use file manager to manage web resources.",
            TitleResName = "SA_FileMan",
            DescResName = "SA_FileManDesc",
            PermssionSetResName = "SA_WebResSystem")]
        public ActionResult Explorer(bool mini = false, string filter = "")
        {
            ViewBag.Filter = filter;
            if (mini)
            {
                return PartialView("_Explorer");
            }
            else
            {
                _InitExplorer(ViewData);
                if (Request.IsAjaxRequest())
                    return PartialView("FileExplorer");
                else
                    return View("FileExplorer");
            }
        }

        public ActionResult Files(string path, string filter = "")
        {
            ViewBag.Filter = filter;
            _initDialog(path);
            return PartialView();
        }

        [Loc, Authorize]
        public ActionResult Folders(string path)
        {
            _initDialog(path);
            return PartialView();
        }

        private void _initDialog(string path)
        {
            var rootPath = App.Get().Context.AppUrl.ToString() + "webshared/" + App.Get().Context.Website + "/";
            var _subPath = !string.IsNullOrEmpty(path) ? path : "";
            var _folderPath = rootPath + _subPath;

            _initPath(rootPath);

            if (!string.IsNullOrEmpty(_subPath))
                _initPath(_folderPath);

            ViewBag.FolderPath = _folderPath;
        }

        private void _initPath(string rootPath)
        {
            var rootUri = new Uri(rootPath);
            var phyicalPath = service.MapPath(rootUri);
            if (!Directory.Exists(phyicalPath))
                Directory.CreateDirectory(phyicalPath);
        }

        private void _InitExplorer(ViewDataDictionary viewData)
        {
            var rootPath = App.Get().Context.AppUrl.ToString() + "/webshared/" + App.Get().Context.Website + "/";
            var rootUri = new Uri(rootPath);
            var phyicalPath = service.MapPath(rootUri);
            if (!Directory.Exists(phyicalPath))
                Directory.CreateDirectory(phyicalPath);
            viewData["RootPath"] = rootPath;
        }

        public ActionResult MimeIcon(string extension)
        {
            var ext = extension.Replace(".", "");
            var fileName = Server.MapPath("~/content/images/mime/" + ext + ".gif");
            if (System.IO.File.Exists(fileName))
                return File(fileName, "image/gif");
            return File(Server.MapPath("~/content/images/mime/unknow.gif"), "image/gif");
        }

        [Authorize]
        public ActionResult ListView(string path)
        {
            ViewData["Path"] = path;
            return PartialView();
        }

        #endregion

        #region Json Objects - Added V2.3

        private dynamic PathToJson(WebResourceInfo fileInfo)
        {
            var fiPath = service.MapPath(fileInfo.Url);
            var fi = new DirectoryInfo(fiPath);
            return new
            {
                type = "path",
                url = fileInfo.Url.ToString(),
                path = fileInfo.Url.LocalPath.Replace(fileInfo.Name, ""),
                totalFiles = service.GetFiles(fileInfo.Url).Count(),
                totalPaths = service.GetPaths(fileInfo.Url).Count(),
                //owner = GetOwner(), //OwnerToJson(fileInfo),
                size = service.GetDirectorySize(fileInfo.Url),
                name = fileInfo.Name,
                published = fi.CreationTime,
                updated = fi.LastWriteTime,
                attrs = service.GetAttributes(fileInfo.Url)
            };
        }

        private dynamic FileToJson(WebResourceInfo fileInfo)
        {
            var fiPath = service.MapPath(fileInfo.Url);
            var fi = new FileInfo(fiPath);
            dynamic exif = null;

            #region Get exif
            if (fileInfo.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase) && DnaConfig.IsFullTrust)
            {
                var executor = new DNA.Utility.ExifExtractor(fiPath);
                if (executor != null)
                {
                    exif = new
                    {
                        height = executor.Height,
                        width = executor.Width,
                        title = executor.Title,
                        equipmentMaker = executor.EquipmentMaker,
                        equipmentModel = executor.EquipmentModel,
                        orientation = executor.Orientation,
                        software = executor.Software,
                        subjectDistance = executor.SubjectDistance,
                        userComment = executor.UserComment,
                        artist = executor.Artist,
                        description = executor.Description,
                        copyright = executor.Copyright,
                        resolutionX = executor.ResolutionX,
                        resolutionY = executor.ResolutionY,
                        focalLength = executor.FocalLength.ToString() + "mm",
                        shutterSpeed = executor.ShutterSpeed.ToString("N2"),
                        aperture = executor.Aperture.ToString("N1"),
                        exposureProgram = Enum.GetName(typeof(ExifExtractor.ExposurePrograms), executor.ExposureProgram),
                        iso = executor.ISO,
                        flashMode = executor.FlashMode,
                        exposureTime = executor.ExposureTime.ToString("N4") + " sec",
                        lightsource = Enum.GetName(typeof(ExifExtractor.LightSources), executor.LightSource),
                        latitude = executor.GpsLatitude,
                        longitude = executor.GpsLongitude
                    };
                    executor.Dispose();
                }
            }
            #endregion

            return new
            {
                type = "file",
                url = fileInfo.Url.ToString(),
                path = fileInfo.Url.LocalPath.Replace(fileInfo.Name, ""),
                //owner = GetOwner(),// OwnerToJson(fileInfo),
                contentType = fileInfo.ContentType,
                size = service.GetFileSize(fileInfo.Url),
                extension = fileInfo.Extension,
                name = fileInfo.Name,
                published = fi.CreationTime,
                updated = fi.LastWriteTime,
                exif = exif != null ? exif : null
                //,attrs = service.GetAttributes(fileInfo.Url)
            };
        }

        private dynamic ToJson(WebResourceInfo info)
        {
            if (info.IsFile)
                return FileToJson(info);
            else
                return PathToJson(info);
        }

        #endregion

    }
}
