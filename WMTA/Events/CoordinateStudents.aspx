<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CoordinateStudents.aspx.cs" Inherits="WMTA.Events.CoordinateStudents" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <fieldset>
                                <legend id="legend" runat="server">Coordinate Students</legend>
                                <asp:UpdatePanel ID="upFullPage" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlFullPage" runat="server">
                                            <asp:UpdatePanel ID="upStudent" runat="server">
                                                <ContentTemplate>
                                                    <asp:Panel ID="pnlStudentSearch" runat="server">
                                                        <div>
                                                            <h4>Student Search</h4>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtStudentId" runat="server" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtStudentId" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnSearchStudent" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchStudent_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFirstName" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudentSearch_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtLastName" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                            </div>
                                                            <hr />
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <asp:UpdatePanel ID="upCoordinates" runat="server">
                                                <ContentTemplate>
                                                    <h4>Students To Coordinate</h4>
                                                    <div class="form-group">
                                                        <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn btn-default btn-min-width-72" OnClick="btnRemove_Click" />
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Table ID="tblCoordinates" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
                                                            <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
                                                                <asp:TableHeaderCell Scope="Column" Text="" />
                                                                <asp:TableHeaderCell Scope="Column" Text="Id" />
                                                                <asp:TableHeaderCell Scope="Column" Text="Name" />
                                                            </asp:TableHeaderRow>
                                                        </asp:Table>
                                                    </div>
                                                    <hr />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="ddlReason" runat="server" CssClass="col-md-3 control-label float-left">Reason</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlReason" runat="server" CssClass="dropdown-list form-control">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        <asp:ListItem Selected="False" Text="Carpool" Value="Carpool"></asp:ListItem>
                                                        <asp:ListItem Selected="False" Text="Sibling" Value="Sibling"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlReason" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Reason is required"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="ddlAuditionType" runat="server" CssClass="col-md-3 control-label float-left">Event Type</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropdown-list form-control">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        <asp:ListItem Selected="False" Text="District Audition" Value="District"></asp:ListItem>
                                                        <asp:ListItem Selected="False" Text="Badger Competition" Value="State"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlAuditionType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Event Type is required" />
                                                </div>
                                            </div>
                                            <asp:UpdatePanel ID="upButtons" runat="server">
                                                <ContentTemplate>
                                                    <div class="form-group">
                                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </div>
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
        $(document).ready(function () {
            //$('#searchHint').popover(
            //{
            //    trigger: 'hover',
            //    html: true,
            //    placement: 'right',
            //    content: 'Fill in any number of the search fields and click "Search" to find students. Clicking "Search" without filling in any fields will return all students linked to you. First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all students with last names containing "sch"."',
            //});

            //$('#MainContent_pnlComposer').hide();
            //$('#MainContent_txtComposition').hide();
        });

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

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };
    </script>
</asp:Content>
