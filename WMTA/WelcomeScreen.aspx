<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="WelcomeScreen.aspx.cs" Inherits="WMTA.WelcomeScreen" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <link href="Styles/WelcomeScreenStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .auto-style3 {
            width: 205px;
        }
    </style>

    <h1>Welcome</h1>
    <table>
        <tr>
            <td class="auto-style3"><a runat="server" id="lnkDistrictAudition" style="font-size:14px;" href="CreateDistrictAudition.aspx"> Manage District Audition Event </a></td>
            <td><a runat="server" id="lnkBadgerAudition" style="font-size:14px;" href="CreateStateAudition.aspx"> Manage Badger Competition Event </a></td>
            <td><a style="font-size:14px;" ></a></td>
        </tr>
        <tr>
            <td class="auto-style3"><a runat="server" id="lnkDistrictRegistration" style="font-size:14px;" href="DistrictRegistration.aspx">District Registration</a></td>
            <td><a runat="server" id="lnkBadgerRegistration" style="font-size:14px;" href="BadgerRegistration.aspx"> Badger Registration </a></td>
            <td><a runat="server" id="lnkCoordinateStudents" style="font-size:14px;" href="CoordinateStudents.aspx">Coordinate Students</a></td>
            
        </tr>
        <tr>
            <td class="auto-style3"><a runat="server" id="lnkDistrictPoints" style="font-size:14px;" href="DistrictPointEntry.aspx">Enter District Points</a></td>
            <td><a runat="server" id="lnkBadgerPoints" style="font-size:14px;" href="BadgerPointEntry.aspx"> Enter Badger Points </a></td>
            <td><a runat="server" id="lnkEnterHsVPoints" style="font-size:14px;" href="HsViruosoCompositionPointEntry.aspx">Enter HS Virtuoso/Composition Points</a></td>
        </tr>
        <tr>
            <td><a runat="server" id="lnkManageStudents" style="font-size:14px;" href="ManageStudents.aspx"> Manage Students </a></td>
            <td><a runat="server" id="lnkManageContacts" style="font-size:14px;" href="ManageContacts.aspx"> Manage Contacts </a></td>
            <td class="auto-style3"><a runat="server" id="lnkManageRepertoire" style="font-size:14px;" href="Repertoire2.aspx"> Manage Repertoire</a></td>
        </tr>
        <tr>
            <td class="auto-style3"><a runat="server" id="lnkRegisterContacts" style="font-size:14px;" href="RegisterContact.aspx">Register Contacts</a></td>
            <td><a runat="server" id="lnkReports" style="font-size:14px;" href="Reports.aspx"> Reports </a></td>
            <td><a runat="server" id="lnkResources" style="font-size:14px;" href="Resources.aspx">Resources</a></td>
        </tr>
    </table>
    <br />
</asp:Content>
