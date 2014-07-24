<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateBadgerAudition.aspx.cs" Inherits="WMTA.Events.CreateBadgerAudition" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runa="server">
    <div class="row">
        <section id="registrationForm">
            <asp:UpdatePanel ID="upFullPage" runat="server">
                <div class="form-horizontal">
                    <fieldset>
                        <legend>Manage Badger Audition</legend>
                        <asp:UpdatePanel ID="upAuditionSearch" runat="server">
                            <ContentTemplate>
                                <div>
                                    <h4>Audition Search</h4>
                                    <br />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlDistrictSearch" CssClass="col-md-3 control-label float-left">Region</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrictSearch" CssClass="text-danger vertical-center font-size-12" ErrorMessage="District is required" ValidationGroup="Required" />
                                        </div>
                                        <asp:Button ID="btnAuditionSearch" runat="server" Text="Search" CssClass="btn btn-default btn-min-width-72" OnClick="btnAuditionSearch_Click" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="col-md-3 control-label float-left">Audition Year</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control"></asp:DropDownList>
                                        </div>
                                        <asp:Button ID="btnClearAuditionSearch" runat="server" Text="Clear" CssClass="btn btn-clear btn-min-width-72" />
                                    </div>
                                    <div class="form-group">
                                        <asp:GridView ID="gvAuditionSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvAuditionSearch_PageIndexChanging" OnRowDataBound="gvAuditionSearch_RowDataBound" OnSelectedIndexChanged="gvAuditionSearch_SelectedIndexChanged"></asp:GridView>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upMain" runat="server">
                            <ContentTemplate>
                                <asp:Panel ID="pnlMain" runat="server" CssClass="display-none">
                                    <asp:TextBox ID="txtIdHidden" runat="server" CssClass="display-none"></asp:TextBox>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlDistrict" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlDistrict_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrict" CssClass="text-danger vertical-center font-size-12" ErrorMessage="District is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtVenue" CssClass="col-md-3 control-label float-left">Venue</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtVenue" runat="server" CssClass="form-control" OnTextChanged="txtVenue_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrict" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Venue is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlDuets" CssClass="col-md-3 control-label float-left">Duets</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlDuets" runat="server" CssClass="dropdown-list form-control">
                                                <asp:ListItem Selected="True" Text="No" Value="No"></asp:ListItem>
                                                <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtNumJudges" CssClass="col-md-3 control-label float-left">Number of Judges</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtNumJudges" runat="server" CssClass="form-control"OnTextChanged="txtNumJudges_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumJudges" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Number of Judges is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlChairPerson" CssClass="col-md-3 control-label float-left">Chairperson</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlChairPerson" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" OnSelectedIndexChanged="ddlChairPerson_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="" SelectCommand="sp_DropDownChairperson" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                            <div>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlChairPerson" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Chairperson is required" ValidationGroup="Required" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtDate" CssClass="col-md-3 control-label float-left">Audition Date</asp:Label>
                                        <div class="col-md-6">
                                            <input runat="server" type="text" id="txtDate"  class="ui-datepicker form-control" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Date is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <%-- lblTimeError Goes Here "The Start Time must be before the End Time --%>
                                        <asp:Label runat="server" AssociatedControlID="txtStartTime" CssClass="col-md-3 control-label float-left">Start Time</asp:Label>
                                        <div class="col-md-6">
                                            <input runat="server" id="txtStartTime" type="text" class="ui-timepicker form-control" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartTime" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Start Time is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtEndTime" CssClass="col-md-3 control-label float-left">End Time</asp:Label>
                                        <div class="col-md-6">
                                            <input runat="server" id="txtEndTime" type="text" class="ui-timepicker form-control" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndTime" CssClass="text-danger vertical-center font-size-12" ErrorMessage="End Time is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <%-- lblFreezeDateError2 goes here "The Freeze DAte must be before the Audition Date --%>
                                        <asp:Label runat="server" AssociatedControlID="txtFreezeDate" CssClass="col-md-3 control-label float-left">Freeze Date</asp:Label>
                                        <div class="col-md-6">
                                            <input runat="server" type="text" id="txtFreezeDate" class="ui-timepicker form-control" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFreezeDate" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Freeze Date is required" ValidationGroup="Required" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upButtons" runat="server">
                            <ContentTemplate>
                                <asp:Panel ID="pnlButtons" runat="server" CssClass="display-none">
                                    <div class="col-lg-10 col-lg-offset-2 float-right">
                                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default float-right" OnClick="btnBack_Click" />
                                        <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" />
                                        <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                    <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                    <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                    <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
                </div>
            </asp:UpdatePanel>
        </section>
    </div>
    <script>
        $(document).ready(function () {

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
    </script>
</asp:Content>
