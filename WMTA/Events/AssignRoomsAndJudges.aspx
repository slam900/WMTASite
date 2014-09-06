<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="AssignRoomsAndJudges.aspx.cs" Inherits="WMTA.Events.AssignRoomsAndJudges" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Assign Rooms & Judges to Event</legend>
                                <asp:UpdatePanel ID="upAuditionSearch" runat="server">
                                    <ContentTemplate>
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
                                        <hr />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="upMain" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlMain" runat="server">
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
                                            <h4>Rooms</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtRoom" CssClass="col-md-3 control-label float-left">Room</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" ID="txtRoom" CssClass="form-control" />
                                                </div>
                                                <asp:Button ID="btnAddRoom" runat="server" Text="Add" CssClass="btn btn-default" OnClick="btnAddRoom_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:GridView ID="gvRooms" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvRooms_PageIndexChanging" OnRowDataBound="gvRooms_RowDataBound" OnSelectedIndexChanged="gvRooms_SelectedIndexChanged">
                                                    <Columns>
                                                        <asp:BoundField DataField="Room" HeaderText="Room" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Button ID="btnRemoveRoom" runat="server" Text="Remove" CssClass="btn btn-default" OnClick="btnRemoveRoom_Click" />
                                            </div>
                                            <hr />
                                            <%-- Theory Test Rooms --%>
                                            <h4>Theory Test Rooms</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlTheoryTest" CssClass="col-md-3 control-label smaller-font">Test</asp:Label>
                                                <div class="col-md-2">
                                                    <asp:DropDownList ID="ddlTheoryTest" runat="server" CssClass="dropdown-list form-control float-left" Width="70px">
                                                        <asp:ListItem Selected="True" Text="0" Value="0" />
                                                        <asp:ListItem Text="15" Value="0.25" />
                                                        <asp:ListItem Text="30" Value="0.5" />
                                                        <asp:ListItem Text="45" Value="0.75" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlRoom" CssClass="col-md-2 control-label float-left smaller-font">Room</asp:Label>
                                                <div class="col-md-2">
                                                    <asp:DropDownList ID="ddlRoom" runat="server" CssClass="dropdown-list form-control float-left" Width="70px">
                                                        <asp:ListItem Selected="True" Text="0" Value="0" />
                                                        <asp:ListItem Text="15" Value="0.25" />
                                                        <asp:ListItem Text="30" Value="0.5" />
                                                        <asp:ListItem Text="45" Value="0.75" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnAddTestRoom" runat="server" Text="Add" CssClass="btn btn-default" OnClick="btnAddTestRoom_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:GridView ID="gvTestRooms" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvTestRooms_PageIndexChanging" OnRowDataBound="gvTestRooms_RowDataBound" OnSelectedIndexChanged="gvTestRooms_SelectedIndexChanged">
                                                    <Columns>
                                                        <asp:DynamicField HeaderText="Test" />
                                                        <asp:DynamicField HeaderText="Room" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Button ID="btnRemoveTestRoom" runat="server" Text="Remove" CssClass="btn btn-default" OnClick="btnRemoveTestRoom_Click" />
                                            </div>
                                            <%-- Judges --%>
                                            <h4>Judges</h4>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlJudge" CssClass="col-md-3 control-label float-left">Judge</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlJudge" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlJudge_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="WmtaDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT GeoName, GeoId FROM ConfigDistrict WHERE (AuditionLevel = @AuditionLevel) ORDER BY GeoName" />
                                                </div>
                                                <asp:Button ID="btnAddJudge" runat="server" Text="Add" CssClass="btn btn-default" OnClick="btnAddJudge_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:GridView ID="gvJudges" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" OnPageIndexChanging="gvJudges_PageIndexChanging" OnRowDataBound="gvJudges_RowDataBound" OnSelectedIndexChanged="gvJudges_SelectedIndexChanged">
                                                    <Columns>
                                                        <asp:BoundField DataField="Judge" HeaderText="Judge" />
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:Button ID="btnRemoveJudge" runat="server" Text="Remove" CssClass="btn btn-default" OnClick="btnRemoveJudge_Click" />
                                            </div>
                                            <%-- Judge Rooms --%>

                                            <%-- need to select judge, room, and time range --%>

                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </fieldset>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
</asp:Content>
