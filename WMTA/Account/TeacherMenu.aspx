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
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Badger Registration
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="#" class="smaller-font">Add Registration</a></li>
                                        <li><a href="#" class="smaller-font">Edit Registration</a></li>
                                        <li><a href="#" class="smaller-font">Delete Registration</a></li>
                                    </ul>
                                </div>
                                <a href="../Events/CoordinateStudents.aspx" class="list-group-item">Coordinate Students</a>
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
                                        <li><a href="../Reporting/TeacherReportsPerDistrict.aspx" class="smaller-font">Teacher Reports by District</a></li>
                                        <li><a href="../Reporting/TeacherReportsPerTeacher.aspx" class="smaller-font">Teacher Reports by Teacher</a></li>
                                        <li><a href="../Reporting/JudgeReports.aspx" class="smaller-font">Judge Reports</a></li>
                                        <li><a href="../Reporting/JudgingForms.aspx" class="smaller-font">Judging Forms</a></li>
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
                                        <li><a href="../Contacts/ManageContacts.aspx?action=2" class="smaller-font">Edit Contacts</a></li>
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
                                    </ul>
                                </div>
                                <asp:Panel runat="server" ID="pnlCompTools" Visible="false">
                                    <a href="../CompositionTools/ReplaceComposerName.aspx" class="list-group-item">Replace Composer Name</a>
                                    <a href="../CompositionTools/ReplaceComposition.aspx" class="list-group-item">Replace Composition</a>
                                    <a href="../CompositionTools/TitleLookup.aspx" class="list-group-item">Composition Title Finder</a>
                                    <a href="../CompositionTools/CompositionUsed.aspx" class="list-group-item">Composition Usage</a>
                                    <a href="../CompositionTools/ManageRepertoire.aspx?action=3" class="list-group-item">Delete Composition</a>
                                </asp:Panel>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
