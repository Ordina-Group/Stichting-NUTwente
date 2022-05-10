using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.Models
{
    public class Comment : BaseEntity
    {
        public string Text { get; set; }
        public UserDetails Commenter { get; set; }

        public CommentType CommentType { get; set; }

        public Comment(string text, UserDetails commenter, CommentType commentType)
        {
            Text = text;
            Commenter = commenter;
            CommentType = commentType;
        }
        public Comment()
        {
            Text = "";
            Commenter = new UserDetails();
            CommentType = CommentType.BUDDY_REJECTION;
        }
    }
}
