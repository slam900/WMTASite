<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewRegisteredContacts.aspx.cs" Inherits="WMTA.Contacts.ViewRegisteredContacts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-12 main-div center" style="overflow: scroll;">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <div class="col-md-12 center">
                            <h4 style="margin-left: 20px">Registered Contacts</h4>
                        <asp:GridView ID="gvContacts" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" PageSize="50" PagerStyle-CssClass="bs-pagination" DataSourceID="SqlDataSource1"
                            OnPageIndexChanging="gvContacts_PageIndexChanging" OnRowDataBound="gvContacts_RowDataBound" HeaderStyle-BackColor="Black" />
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_RegisteredContactSelect" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
