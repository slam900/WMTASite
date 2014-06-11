<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageContacts.aspx.cs" Inherits="WMTA.ManageContacts" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <asp:UpdatePanel ID="upContactSearch" runat="server">
        <ContentTemplate>
            <h1 id="header" runat="server">Manage Contacts</h1>
            <div style="text-align: center">
                <asp:Label ID="lblError" runat="server" Text="There was an error adding the new contact" CssClass="labelError" Visible="False"></asp:Label>
            </div>
            <asp:Panel ID="pnlContactSearch" runat="server" Visible="false">
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
            <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                        <label for="ContactId" style="font-weight: bold">Contact Id</label>
                        <asp:Label ID="lblId" runat="server" Text="" ForeColor="DarkBlue"></asp:Label><br />
                    <p>
                        <label for="FirstName" style="font-weight: bold">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" AutoPostBack="True" OnTextChanged="txtFirstName_TextChanged"></asp:TextBox>
                        <label id="lblFirstNameError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="MiddleInitial">Middle Initial</label>
                        <asp:TextBox ID="txtMiddleInitial" runat="server" AutoPostBack="True" OnTextChanged="txtMiddleInitial_TextChanged"></asp:TextBox>
                        <label id="lblMiddleNameError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="LastName">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" AutoPostBack="True" OnTextChanged="txtLastName_TextChanged"></asp:TextBox>
                        <label id="lblLastNameError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="Street">Street</label>
                        <asp:TextBox ID="txtStreet" runat="server"></asp:TextBox>
                        &nbsp;
                    </p>
                    <p>
                        <label for="City">City</label>
                        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                    </p>
                    <p>
                        <label for="State">State</label>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="dropDownList" AutoPostBack="True" DataSourceID="SqlDataSource5" DataTextField="State" DataValueField="State" OnDataBound="ddlState_DataBound">
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [State] FROM [ConfigStates] ORDER BY [State]"></asp:SqlDataSource>
                        &nbsp;
                    </p>
                    <p>
                        <asp:Label ID="lblZipErrorDesc" runat="server" Text="Please enter a valid zipcode." CssClass="labelError" Visible="false"></asp:Label>
                        <label for="Zip">ZIP</label>
                        <asp:TextBox ID="txtZip" runat="server" MaxLength="5" TextMode="Number" AutoPostBack="true" OnTextChanged="txtZip_TextChanged"></asp:TextBox>
                    </p>
                    <p>
                        <asp:Label ID="lblEmailErrorDesc" runat="server" Text="" CssClass="labelError" Visible="false"></asp:Label>
                        <label for="Email">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" OnTextChanged="txtEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <label id="lblEmailError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <asp:Label ID="lblPhoneErrorDesc" runat="server" Text="" CssClass="labelError" Visible="false"></asp:Label>
                        <label for="Phone">Phone</label>
                        <asp:TextBox ID="txtPhone" runat="server" TextMode="Phone" OnTextChanged="txtPhone_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <label id="lblPhoneError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="District">District</label>
                        <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlDistrict_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                            <asp:ListItem Selected="False" Text="Not WMTA Member" Value="-1"></asp:ListItem>
                        </asp:DropDownList>
                        <label id="lblDistrictError" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                    </p>
                    <p>
                        <label for="Contact Type">Contact Type</label>
                        <asp:DropDownList ID="ddlContactType" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource2" DataTextField="Contact Type" DataValueField="ContactTypeId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlContactType_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <label id="lblContactTypeError" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownContactType" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                    </p>
                    <asp:Panel ID="pnlJudges" runat="server" Visible="false">
                        <p>
                            <label for="Track">Track</label>
                            <div style="float: left; margin-left: .1em; width: 160px">
                                <asp:CheckBoxList ID="chkLstTrack" runat="server">
                                    <asp:ListItem>District</asp:ListItem>
                                    <asp:ListItem>State</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </p>
                        <p>
                            <label for="Type">Type</label>
                            <div style="float: left; margin-left: .1em; width: 160px">
                                <asp:CheckBoxList ID="chkLstType" runat="server">
                                    <asp:ListItem>Solo</asp:ListItem>
                                    <asp:ListItem>Duet</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </p>
                        <p>
                            <label for="Composition Level">Composition Level</label>
                            <div style="float: left; margin-left: .1em; width: 160px">
                                <asp:CheckBoxList ID="chkLstCompLevel" runat="server" DataSourceID="SqlDataSource3" DataTextField="Description" DataValueField="CompLevelId">
                                </asp:CheckBoxList>
                            </div>
                            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                        </p>
                        <p>
                            <label for="Instrument">Instruments</label>
                            <div style="float: left; margin-left: .1em; width: 160px">
                                <asp:CheckBoxList ID="chkLstInstrument" runat="server" DataSourceID="SqlDataSource4" DataTextField="Instrument" DataValueField="Instrument">
                                </asp:CheckBoxList>
                            </div>
                            <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownInstrument" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                        </p>
                    </asp:Panel>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upButtons" runat="server">
        <ContentTemplate>
            <div style="clear: both">
                <asp:Panel ID="pnlButtons" runat="server" Visible="false">
                    <p>
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                        <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="button" OnClick="btnAdd_Click" />
                    </p>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="true">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The contact was successfully created." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="False" Text="Edit Existing Contact" Value="Edit Existing"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <br />
                <div style="width: 60%; margin-left: 5%">
                    <asp:Button ID="btnBackOption" runat="server" Text="Back" CssClass="button" Font-Bold="true" OnClick="btnBackOption_Click" />
                    <asp:Button ID="btnGo" runat="server" Text="Go" CssClass="button" Font-Bold="true" OnClick="btnGo_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upAreYouSure" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlAreYouSure" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Label ID="lblWarning" runat="server" Text="Are you sure that you want to permanently delete this contact?  All of the contact's associated information will also be deleted.  This action cannot be undone." CssClass="labelSuccess" ForeColor="Red" Width="300px"></asp:Label><br />
                    <br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Button ID="btnCancelDelete" runat="server" Text="Cancel" CssClass="button" Font-Bold="true" OnClick="btnCancelDelete_Click" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="button" Font-Bold="true" OnClick="btnDelete_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
