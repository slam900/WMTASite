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
                                            <asp:UpdatePanel ID="upStudent1" runat="server">
                                                <ContentTemplate>
                                                    <div>
                                                        <div class="form-group">
                                                            <div>
                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStudent1" CssClass="text-danger vertical-center font-size-12 col-md-3-margin" ErrorMessage="Student 1 is required" />
                                                            </div>
                                                            <asp:Label AssociatedControlID="ddlStudent1" runat="server" CssClass="col-md-3 control-label float-left">Student 1</asp:Label>
                                                            <div class="col-md-6">
                                                                <asp:DropDownList ID="ddlStudent1" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="StudentId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlStudent1_SelectedIndexChanged1">
                                                                    <asp:ListItem Selected="True" Text="" Value="" />
                                                                </asp:DropDownList>
                                                            </div>
                                                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownStudent" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                            <asp:Button ID="btnStudent1Search" runat="server" Text="Search" OnClick="btnStudent1Search_Click" CssClass="btn btn-primary btn-min-width-72" CausesValidation="false" />
                                                            <asp:Label ID="lblStudent1Id" runat="server" Visible="false" />
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="pnlStudent1Search" runat="server" Visible="false">
                                                        <hr />
                                                        <div>
                                                            <h5>Student 1 Search</h5>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtStudent1Id" runat="server" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtStudent1Id" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnSearchStudent1" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchStudent1_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFirstName1" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFirstName1" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnClearStudent1Search" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudent1Search_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtLastName1" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtLastName1" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:GridView ID="gvStudent1Search" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvStudent1Search_PageIndexChanging" OnRowDataBound="gvStudent1Search_RowDataBound" OnSelectedIndexChanged="gvStudent1Search_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                            </div>
                                                        <hr />
                                                    </asp:Panel>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <asp:UpdatePanel ID="upStudent2" runat="server">
                                                <ContentTemplate>
                                                    <div>
                                                        <div class="form-group">
                                                            <div>
                                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStudent2" CssClass="text-danger vertical-center font-size-12 col-md-3-margin" ErrorMessage="Student 2 is required" />
                                                            </div>
                                                            <asp:Label AssociatedControlID="ddlStudent2" runat="server" CssClass="col-md-3 control-label float-left">Student 2</asp:Label>
                                                            <div class="col-md-6">
                                                                <asp:DropDownList ID="ddlStudent2" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="StudentId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlStudent2_SelectedIndexChanged1">
                                                                    <asp:ListItem Selected="True" Text="" Value="" />
                                                                </asp:DropDownList>
                                                            </div>
                                                            <asp:Button ID="btnStudent2Search" runat="server" Text="Search" OnClick="btnStudent2Search_Click" CssClass="btn btn-primary btn-min-width-72" CausesValidation="false" />
                                                            <asp:Label ID="lblStudent2Id" runat="server" Visible="false" />
                                                        </div>
                                                    </div>
                                                    <asp:Panel ID="pnlStudent2Search" runat="server" Visible="false">
                                                        <hr />
                                                        <div>
                                                            <h5>Student 2 Search</h5>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtStudent2Id" runat="server" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtStudent2Id" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnSearchStudent2" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchStudent2_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label AssociatedControlID="txtFirstName2" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtFirstName2" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <asp:Button ID="btnClearStudent2Search" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudent2Search_Click" CausesValidation="false" />
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:Label ID="Label5" AssociatedControlID="txtLastName2" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                                <div class="col-md-6">
                                                                    <asp:TextBox ID="txtLastName2" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <asp:GridView ID="gvStudent2Search" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination" OnPageIndexChanging="gvStudent2Search_PageIndexChanging" OnRowDataBound="gvStudent2Search_RowDataBound" OnSelectedIndexChanged="gvStudent2Search_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                            </div>
                                                        </div>
                                                        <hr />
                                                    </asp:Panel>
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
