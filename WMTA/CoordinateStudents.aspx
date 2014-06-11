<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CoordinateStudents.aspx.cs" Inherits="WMTA.CoordinateStudents" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <h1>Coordinate Students</h1>
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server">
            <asp:UpdatePanel ID="upStudent1" runat="server">
                <ContentTemplate>
                     <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                        <asp:Label ID="lblMainError" runat="server" Text="Please make sure the selected students are not the same." CssClass="labelError" Visible="false"></asp:Label><br />
                        <p>
                            <label for="Student1">Student 1</label>
                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownStudent" SelectCommandType="StoredProcedure"></asp:SqlDataSource>

                            <asp:DropDownList ID="ddlStudent1" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="StudentId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlStudent1_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Text="" Value="" />
                            </asp:DropDownList>
                            <label id="lblStudent1Error" runat="server" class="labelStarError" visible="false">*</label>
                            <asp:Button ID="btnStudent1Search" runat="server" Text="Search" Font-Bold="true" OnClick="btnStudent1Search_Click" />
                            <label id="lblStudent1Id" runat="server" visible="false"></label>
                        </p>
                    </div>
                    <asp:Panel ID="pnlStudent1Search" runat="server" Visible="false">
                        <fieldset>
                            <legend>Student 1 Search</legend>
                            <asp:Label ID="lblStudent1SearchError" runat="server" Text="" CssClass="labelError"></asp:Label><br />
                            <label for="StudentId" style="font-weight: bold">Student Id</label>
                            <asp:TextBox ID="txtStudent1Id" runat="server" CssClass="input"></asp:TextBox>
                            <asp:Button ID="btnSearchStudent1" runat="server" Text="Search" CssClass="button" OnClick="btnSearchStudent1_Click" />
                            <p>
                                <label for="FirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName1" runat="server" CssClass="input"></asp:TextBox>
                                <asp:Button ID="btnClearStudent1Search" runat="server" Text="Clear" CssClass="button" OnClick="btnClearStudent1Search_Click" />
                            </p>
                            <p>
                                <label for="LastName">Last Name</label>
                                <asp:TextBox ID="txtLastName1" runat="server" CssClass="input"></asp:TextBox>
                            </p>
                            <asp:GridView ID="gvStudent1Search" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudent1Search_PageIndexChanging" OnRowDataBound="gvStudent1Search_RowDataBound" OnSelectedIndexChanged="gvStudent1Search_SelectedIndexChanged"></asp:GridView>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="upStudent2" runat="server">
                <ContentTemplate>
                     <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                        <p>
                            <label for="Student2">Student 2</label>
                            <asp:DropDownList ID="ddlStudent2" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="ComboName" DataValueField="StudentId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlStudent2_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Text="" Value="" />
                            </asp:DropDownList>
                            <label id="lblStudent2Error" runat="server" class="labelStarError" visible="false">*</label>
                            <asp:Button ID="btnStudent2Search" runat="server" Text="Search" Font-Bold="true" OnClick="btnStudent2Search_Click" />
                            <label id="lblStudent2Id" runat="server" visible="false"></label>
                        </p>
                    </div>
                    <asp:Panel ID="pnlStudent2Search" runat="server" Visible="false">
                        <fieldset>
                            <legend>Student 2 Search</legend>
                            <asp:Label ID="lblStudent2SearchError" runat="server" Text="" CssClass="labelError"></asp:Label><br />
                            <label for="StudentId" style="font-weight: bold">Student Id</label>
                            <asp:TextBox ID="txtStudent2Id" runat="server" CssClass="input"></asp:TextBox>
                            <asp:Button ID="btnSearchStudent2" runat="server" Text="Search" CssClass="button" OnClick="btnSearchStudent2_Click" />
                            <p>
                                <label for="FirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName2" runat="server" CssClass="input"></asp:TextBox>
                                <asp:Button ID="btnClearStudent2Search" runat="server" Text="Clear" CssClass="button" OnClick="btnClearStudent2Search_Click" />
                            </p>
                            <p>
                                <label for="LastName">Last Name</label>
                                <asp:TextBox ID="txtLastName2" runat="server" CssClass="input"></asp:TextBox>
                            </p>
                            <asp:GridView ID="gvStudent2Search" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudent2Search_PageIndexChanging" OnRowDataBound="gvStudent2Search_RowDataBound" OnSelectedIndexChanged="gvStudent2Search_SelectedIndexChanged"></asp:GridView>
                        </fieldset>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
             <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                <p>
                    <label for="Reason">Reason</label>
                    <asp:DropDownList ID="ddlReason" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Carpool" Value="Carpool"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Sibling" Value="Sibling"></asp:ListItem>
                    </asp:DropDownList>
                    <label id="lblReasonError" runat="server" class="labelStarError" visible="false">*</label>
                </p>
                <p>
                    <label for="AuditionType">Event Type</label>
                    <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        <asp:ListItem Selected="False" Text="District Audition" Value="District"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Badger Competition" Value="State"></asp:ListItem>
                    </asp:DropDownList>
                    <label id="lblAudTypeError" runat="server" class="labelStarError" visible="false">*</label><br />
                </p>
            </div>
            <asp:UpdatePanel ID="upButtons" runat="server">
                <ContentTemplate>
                    <p>
                         <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                            <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Width="350px" Visible="false"></asp:Label><br />
                        </div>
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                        <asp:Button ID="btnRegister" runat="server" Text="Submit" CssClass="button" OnClick="btnRegister_Click" />
                    </p>
                </ContentTemplate>
            </asp:UpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The audition was successfully created" CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create New Coordinate" Value="Create New"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <br />
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Button ID="btnBackOption" runat="server" Text="Back" CssClass="button" Font-Bold="true" OnClick="btnBackOption_Click" />
                    <asp:Button ID="btnGo" runat="server" Text="Go" CssClass="button" Font-Bold="true" OnClick="btnGo_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>