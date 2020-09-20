using System;
using HtmlAgilityPack;
using System.Linq;

namespace Vainolo.WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scraping using method 1");
            Console.WriteLine("-----------------------");
            Method1.Scrape();
            Console.WriteLine("");
            Console.WriteLine("Scraping using method 2");
            Console.WriteLine("-----------------------");
            Method2.Scrape();
            Console.ReadLine();
        }
    }
}
