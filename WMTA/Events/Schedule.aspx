<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="WMTA.Events.Schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <section id="scheduleViewForm">
                <div class="form-horizontal">
                    <asp:Panel runat="server" ID="pnlMinusSchedule">
                        <div class="well bs-component col-md-6 main-div center">
                            <asp:UpdatePanel ID="upAuditionSearch" runat="server">
                                <ContentTemplate>
                                    <h4>Create Event Schedule</h4>
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
                            <asp:UpdatePanel ID="pnlValidateSchedule" runat="server" Visible="false">
                                <ContentTemplate>
                                    <h4>
                                        <asp:Label ID="lblAudition" runat="server" /><asp:Label ID="lblAuditionId" runat="server" Visible="false" />
                                    </h4>
                                    <hr />
                                    <div class="center text-center">
                                        <asp:Label runat="server">The table below displays the amount of time, in minutes, not covered<br /> by the event's currently assigned judges.<br /><br />Please go <a href="../Events/AssignDistrictRoomsAndJudges.aspx">here</a> to add additional judges to cover these categories before creating the schedule.</asp:Label>
                                        <asp:GridView ID="gvJudgeValidation" runat="server" AllowSorting="true" AutoGenerateColumns="true" CssClass="table table-bordered label-top-margin" AllowPaging="true" RowStyle-Wrap="true" PageSize="50" OnPageIndexChanging="gvJudgeValidation_PageIndexChanging" OnRowDataBound="gvJudgeValidation_RowDataBound" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="pnlCreateSchedule" runat="server" Visible="false">
                                <ContentTemplate>
                                    <h4>
                                        <asp:Label ID="lblAudition2" runat="server" />
                                    </h4>
                                    <hr />
                                    <div class="center text-center">
                                        <asp:Label runat="server">This event has the required number of judges for each category. <br />Would you like to create the event schedule now?</asp:Label>
                                        <div>
                                            <asp:Button ID="btnCreateSchedule" Text="Create Schedule" runat="server" CssClass="btn btn-primary label-top-margin" OnClick="btnCreateSchedule_Click" />
                                        </div>
                                    </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </asp:Panel>
                    <asp:UpdatePanel ID="pnlViewSchedule" runat="server" Visible="false">
                        <ContentTemplate>
                            <div class="well bs-component col-md-12 main-div center">
                                <h4>
                                    <asp:Label ID="lblAudition3" runat="server" />
                                </h4>
                                <hr />
                                <div class="form-group">
                                    <asp:Button ID="btnSave" Text="Save Schedule" runat="server" CssClass="btn btn-primary float-right margin-right-15px" OnClick="btnSave_Click" />
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
