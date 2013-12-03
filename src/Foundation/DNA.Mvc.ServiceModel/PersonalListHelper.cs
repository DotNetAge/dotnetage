//  Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
//  Licensed under the GPLv2: http://dotnetage.codeplex.com/license
//  Project owner : Ray Liang (csharp2002@hotmail.com)

namespace DNA.Web.ServiceModel
{
    /// <summary>
    /// Represents the personal list helper object.
    /// </summary>
    public class PersonalListHelper
    {
        private ContentListDecorator _parent;
        private User _identity = null;

        /// <summary>
        /// Gets parent list.
        /// </summary>
        public ContentListDecorator Parent
        {
            get { return _parent; }
        }
        
        /// <summary>
        /// Gets current user object.
        /// </summary>
        public User Identity
        {
            get
            {
                if (_identity == null)
                    _identity = AppModel.Get().User;
                return _identity;
            }
        }

        internal PersonalListHelper(ContentListDecorator parent)
        {
            this._parent = parent;
        }

        /// <summary>
        /// Gets my default data item query.
        /// </summary>
        public ContentQuery Query
        {
            get
            {
                var query = new ContentQuery();
                query.Eq(query.SysFieldNames.Owner, Identity.UserName);
                return query;
            }
        }
    }
}
