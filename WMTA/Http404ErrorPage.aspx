<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Http404ErrorPage.aspx.cs" Inherits="WMTA.Http404ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/Style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <h1 id="Head1" runat="server">HTTP 404 Error Page</h1>
    <div>
        <h2>HTTP 404 Error Page</h2>
        File not found.  Please contact the system administrator.<br />
        <br />
        Return to the <a href='WelcomeScreen.aspx'>Home Page</a>
    </div>
</asp:Content>
