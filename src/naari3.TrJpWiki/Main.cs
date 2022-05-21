//using AngleSharp.Html.Parser;
//using HtmlAgilityPack;
using ManagedCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Wox.Plugin;

namespace naari3.TrJpWiki
{
    public class Main : IPlugin
    {
        private string IconPath { get; set; }
        static readonly HttpClient client = new HttpClient();
        static readonly Uri WIKI_HOST = new Uri("http://terraria.arcenserv.info/");

        private PluginInitContext Context { get; set; }
        public string Name => "Guid";

        public string Description => "Guid Generator";

        public List<Result> Query(Query query)
        {
            var searchResults = Search(query.RawQuery).Result;
            var guid = System.Guid.NewGuid().ToString();
            var guidUpper = guid.ToUpperInvariant();

            var results = searchResults.Select(result =>
            {
                return new Result
                {
                    Title = result.title,
                    IcoPath = IconPath,
                    Action = e =>
                    {
                        System.Diagnostics.Process.Start(result.link);
                        return true;
                    },
                };
            });

            return results.ToList();
        }

        public void Init(PluginInitContext context)
        {
            Context = context;
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                IconPath = "images/guid.light.png";
            }
            else
            {
                IconPath = "images/guid.dark.png";
            }
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private async Task<IEnumerable<SearchResult>> Search(String keyword)
        {
            return new List<SearchResult> {
                new SearchResult
                {
                    title = "test",
                    detail = "detail",
                    link = "https://example.com"
                }
            };
            //try
            //{
            //    var encodedKeyword = HttpUtility.UrlEncode(keyword);
            //    var searchUrl = new Uri(WIKI_HOST, $"/w/index.php?title=%E7%89%B9%E5%88%A5%3A%E6%A4%9C%E7%B4%A2&redirs=0&search={encodedKeyword}&fulltext=Search&ns0=1");
            //    var response = await client.GetAsync(searchUrl);
            //    response.EnsureSuccessStatusCode();
            //    var responseBody = await response.Content.ReadAsStringAsync();
            //    // Above three lines can be replaced with new helper method below
            //    // string responseBody = await client.GetStringAsync(uri
            //    var parser = new HtmlParser();
            //    var doc = await parser.ParseDocumentAsync(responseBody);
            //    var resultDOMs = doc.QuerySelectorAll("ul.mw-search-results li");
            //    return resultDOMs.Select(dom =>
            //    {
            //        var link = new Uri(WIKI_HOST, dom.QuerySelector("a").GetAttribute("href"));
            //        return new SearchResult
            //        {
            //            title = dom.QuerySelector("a").GetAttribute("a"),
            //            detail = dom.QuerySelector(".searchresult").TextContent,
            //            link = link.ToString()
            //        };
            //    });
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine("\nException Caught!");
            //    Console.WriteLine("Message :{0} ", e.Message);
            //    return new List<SearchResult>();
            //}
        }

        //private async Task<IEnumerable<SearchResult>> Search2(String keyword)
        //{
        //    try
        //    {
        //        var encodedKeyword = HttpUtility.UrlEncode(keyword);
        //        var searchUrl = new Uri(WIKI_HOST, $"/w/index.php?title=%E7%89%B9%E5%88%A5%3A%E6%A4%9C%E7%B4%A2&redirs=0&search={encodedKeyword}&fulltext=Search&ns0=1");
        //        var response = await client.GetAsync(searchUrl);
        //        response.EnsureSuccessStatusCode();
        //        var responseBody = await response.Content.ReadAsStringAsync();
        //        // Above three lines can be replaced with new helper method below
        //        // string responseBody = await client.GetStringAsync(uri
        //        var doc = new HtmlDocument();
        //        doc.LoadHtml(responseBody);
        //        var resultDOMs = doc.DocumentNode.SelectNodes("//ul[@class='mw-search-results']/li");
        //        return resultDOMs.Select(dom =>
        //        {
        //            var link = new Uri(WIKI_HOST, dom.SelectSingleNode("//a").GetAttributeValue("href", "/"));
        //            return new SearchResult
        //            {
        //                title = dom.SelectSingleNode("//a").GetAttributeValue("title", "Unknown"),
        //                detail = dom.SelectSingleNode("//div[@class='.searchresult']").InnerText,
        //                link = link.ToString()
        //            };
        //        });
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine("\nException Caught!");
        //        Console.WriteLine("Message :{0} ", e.Message);
        //        return new List<SearchResult>();
        //    }
        //}

        private class SearchResult
        {
            public string title;
            public string detail;
            public string link;
        }
    }
}

