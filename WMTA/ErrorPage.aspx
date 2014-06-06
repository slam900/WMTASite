<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="WMTA.ErrorPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/Style.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <h1 id="Head1" runat="server"> Error Page </h1>
    <div>
        <h2>Error Page</h2>
        <asp:Panel ID="InnerErrorPanel" runat="server" Visible="false">
            <p>
                Inner Error Message:<br />
                <asp:Label ID="innerMessage" runat="server" Font-Bold="true"
                    Font-Size="Large" /><br />
            </p>
            <pre>
        <asp:Label ID="innerTrace" runat="server" />
      </pre>
        </asp:Panel>
        <p>
            Error Message:<br />
            <asp:Label ID="exMessage" runat="server" Font-Bold="true"
                Font-Size="Large" />
        </p>
        <pre>
      <asp:Label ID="exTrace" runat="server" Visible="false" />
    </pre>
        <br />
        Return to the <a href='WelcomeScreen.aspx'>Home Page</a>
    </div>
</asp:Content>
