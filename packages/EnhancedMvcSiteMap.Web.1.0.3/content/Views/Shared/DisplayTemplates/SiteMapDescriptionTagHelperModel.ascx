<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.SiteMapDescriptionTagHelperModel,MvcSiteMapProvider]]" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="MvcSiteMapProvider.Web.Html.Models" %>

<% if (Model.CurrentNode != null && !string.IsNullOrEmpty(Model.CurrentNode.Description)) { %>
<meta name="description" content="<%= Model.CurrentNode.Description %>" />
<meta itemprop="description" content="<%= Model.CurrentNode.Description %>" />
<meta property="og:description" content="<%= Model.CurrentNode.Description %>" />
<% } %>