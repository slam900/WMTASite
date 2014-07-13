<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CompositionUsed.aspx.cs" Inherits="WMTA.CompositionTools.CompositionUsed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- start of form --%>
                            <fieldset>
                                <legend>Composition Usage</legend>
                                <br />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="ddlComposition" CssClass="col-md-4 control-label float-left">Composition to Find</asp:Label>
                                    <div class="col-md-8">
                                        <asp:DropDownList ID="ddlComposition" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true" >
                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                    </div>
                                    <div class="col-md-8 col-md-4-margin">
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlComposition" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composition is required" /><br />
                                    </div>
                                </div>
                                <div>
                                    <p  id="pUsed" runat="server" class="text-info text-align-center" visible="false">The selected composition <strong>has</strong> been used in a student audition.</p>
                                    <p  id="pNotUsed" runat="server" class="text-info text-align-center" visible="false">The selected composition <strong>has not</strong> been used in a student audition.</p>
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
    </script>
</asp:Content>
