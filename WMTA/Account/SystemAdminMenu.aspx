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
                                        <li><a href="#" class="smaller-font">Add Event</a></li>
                                        <li><a href="#" class="smaller-font">Edit Event</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Badger Event
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="#" class="smaller-font">Add Event</a></li>
                                        <li><a href="#" class="smaller-font">Edit Event</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        District Registration
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="../Events/DistrictRegistration.aspx?action=1" class="smaller-font">Add Registration</a></li>
                                        <%--<li><a href="../DistrictRegistration.aspx?action=1" class="smaller-font">Add Registration</a></li>--%>
                                        <li><a href="../DistrictRegistration.aspx?action=2" class="smaller-font">Edit Registration</a></li>
                                        <li><a href="../DistrictRegistration.aspx?action=3" class="smaller-font">Delete Registration</a></li>
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
                                <a href="../CoordinateStudents.aspx" class="list-group-item">Coordinate Students</a>
                                <a href="../DistrictPointEntry.aspx" class="list-group-item">Enter District Points</a>
                                <a href="../BadgerPointEntry.aspx" class="list-group-item">Enter Badger Points</a>
                                <a href="../HsViruosoCompositionPointEntry.aspx" class="list-group-item">Enter HS Virtuoso Points</a>
                            </div>
                            <h4>Tools</h4>
                            <div class="list-group smaller-font">
                                <a href="../Reports.aspx" class="list-group-item">Reports</a>
                                <a href="../Resources.aspx" class="list-group-item">Resources</a>
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
                                        <li><a href="#" class="smaller-font">Add Students</a></li>
                                        <li><a href="#" class="smaller-font">Edit Students</a></li>
                                        <li><a href="#" class="smaller-font">Delete Students</a></li>
                                    </ul>
                                </div>
                                <div class="btn-group full-width smaller-font">
                                    <button type="button" class="list-group-item dropdown-toggle dropdown-list-item" data-toggle="dropdown">
                                        Manage Contacts
                                    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu">
                                        <li><a href="#" class="smaller-font">Add Contacts</a></li>
                                        <li><a href="../RegisterContacts.aspx" class="smaller-font">Register Contacts</a></li>
                                        <li><a href="#" class="smaller-font">Edit Contacts</a></li>
                                        <li><a href="#" class="smaller-font">Delete Contacts</a></li>
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
                                        <li><a href="#" class="smaller-font">Add Composition</a></li>
                                        <li><a href="#" class="smaller-font">Edit Composition</a></li>
                                        <li><a href="#" class="smaller-font">Delete Composition (placeholder)</a></li>
                                    </ul>
                                </div>
                                <a href="../Repertoire2.aspx" class="list-group-item">Replace Composer Name (placeholder)</a>
                                <a href="../Repertoire2.aspx" class="list-group-item">Replace Composition (placeholder)</a>
                                <a href="../Repertoire2.aspx" class="list-group-item">Composition Title Finder (placeholder)</a>
                                <a href="../Repertoire2.aspx" class="list-group-item">Composition Usage Finder (placeholder)</a>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>

    <script>
        $(document).ready(function () {

            $('#lnkDistrictReg').click(function () {
                
            });
        });
    </script>
</asp:Content>
