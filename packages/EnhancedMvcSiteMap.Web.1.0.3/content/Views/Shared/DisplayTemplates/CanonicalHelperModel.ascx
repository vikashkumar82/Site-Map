<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl`1[[MvcSiteMapProvider.Web.Html.Models.CanonicalHelperModel,MvcSiteMapProvider]]" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="MvcSiteMapProvider.Web.Html.Models" %>

<% if (Model.CurrentNode != null && !string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrl)) { %>
<link rel="canonical" href="<%= Model.CurrentNode.CanonicalUrl %>" />
<meta property="og:url" content="<%= Model.CurrentNode.CanonicalUrl %>" />
<% } %>
<% if (Model.CurrentNode != null && string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrl) && !string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrlSeo)) { %>
<link rel="canonical" href="<%= Model.CurrentNode.CanonicalUrlSeo %>" />
<meta property="og:url" content="<%= Model.CurrentNode.CanonicalUrlSeo %>" />
<% } %>
<% if (Model.CurrentNode != null && string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrl) && string.IsNullOrEmpty(Model.CurrentNode.CanonicalUrlSeo)) { %>
<link rel="canonical" href="<%= Request.Url %>" />
<meta property="og:url" content="<%= Request.Url %>" />
<% } %>