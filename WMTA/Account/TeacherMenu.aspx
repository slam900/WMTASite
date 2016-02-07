<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="TeacherMenu.aspx.cs" Inherits="WMTA.Account.TeacherMenu" %>

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
                                        <li><a href="../Reporting/DistrictJudgingForms.aspx" class="smaller-font">District Judging Forms</a></li>
                                        <li><a href="../Reporting/TeacherFeeSummary.aspx" class="smaller-font">Teacher Fee Summary</a></li>
                                        <li><a href="../Reporting/BadgerRegistrationReport.aspx" class="smaller-font">Badger Registration</a></li>
                                        <li><a href="../Reporting/BadgerTeacherFees.aspx" class="smaller-font">Badger Teacher Fee Summary</a></li>
                                        <li><a href="../Reporting/AwardsView.aspx" class="smaller-font">Awards View</a></li>
                                        <li><a href="../Reporting/FullDataDump.aspx" class="smaller-font">Full Year Data</a></li>
                                    </ul>
                                </div>
                            </div>
                            <asp:Panel runat="server" ID="pnlHelp" Visible="false">
                                <h4>Help Requests</h4>
                                <div class="list-group smaller-font">
                                    <a href="../Resources/ViewHelpRequests.aspx" class="list-group-item">View Help Requests</a>
                                </div>
                            </asp:Panel>
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
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Contacts
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
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
                                    <a href="../CompositionTools/MarkCompositionsReviewed.aspx" class="list-group-item">Mark Compositions Reviewed</a>
                                </div>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlNoCompositionPermissions">
                                <div class="list-group smaller-font">
                                    <a href="../CompositionTools/TitleLookup.aspx" class="list-group-item">Composition Title Finder</a>
                                    <a href="../CompositionTools/ManageRepertoire.aspx?action=1" class="list-group-item">Manage Composition</a>
                                </div>
                            </asp:Panel>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
