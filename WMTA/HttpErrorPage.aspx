<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="HttpErrorPage.aspx.cs" Inherits="WMTA.HttpErrorPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <h1 id="Head1" runat="server">Http Error Page</h1>
    <div>
        <h2>Http Error Page</h2>
        <asp:Panel ID="InnerErrorPanel" runat="server" Visible="false">
            <asp:Label ID="innerMessage" runat="server" Font-Bold="true"
                Font-Size="Large" /><br />
            <pre>
        <asp:Label ID="innerTrace" runat="server" />
      </pre>
        </asp:Panel>
        Error Message:<br />
        <asp:Label ID="exMessage" runat="server" Font-Bold="true"
            Font-Size="Large" />
        <pre>
      <asp:Label ID="exTrace" runat="server" Visible="false" />
    </pre>
        <br />
        Return to the <a href='WelcomeScreen.aspx'>Home Page</a>
    </div>
</asp:Content>
