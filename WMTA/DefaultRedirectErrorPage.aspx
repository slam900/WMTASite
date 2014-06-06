<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="DefaultRedirectErrorPage.aspx.cs" Inherits="WMTA.DefaultRedirectErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/Style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <h1 id="Head1" runat="server">>DefaultRedirect Error Page</h1>
    <div>
        <h2>DefaultRedirect Error Page</h2>
        An error occurred.  Please contat the system administrator.<br />
        <br />
        Return to the <a href='WelcomeScreen.aspx'>Home Page</a>
    </div>
</asp:Content>
