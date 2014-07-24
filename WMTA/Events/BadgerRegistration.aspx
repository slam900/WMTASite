<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="BadgerRegistration.aspx.cs" Inherits="WMTA.Events.BadgerRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <section id="registrationForm">
            <asp:UpdatePanel ID="upFullPage" runat="server">
                <div class="form-horizontal">
                    <%-- Start of form --%>
                    <fieldset>
                        <legend>Badger State Competition Registration</legend>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div>
                                    <h4>Student Search</h4>
                                    <br />
                                    <div class="form-group">
                                        <asp:Label AssociatedControlID="txtStudentId" runat="server" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtStudentId" runat="server" CssClass="input"></asp:TextBox>
                                        </div>
                                        <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="btn btn-default btn-min-width-72" OnClick="btnStudentSearch_Click" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label AssociatedControlID="txtFirstName" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="input"></asp:TextBox>
                                        </div>
                                        <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="btn btn-clear btn-min-width-72" OnClick="btnClearStudentSearch_Click" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label AssociatedControlID="txtLastName" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="input"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:Panel ID="pnlInfo" runat="server" Visible="false">
                            
                        </asp:Panel>
                    </fieldset>
                </div>
            </asp:UpdatePanel>
        </section>
    </div>
</asp:Content>