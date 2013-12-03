//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;

namespace DNA.Web
{
    /// <summary>
    /// Defines the methods to mainpluate web resources.
    /// </summary>
     public interface INetDriveService
    {
        /// <summary>
        /// Deletes the specified file. 
        /// </summary>
        /// <param name="url">Specified the resource uri to delete.</param>
        void Delete(Uri url);

         /// <summary>
         /// Map the uri to the server path.
         /// </summary>
         /// <param name="url"></param>
        string MapPath(Uri url);

        /// <summary>
        /// Create the parth for specified uri.
        /// </summary>
        /// <param name="url">The directory path to create.</param>
        void CreatePath(Uri url);

        /// <summary>
        ///  Save the data to destaintion path.
        /// </summary>
        /// <param name="data">The raw data of file.</param>
        /// <param name="fileName"></param>
        /// <param name="destURI">the specified destaintion path where to save.</param>
        Uri SaveFile(byte[] data,string fileName,Uri destURI);

        /// <summary>
        ///  Move a file or a directory and its contents to a new location.
        /// </summary>
        /// <param name="sourceURI"> The path of the file or directory to move.</param>
        /// <param name="destURI">The path to the new location for sourceURI</param>
        void Move(Uri sourceURI, Uri destURI);

        /// <summary>
        /// Get the file's raw data for specified filename
        /// </summary>
        /// <param name="url">The uri of the resource file</param>
        /// <returns>A byte array contains the file raw data.</returns>
        byte[] Open(Uri url);


        /// <summary>
        /// Gets the file info objects by the specified path.
        /// </summary>
        /// <param name="url">The specified path</param>
        /// <returns>A collection contains resource uri.</returns>
        IEnumerable<Uri> GetFiles(Uri url);

        /// <summary>
        ///  Gets the sub path uris by the specified path.
        /// </summary>
        /// <param name="url">The specified path.</param>
        /// <returns>A collection contains resource path uri.</returns>
        IEnumerable<Uri> GetPaths(Uri url);

        #region Added V2.3
        
         /// <summary>
        /// Get file's attributes
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
         IDictionary<string, object> GetAttributes(Uri url);

         /// <summary>
         /// Set the additional attributes by specified url and name value dictionary.
         /// </summary>
         /// <param name="url"></param>
         /// <param name="attributes"></param>
         void SetAttributes(Uri url, IDictionary<string, object> attributes);

         /// <summary>
         /// Identity whether the specified url path or file is exist.
         /// </summary>
         /// <param name="url">The url object</param>
         /// <returns>If the path of file exists return true.</returns>
        bool Exists(Uri url);

        /// <summary>
        /// Copy the specified url file(path) to desctination.
        /// </summary>
        /// <param name="source">The source uri</param>
        /// <param name="destination">The destination uri</param>
         void Copy(Uri source, Uri destination);

         /// <summary>
         /// Rename the specified uri file(path) to new name.
         /// </summary>
         /// <param name="source">The source file/path uri.</param>
         /// <param name="name">The file/path name.</param>
         /// <returns>The new uri path will return.</returns>
        Uri Rename(Uri source, string name);

        /// <summary>
        /// Gets the specified file size
        /// </summary>
        /// <param name="url">The file uri.</param>
        /// <returns>A long value contains total bytes of the file.</returns>
        long GetFileSize(Uri url);

        /// <summary>
        /// Gets the specified directory size.
        /// </summary>
        /// <param name="url">The dir uri</param>
        /// <returns>A long value contains total bytes of the specified directory</returns>
        long GetDirectorySize(Uri url);

        #endregion

        #region Add V3.0.1

        /// <summary>
        ///  Opens a text file, reads all lines of the file, and then closes the file.
        /// </summary>
        /// <param name="url">The file to open for reading.</param>
        /// <returns>A string containing all lines of the file.</returns>
        string ReadText(Uri url);

        /// <summary>
        ///  Reads the lines of a file.
        /// </summary>
        /// <param name="url"> The file to read.</param>
        /// <returns> The lines of the file.</returns>
        string[] ReadLines(Uri url);

        /// <summary>
        ///     Creates a new file, writes the specified string to the file, and then closes
        ///     the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="url"> The file to write to.</param>
        /// <param name="contents">The content to write to the file</param>
        void WriteText(Uri url, string contents);

        /// <summary>
        /// Creates a new file, writes a collection of strings to the file, and then closes the file.
        /// </summary>
        /// <param name="url">The file to write to.</param>
        /// <param name="lines">The lines to write to the file.</param>
        void WriteLines(Uri url, string[] lines);

        #endregion
    }
}
