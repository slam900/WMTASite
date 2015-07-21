<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageAuditionTrackFees.aspx.cs" Inherits="WMTA.Admin.ManageAuditionTrackFees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="repertoireForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Manage Audition Track Fees</legend>
                                <div class="form-group">
                                    <h4 style="margin-left: 20px">Fees</h4>
                                    <asp:GridView ID="gvFees" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PageSize="15" PagerStyle-CssClass="bs-pagination" DataSourceID="SqlDataSource1"
                                        OnPageIndexChanging="gvFees_PageIndexChanging" OnRowDataBound="gvFees_RowDataBound" OnSelectedIndexChanged="gvFees_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_FeesSelect" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="lblTrack" CssClass="col-md-3 control-label float-left">Track</asp:Label>
                                    <div class="col-md-6 label-top-margin">
                                        <asp:Label ID="lblTrack" runat="server"></asp:Label>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFee" CssClass="col-md-3 control-label float-left">Fee ($)</asp:Label>
                                    <asp:TextBox ID="txtFee" runat="server" CssClass="form-control float-left" TextMode="Number"></asp:TextBox>
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
