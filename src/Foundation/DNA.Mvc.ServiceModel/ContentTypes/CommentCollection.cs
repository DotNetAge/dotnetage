//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents a collection of CommentDecorator objects
    /// </summary>
    public class CommentCollection : IEnumerable<CommentDecorator>
    {
        internal IDataContext DataContext { get; set; }

        internal string Url { get; set; }

        internal string Owner { get; set; }

        /// <summary>
        /// Initializes a new instance of the CommentCollection class with data context , url and owner
        /// </summary>
        /// <param name="context">The data context object</param>
        /// <param name="url">The url which has comments</param>
        /// <param name="owner">The comments owner</param>
        public CommentCollection(IDataContext context, string url, string owner = "")
        {
            if (context == null)
                throw new ArgumentNullException("context");

            //if (string.IsNullOrEmpty(url))
            //    throw new ArgumentNullException("url");
            this.Url = url;
            this.Owner = owner;
            this.DataContext = context;
        }

        /// <summary>
        /// Find a comment object by specified id.
        /// </summary>
        /// <param name="commentID">The comment id.</param>
        /// <returns>A comment decorator wraps the comment object.</returns>
        public CommentDecorator Find(int commentID)
        {
            var comment = DataContext.Find<Comment>(commentID);
            if (comment != null)
                return new CommentDecorator(DataContext, comment);
            return null;
        }

        /// <summary>
        /// Delete the comment object and remove from collection by specified id.
        /// </summary>
        /// <param name="commentID"></param>
        public void Delete(int commentID)
        {
            var comment = DataContext.Find<Comment>(commentID);
            if (comment == null)
                throw new Exception("Comment not found");

            DataContext.Delete<Comment>(c => c.Path.StartsWith(comment.Path));
            DataContext.SaveChanges();

            //var replyTo = comment.ReplyTo;
            //var parentIDs = string.IsNullOrEmpty(comment.Parents) ? comment.ID.ToString() : "," + commentID.ToString();

            ////Delete all children comments
            //DataContext.Delete<Comment>(c => c.ReplyTo == commentID || c.Parents.StartsWith(parentIDs));
            //DataContext.Delete(comment);
            //DataContext.SaveChanges();

            ////Updte the reply parent comments
            //if (replyTo > 0)
            //{
            //    var reply = DataContext.Find<Comment>(replyTo);
            //    if (reply != null)
            //    {
            //        comment.Parents += string.IsNullOrEmpty(reply.Parents) ? replyTo.ToString() : ("," + replyTo.ToString());
            //        var parentArgs = string.IsNullOrEmpty(comment.Parents) ? new int[0] : comment.Parents.Split(',').Select(c => int.Parse(c)).ToArray();

            //        var parents = DataContext.Where<Comment>(c => parentArgs.Contains(c.ID));
            //        foreach (var parent in parents)
            //        {
            //            parent.TotalReplies = DataContext.Count<Comment>(c => c.ReplyTo == parent.ID);
            //            DataContext.Update(parent);
            //        }
            //    }
            //}
        }

        /// <summary>
        ///  Returns a comment enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CommentDecorator> GetEnumerator()
        {
            var results = new List<CommentDecorator>();
            var comments = !string.IsNullOrEmpty(Owner) ?
                DataContext.Where<Comment>(c => c.UserName.Equals(Owner, StringComparison.OrdinalIgnoreCase) && c.ReplyTo == 0).OrderByDescending(c => c.Posted).ToList() :
                DataContext.Where<Comment>(c => c.TargetUri.Equals(Url, StringComparison.OrdinalIgnoreCase) && c.ReplyTo == 0).OrderByDescending(c => c.Posted).ToList();

            if (comments.Count > 0)
                return comments.Select(c => new CommentDecorator(DataContext, c)).ToList().GetEnumerator();
            return results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
