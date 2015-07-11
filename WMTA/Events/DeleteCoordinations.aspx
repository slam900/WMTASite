<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="DeleteCoordinations.aspx.cs" Inherits="WMTA.Events.DeleteCoordinations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <div class="form-horizontal">
                                    <%-- Start of form --%>
                                    <fieldset>
                                        <legend id="legend" runat="server">Delete Student Coordinations</legend>
                                        <%-- Student search --%>
                                        <asp:UpdatePanel ID="upStudentSearch" runat="server">
                                            <ContentTemplate>
                                                <div>
                                                    <h4>Student Search</h4>
                                                    <br />
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtStudentId" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtStudentId" CssClass="form-control" />
                                                        </div>
                                                        <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnStudentSearch_Click" />
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtFirstName" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" />
                                                        </div>
                                                        <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudentSearch_Click" />
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtLastName" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3-margin popover-font">
                                                        <a href="#" id="searchHint">Search Tip</a>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Panel ID="pnlMyStudents" runat="server" Visible="false">
                                                            <div class="checkbox">
                                                                <label>
                                                                    <input type="checkbox" id="chkMyStudentsOnly" runat="server" />
                                                                    Only My Students
                                                                </label>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                            OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged" />
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%-- End Student Search --%>
                                    </fieldset>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- Audition selection --%>
                        <asp:UpdatePanel ID="upAuditions" runat="server" Visible="false">
                            <ContentTemplate>
                                <div>
                                    <h4>Audition Points</h4>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="lblStudent" CssClass="col-md-3 control-label float-left">Student</asp:Label>
                                        <div class="col-md-6 label-top-margin">
                                            <asp:Label runat="server" ID="lblStudent" /><label runat="server" id="lblStudId" visible="false" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="col-md-3 control-label">Year</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlYear" CssClass="txt-danger vertical-center font-size-12" ErrorMessage="Year is required"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label AssociatedControlID="ddlAuditionType" runat="server" CssClass="col-md-3 control-label float-left">Event Type</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropdown-list form-control" OnSelectedIndexChanged="ddlAuditionType_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Selected="False" Text="District Audition" Value="District"></asp:ListItem>
                                                <asp:ListItem Selected="False" Text="Badger Competition" Value="State"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlAuditionType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Event Type is required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="cboAudition" CssClass="col-md-3 control-label">Audition</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="cboAudition" CssClass="txt-danger vertical-center font-size-12" ErrorMessage="Audition is required"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- End audition selection --%>
                        <%-- Table --%>
                        <asp:UpdatePanel runat="server" Visible="false" ID="pnlCoordinates">
                            <ContentTemplate>
                                <div class="form-group">
                                    <div class="form-group">
                                        <asp:Table ID="tblCoordinates" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
                                            <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
                                                <asp:TableHeaderCell Scope="Column" Text="" />
                                                <asp:TableHeaderCell Scope="Column" Text="Id" Visible="false" />
                                                <asp:TableHeaderCell Scope="Column" Text="Composition" />
                                                <asp:TableHeaderCell Scope="Column" Text="Composer" />
                                                <asp:TableHeaderCell Scope="Column" Text="Period" />
                                                <asp:TableHeaderCell Scope="Column" Text="Level" />
                                                <asp:TableHeaderCell Scope="Column" Text="Time" />
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <%-- End table --%>
                        <hr />
                        <%-- Buttons --%>
                        <asp:Panel runat="server" ID="pnlButtons" Visible="false">
                            <div class="form-group">
                                <div class="col-lg-10 col-lg-offset-2 float-right">
                                    <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                    <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                        </asp:Panel>
                        <%-- End buttons --%>
                        <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>

    <script>
        //show an error message
        function showMainError() {
            var message = $('#MainContent_lblErrorMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "error" });
        };

        //show a warning message
        function showWarningMessage() {
            var message = $('#MainContent_lblWarningMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "warning" });
        };

        //show an informational message
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };
    </script>
</asp:Content>
