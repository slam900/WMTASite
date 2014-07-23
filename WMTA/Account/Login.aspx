<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WMTA.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin-bottom: 30px"></div>
    <div class="row">
        <div class="well bs-component col-md-5 main-div center" style="margin-bottom: 200px">
            <section id="loginForm">
                <div class="form-horizontal">
                    <fieldset>
                        <legend>Login</legend>
                        <div class="form-group">
                            <label for="txtUsername" class="col-lg-2 control-label">Username</label>
                            <div class="col-lg-10">
                                <asp:TextBox runat="server" ID="txtUsername" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername"
                                    CssClass="text-danger" ErrorMessage="Please enter your username" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtPassword" class="col-lg-2 control-label">Password</label>
                            <div class="col-lg-10">
                                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="text-danger" ErrorMessage="Please enter your password" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-lg-10 col-lg-offset-2">
                                <a href="LoginTrouble.aspx">Trouble logging in?</a>
                            </div>
                        </div>
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <div class="col-lg-10 col-lg-offset-2">
                                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                            </div>
                        </div>
                    </fieldset>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
