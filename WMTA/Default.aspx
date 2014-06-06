<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WMTA.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="Styles/LoginStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" Runat="Server">
    <br /><br /><br />
    <table>
        <tr>
            <th class="th" style="border-bottom: 1px solid black">Login</th>
        </tr>
        <tr>
            <td>
                <p> <label for="Username">Username</label> <input type="text" runat="server" id="txtUsername" /><br /><br />
                <label for="Password">Password</label> <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" /></p>
            </td>
        </tr>
        <tr>
            <td style="background-color: #ccccff; border-top: 1px solid black; height: 35px">
                <asp:Button ID="btnLogin" runat="server" Text="Login" Class="button" OnClick="btnLogin_Click" />
            </td>
        </tr>
    </table>
    <label id="lblError" runat="server" visible="false" style="color:red; text-align:center; margin-left:auto; margin-right:auto">Invalid Login</label>
</asp:Content>
