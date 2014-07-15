<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReplaceComposerName.aspx.cs" Inherits="WMTA.CompositionTools.ReplaceComposerName" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- start of form --%>
                            <fieldset>
                                <legend>Replace Composer Name</legend>
                                <br />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlComposer" CssClass="col-md-4 control-label float-left">Composer to replace</asp:Label>
                                    <div class="col-md-5">
                                        <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true">
                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlComposer" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composer to Replace is required" /><br />
                                    </div>
                                    <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                </div>
                                <hr />
                                <div>
                                    <asp:Label runat="server" CssClass="section-heading">Replace With:</asp:Label>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtComposerLast" CssClass="col-md-4 control-label float-left">Last Name</asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox runat="server" ID="txtComposerLast" CssClass="form-control" />
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtComposerLast" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Last name is required" /><br />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtComposerFI" CssClass="col-md-4 control-label float-left" >F.I.</asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox runat="server" ID="txtComposerFI" CssClass="form-control small-txtbx-width" MaxLength="1" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtComposerMI" CssClass="col-md-4 control-label float-left">M.I.</asp:Label>
                                    <div class="col-md-5">
                                        <asp:TextBox runat="server" ID="txtComposerMI" CssClass="form-control small-txtbx-width" MaxLength="1" />
                                    </div>
                                </div>
                                <hr />
                                <div class="form-group">
                                    <div class="col-lg-10 col-lg-offset-2 float-right display-inline">
                                        <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" />
                                        <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                        <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
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

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };
    </script>
</asp:Content>
