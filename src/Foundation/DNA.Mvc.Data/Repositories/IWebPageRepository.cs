//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using DNA.Data;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Defines web page repository methods.
    /// </summary>
    public interface IWebPageRepository : IRepository<WebPage>
    {
        /// <summary>
        /// Delete the page by specified id.
        /// </summary>
        /// <param name="id">Specified the page id for delete</param>
        void Delete(int id);

        /// <summary>
        /// Get the WebPage instance for sepecifed path.
        /// </summary>
        /// <param name="path">The path of the page.</param>
        /// <returns>The WebPage instance.</returns>
        WebPage Find(string path);

        /// <summary>
        ///  Get the WebPageRole collection by specified the page id.
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <returns>A collection of the WebPageRole instances.</returns>
        string[] GetRoles(int id);

        //IEnumerable<WebPageRole> GetRoles(int[] ids);

        /// <summary>
        /// Add roles to specified page.
        /// </summary>
        /// <param name="id">The web page id.</param>
        /// <param name="roles">the roles to add</param>
        void AddRoles(int id, string[] roles);

        /// <summary>
        /// Add access roles to page.
        /// </summary>
        /// <param name="page">The page object.</param>
        /// <param name="roles">The role name array.</param>
        void AddRoles(WebPage page, string[] roles);

        /// <summary>
        /// Clear roles from specified page.
        /// </summary>
        /// <param name="id">The web page id.</param>
        void ClearRoles(int id);

        /// <summary>
        /// Get the child pages for specified page id.
        /// </summary>
        /// <param name="webID">The parent web id.</param>
        /// <param name="parentID">Specified th page id.</param>
        /// <returns>The WebPages of the specified id.</returns>
        IEnumerable<WebPage> GetChildren(int webID, int parentID);

        /// <summary>
        /// Move the specified page to the new position
        /// </summary>
        /// <param name="parentID">Specified the page parent id which page belongs to.</param>
        /// <param name="id">Specified the exisits page id.</param>
        /// <param name="pos">Specified the new position</param>
        void Move(int parentID, int id, int pos);

        /// <summary>
        /// Set the page as the default page of web by specified page id.
        /// </summary>
        /// <param name="webID">Specified the website id.</param>
        /// <param name="pageID">Specified the page id</param>
        void SetToDefault(int webID, int pageID);

        /// <summary>
        /// Create new page by page data object.
        /// </summary>
        /// <param name="web">The parent web object.</param>
        /// <param name="parentID">The parent page id.</param>
        /// <param name="pageData">The page data object.</param>
        /// <returns>A new web page object returns.</returns>
        WebPage Create(Web web, int parentID, IWebPage pageData);

        /// <summary>
        /// Publish the page and save a new version
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        int Publish(int id, string remarks = "");

        /// <summary>
        /// Rollback the page to specified version
        /// </summary>
        /// <param name="id">The page id.</param>
        /// <param name="version">The page version.</param>
        /// <returns></returns>
        WebPage Rollback(int id, int version);
    }
}
