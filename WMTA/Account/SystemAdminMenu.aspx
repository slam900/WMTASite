<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="SystemAdminMenu.aspx.cs" Inherits="WMTA.Account.SystemAdminMenu" %>

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
                                        Manage District Event
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/CreateDistrictAudition.aspx?action=1" class="smaller-font">Add Event</a></li>
                                        <li><a href="../Events/CreateDistrictAudition.aspx?action=2" class="smaller-font">Edit Event</a></li>
                                        <li><a href="../Contacts/JudgeSetup.aspx?action=2" class="smaller-font">Judge Setup</a></li>
                                        <li><a href="../Events/AssignDistrictRoomsAndJudges.aspx" class="smaller-font">Assign Rooms and Judges</a></li>
                                        <li><a href="../Events/Schedule.aspx" class="smaller-font">Create Schedule</a></li>
                                        <%--<li><a href="../Events/ScheduleUpdate.aspx" class="smaller-font">Edit Schedule</a></li>--%>
                                    </ul>
                                </div>
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
                                        <li><a href="../Reporting/DistrictDataDump.aspx" class="smaller-font">View District Registrations</a></li>
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
                                        <li><a href="../Reporting/BadgerChairDataDump.aspx" class="smaller-font">View Badger Registrations</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Student Coordinations
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/CoordinateStudents.aspx" class="smaller-font">Add Coordinations</a></li>
                                        <li><a href="../Events/DeleteCoordinations" class="smaller-font">Delete Coordinations</a></li>
                                    </ul>
                                </div>
                                <a href="../Events/DistrictPointEntry.aspx" class="list-group-item">Enter District Points</a>
                                <a href="../Events/BadgerPointEntry.aspx" class="list-group-item">Enter Badger Points</a>
                                <a href="../Events/HsVirtuosoCompositionPointEntry.aspx" class="list-group-item">Enter Other WMTA Event Points</a>
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
                                        <li><a href="../Reporting/TeacherFeeSummary.aspx" class="smaller-font">Teacher Fee Summary</a></li>
                                        <li><a href="../Reporting/DistrictJudgingForms.aspx" class="smaller-font">District Judging Forms</a></li>
                                        <li><a href="../Reporting/DistrictCheckInForms.aspx" class="smaller-font">District Check-In Forms</a></li>
                                        <li><a href="../Reporting/DistrictRoomSchedules.aspx" class="smaller-font">District Room Schedules</a></li>
                                        <li><a href="../Reporting/DistrictAuditionStatsReport.aspx" class="smaller-font">District Audition Statistics</a></li>
                                        <li><a href="../Reporting/DistrictChairSummaryReport.aspx" class="smaller-font">District Chair Summary</a></li>
                                        <li><a href="../Reporting/TheoryTestReports.aspx" class="smaller-font">Theory Test Reports</a></li>
                                        <li><a href="../Reporting/DistrictExecutiveSummary.aspx" class="smaller-font">District Executive Summary</a></li>
                                        <li><a href="../Reporting/BadgerRegistrationReport.aspx" class="smaller-font">Badger Registration</a></li>
                                        <li><a href="../Reporting/BadgerResults.aspx" class="smaller-font">Badger Results</a></li>
                                        <li><a href="../Reporting/BadgerTeacherFees.aspx" class="smaller-font">Badger Teacher Fee Summary</a></li>
                                        <li><a href="../Reporting/BadgerJudgingForms.aspx" class="smaller-font">Badger Judging Forms</a></li>
                                        <li><a href="../Reporting/BadgerExecutiveSummary.aspx" class="smaller-font">Badger Executive Summary</a></li>
                                        <li><a href="../Reporting/AwardsView.aspx" class="smaller-font">Awards View</a></li>
                                    </ul>
                                </div>
                                <a href="../Resources/ViewHelpRequests.aspx" class="list-group-item">View Help Requests</a>
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
                                        <li><a href="../Contacts/RegisterContact.aspx" class="smaller-font">Register Contacts</a></li>
                                        <li><a href="../Contacts/ManageContacts.aspx?action=2" class="smaller-font">Edit Contacts</a></li>
                                        <li><a href="../Contacts/ManageContacts.aspx?action=3" class="smaller-font">Delete Contacts</a></li>
                                        <li><a href="../Contacts/TransferStudents.aspx" class="smaller-font">Transfer Students</a></li>
                                    </ul>
                                </div>
                            </div>
                            <h4>Repertoire</h4>
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
                                <a href="../CompositionTools/MarkCompositionsReviewed.aspx" class="list-group-item">Mark Compositions Reviewed</a>
                            </div>
                            <h4>Administration</h4>
                            <div class="list-group smaller-font">
                                <a href="../Admin/ManageAuditionLengths.aspx" class="list-group-item">Update Audition Lengths</a>
                                <a href="../Admin/ManageAuditionTrackFees.aspx" class="list-group-item">Update Audition Track Fees</a>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
