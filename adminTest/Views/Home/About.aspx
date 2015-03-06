<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    关于 - 我的 ASP.NET MVC 应用程序
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1>关于。</h1>
        <h2><%: ViewBag.Message %></h2>
    </hgroup>

    <article>
        <p>
            使用此区域可提供附加信息。
        </p>

        <p>
            使用此区域可提供附加信息。
        </p>

        <p>
            使用此区域可提供附加信息。
        </p>
    </article>

    <aside>
        <h3>副标题</h3>
        <p>
            使用此区域可提供附加信息。
        </p>
        <ul>
            <li><%: Html.ActionLink("主页", "Index", "Home") %></li>
            <li><%: Html.ActionLink("关于", "About", "Home") %></li>
            <li><%: Html.ActionLink("联系方式", "Contact", "Home") %></li>
        </ul>
    </aside>
</asp:Content>