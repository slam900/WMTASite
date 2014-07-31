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
                                        District Registration
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/DistrictRegistration.aspx?action=1" class="smaller-font">Add Registration</a></li>
                                        <li><a href="../Events/DistrictRegistration.aspx?action=2" class="smaller-font">Edit Registration</a></li>
                                        <li><a href="../Events/DistrictRegistration.aspx?action=3" class="smaller-font">Delete Registration</a></li>
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
                                <a href="../BadgerPointEntry.aspx" class="list-group-item">Enter Badger Points</a>
                            </div>
                            <h4>Tools</h4>
                            <div class="list-group smaller-font">
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Reports
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Reporting/StudentReportsPerDistrict.aspx" class="smaller-font">Student Reports</a></li>
                                        <li><a href="../Reporting/StudentResultReports.aspx" class="smaller-font">Student Results</a></li>
                                        <li><a href="../Reporting/TeacherReportsPerDistrict.aspx" class="smaller-font">Teacher Reports by District</a></li>
                                        <li><a href="../Reporting/TeacherReportsPerTeacher.aspx" class="smaller-font">Teacher Reports by Teacher</a></li>
                                        <li><a href="../Reporting/JudgeReports.aspx" class="smaller-font">Judge Reports</a></li>
                                        <li><a href="../Reporting/JudgingForms.aspx" class="smaller-font">Judging Forms</a></li>
                                        <li><a href="../Reporting/CheckInReports.aspx" class="smaller-font">Check-In Forms</a></li>
                                        <li><a href="../Reporting/RoomScheduleReport.aspx" class="smaller-font">Room Schedule</a></li>
                                        <li><a href="../Reporting/DistrictAuditionStatsReport.aspx" class="smaller-font">District Audition Statistics</a></li>
                                        <li><a href="../Reporting/DistrictChairSummaryReport.aspx" class="smaller-font">District Chair Summary</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div class="control-column smaller-font">
                            <h4>People</h4>
                            <div class="list-group">
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Students
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Contacts/ManageStudents.aspx?action=1" class="smaller-font">Add Students</a></li>
                                        <li><a href="../Contacts/ManageStudents.aspx?action=2" class="smaller-font">Edit Students</a></li>
                                        <li><a href="../Contacts/ManageStudents.aspx?action=3" class="smaller-font">Delete Students</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Contacts
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Contacts/ManageContacts.aspx?action=1" class="smaller-font">Add Contacts</a></li>
                                        <li><a href="../Contacts/ManageContacts.aspx?action=2" class="smaller-font">Edit Contacts</a></li>
                                    </ul>
                                </div>
                            </div>
                            <h4>Repertoire</h4>
                            <asp:Panel runat="server" ID="pnlCompositionPermissions" Visible="false">
                                <div class="list-group smaller-font">
                                    <div class="btn-group full-width smaller-font">
                                        <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                            Manage Repertoire
                                    <span class="caret"></span>
                                        </button>
                                        <ul class="dropdown-menu">
                                            <li><a href="../CompositionTools/ManageRepertoire.aspx?action=1" class="smaller-font">Add Composition</a></li>
                                            <li><a href="../CompositionTools/ManageRepertoire.aspx?action=2" class="smaller-font">Edit Composition</a></li>
                                            <li><a href="../CompositionTools/ManageRepertoire.aspx?action=3" class="smaller-font">Delete Composition</a></li>
                                        </ul>
                                    </div>
                                    <a href="../CompositionTools/ReplaceComposerName.aspx" class="list-group-item">Replace Composer Name</a>
                                    <a href="../CompositionTools/ReplaceComposition.aspx" class="list-group-item">Replace Composition</a>
                                    <a href="../CompositionTools/TitleLookup.aspx" class="list-group-item">Composition Title Finder</a>
                                    <a href="../CompositionTools/CompositionUsed.aspx" class="list-group-item">Composition Usage</a>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlNoCompositionPermissions">
                                <div class="list-group smaller-font">
                                    <a href="../CompositionTools/ManageRepertoire.aspx?action=1" class="list-group-item">Add Composition</a>
                                </div>
                            </asp:Panel>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
