<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="AssignDistrictRoomsAndJudges.aspx.cs" Inherits="WMTA.Events.AssignDistrictRoomsAndJudges" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Assign Rooms & Judges to District Event</legend>
                                <asp:UpdatePanel ID="upAuditionSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Audition Search</h4>
                                            <br />
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlDistrictSearch" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSourceDistrict" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="SqlDataSourceDistrict" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
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
                                        <hr />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="upMain" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlMain" runat="server" Visible="false">
                                            <asp:TextBox ID="txtIdHidden" runat="server" CssClass="display-none"></asp:TextBox>
                                            <%-- Event Info --%>
                                            <h4>Event Information</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="lblAuditionSite" CssClass="col-md-3 control-label float-left">Audition Site</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label runat="server" ID="lblAuditionSite" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="lblAuditionDate" CssClass="col-md-3 control-label float-left">Audition Date</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label runat="server" ID="lblAuditionDate" />
                                                </div>
                                            </div>
                                            <hr />
                                            <%-- Rooms --%>
                                            <asp:UpdatePanel runat="server">
                                                <ContentTemplate>
                                                    <h4>Rooms</h4>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtRoom" CssClass="col-md-3 control-label float-left">Room</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtRoom" CssClass="form-control" />
                                                        </div>
                                                        <asp:Button ID="btnAddRoom" runat="server" Text="Add" CssClass="btn btn-primary btn-sm" OnClick="btnAddRoom_Click" />
                                                    </div>
                                                    <asp:Panel ID="pnlRooms" runat="server" Visible="false">
                                                        <div>
                                                            <asp:Button ID="btnRemoveRoom" runat="server" Text="Remove" CssClass="btn btn-link btn-sm col-md-9-margin" OnClick="btnRemoveRoom_Click" />
                                                            <div class="form-group col-md-9 center">
                                                                <asp:Table ID="tblRooms" runat="server" CssClass="table table-striped table-bordered table-hover center text-align-center">
                                                                    <asp:TableHeaderRow BorderStyle="Solid">
                                                                        <asp:TableHeaderCell Scope="Column" Text="" Width="20px" />
                                                                        <asp:TableHeaderCell Scope="Column" Text="Room" />
                                                                    </asp:TableHeaderRow>
                                                                </asp:Table>
                                                            </div>
                                                        </div>
                                                    </asp:Panel>
                                                    <hr />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                            <%-- Theory Test Rooms --%>
                                            <h4>Theory Test Rooms</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlTheoryTest" CssClass="col-md-3 control-label">Test</asp:Label>
                                                <div class="col-md-2">
                                                    <asp:DropDownList ID="ddlTheoryTest" runat="server" CssClass="dropdown-list form-control float-left" Width="70px" AppendDataBoundItems="true" DataSourceID="SqlDataSourceTheory" DataTextField="Test" DataValueField="Test">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="SqlDataSourceTheory" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTheoryTest" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlRoom" CssClass="col-md-1 control-label float-left">Room</asp:Label>
                                                <div class="col-md-3">
                                                    <asp:DropDownList ID="ddlRoom" runat="server" CssClass="dropdown-list form-control float-left" AppendDataBoundItems="true">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnAddTestRoom" runat="server" Text="Add" CssClass="btn btn-primary btn-sm" OnClick="btnAddTestRoom_Click" />
                                            </div>
                                            <asp:Panel ID="pnlTheoryRooms" runat="server" Visible="false">
                                                <asp:Button ID="btnRemoveTestRoom" runat="server" Text="Remove" CssClass="btn btn-default btn-sm" OnClick="btnRemoveTestRoom_Click" />
                                                <div class="form-group col-md-9 center">
                                                    <asp:Table ID="tblTheoryRooms" runat="server" CssClass="table table-striped table-bordered table-hover center text-align-center">
                                                        <asp:TableHeaderRow BorderStyle="Solid">
                                                            <asp:TableHeaderCell Scope="Column" Text="" Width="20px" />
                                                            <asp:TableHeaderCell Scope="Column" Text="Test" />
                                                            <asp:TableHeaderCell Scope="Column" Text="Room" />
                                                        </asp:TableHeaderRow>
                                                    </asp:Table>
                                                </div>
                                            </asp:Panel>
                                            <hr />
                                            <%-- Judges --%>
                                            <h4>Judges</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlJudge" CssClass="col-md-3 control-label float-left">Judge</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlJudge" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlJudge_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnAddJudge" runat="server" Text="Add" CssClass="btn btn-primary btn-sm" OnClick="btnAddJudge_Click" />
                                            </div>
                                            <div class="form-group" hidden="hidden">
                                                <asp:GridView ID="gvJudges" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvJudges_PageIndexChanging" OnRowDataBound="gvJudges_RowDataBound" OnSelectedIndexChanged="gvJudges_SelectedIndexChanged">
                                                    <Columns>
                                                        <asp:BoundField DataField="Judge" HeaderText="Judge" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Button ID="btnRemoveJudge" runat="server" Text="Remove" CssClass="btn btn-default btn-sm" OnClick="btnRemoveJudge_Click" />
                                            </div>
                                            <hr />
                                            <%-- Judge Rooms --%>
                                            <h4>Judge Rooms</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlAuditionJudges" CssClass="col-md-3 control-label">Judge</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlAuditionJudges" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlAuditionJudges_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnAddJudgeRoom" runat="server" Text="Add" CssClass="btn btn-primary btn-sm" OnClick="btnAddJudgeRoom_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlJudgeRoom" CssClass="col-md-3 control-label float-left">Room</asp:Label>
                                                <div class="col-md-3" style="width: 100px">
                                                    <asp:DropDownList ID="ddlJudgeRoom" runat="server" CssClass="dropdown-list form-control float-left" Width="90px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlJudgeRoom_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlJudgeTime" CssClass="col-md-1 control-label float-left text-align-right" Width="12%">Time</asp:Label>
                                                <div class="col-md-3">
                                                    <asp:DropDownList ID="ddlJudgeTime" runat="server" CssClass="dropdown-list form-control float-left" AppendDataBoundItems="true" Width="90px">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="form-group" hidden="hidden">
                                                <asp:GridView ID="gvJudgeRooms" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvJudgeRooms_PageIndexChanging" OnRowDataBound="gvJudgeRooms_RowDataBound" OnSelectedIndexChanged="gvJudgeRooms_SelectedIndexChanged">
                                                    <Columns>
                                                        <asp:DynamicField HeaderText="Test" />
                                                        <asp:DynamicField HeaderText="Room" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Button ID="btnRemoveJudgeRoom" runat="server" Text="Remove" CssClass="btn btn-default btn-sm" OnClick="btnRemoveJudgeRoom_Click" />
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                            <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                            <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                            <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                            <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
                        </div>
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
