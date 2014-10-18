<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="TitleLookup.aspx.cs" Inherits="WMTA.CompositionTools.TitleLookup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-11 main-div center">
            <section id="form">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <asp:Panel runat="server" DefaultButton="btnSubmit">
                            <div class="form-horizontal">
                                <%-- start of form --%>
                                <fieldset>
                                    <legend>Composition Title Finder</legend>
                                    <br />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtComposition" CssClass="col-md-4 control-label float-left">Title Search String *</asp:Label>
                                        <div class="col-md-5">
                                            <asp:TextBox runat="server" ID="txtComposition" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="ddlComposer" CssClass="col-md-4 control-label float-left">Composer</asp:Label>
                                        <div class="col-md-5">
                                            <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                    </div>
                                    <div class="form-group">
                                        <asp:Table ID="tblCompositions" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center" Visible="false">
                                            <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
                                                <asp:TableHeaderCell Scope="Column" Text="Id" />
                                                <asp:TableHeaderCell Scope="Column" Text="Title" />
                                                <asp:TableHeaderCell Scope="Column" Text="Composer" />
                                                <asp:TableHeaderCell Scope="Column" Text="Style" />
                                                <asp:TableHeaderCell Scope="Column" Text="Level" />
                                                <asp:TableHeaderCell Scope="Column" Text="Time" />
                                                <asp:TableHeaderCell Scope="Column" Text="Times Used" />
                                            </asp:TableHeaderRow>
                                        </asp:Table>
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
                            <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
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

        //show an informational message
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };
    </script>
</asp:Content>
