<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RegisterContact.aspx.cs" Inherits="WMTA.Contacts.RegisterContact" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="form">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of Form --%>
                            <fieldset>
                                <legend id="legend" runat="server">Register Contact</legend>
                                <%-- Contact Search --%>
                                <asp:UpdatePanel ID="upContactSearch" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlContactSearch" runat="server" Visible="false">
                                            <div>
                                                <h4>Contact Search</h4>
                                                <br />
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtContactId" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtContactId" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtFirstNameSearch" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtFirstNameSearch" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" CausesValidation="false" OnClick="btnClearSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtLastNameSearch" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtLastNameSearch" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div>
                                                    <div class="col-md-3-margin popover-font">
                                                        <a href="#" id="searchHint">Search Tip</a>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="form-group">
                                                    <asp:GridView ID="gvSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                        OnPageIndexChanging="gvSearch_PageIndexChanging" OnRowDataBound="gvSearch_RowDataBound" OnSelectedIndexChanged="gvSearch_SelectedIndexChanged" HeaderStyle-BackColor="Black" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Contact Search --%>
                                <%-- Contact Information --%>
                                <asp:UpdatePanel ID="upFullPage" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlFullPage" runat="server">
                                            <div>
                                                <h4>Contact Information</h4>
                                                <asp:Panel runat="server" ID="pnlId">
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="lblId" CssClass="col-md-3 control-label float-left">Contact Id</asp:Label>
                                                        <div class="col-md-6 label-top-margin">
                                                            <asp:Label runat="server" ID="lblId" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblName" CssClass="col-md-3 control-label">Name</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:Label runat="server" ID="lblName" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblDistrict" CssClass="col-md-3 control-label">District</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:Label runat="server" ID="lblDistrict" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblContactType" CssClass="col-md-3 control-label">Contact Type</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:Label runat="server" ID="lblContactType" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtMtnaId" CssClass="col-md-3 control-label float-left">MTNA Id</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtMtnaId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMtnaId" CssClass="text-danger vertical-center font-size-12" ErrorMessage="MTNA Id is required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlYear" CssClass="col-md-3 control-label float-left">Year</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropdown-list form-control"></asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlYear" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Year is required" />
                                                    </div>
                                                </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Panel runat="server" ID="pnlButtons">
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnBack" Text="Back" runat="server" CssClass="btn btn-default float-right" OnClick="btnBack_Click" />
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
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
    <script>
        $(document).ready(function () {
            $('#searchHint').popover(
            {
                trigger: 'hover',
                html: true,
                placement: 'right',
                content: 'Fill in any number of the search fields and click "Search" to find contacts. Clicking "Search" without filling in any fields will return all contacts.  First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all contacts with last names containing "sch"."',
            });
        });



        function showMainError() {
            var message = $('#MainContent_lblErrorMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'error' });
        };
        function showWarningMessage() {
            var message = $('#MainContent_lblWarningMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'warning' });
        };
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'info' });
        }

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };

    </script>
</asp:Content>
