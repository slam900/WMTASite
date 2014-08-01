<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="WMTA.Resources.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend>Get Help or Send Feedback</legend>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtName" CssClass="col-md-3 control-label">Name</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" />
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Name is required" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="col-md-3 control-label float-left">Email</asp:Label>
                                    <div class="col-md-6">
                                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Email is required" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="rblTimePreference" CssClass="col-md-3 control-label float-left">Feedback Type</asp:Label>
                                    <div class="col-md-6">
                                        <asp:RadioButtonList ID="rblFeedbackType" runat="server" CssClass="radio" RepeatLayout="Flow">
                                            <asp:ListItem>Comment</asp:ListItem>
                                            <asp:ListItem>Question</asp:ListItem>
                                            <asp:ListItem>Enhancement</asp:ListItem>
                                            <asp:ListItem>Bug</asp:ListItem>
                                            <asp:ListItem>Other</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="rblFeedbackType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Type is required" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="rblImportance" CssClass="col-md-3 control-label float-left">Importance</asp:Label>
                                    <div class="col-md-6">
                                        <asp:RadioButtonList ID="rblImportance" runat="server" CssClass="radio" RepeatLayout="Flow">
                                            <asp:ListItem Selected="True">Low</asp:ListItem>
                                            <asp:ListItem>Medium</asp:ListItem>
                                            <asp:ListItem>High</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="rblImportance" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Importance is required" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtFunctionality" CssClass="col-md-3 control-label">Functionality</asp:Label>
                                    <label class="info smaller-font">Ex: Adding Students</label>
                                    <div class="col-md-6">
                                        <asp:TextBox runat="server" ID="txtFunctionality" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="col-md-3 control-label">Description</asp:Label>
                                    <label class="info smaller-font">Ex: Adding Students</label>
                                    <div class="col-md-6">
                                        <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" Rows="4" />
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Description is required" />
                                    </div>
                                </div>
                                <hr />
                                <div class="form-group">
                                    <div class="col-lg-10 col-lg-offset-2 float-right">
                                        <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" />
                                        <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                        <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
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

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };
    </script>
</asp:Content>
