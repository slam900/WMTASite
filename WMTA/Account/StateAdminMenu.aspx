<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="StateAdminMenu.aspx.cs" Inherits="WMTA.Account.StateAdminMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divMain" class="row">
        <div class="well bs-component col-md-8 main-div center">
            <section id="menuForm">
                <div class="form-horizontal">
                    <fieldset>
                        <legend>Menu</legend>
                        <div class="control-column">
                            <h4>Events</h4>
                            <div class="list-group smaller-font">
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Badger Event
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/CreateBadgerAudition.aspx?action=1" class="smaller-font">Add Event</a></li>
                                        <li><a href="../Events/CreateBadgerAudition.aspx?action=2" class="smaller-font">Edit Event</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Badger Registration
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/BadgerRegistration.aspx?action=1" class="smaller-font">Add Registration</a></li>
                                        <li><a href="../Events/BadgerRegistration.aspx?action=2" class="smaller-font">Edit Registration</a></li>
                                        <li><a href="../Events/BadgerRegistration.aspx?action=3" class="smaller-font">Delete Registration</a></li>
                                    </ul>
                                </div>
                                <a href="../Events/CoordinateStudents.aspx" class="list-group-item">Coordinate Students</a>
                                <a href="../Events/BadgerPointEntry.aspx" class="list-group-item">Enter Badger Points</a>
                            </div>
                            <h4>Tools</h4>
                            <div class="list-group smaller-font">
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Reports
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Reporting/StudentHistoryReports.aspx" class="smaller-font">Student History</a></li>
                                        <li><a href="../Reporting/StudentReportsPerDistrict.aspx" class="smaller-font">Student Registration</a></li>
                                        <li><a href="../Reporting/StudentResultReports.aspx" class="smaller-font">Student Results</a></li>
                                        <li><a href="../Reporting/DistrictCollationRoomReports.aspx" class="smaller-font">District Collation Room Reports</a></li>
                                        <li><a href="../Reporting/DistrictChairFeeSummary.aspx" class="smaller-font">District Chair Fee Summary</a></li>
                                        <li><a href="../Reporting/DistrictJudgingForms.aspx" class="smaller-font">District Judging Forms</a></li>
                                        <li><a href="../Reporting/DistrictCheckInForms.aspx" class="smaller-font">District Check-In Forms</a></li>
                                        <li><a href="../Reporting/DistrictRoomSchedules.aspx" class="smaller-font">District Room Schedules</a></li>
                                        <li><a href="../Reporting/DistrictAuditionStatsReport.aspx" class="smaller-font">District Audition Statistics</a></li>
                                        <li><a href="../Reporting/DistrictChairSummaryReport.aspx" class="smaller-font">District Chair Summary</a></li>
                                        <li><a href="../Reporting/TheoryTestReports.aspx" class="smaller-font">Theory Test Reports</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="control-column smaller-font">
                            <asp:Panel runat="server" ID="pnlHelp" Visible="false">
                                <h4>Help Requests</h4>
                                <div class="list-group smaller-font">
                                    <a href="../Resources/ViewHelpRequests.aspx" class="list-group-item">View Help Requests</a>
                                </div>
                            </asp:Panel>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
