<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="RegisterContact.aspx.cs" Inherits="WMTA.RegisterContact" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <asp:UpdatePanel ID="upContactSearch" runat="server">
        <ContentTemplate>
            <h1 id="header" runat="server">Register Contact</h1>
            <div style="text-align: center">
                <asp:Label ID="lblError" runat="server" Text="There was an error registering the contact" CssClass="labelError" Visible="False"></asp:Label>
            </div>
            <asp:Panel ID="pnlContactSearch" runat="server">
                <fieldset>
                    <legend>Contact Search</legend>
                    <asp:Label ID="lblContactSearchError" runat="server" Text="" CssClass="labelError"></asp:Label><br />
                    <label for="ContactId" style="font-weight: bold">Contact Id</label>
                    <asp:TextBox ID="txtContactId" runat="server" CssClass="input"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button" OnClick="btnSearch_Click" />
                    <p>
                        <label for="FirstName">First Name</label>
                        <asp:TextBox ID="txtFirstNameSearch" runat="server" CssClass="input"></asp:TextBox>
                        <asp:Button ID="btnClearSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearSearch_Click" />
                    </p>
                    <p>
                        <label for="LastName">Last Name</label>
                        <asp:TextBox ID="txtLastNameSearch" runat="server" CssClass="input"></asp:TextBox>
                    </p>
                    <asp:GridView ID="gvSearch" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvSearch_PageIndexChanging" OnRowDataBound="gvSearch_RowDataBound" OnSelectedIndexChanged="gvSearch_SelectedIndexChanged"></asp:GridView>
                </fieldset>
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <p>
                        <label for="Id" style="font-weight: bold">Id</label>
                        <asp:Label ID="lblId" runat="server" Font-Bold="false"></asp:Label><br />
                    </p>
                    <p>
                        <label for="Name" style="font-weight: bold">Name</label>
                        <asp:Label ID="lblName" runat="server" Font-Bold="false"></asp:Label><br />
                    </p>
                    <p>
                        <label for="District">District</label>
                        <asp:Label ID="lblDistrict" runat="server" Font-Bold="false"></asp:Label><br />
                    </p>
                    <p>
                        <label for="Contact Type">Contact Type</label>
                        <asp:Label ID="lblContactType" runat="server" Font-Bold="false"></asp:Label><br />
                    </p>
                    <p>
                        <label for="MTNA Id">MTNA Id</label>
                        <asp:TextBox ID="txtMtnaId" runat="server" />
                        <label id="lblMtnaIdError" runat="server" class="labelStarError" visible="false">*</label><br />
                    </p>
                    <p>
                        <label for="Year">Year</label>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropDownList" />
                        <label id="lblYearError" runat="server" class="labelStarError" visible="false">*</label><br />
                    </p>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upButtons" runat="server">
        <ContentTemplate>
            <div style="clear: both">
                <asp:Panel ID="pnlButtons" runat="server">
                    <p>
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="button" OnClick="btnRegister_Click" />
                    </p>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The contact was successfully registered. Click 'OK' to register another contact or 'Back' to return to the mains screen." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                <br />
                <div style="width: 60%; margin-left: 5%">
                    <asp:Button ID="btnBackOption" runat="server" Text="Back" CssClass="button" Font-Bold="true" OnClick="btnBackOption_Click" />
                    <asp:Button ID="btnGo" runat="server" Text="OK" CssClass="button" Font-Bold="true" OnClick="btnGo_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
