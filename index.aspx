<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="demoSitemap.index" Async="true" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Style/StyleSheet1.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN" crossorigin="anonymous" />
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://kit.fontawesome.com/348356cdb0.js" crossorigin="anonymous"></script>

</head>
<body>
    <form id="form1" runat="server">

        <div class="container sitemap-hero p-5">
            <div class="row">
                <div class="col-md-8">
                    <div class="sitemap-tool">
                        <h2 style="text-align: center;">Sitemap Downloader</h2>
                        <div class="sitemap-content mt-2 p-3">
                            <center>
                                <p>To create a sitemap enter your website URL.</p>
                            </center>
                            <div class="row">

                                <div class="col-md-2">
                                    <label for="urlTextBox">Enter URL:</label>
                                </div>
                                <div class="col-md-10">
                                    <asp:TextBox ID="urlTextBox" runat="server" CssClass="form-control" placeholder="https://www.ranknotebook.org/"></asp:TextBox>
                                </div>
                            </div>
                            <p></p>
                            <div class="button-one">
                                <div class="row">
                                    <div class="col-md-5">
                                        <asp:Button ID="btn_popup" runat="server" Text="Why do you need a sitemap?" OnClick="btn_popup_Click" />
                                        <div class="popupContent" id="popup" runat="server" visible="false">
                                            <div class="button-close" style="float:right;">
                                                <asp:ImageButton ID="btn_close" runat="server" ImageUrl="~/picture/close.png" OnClick="btn_close_Click"/>
                                            </div>
                                            
                                            <p>The purpose of this website is to enable you to:</p>
                                            
                                            <li>Generate an XML sitemap that can be submitted to search engines like Google and Bing, aiding in the improved crawling of your website.</li>
                                            <li>Produce a Text sitemap for a straightforward list of all your web pages.</li>
                                            <li>Create an HTML site map, enhancing the navigation experience for visitors to your website.</li>
                                             <span><p class="mt-2">What is sitemap</p></span>
                                            <li>Uploading a properly structured XML sitemap onto your web server allows Search Engine crawlers, such as Google, to identify and track the pages on your website. It helps them determine the existing pages, identify recent changes, and systematically navigate and index your site based on the provided sitemap information.</li><p></p>
                                            <li>A sitemap organizes a website by detailing URLs and corresponding data in each section. Initially user-focused, modern sitemaps, especially Google's XML format, optimize search engine efficiency, accelerating data discovery and retrieval.</li>
                                        </div>
                                    </div>
                                    <div class="col-md-7">
                                        <asp:Button ID="generateButton" runat="server" Text="Generate" OnClick="GenerateSitemap" OnClientClick="showLoading();" class="btn btn-success" />
                                    </div>
                                </div>
                            </div>
                            <div id="loadingDiv" style="display: none;">
                                <div class="loading-spinner"></div>
                            </div>
                        </div>
                        <div class="sitemap-url-section mt-5 p-4" runat="server" id="urlSectionDiv" visible="false">
                            <div class="logo-centre">
                                <div class="result-icon">
                                    <p><span class="icon-logo"><i class="fa fa-outdent" aria-hidden="true"></i></span>&nbsp;&nbsp;Sitemap Preview</p>
                                </div>
                                <div class="sitemap-url-section">
                                    <div>
                                        <asp:Button ID="btn_generate" runat="server" Text="VIEW FULL XML SITEMAP" CssClass="btn btn-success" Width="250px" OnClick="btn_generate_Click" />
                                    </div>
                                </div>
                            </div>
                            <div class="sitemap-urls mt-2 p-2" runat="server" id="resulturl">
                            </div>
                        </div>
                        <div class="sitemap-content-xml mt-5 p-3" visible="false" runat="server" id="result_div">
                            <asp:TextBox ID="resultTextBox" runat="server" TextMode="MultiLine" Columns="50" Rows="50" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                            <p></p>
                            <center>
                                <asp:Button ID="btn_downloadXML" runat="server" Text="Download XML" CssClass="btn btn-success " OnClientClick="downloadXML()" /></center>
                        </div>
                        <div class="broken-links mt-5" id="linksbroken" runat="server" visible="false">

                            <div class="counter-section " style="justify-content: center;">
                                <div class="counter-cards1" style="background: #5633cf; color: aliceblue">
                                    <span>Size Of Page</span>&nbsp;&nbsp;<span class="same"><asp:Label ID="sizeLabel" runat="server" Text="0"></asp:Label></span>
                                </div>
                                <div class="counter-cards2" style="background: #ff9933; color: aliceblue">
                                    <span>Crawled pages</span>&nbsp;&nbsp;<span class="same"><asp:Label ID="crawlresult" runat="server" Text="0"></asp:Label></span>
                                </div>
                                <div class="counter-cards3" style="background: #3d75bb; color: aliceblue">
                                    <span>Broken Links</span>&nbsp;&nbsp;<span class="same"><asp:Label ID="lbl_broken" runat="server" Text="0"></asp:Label></span>
                                </div>
                            </div>

                            <div class="row p-3">
                                <div class="col-md-9">
                                    <h3>BROKEN LINKS</h3>
                                </div>
                                <div class="col-md-3 broken-link-labelDiv" runat="server" visible="false" id="brokenCountDiv">
                                    <span class="badge text-bg-secondary">
                                        <asp:Label ID="brokenLinksLabel" runat="server" Text="0"></asp:Label></span>
                                </div>
                            </div>
                            <div class="broken-linksURL" style="margin-top: -50px; padding: 22px;">
                                <span>
                                    <asp:Label ID="lblcount" runat="server" Text=""></asp:Label></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-4 mt-4 p-3">
                </div>
            </div>

        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL" crossorigin="anonymous"></script>
    <script>
        function showLoading() {
            // Show the loading animation when the button is clicked
            document.getElementById('loadingDiv').style.display = 'block';
        }
    </script>
    <script>
        function downloadXML() {

            // date start 
            var currentDate = new Date();

            // Get the day, month (+1 because getMonth() returns 0-11), and year
            var day = currentDate.getDate();
            var month = currentDate.getMonth() + 1; // Add 1 because January is 0
            var year = currentDate.getFullYear();

            // Format the day and month to ensure they are always two digits
            var formattedDay = day < 10 ? '0' + day : day;
            var formattedMonth = month < 10 ? '0' + month : month;

            // Concatenate to get the desired format "date-month-year"
            var dateString = formattedDay + '-' + formattedMonth + '-' + year;


            // date ends


            // Get content from the textarea
            var xmlContent = document.getElementById("resultTextBox").value;

            // Create a Blob containing the XML content
            var blob = new Blob([xmlContent], { type: "application/xml" });

            // Create a link element
            var a = document.createElement("a");

            // Set the href attribute to the Blob URL
            a.href = window.URL.createObjectURL(blob);

            // Set the download attribute with the desired filename
           
            a.download = "output- " + dateString +".xml" ; /**/

            // Append the link to the document
            document.body.appendChild(a);

            // Programmatically trigger a click event on the link
            a.click();

            // Remove the link from the document
            document.body.removeChild(a);
        }
    </script>

</body>
</html>

