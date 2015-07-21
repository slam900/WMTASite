<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CompositionMenu.aspx.cs" Inherits="WMTA.Account.CompositionMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divMain" class="row">
        <div class="well bs-component col-md-8 main-div center">
            <section id="menuForm">
                <div class="form-horizontal">
                    <fieldset>
                        <legend>Menu</legend>
                        <div class="control-column smaller-font">
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
                        </div>
                        <div class="control-column smaller-font">
                            <h4>Help Requests</h4>
                            <div class="list-group smaller-font">
                                <a href="../Resources/ViewHelpRequests.aspx" class="list-group-item">View Help Requests</a>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
