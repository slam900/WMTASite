<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewHelpRequests.aspx.cs" Inherits="WMTA.Resources.ViewHelpRequests" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <h4>View Help Requests</h4>
                            <div class="form-group">
                                <div class="col-lg-10">
                                    <asp:RadioButtonList ID="rblFilter" runat="server" CssClass="radio" RepeatLayout="Flow" OnSelectedIndexChanged="rblFilter_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Selected="True">Open Items</asp:ListItem>
                                        <asp:ListItem>All Items</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-11">
                                    <asp:GridView ID="gvRequests" runat="server" AllowSorting="true" AutoGenerateEditButton="true" AutoGenerateColumns="false" DataKeyNames="Id" DataSourceID="SqlDataSource1" CssClass="table table-bordered" AllowPaging="true" RowStyle-Wrap="false" EditRowStyle-Wrap="true">
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
                                            <asp:BoundField DataField="Description" HeaderText="Description" InsertVisible="false" ReadOnly="true" SortExpression="Description" />
                                            <asp:TemplateField HeaderText="Assigned To" SortExpression="AssignedTo">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAssignedTo" runat="server" Text='<%#Bind("AssignedTo") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtAssignedTo" runat="server" Text='<%#Bind("AssignedTo") %>' CssClass="form-control" />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Complete?" SortExpression="Completed">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblComplete" runat="server" Text='<%#Bind("Completed") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:Checkbox ID="txtAssignedTo" runat="server" Text='<%#Bind("Completed") %>' CssClass="form-control" />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="DateEntered" HeaderText="Date Entered" InsertVisible="false" ReadOnly="true" SortExpression="DateEntered" />
                                            <asp:BoundField DataField="DateComplete" HeaderText="Date Complete" InsertVisible="false" ReadOnly="true" SortExpression="DateComplete" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:WmtaConnectionString %>' SelectCommand="SELECT * FROM [Feedback] ORDER BY [Completed], [Id]"></asp:SqlDataSource>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
</asp:Content>
