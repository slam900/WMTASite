﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="AwardsView.aspx.cs" Inherits="WMTA.Events.AwardsView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend id="legend" runat="server">View Awards</legend>
                                <%-- Audition search --%>
                                <asp:UpdatePanel ID="upSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Select an Audition</h4>
                                            <br />
                                            <div class="form-group">
                                                <div class="col-md-3-margin">
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDistrictSearch" CssClass="txt-danger vertical-center font-size-12" ErrorMessage="District is required"></asp:RequiredFieldValidator>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlDistrictSearch" CssClass="col-md-3 control-label float-left">District *</asp:Label>
                                                <div class="col-md-6">
                                                    <%--                                                    <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropdown-list form-control"  AppendDataBoundItems="true" OnSelectedIndexChanged="ddlDistrictSearch_SelectedIndexChanged" AutoPostBack="true">--%>
                                                    <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearch_Click" />
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-3-margin">
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlYear" CssClass="txt-danger vertical-center font-size-12" ErrorMessage="Year is required"></asp:RequiredFieldValidator>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="col-md-3 control-label">Year *</asp:Label>
                                                <div class="col-md-6">
                                                    <%--                                                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true" />--%>
                                                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control" />
                                                </div>
                                            </div>
                                            <%--<div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlTeacher" CssClass="col-md-3 control-label">Teacher</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlTeacher" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" />
                                                </div>
                                            </div>
                                            <div class="center text-align-center">
                                                <label class="text-info smaller-font">Please be patient after clicking 'Search'.  Your reports may take several minutes.</label>
                                            </div>--%>
                                            <div class="form-group">
                                                <div class="col-md-3-margin">
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlAuditionType" CssClass="txt-danger vertical-center font-size-12" ErrorMessage="Audition type is required"></asp:RequiredFieldValidator>
                                                </div>
                                                <asp:Label runat="server" AssociatedControlID="ddlAuditionType" CssClass="col-md-3 control-label">Audition Type *</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropdown-list form-control">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                        <asp:ListItem Text="District" Value="District" />
                                                        <asp:ListItem Text="State" Value="State" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Audition Search --%>
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
    <div class="row">
        <div class="well bs-component col-md-12 main-div center">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <div class="col-md-12 center">
                            <asp:GridView ID="gvAuditions" runat="server" AllowSorting="true" AutoGenerateColumns="false" CssClass="table table-bordered" AllowPaging="false" RowStyle-Wrap="true">
                                <HeaderStyle BackColor="#EFEFEF" />
                                <SelectedRowStyle BackColor="#CDCDEF" />
                                <AlternatingRowStyle BackColor="#EFEFEF" />
                                <Columns>
                                    <asp:BoundField DataField="StudentId" HeaderText="Student Id" InsertVisible="false" ReadOnly="true" SortExpression="StudentId" />
                                    <asp:BoundField DataField="LastName" HeaderText="Last Name" InsertVisible="false" ReadOnly="true" SortExpression="LastName" />
                                    <asp:BoundField DataField="FirstName" HeaderText="First Name" InsertVisible="false" ReadOnly="true" SortExpression="FirstName" />
                                    <asp:BoundField DataField="AwardType" HeaderText="Award" InsertVisible="false" ReadOnly="true" SortExpression="AwardType" />
                                    <asp:BoundField DataField="TotalPoints" HeaderText="Total Points" InsertVisible="false" ReadOnly="true" SortExpression="TotalPoints" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
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
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };
    </script>
</asp:Content>