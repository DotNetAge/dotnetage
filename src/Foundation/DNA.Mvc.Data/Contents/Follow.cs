using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.Web
{
    public class Follow
    {
        public int ID { get; set; }

        public int ListID { get; set; }

        /// <summary>
        /// Gets/Sets the list owner that who to follow
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets/Sets who is following this list
        /// </summary>
        public string Follower { get; set; }

        public virtual ContentList List { get; set; }
    }
}
