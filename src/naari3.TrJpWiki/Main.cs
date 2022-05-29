using AngleSharp.Html.Parser;
using ManagedCommon;
using System.Diagnostics;
using System.Net.Http;
using System.Web;
using Wox.Plugin;

namespace naari3.TrJpWiki
{
    public class Main : IPlugin
    {
        private string? IconPath { get; set; }
        static readonly HttpClient client = new HttpClient();
        static readonly Uri WIKI_HOST = new Uri("http://terraria.arcenserv.info/");

        private PluginInitContext? Context { get; set; }
        public string Name => "Terraria Jp Wiki";

        public string Description => "Terraria Jp Wiki";

        public List<Result> Query(Query query)
        {
            if (query?.Search is null)
            {
                return new List<Result>(0);
            }

            var searchResults = Search(query.Search);

            var results = searchResults.Select(result =>
            {
                return new Result
                {
                    Title = result.title,
                    SubTitle = result.detail,
                    IcoPath = IconPath,
                    Action = e =>
                    {
                        var pi = new ProcessStartInfo()
                        {
                            FileName = result.link,
                            UseShellExecute = true,
                        };

                        Process.Start(pi);

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
            IconPath = "images/trjp.png";
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private IEnumerable<SearchResult> Search(String keyword)
        {
            try
            {
                var encodedKeyword = HttpUtility.UrlEncode(keyword);
                var searchUrl = new Uri(WIKI_HOST, $"/w/index.php?title=%E7%89%B9%E5%88%A5%3A%E6%A4%9C%E7%B4%A2&redirs=0&search={encodedKeyword}&fulltext=Search&ns0=1");
                var response = AsyncHelper.RunSync(() => client.GetAsync(searchUrl));
                response.EnsureSuccessStatusCode();
                var responseBody = AsyncHelper.RunSync(() => response.Content.ReadAsStringAsync());
                var parser = new HtmlParser();
                var doc = parser.ParseDocument(responseBody);
                var resultDOMs = doc.QuerySelectorAll("ul.mw-search-results li");
                return resultDOMs.Where(dom =>
                    dom.QuerySelector("a") != null &&
                    dom.QuerySelector(".searchresult") != null
                ).Select(dom =>
                {
                    var link = new Uri(WIKI_HOST, dom.QuerySelector("a")!.GetAttribute("href"));
                    return new SearchResult
                    {
                        title = dom.QuerySelector("a")!.GetAttribute("title"),
                        detail = dom.QuerySelector(".searchresult")!.TextContent.Replace("\n", "").Replace("<br>", " "),
                        link = link.ToString()
                    };
                });
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        private class SearchResult
        {
            public string? title;
            public string? detail;
            public string? link;
        }
    }
}
