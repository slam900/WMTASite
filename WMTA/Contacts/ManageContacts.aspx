<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageContacts.aspx.cs" Inherits="WMTA.Contacts.ManageContacts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of Form --%>
                            <fieldset>
                                <legend id="legend" runat="server">Add Contacts</legend>
                                <%-- Contact Search --%>
                                <asp:UpdatePanel ID="upContactSearch" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlContactSearch" runat="server" Visible="false">
                                            <div>
                                                <h4>Contact Search</h4>
                                                <br />
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtContactId" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtContactId" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstNameSearch" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtFirstNameSearch" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" CausesValidation="false" OnClick="btnClearSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLastNameSearch" CssClass="col-md-3 control-label">Last Name</asp:Label>
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
                                                    <asp:GridView ID="gvSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                        OnPageIndexChanging="gvSearch_PageIndexChanging" OnRowDataBound="gvSearch_RowDataBound" OnSelectedIndexChanged="gvSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Contact Search --%>
                                <%-- Contact Information --%>
                                <asp:UpdatePanel ID="upFullPage" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                                            <div>
                                                <h4>Contact Information</h4>
                                                <asp:Panel runat="server" ID="pnlId">
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="lblId" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                        <div class="col-md-6 label-top-margin">
                                                            <asp:Label runat="server" ID="lblId" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstName" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="First Name is required" />
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
                                                    <asp:Label runat="server" AssociatedControlID="txtStreet" CssClass="col-md-3 control-label">Street</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtStreet" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtCity" CssClass="col-md-3 control-label">City</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtCity" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlState" CssClass="col-md-3 control-label float-left">State</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlState" runat="server" CssClass="dropdown-list form-control" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlDataSource5" DataTextField="State" DataValueField="State" OnDataBound="ddlState_DataBound">
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [State] FROM [ConfigStates] ORDER BY [State]"></asp:SqlDataSource>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtZip" CssClass="col-md-3 control-label">ZIP</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtZip" runat="server" MaxLength="5" TextMode="Number" CssClass="form-control"></asp:TextBox><%--AutoPostBack="true" OnTextChanged="txtZip_TextChanged"--%>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="col-md-3 control-label float-left">Email</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Email is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtPhone" CssClass="col-md-3 control-label float-left">Phone</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtPhone" runat="server" TextMode="Phone" MaxLength="12" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhone" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Phone # is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlDistrict" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Not WMTA Member" Value="-1"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrict" CssClass="text-danger vertical-center font-size-12" ErrorMessage="District is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlContactType" CssClass="col-md-3 control-label float-left">Contact Type</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlContactType" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource2" DataTextField="Contact Type" DataValueField="ContactTypeId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlContactType_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True" Value="" Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommandType="StoredProcedure" SelectCommand="sp_DropDownContactType"></asp:SqlDataSource>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlContactType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Contact type is required" />
                                                    </div>
                                                </div>
                                                <asp:Panel ID="pnlJudges" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="chkLstTrack" CssClass="col-md-3 control-label float-left">Track</asp:Label>
                                                        <div class="float-left">
                                                            <asp:CheckBoxList ID="chkLstTrack" runat="server" CssClass="checkboxlist">
                                                                <asp:ListItem>District</asp:ListItem>
                                                                <asp:ListItem>State</asp:ListItem>
                                                            </asp:CheckBoxList>
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="chkLstType" CssClass="col-md-3 control-label float-left">Type</asp:Label>
                                                        <div class="float-left">
                                                            <asp:CheckBoxList ID="chkLstType" runat="server" CssClass="checkboxlist">
                                                                <asp:ListItem>Solo</asp:ListItem>
                                                                <asp:ListItem>Duet</asp:ListItem>
                                                            </asp:CheckBoxList>
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="chkLstCompLevel" CssClass="col-md-3 control-label float-left">Composition Level</asp:Label>
                                                        <div class="float-left">
                                                            <asp:CheckBoxList ID="chkLstCompLevel" runat="server" CssClass="checkboxlist" DataSourceID="SqlDataSource3" DataTextField="Description" DataValueField="CompLevelId">
                                                            </asp:CheckBoxList>
                                                        </div>
                                                        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="chkLstInstrument" CssClass="col-md-3 control-label float-left">Instruments</asp:Label>
                                                        <div class="float-left">
                                                            <asp:CheckBoxList runat="server" ID="chkLstInstrument" CssClass="checkboxlist" DataSourceID="SqlDataSource4" DataTextField="Instrument" DataValueField="Instrument">
                                                            </asp:CheckBoxList>
                                                        </div>
                                                        <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownInstrument" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    </div>
                                                    <%--<div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="chkLstTime" CssClass="col-md-3 control-label float-left">Time Preference</asp:Label>
                                                        <div class="float-left">
                                                            <asp:CheckBoxList runat="server" ID="chkLstTime" CssClass="checkboxlist" DataSourceID="SqlDataSource6" DataTextField="TimeRange" DataValueField="ScheduleId" />
                                                        </div>
                                                        <asp:SqlDataSource ID="SqlDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownJudgeTimePref" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    </div>--%>
                                                </asp:Panel>
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

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };

    </script>
</asp:Content>

