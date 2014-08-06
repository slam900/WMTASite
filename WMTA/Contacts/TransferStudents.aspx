<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="TransferStudents.aspx.cs" Inherits="WMTA.Contacts.TransferStudents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of Form --%>
                            <fieldset>
                                <legend id="legend" runat="server">Transfer Contacts</legend>
                                <asp:UpdatePanel ID="upFullPage" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlFullPage" runat="server">
                                            <asp:UpdatePanel ID="upFromTeacher" runat="server">
                                                <ContentTemplate>
                                                    <div>
                                                        <div class="form-group">
                                                            <div>
                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFrom" CssClass="text-danger vertical-center font-size-12 col-md-3-margin" ErrorMessage="Contact 1 is required" />
                                                            </div>
                                                            <asp:Label AssociatedControlID="ddlFrom" runat="server" CssClass="col-md-3 control-label float-left">Transfer From</asp:Label>
                                                            <div class="col-md-6">
                                                                <asp:DropDownList ID="ddlFrom" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true">
                                                                    <asp:ListItem Selected="True" Text="" Value="" />
                                                                </asp:DropDownList>
                                                            </div>
                                                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTeacher" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                            <asp:Button ID="btnFromSearch" runat="server" Text="Search" OnClick="btnFromSearch_Click" CssClass="btn btn-primary btn-min-width-72" CausesValidation="false" />
                                                            <asp:Label ID="lblFromId" runat="server" Visible="false" />
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="pnlFromSearch" runat="server" Visible="false">
                                                        <hr />
                                                        <div>
                                                            <h5>Search for Teacher to Transfer Students From</h5>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFromId" runat="server" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFromId" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnSearchFrom" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchFrom_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFromFirstName" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFromFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnClearFromSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearFromSearch_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFromLastName" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFromLastName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:GridView ID="gvFromSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvFromSearch_PageIndexChanging" OnRowDataBound="gvFromSearch_RowDataBound" OnSelectedIndexChanged="gvFromSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                            </div>
                                                        <hr />
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <asp:UpdatePanel ID="upTo" runat="server">
                                                <ContentTemplate>
                                                    <div>
                                                        <div class="form-group">
                                                            <div>
                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTo" CssClass="text-danger vertical-center font-size-12 col-md-3-margin" ErrorMessage="Contact 2 is required" />
                                                            </div>
                                                            <asp:Label AssociatedControlID="ddlTo" runat="server" CssClass="col-md-3 control-label float-left">Transfer To</asp:Label>
                                                            <div class="col-md-6">
                                                                <asp:DropDownList ID="ddlTo" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true">
                                                                    <asp:ListItem Selected="True" Text="" Value="" />
                                                                </asp:DropDownList>
                                                            </div>
                                                            <asp:Button ID="btnToSearch" runat="server" Text="Search" OnClick="btnToSearch_Click" CssClass="btn btn-primary btn-min-width-72" CausesValidation="false" />
                                                            <asp:Label ID="lblToId" runat="server" Visible="false" />
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="pnlToSearch" runat="server" Visible="false">
                                                        <hr />
                                                        <div>
                                                            <h5>Select Teacher to Transfer Students To</h5>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtToId" runat="server" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtToId" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnSearchTo" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchTo_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtToFirstName" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtToFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnClearToSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearToSearch_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label ID="Label5" AssociatedControlID="txtToLastName" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtToLastName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:GridView ID="gvToSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination" OnPageIndexChanging="gvToSearch_PageIndexChanging" OnRowDataBound="gvToSearch_RowDataBound" OnSelectedIndexChanged="gvToSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                            </div>
                                                        </div>
                                                        <hr />
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <asp:UpdatePanel ID="upButtons" runat="server">
                                                <ContentTemplate>
                                                    <div class="form-group">
                                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" />
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
                        <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('#searchHint').popover(
            {
                trigger: 'hover',
                html: true,
                placement: 'right',
                content: 'Fill in any number of the search fields and click "Search" to find teachers. Clicking "Search" without filling in any fields will return all teachers.  First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all teachers with last names containing "sch"."',
            });
        });

        function showMainError() {
            var message = $('#MainContent_lblErrorMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'error' });
        };

        function showWarningMessage() {
            var message = $('#MainContent_lblWarningMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'warning' });
        };

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };

    </script>
</asp:Content>
