<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageAuditionLengths.aspx.cs" Inherits="WMTA.Admin.ManageAuditionLengths" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="repertoireForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Manage Audition Length Limits</legend>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlCompLevel" CssClass="col-md-3 control-label float-left">Level</asp:Label>
                                    <div>
                                        <asp:DropDownList ID="ddlCompLevel" runat="server" CssClass="dropdown-list form-control float-left" DataSourceID="WmtaDataSource7" DataTextField="Description" DataValueField="CompLevelId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlCompLevel_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Text="" Value="" />
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCompLevel" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Level is required" /><br />
                                    </div>
                                    <asp:SqlDataSource ID="WmtaDataSource7" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtMinimum" CssClass="col-md-3 control-label float-left">Minimum</asp:Label>
                                    <asp:TextBox ID="txtMinimum" runat="server" CssClass="form-control float-left" TextMode="Number"></asp:TextBox>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCompLevel" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Minimum is required" /><br />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtMaximum" CssClass="col-md-3 control-label float-left">Maximum</asp:Label>
                                    <asp:TextBox ID="txtMaximum" runat="server" CssClass="form-control float-left" TextMode="Number"></asp:TextBox>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCompLevel" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Maximum is required" /><br />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-lg-10 col-lg-offset-2 float-right">
                                        <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                    </div>
                                </div>
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
