using HtmlAgilityPack;
using System;
using System.Linq;

namespace Vainolo.WebScraping
{
    class Method1
    {
        public static void Scrape()
        {
            var scraper = new HtmlWeb();
            var page = scraper.Load("https://vainolo.z14.web.core.windows.net/WebScraping.html"); 
            var techniquesTitle = page.GetElementbyId("Techniques");
            var currNode = techniquesTitle.ParentNode.NextSibling;
            while(currNode.Name != "h2")
            {
                if(currNode.GetClasses().Contains("mw-headline"))
                {
                    var headline = currNode.InnerText;
                    Console.WriteLine(headline);
                }
                if(currNode.HasChildNodes)
                {
                    currNode = currNode.FirstChild;
                }
                else if(currNode == currNode.ParentNode.LastChild)
                {
                    while(currNode.ParentNode.NextSibling == null)
                    {
                        currNode = currNode.ParentNode;
                    }
                    currNode = currNode.ParentNode.NextSibling;
                }
                else
                {
                    currNode = currNode.NextSibling;
                }
            }
        }
    }
}