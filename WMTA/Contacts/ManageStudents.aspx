<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageStudents.aspx.cs" Inherits="WMTA.Contacts.ManageStudents" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of Form --%>
                            <fieldset>
                                <legend id="legend" runat="server">Add Students</legend>
                                <%-- Student Search --%>
                                <asp:UpdatePanel ID="upStudentSearch" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlStudentSearch" runat="server" Visible="false">
                                            <div>
                                                <h4>Student Search</h4>
                                                <br />
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtStudentId" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtStudentId" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstNameSearch" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtFirstNameSearch" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" CausesValidation="false" OnClick="btnClearSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLastNameSearch" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtLastNameSearch" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div>
                                                    <div class="col-md-3-margin popover-font">
                                                        <a href="#" id="searchHint">Search Tip</a>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="form-group">
                                                    <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                        OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Student Search --%>
                                <%-- Student Information --%>
                                <asp:UpdatePanel ID="upFullPage" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                                            <div>
                                                <h4>Student Information</h4>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" runat="server" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtFirstName" runat="server" AutoPostBack="true" OnTextChanged="txtFirstName_TextChanged" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="First Name is required." />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" CssClass="col-md-3 control-label">Middle Initial</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtMiddleInitial" CssClass="form-control small-txtbx-width" MaxLength="2" AutoPostBack="true" OnTextChanged="txtMiddleInitial_TextChanged" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMiddleInitial" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Middle Initial is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLastName" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtLastName_TextChanged" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Last Name is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtGrade" CssClass="col-md-3 control-label">Grade</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtGrade" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtGrade_TextChanged" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGrade" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Grade is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlDistrict" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlDistrict_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value="" />
                                                        </asp:DropDownList>
                                                    </div>
                                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrict" CssClass="text-danger vertical-center font-size-12" ErrorMessage="District is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="cboCurrTeacher" CssClass="col-md-3 control-label float-left">Current Teacher</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="cboCurrTeacher" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource2" DataTextField="ContactId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="cboCurrTeacher_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTeacher" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="cboCurrTeacher" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Current Teacher is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="cboPrevTeacher" CssClass="col-md-3 control-label float-left">Previous Teacher</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="cboPrevTeacher" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLegacyPoints" ID="lblLegacyPoints" CssClass="col-md-3 control-label float-left">Legacy Points</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtLegacyPoints" runat="server" TextMode="Number">0</asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label AssociatedControlID="lblId" runat="server" CssClass="col-md-3 control-label float-left">Student Id:</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblId" runat="server" Text="" ForeColor="DarkBlue"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Panel runat="server" ID="pnlButtons">
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default float-right" OnClick="btnBack_Click" />
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
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
                $('#searchHint').popover(
                {
                    trigger: 'hover',
                    html: true,
                    placement: 'right',
                    content: 'Fill in any number of the search fields and click "Search" to find contacts. Clicking "Search" without filling in any fields will return all contacts.  First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all contacts with last names containing "sch"."',
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
            function showInfoMessage() {
                var message = $('#MainContent_lblInfoMessage').text();

                $.notify(message.toString(), { position: 'left-top', className: 'info' });
            }

            //show a success message
            function showSuccessMessage() {
                var message = $('#MainContent_lblSuccessMessage').text();

                $.notify(message.toString(), { position: "left-top", className: "info" });
            };

    </script>
</asp:Content>
