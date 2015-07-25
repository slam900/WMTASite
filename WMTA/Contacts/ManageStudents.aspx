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
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearch_Click" CausesValidation="false" />
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
                                        <asp:Panel ID="pnlFullPage" runat="server" Visible="false" DefaultButton="btnSubmit">
                                            <div>
                                                <h4>Student Information</h4>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="First Name is required." />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtMiddleInitial" CssClass="col-md-3 control-label">Middle Initial</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtMiddleInitial" CssClass="form-control small-txtbx-width" MaxLength="2" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLastName" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Last Name is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtGrade" CssClass="col-md-3 control-label">Grade</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtGrade" CssClass="form-control" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGrade" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Grade is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlDistrict" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true">
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
                                                        <asp:DropDownList ID="cboCurrTeacher" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true">
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
                                                <asp:UpdatePanel runat="server">
                                                    <ContentTemplate>
                                                        <asp:Panel runat="server" ID="pnlLegacyPoints" Visible="false">
                                                            <asp:Label runat="server" ID="lblId" Visible=" false" />
                                                            <div class="form-group">
                                                                <asp:Label runat="server" AssociatedControlID="lblLegacyPoints" CssClass="col-md-3 control-label float-left">Legacy Points</asp:Label>
                                                                <div class="col-md-6" style="padding-top: 8px">
                                                                    <asp:Label ID="lblLegacyPoints" runat="server" CssClass="label-top-margin">0</asp:Label>
                                                                    <asp:Button runat="server" ID="btnEditLegacyPoints" CssClass="btn btn-link" Text="Edit" Visible="false" OnClick="btnEditLegacyPoints_Click" />
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Panel runat="server" ID="pnlEditLegacyPts" Visible="false">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" ID="lblLegacyPtsEdit" AssociatedControlID="txtLegacyPoints" CssClass="col-md-3 control-label float-left">Legacy Points</asp:Label>
                                                                <div class="col-md-2">
                                                                    <asp:TextBox ID="txtLegacyPoints" runat="server" TextMode="Number" CssClass="form-control">0</asp:TextBox>
                                                                </div>
                                                                <div class="col-md-4">
                                                                    <asp:Label runat="server" ID="lblLegacyPtsYear" AssociatedControlID="ddlLegacyPtsYear" CssClass="col-md-3 control-label float-left">Year</asp:Label>
                                                                    <div class="col-md-9">
                                                                        <asp:DropDownList ID="ddlLegacyPtsYear" runat="server" CssClass="dropdown-list form-control float-left" AutoPostBack="true" OnSelectedIndexChanged="ddlLegacyPtsYear_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                        <asp:Panel runat="server" ID="pnlTotalPoints" Visible="false">
                                                            <div class="form-group">
                                                                <asp:Label runat="server" AssociatedControlID="lblTotalPoints" CssClass="col-md-3 control-label float-left">Total Points</asp:Label>
                                                                <div class="col-md-6" style="padding-top: 8px">
                                                                    <asp:Label ID="lblTotalPoints" runat="server" CssClass="label-top-margin">0</asp:Label>
                                                                </div>
                                                            </div>
                                                            <div class="form-group text-align-center" style="margin-left:10%; margin-right:10%">
                                                                Note: The Total Points will not update until you submit the updated data and reload the student's information.
                                                            </div>
                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Panel runat="server" ID="pnlButtons">
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <hr />
                                <asp:Panel runat="server" ID="pnlConfirmDuplicate" Visible="false">
                                    <div class="center text-align-center">
                                        <label class="text-info center">There is already a student with the name you have entered.  Do you want to add the student anyways?</label>
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnYes" Text="Yes" runat="server" CssClass="btn btn-default float-right" OnClick="btnYes_Click" CausesValidation="false" />
                                            <asp:Button ID="btnNo" Text="No" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btNo_Click" CausesValidation="false" />
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
                content: 'Fill in any number of the search fields and click "Search" to find students. Clicking "Search" without filling in any fields will return all students linked to you. First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all students with last names containing "sch"."',
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

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };

    </script>
</asp:Content>
