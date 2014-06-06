<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageStudents.aspx.cs" Inherits="WMTA.ManageStudents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/AddStudentStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <asp:UpdatePanel ID="upStudentSearch" runat="server">
        <ContentTemplate>
            <h1 id="header" runat="server">Manage Students</h1>
            <asp:Label ID="lblError" runat="server" Text="There was an error adding the new student" CssClass="labelError" Visible="False"></asp:Label>
            <asp:Panel ID="pnlStudentSearch" runat="server" Visible="false">
                <fieldset>
                    <legend>Student Search</legend>
                    <asp:Label ID="lblStudentSearchError" runat="server" Text="" CssClass="labelError"></asp:Label><br />
                    <label for="StudentId" style="font-weight: bold">Student Id</label>
                    <asp:TextBox ID="txtStudentId" runat="server" CssClass="input"></asp:TextBox>
                    <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="button" OnClick="btnStudentSearch_Click" />
                    <p>
                        <label for="FirstName">First Name</label>
                        <asp:TextBox ID="txtFirstNameSearch" runat="server" CssClass="input"></asp:TextBox>
                        <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearStudentSearch_Click" />
                    </p>
                    <p>
                        <label for="LastName">Last Name</label>
                        <asp:TextBox ID="txtLastNameSearch" runat="server" CssClass="input"></asp:TextBox>
                    </p>
                    <asp:GridView ID="gvStudentSearch" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                </fieldset>
                <br />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="FirstName" style="font-weight: bold">First Name</label>
                    <asp:TextBox ID="txtFirstName" runat="server" AutoPostBack="True" OnTextChanged="txtFirstName_TextChanged"></asp:TextBox>
                    <label id="lblFirstNameError" runat="server" class="labelStarError" visible="false">*</label>
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
                        <label for="Grade">Grade</label>
                        <asp:TextBox ID="txtGrade" runat="server" AutoPostBack="True" OnTextChanged="txtGrade_TextChanged"></asp:TextBox>
                        <label id="lblGradeError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="District">District</label>
                        <asp:DropDownList ID="cboDistrict" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="True" OnSelectedIndexChanged="cboDistrict_SelectedIndexChanged">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <label id="lblDistrictError" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                    </p>
                    <p>
                        <label for="CurrentTeacher">Current Teacher</label>
                        <asp:DropDownList ID="cboCurrTeacher" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true" AutoPostBack="True" OnSelectedIndexChanged="cboCurrTeacher_SelectedIndexChanged">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <label id="lblCurrTeacherError" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTeacher" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                    </p>
                    <p>
                        <label for="Previous Teacher">Previous Teacher</label>
                        <asp:DropDownList ID="cboPrevTeacher" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </p>
                    <p>
                        <label for="Legacy Points" id="lblLegacyPoints" runat="server">Legacy Points</label>
                        <asp:TextBox ID="txtLegacyPoints" runat="server" TextMode="Number">0</asp:TextBox>
                    </p>
                    <p>
                        <label for="StudentId">Student Id:</label>
                        <asp:Label ID="lblId" runat="server" Text="" ForeColor="DarkBlue"></asp:Label>
                    </p>
                    <br />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upButtons" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlButtons" runat="server" Visible="false">
                <p>
                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                    <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="button" OnClick="btnAdd_Click" />
                </p>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="true">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The student was successfully created." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create New Student" Value="Create New"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Edit Existing Student" Value="Edit Existing"></asp:ListItem>
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
    <asp:UpdatePanel ID="upAreYouSure" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlAreYouSure" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblWarning" runat="server" Text="Are you sure that you want to permanently delete this student?  All of the student's associated information will also be deleted including awards, coordinate student records, compositions, points, and auditions.  This action cannot be undone." CssClass="labelSuccess" ForeColor="Red" Width="300px"></asp:Label><br />
                    <br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Button ID="btnCancelDelete" runat="server" Text="Cancel" CssClass="button" Font-Bold="true" OnClick="btnCancelDelete_Click" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="button" Font-Bold="true" OnClick="btnDelete_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upDuplicateStudent" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlDuplicateStudent" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblDuplicateWarning" runat="server" Text="There is already a student with the name you have entered. Please contact your district chair to make sure the student doesn't already exist." CssClass="labelSuccess" Width="325px"></asp:Label><br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Button ID="btnCancelAdd" runat="server" Text="Cancel" CssClass="button" Font-Bold="true" OnClick="btnCancelAdd_Click" />
                    <asp:Button ID="btnAddAnyways" runat="server" Visible="false" Text="Add" CssClass="button" Font-Bold="true" OnClick="btnAddAnyways_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
