<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/WidePage.Master" AutoEventWireup="true" CodeBehind="ViewHelpRequests.aspx.cs" Inherits="WMTA.Resources.ViewHelpRequests" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div class="well bs-component col-md-12 main-div center">
                <section id="registrationForm">
                    <asp:UpdatePanel ID="upFullPage" runat="server">
                        <ContentTemplate>
                            <div class="form-horizontal">
                                <h4>View Help Requests</h4>
                                <hr />
                                <div class="form-group">
                                    <div class="col-lg-10">
                                        <asp:RadioButtonList ID="rblFilter" runat="server" CssClass="radio" RepeatLayout="Flow" OnSelectedIndexChanged="rblFilter_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Selected="True">Open Items</asp:ListItem>
                                            <asp:ListItem>All Items</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-12 center">
                                        <asp:GridView ID="gvRequests" runat="server" AllowSorting="true" AutoGenerateSelectButton="true" AutoGenerateColumns="false" DataKeyNames="Id" DataSourceID="SqlDataSource1" CssClass="table table-bordered" AllowPaging="true" RowStyle-Wrap="true" OnSelectedIndexChanged="gvRequests_SelectedIndexChanged" PageSize="100">
                                            <HeaderStyle BackColor="#EFEFEF" />
                                            <SelectedRowStyle BackColor="#CDCDEF" />
                                            <AlternatingRowStyle BackColor="#EFEFEF" />
                                            <Columns>
                                                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="false" ReadOnly="true" SortExpression="Id" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" InsertVisible="false" ReadOnly="true" SortExpression="Name" />
                                                <asp:BoundField DataField="Email" HeaderText="Email" InsertVisible="false" ReadOnly="true" SortExpression="Email" />
                                                <asp:TemplateField HeaderText="Type" SortExpression="FeedbackType">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFeedbackType" runat="server" Text='<%# Bind("FeedbackType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlFeedbackType" CssClass="dropdown-list form-control" SelectedValue='<%#Bind("FeedbackType") %>'>
                                                            <asp:ListItem>Comment</asp:ListItem>
                                                            <asp:ListItem>Composition Question</asp:ListItem>
                                                            <asp:ListItem>Site Usage Question</asp:ListItem>
                                                            <asp:ListItem>Enhancement</asp:ListItem>
                                                            <asp:ListItem>Bug</asp:ListItem>
                                                            <asp:ListItem>Other</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFeedbackType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Type is required" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Importance" SortExpression="Importance">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblImportance" runat="server" Text='<%#Bind("Importance") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlImportance" CssClass="dropdown-list form-control" SelectedValue='<%#Bind("Importance") %>'>
                                                            <asp:ListItem>Low</asp:ListItem>
                                                            <asp:ListItem>Medium</asp:ListItem>
                                                            <asp:ListItem>High</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlImportance" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Importance is required" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Functionality" HeaderText="Functionality" InsertVisible="false" ReadOnly="true" SortExpression="Functionality" />
                                                <asp:BoundField DataField="Description" HeaderText="Description" InsertVisible="false" ReadOnly="true" SortExpression="Description" ItemStyle-Wrap="true" ItemStyle-Width="300px" />
                                                <asp:TemplateField HeaderText="Assigned To" SortExpression="AssignedTo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAssignedTo" runat="server" Text='<%#Bind("AssignedTo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtAssignedTo" runat="server" Text='<%#Bind("AssignedTo") %>' CssClass="form-control" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Complete" SortExpression="Completed">
                                                    <ItemTemplate>
                                                        <div class="center text-align-center">
                                                            <asp:CheckBox ID="chkComplete" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.Completed").ToString().Equals("True") %>' Enabled="false" />
                                                        </div>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <div class="center text-align-center">
                                                            <asp:CheckBox ID="chkComplete" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.Completed").ToString().Equals("True") %>' />
                                                        </div>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="DateEntered" HeaderText="Date Entered" InsertVisible="false" ReadOnly="true" SortExpression="DateEntered" />
                                                <asp:BoundField DataField="DateComplete" HeaderText="Date Complete" InsertVisible="false" ReadOnly="true" SortExpression="DateComplete" />
                                            </Columns>
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:WmtaConnectionString %>' SelectCommand="SELECT * FROM [Feedback] WHERE [Completed] = 0 ORDER BY [Completed], [Id]" UpdateCommand="sp_FeedbackUpdate" UpdateCommandType="StoredProcedure" />
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </section>
            </div>
            <asp:UpdatePanel ID="upEdit" runat="server" Visible="false">
                <ContentTemplate>
                    <div class="well bs-component col-md-6 main-div center">
                        <section id="editForm">
                            <div class="form-horizontal">
                                <fieldset>
                                    <legend>Edit Request</legend>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="rblFeedbackType" CssClass="col-md-3 control-label float-left">Feedback Type</asp:Label>
                                        <div class="col-md-6">
                                            <asp:RadioButtonList ID="rblFeedbackType" runat="server" CssClass="radio" RepeatLayout="Flow">
                                                <asp:ListItem>Comment</asp:ListItem>
                                                <asp:ListItem>Composition Question</asp:ListItem>
                                                <asp:ListItem>Site Usage Question</asp:ListItem>
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
                                                <asp:ListItem>Low</asp:ListItem>
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
                                        <div class="col-md-6">
                                            <asp:TextBox runat="server" ID="txtFunctionality" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="col-md-3 control-label">Description</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" Rows="4" TextMode="MultiLine" />
                                        </div>
                                        <div>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Description is required" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="txtAssignedTo" CssClass="col-md-3 control-label">Assigned To</asp:Label>
                                        <div class="col-md-6">
                                            <asp:TextBox runat="server" ID="txtAssignedTo" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="chkComplete" CssClass="col-md-3 control-label">Complete</asp:Label>
                                        <div class="col-md-6">
                                            <input id="chkComplete" type="checkbox" runat="server" class="float-left" />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </fieldset>
                            </div>
                        </section>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
            <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
            <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
        </ContentTemplate>
    </asp:UpdatePanel>
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
