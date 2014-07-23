<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ReplaceComposition.aspx.cs" Inherits="WMTA.CompositionTools.ReplaceComposition" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-7 main-div center">
            <section id="form">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <asp:Panel runat="server" DefaultButton="btnSubmit">
                            <div class="form-horizontal">
                                <%-- start of form --%>
                                <fieldset>
                                    <legend>Composition Replacement</legend>
                                    <br />
                                    <h4>Select Composition to Replace</h4>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtIdReplace" CssClass="col-md-4 control-label">Id</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="txtIdReplace" CssClass="form-control small-txtbx-width float-left" />
                                            <asp:Button ID="btnIdReplace" Text="Search Id" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnIdReplace_Click" CausesValidation="false" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlStyleSearch" CssClass="col-md-4 control-label float-left">Style</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlStyleSearch" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource2" DataTextField="Style" DataValueField="Style" AppendDataBoundItems="true" OnSelectedIndexChanged="cboStyle_SelectedIndexChanged" AutoPostBack="True">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlComposerSearch" CssClass="col-md-4 control-label float-left">Composer</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlComposerSearch" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource4" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlComposerSearch_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlCompositionToReplace" CssClass="col-md-4 control-label float-left">Composition to Replace</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlCompositionToReplace" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                        </div>
                                        <div class="col-md-8 col-md-4-margin">
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCompositionToReplace" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composition to replace is required" /><br />
                                        </div>
                                    </div>
                                    <div>
                                        <p runat="server" class="text-info text-align-center">Note: The composition to replace will be <strong>permanently</strong> removed from the system.</p>
                                    </div>
                                    <hr />
                                    <h4>Select Replacement Composition</h4>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtIdReplacement" CssClass="col-md-4 control-label">Id</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="txtIdReplacement" CssClass="form-control small-txtbx-width float-left" />
                                            <asp:Button ID="btnIdReplacement" Text="Search Id" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnIdReplacement_Click" CausesValidation="false" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlStyleSearch2" CssClass="col-md-4 control-label float-left">Style</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlStyleSearch2" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource5" DataTextField="Style" DataValueField="Style" AppendDataBoundItems="true" OnSelectedIndexChanged="cboStyle2_SelectedIndexChanged" AutoPostBack="True">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlComposerSearch2" CssClass="col-md-4 control-label float-left">Composer</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlComposerSearch2" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource7" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlComposerSearch2_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource7" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlReplacement" CssClass="col-md-4 control-label float-left">Replacement Composition</asp:Label>
                                        <div class="col-md-8">
                                            <asp:DropDownList ID="ddlReplacement" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                        </div>
                                        <div class="col-md-8 col-md-4-margin">
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlReplacement" CssClass="text-danger vertical-center font-size-12" ErrorMessage="A replacement composition is required" /><br />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right display-inline">
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                            <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                            <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                            <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                        </asp:Panel>
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
