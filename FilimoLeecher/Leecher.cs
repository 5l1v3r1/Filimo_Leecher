using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using HtmlAgilityPack;

namespace FilimoLeecher
{
    public class Leecher
    {
        public String _Url { get; set; }
        public String SavePath { get; set; }

        public delegate void Leeched(List<String> data);
        public event Leeched LeechedEvent;

        public List<String> UserList = new List<string>();
        public Leecher(String Url, String savePath)
        {
            this._Url = Url;
            this.SavePath = savePath;
        }
        public void StartLeech()
        {
            new System.Threading.Thread(() => { LeechUsernames(); }).Start();
        }
        private void AppendUsers(List<String> users)
        {
            foreach (var i in users)
            {
                if (!this.UserList.Exists(c => c == i))
                    UserList.Add(i);
            }
        }
        private void LeechUsernames()
        {
            var links = LeechGenreLinks();

            if (links.Count == 0)
            {
                //Error page is wrong 
            }
            else
            {
                foreach (var link in links)
                {
                    try
                    {
                        var _client = new RestClient(link);
                        var _req = new RestRequest(Method.GET);

                        var _resp = _client.Get(_req);
                        String content = _resp.Content;

                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(content);

                        var usersHtml = htmlDoc.DocumentNode.SelectNodes("//li[@class='comment-item clearfix']");
                        var usersList = new List<String>();

                        foreach (var u in usersHtml)
                        {
                            usersList.Add(u.ChildNodes[3].ChildNodes[1].ChildNodes[1].InnerText);
                        }
                        if (usersList.Count > 0)
                            AppendUsers(usersList);
                    }
                    catch { }
                }
            }
            LeechedEvent(this.UserList);
        }
        private List<String> LeechGenreLinks()
        {
            List<String> links = new List<String>();
            try
            {
                var _client = new RestClient(_Url);
                var _req = new RestRequest(Method.GET);

                var _resp = _client.Get(_req);
                String content = _resp.Content;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);

                var _links = htmlDoc.DocumentNode.SelectNodes("//div[@class='item']");
                foreach (var l in _links)
                {
                    try
                    {
                        var _childNode = l.ChildNodes[1].ChildNodes;
                        if (_childNode.Count == 5)
                        {
                            links.Add(_childNode[1].Attributes[0].Value);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return links;
        }
    }
}
