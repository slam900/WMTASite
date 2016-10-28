<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="MarkCompositionsReviewed.aspx.cs" Inherits="WMTA.CompositionTools.MarkCompositionsReviewed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-10 main-div center">
            <section id="repertoireForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Mark Compositions Reviewed</legend>
                                <div class="form-group">
                                    <asp:Label runat="server" Style="font-weight: bold; margin-left: 15px" CssClass="control-label float-left">Enter the range of composition ids to mark as reviewed:</asp:Label>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFromId" CssClass="col-md-1 control-label float-left">From</asp:Label>
                                    <asp:TextBox ID="txtFromId" runat="server" CssClass="form-control float-left" TextMode="Number"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtToId" CssClass="col-md-1 control-label float-left">To</asp:Label>
                                    <asp:TextBox ID="txtToId" runat="server" CssClass="form-control float-left" TextMode="Number"></asp:TextBox>
                                    <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                </div>
                                <div class="form-group">
                                    <h4 style="margin-left:20px">Compositions To Review</h4>
                                    <asp:GridView ID="gvCompositions" runat="server" CssClass="td table table-hover table-striped smaller-font width-95 center" AllowPaging="true" PageSize="50" PagerStyle-CssClass="bs-pagination" DataSourceID="SqlDataSource1"
                                        OnPageIndexChanging="gvCompositions_PageIndexChanging" OnRowDataBound="gvCompositions_RowDataBound" HeaderStyle-BackColor="Black" />
                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_CompositionsSelectNotReviewed" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
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
