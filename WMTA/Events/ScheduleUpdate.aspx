<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ScheduleUpdate.aspx.cs" Inherits="WMTA.Events.ScheduleUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <section id="scheduleViewForm">
                <div class="form-horizontal">
                    <div class="well bs-component col-md-6 main-div center">
                        <asp:UpdatePanel ID="upAuditionSearch" runat="server">
                            <ContentTemplate>
                                <h4>Edit Event Schedule</h4>
                                <hr />
                                <div>
                                    <h4>Audition Search</h4>
                                    <br />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlDistrictSearch" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <asp:Button ID="btnAuditionSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnAuditionSearch_Click" CausesValidation="false" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="col-md-3 control-label float-left">Audition Year</asp:Label>
                                        <div class="col-md-6">
                                            <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control" />
                                        </div>
                                        <asp:Button ID="btnClearAuditionSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearAuditionSearch_Click" CausesValidation="false" />
                                    </div>
                                    <div class="form-group">
                                        <asp:GridView ID="gvAuditionSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvAuditionSearch_PageIndexChanging" OnRowDataBound="gvAuditionSearch_RowDataBound" OnSelectedIndexChanged="gvAuditionSearch_SelectedIndexChanged">
                                            <Columns>
                                                <asp:BoundField DataField="AuditionOrgId" HeaderText="AuditionOrgId" ItemStyle-Width="0%" />
                                                <asp:BoundField DataField="GeoName" HeaderText="District" />
                                                <asp:BoundField DataField="Year" HeaderText="Year" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel runat="server" ID="upSelectAudition" Visible="false">
                            <ContentTemplate>
                                <h4>
                                    <asp:Label ID="lblAudition" runat="server" /><asp:Label runat="server" ID="lblAuditionId" Visible="false" />
                                </h4>
                                <hr />
                                <h4>Select Audition To Move</h4>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtSlot" CssClass="col-md-3 control-label">Slot</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox runat="server" ID="txtSlot" CssClass="form-control medium-txtbx-width float-left" />
                                        <%--                                        <asp:Button ID="btnSelectAudition" Text="Search Id" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSelectAudition_Click" CausesValidation="false" />--%>
                                    </div>
                                    <div style="margin-top: 50px" class="center text-align-center">
                                        <asp:Label runat="server" ID="lblAuditionInformation" CssClass="text-info" /><asp:Label runat="server" ID="lblSelectedAuditionId" Visible="false" />
                                    </div>
                                </div>
                                <h4>Select Slot To Move Audition To</h4>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtNewSlot" CssClass="col-md-3 control-label">New Slot</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox runat="server" ID="txtNewSlot" CssClass="form-control medium-txtbx-width float-left" />
                                        <asp:Button ID="btnMoveAudition" Text="Move" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnMoveAudition_Click" CausesValidation="false" />
                                    </div>
                                </div>
                                <%--<div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlTimes" CssClass="col-md-3 control-label">Time</asp:Label>
                                    <div class="col-md-6">
                                        <asp:DropDownList ID="ddlTimes" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true">
                                            <asp:ListItem Selected="True" Text="" Value="" />
                                        </asp:DropDownList>
                                    </div>
                                </div>--%>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel ID="upViewSchedule" runat="server" Visible="false">
                        <ContentTemplate>
                            <div class="well bs-component col-md-12 main-div center">
                                <hr />
                                <h4>Event Schedule</h4>
                                <div class="form-group">
                                    <asp:Button ID="btnAssignTimes" Text="Assign New Times" Visible="false" runat="server" CssClass="btn btn-primary float-right margin-right-15px" OnClick="btnAssignTimes_Click" />
                                    <asp:Button ID="btnSaveOrder" Text="Save Order" Visible="false" runat="server" CssClass="btn btn-primary float-right margin-right-15px" OnClick="btnSaveOrder_Click" />
                                    <asp:Button ID="btnSave" Text="Save Schedule" Visible="false" runat="server" CssClass="btn btn-primary float-right margin-right-15px" OnClick="btnSave_Click" />
                                </div>
                                <div class="form-group">
                                    <div class="col-md-12 center">
                                        <asp:GridView ID="gvSchedule" runat="server" AllowSorting="true" AutoGenerateColumns="true" CssClass="table table-bordered" AllowPaging="true" RowStyle-Wrap="true" PageSize="50" OnPageIndexChanging="gvSchedule_PageIndexChanging" OnRowDataBound="gvSchedule_RowDataBound" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </section>
            <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
            <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
            <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
            <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
        </ContentTemplate>
    </asp:UpdatePanel>
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

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };

        //show an informational message
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };
    </script>
</asp:Content>
