using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedditMention
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Connecting...");
            var tools = new RAOATools("Random_Acts_Of_Amazon");
            Console.WriteLine("Beginning loop.");
            var dt = DateTime.Now;
            while (true)
            {
                try
                {
                    var reddithandles = tools.GetNewRedditHandles(dt);
                    foreach (var msg in reddithandles)
                    {
                        tools.SendMessage(msg.UserName, msg.Subject, msg.Text);
                    }
                }
                catch(Exception ex)
                {}
            }
        }
    }

    class RAOATools
    {
        private Subreddit Sub { get; set; }
        readonly Reddit _reddit;

        public RAOATools(string modMailSub)
        {
            _reddit = new Reddit();
            Sub = _reddit.GetSubreddit(modMailSub);
        }

        public void SendMessage(string userName, string subject, string body)
        {
            _reddit.ComposePrivateMessage(subject, body, userName);
        }

        public IEnumerable<UserNameMention> GetNewRedditHandles(DateTime dt)
        {
            _userNameMentions = new List<UserNameMention>();
            var posts = Sub.GetNew().Where(a => a.Created > dt);
            foreach (var post in posts)
            {
                foreach (var comment in post.GetComments())
                {
                    GetUserMentions(comment);
                }
            }
            return _userNameMentions;
        }

        public class UserNameMention
        {
            public string UserName { get; set; }
            public string Subject { get; set; }
            public string Text { get; set; }
        }

        List<UserNameMention> _userNameMentions;

        private void GetUserMentions(Comment comment)
        {
            foreach (var comm in comment.Comments)
            {
                var regx = new Regex("//u/(?:.*) /", RegexOptions.IgnoreCase);
                var ms = regx.Matches(comm.Body);
                foreach (Match m in ms)
                {
                    _userNameMentions.Add(new UserNameMention
                    {
                        UserName = m.ToString(),
                        Subject = "userName mention",
                        Text = "You were mentioned in a [comment](" + comm.Shortlink + ")"
                    });
                }
                GetUserMentions(comment);
            }
        }
    }
}
