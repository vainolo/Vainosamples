using HtmlAgilityPack;
using System;
using System.Linq;

namespace Vainolo.WebScraping
{
    class Method2
    {
        public static void Scrape()
        {
            var page = new HtmlDocument();
            page.Load("WebScraping.html");
            var nodes = page.DocumentNode.Descendants().SkipWhile(e => e.Id != "Techniques").Skip(1).TakeWhile(e => e.Name != "h2");

            foreach (var currNode in nodes)
            {
                 if(currNode.GetClasses().Contains("mw-headline"))
                {
                    var headline = currNode.InnerText;
                    Console.WriteLine(headline);
                }
            }
        }
    }
}