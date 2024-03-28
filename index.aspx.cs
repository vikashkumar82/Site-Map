using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;



namespace demoSitemap
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void GenerateSitemap(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(urlTextBox.Text))
            {
                string websiteUrl = urlTextBox.Text.Trim();
                urlSectionDiv.Visible = true;
                brokenCountDiv.Visible = true;
                linksbroken.Visible = true;

                if (!string.IsNullOrEmpty(websiteUrl))
                {
                    string sitemapXml = GenerateSitemapXml(websiteUrl);
                    resultTextBox.Text = sitemapXml;
                }
                else
                {
                    resultTextBox.Text = "Please provide a valid website URL.";
                }

                //------------------------------------------------------------------------------------------------



                if (Uri.TryCreate(websiteUrl, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    List<string> webpageUrls = FetchWebPageUrls(websiteUrl);
                    string htmlResult = GenerateHtmlCode(webpageUrls);

                    resulturl.InnerHtml = htmlResult;
                }

                //----------------------------------------------------broken links-----------------------------------------------
                string url = urlTextBox.Text;

                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    // Get web page size
                    long pageSizeInBytes = GetWebPageSize(url);
                    double pageSizeInMB = ConvertBytesToMegabytes(pageSizeInBytes);

                    sizeLabel.Text = $"{pageSizeInMB:F2} MB";

                    // Check for broken links
                    var (brokenLinksCount, brokenLinksText) = FindBrokenLinksInfo(url);
                    brokenLinksLabel.Text = $"Broken Links: {brokenLinksCount} ";
                    lbl_broken.Text = $"{brokenLinksCount} ";
                    lblcount.Text =  $"{brokenLinksText}";
                }
                else
                {
                    sizeLabel.Text = "Invalid URL";
                    brokenLinksLabel.Text = "";
                    lblcount.Text = "0"; // Set count to zero for an invalid URL
                }

                //------------------------------------------crowled pages----------------------------
                string apiKey = "AIzaSyAWuOOGN9OsjCZRawPWtomBeb74BfFZvVE"; // Replace with your Google API key
                string cx = "34e2581094086479b\r\n"; // Replace with your Custom Search Engine ID

                string urlCrawl = urlTextBox.Text;
                int crawledPageCount = await GetCrawledPageCount(apiKey, cx, urlCrawl);
                crawlresult.Text = crawledPageCount.ToString();
                //----------------------------------------------crawled pahes end---------------------------
            }
            

        }


        //----------------------------------------------------------------url----------------------------------------------------------------------

        private List<string> FetchWebPageUrls(string websiteUrl)
        {
            List<string> webpageUrls = new List<string>();

            try
            {
                WebClient client = new WebClient();
                string htmlContent = client.DownloadString(websiteUrl);

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                // Modify this XPath expression based on the structure of the website
                var hrefNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

                if (hrefNodes != null)
                {
                    HashSet<string> uniqueUrls = new HashSet<string>();

                    foreach (var node in hrefNodes)
                    {
                        string hrefValue = node.GetAttributeValue("href", "");
                        Uri absoluteUri;

                        if (Uri.TryCreate(new Uri(websiteUrl), hrefValue, out absoluteUri))
                        {
                            uniqueUrls.Add(absoluteUri.AbsoluteUri);
                        }
                    }

                    webpageUrls = uniqueUrls.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

            return webpageUrls;
        }

        private string GenerateHtmlCode(List<string> webpageUrls)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.AppendLine("<table class=\"table table-striped\">");

            // Iterate through the URLs and add rows to the table with alternating styles
            foreach (var url in webpageUrls)
            {
                // Use the modulo operator to alternate between dark and light rows (zebra style)
                string rowClass = (webpageUrls.IndexOf(url) % 2 == 0) ? "even-row" : "odd-row";

                result.AppendLine($"<tr><td colspan=\"2\">{url}</td></tr>"); /*class=\"{rowClass}\"*/
            }

            result.AppendLine("</table>");

            return result.ToString();

        }
        //-------------------------------------------------------url end-------------------------------------------------------------------------------

        //---------------------------------------------------------------sitemap xml-------------------------------------------------------------------------------
        private string GenerateSitemapXml(string websiteUrl)
        {

            var pages = CrawlWebsite(websiteUrl);

           
            var distinctPages = pages.GroupBy(page => page.Url).Select(g => g.First()).ToList();

            var result = new StringBuilder();
            result.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            result.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var page in distinctPages)
            {
                var sanitizedUrl = page.Url.Replace("&amp;", "").Replace("&", "&amp;");
                result.AppendLine("  <url>");
                result.AppendLine($"    <loc>{sanitizedUrl}</loc>");
                result.AppendLine($"    <lastmod>{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00")}</lastmod>");
                result.AppendLine("    <priority>1.00</priority>");
                // Uncomment and use if you wish to include title and description in the future
                //result.AppendLine($"    <title>{page.Title}</title>");
                //result.AppendLine($"    <description>{page.MetaDescription}</description>");
                result.AppendLine("  </url>");
            }

            result.AppendLine("</urlset>");

            return result.ToString();

        }

            private List<PageInfo> CrawlWebsite(string websiteUrl)
        {
            var pages = new List<PageInfo>();
            var web = new HtmlWeb();

            try
            {
                var doc = web.Load(websiteUrl);
                var baseUrl = new Uri(websiteUrl);

                foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string href = link.GetAttributeValue("href", "");
                    if (!string.IsNullOrEmpty(href) && Uri.TryCreate(baseUrl, href, out var absoluteUri))
                    {
                        var pageInfo = GetPageInfo(absoluteUri.ToString());
                        if (pageInfo != null)
                        {
                            pages.Add(pageInfo);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle web-related exceptions (e.g., invalid URL, network issues)
                Console.WriteLine($"WebException: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return pages;
        }

        private PageInfo GetPageInfo(string url)
        {
            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var pageInfo = new PageInfo
                {
                    Url = url,
                    Title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim(),
                    MetaDescription = doc.DocumentNode.SelectSingleNode("//meta[@name='description']")?.GetAttributeValue("content", "").Trim()
                };

                return pageInfo;
            }
            catch (WebException ex)
            {
                // Handle web-related exceptions (e.g., inaccessible page)
                Console.WriteLine($"WebException: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

     
        public class PageInfo
        {
            public string Url { get; set; }
            public string Title { get; set; }
            public string MetaDescription { get; set; }
        }

        protected void btn_generate_Click(object sender, EventArgs e)
        {
            result_div.Visible = true;
        }

        //-----------------------------------------------------------------sitemap xml ends----------------------------------------------------------------
        //-----------------------------------------------------------------Broken Links--------------------------------------------------------------------

        private long GetWebPageSize(string url)
        {
            using (WebClient client = new WebClient())
            {
                byte[] data = client.DownloadData(url);
                return data.Length;
            }
        }


        private double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        private (int brokenLinksCount, string brokenLinksText) FindBrokenLinksInfo(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(url);

            int brokenLinksCount = 0;
            StringBuilder brokenLinksText = new StringBuilder();

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                string href = link.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(href) && !Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    Uri absoluteUri = new Uri(new Uri(url), href);

                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.OpenRead(absoluteUri); // If this succeeds, the link is not broken
                        }
                    }
                    catch (WebException)
                    {
                        brokenLinksCount++;
                        brokenLinksText.AppendLine($"<br/>{absoluteUri} ");
                    }
                }
            }

            return (brokenLinksCount, brokenLinksText.ToString());
        }
        //-----------------------------------------------------------------Broken Links ends-------------------------------------------------------------------

        //-----------------------------------------------------------------Crawling Starts-------------------------------------------------------------------

        private async System.Threading.Tasks.Task<int> GetCrawledPageCount(string apiKey, string cx, string urlCrawl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string apiUrl = $"https://www.googleapis.com/customsearch/v1?q=site:{urlCrawl}&key={apiKey}&cx={cx}";
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(json);

                        if (result.searchInformation != null)
                        {
                            return Convert.ToInt32(result.searchInformation.totalResults);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        protected void btn_popup_Click(object sender, EventArgs e)
        {
            if (popup.Visible == false)
            {
                popup.Visible = true;
            }
            else
            {
                popup.Visible = false;
            }
        }

        protected void btn_close_Click(object sender, ImageClickEventArgs e)
        {
            popup.Visible=false;
        }



        //-----------------------------------------------------------------Crawling ends-------------------------------------------------------------------
    }


}