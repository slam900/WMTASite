<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="BadgerRegistration.aspx.cs" Inherits="WMTA.BadgerRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/ControlsStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <h1>Badger State Competition Registration</h1>
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <fieldset>
                            <legend>Student Search</legend>
                            <asp:Label ID="lblStudentSearchError" runat="server" Text="" CssClass="labelError"></asp:Label><br />
                            <label for="StudentId" style="font-weight: bold">Student Id</label>
                            <asp:TextBox ID="txtStudentId" runat="server" CssClass="input"></asp:TextBox>
                            <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="button" OnClick="btnStudentSearch_Click" />
                            <p>
                                <label for="FirstName">First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="input"></asp:TextBox>
                                <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearStudentSearch_Click" />
                            </p>
                            <p>
                                <label for="LastName">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="input"></asp:TextBox>
                            </p>
                            <asp:GridView ID="gvStudentSearch" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <p></p>
                <asp:Panel ID="pnlInfo" runat="server" Visible="false">
                    <fieldset>
                        <legend>Student Information</legend>
                        <asp:Label ID="lblStudentTooYoung" runat="server" Text="Students must be in at least 4th grade to register" CssClass="labelError2"></asp:Label><br />
                        <div style="font-weight: bold">
                            <label for="StudentId">Student Id:</label><asp:Label ID="lblStudentId" runat="server" Text=""></asp:Label><br />
                            <br />
                            <label for="Name">Name:</label><asp:Label ID="lblName" runat="server" Text=""></asp:Label><br />
                            <br />
                            <label for="Grade">Grade:</label><asp:Label ID="lblGrade" runat="server" Text=""></asp:Label><br />
                            <br />
                            <label for="District">District:</label><asp:Label ID="lblDistrict" runat="server" Text=""></asp:Label><br />
                            <br />
                            <label for="Teacher">Teacher:</label><asp:Label ID="lblTeacher" runat="server" Text=""></asp:Label><br />
                        </div>
                    </fieldset>
                    <p></p>
                    <fieldset>
                        <legend>Competition Information</legend>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <p>
                                    <label id="lblAuditionError" runat="server" class="labelError" visible="false">This student has no eligible auditions that may be edited</label>
                                    <label for="ChooseAudition">Choose Audition</label>
                                    <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropDownList" AutoPostBack="True" AppendDataBoundItems="True" DataSourceID="SqlDataSource1" DataTextField="DropDownInfo" DataValueField="AuditionId" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownStateCompOptions" SelectCommandType="StoredProcedure">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="txtStudentId" Name="studentId" PropertyName="Text" Type="Int32" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
                                    <label id="lblAuditionSelectError" runat="server" class="labelStarError" visible="false">*</label>
                                </p>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <p>
                                    <label id="lblSiteError" runat="server" class="labelError" visible="false">No state competition sites have been created</label>
                                    <label for="BadgerSite">Regional Site</label>
                                    <asp:DropDownList ID="cboSite" runat="server" CssClass="dropDownList" AppendDataBoundItems="true" OnSelectedIndexChanged="cboSite_SelectedIndexChanged" AutoPostBack="True">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <label id="lblSiteError2" runat="server" class="labelStarError" visible="false">*</label>
                                </p>
                                <p><div style="font-weight: bold">
                                    <label for="AuditionDate">Audition Date</label>
                                </div>
                                <asp:Label ID="lblAuditionDate" runat="server" Text=""></asp:Label><br />
                                    <p>
                                    </p>
                                    <p>
                                        <label for="DriveTime">
                                        Drive Time</label>
                                        <asp:TextBox ID="txtDriveTime" runat="server" AutoPostBack="True" CssClass="textBox2" OnTextChanged="txtDriveTime_TextChanged" TextMode="Number"></asp:TextBox>
                                        <label id="lblDriveTimeError" runat="server" class="labelStarError" visible="false">
                                        *</label> minutes
                                    </p>
                                </p>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                    <br />
                    <fieldset>
                        <legend>Time Constraints</legend>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <label id="lblTimePrefError" runat="server" class="labelError" style="width: 90%; margin-left: 5%; margin-right: 5%" visible="false">Please choose the preferred time or select "No Preference".</label>
                                <asp:RadioButtonList ID="rblTimePreference" runat="server" Width="301px" CssClass="radioBtnList" RepeatLayout="Flow" OnSelectedIndexChanged="rblTimePreference_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True">No Preference</asp:ListItem>
                                    <asp:ListItem>Preferred Time</asp:ListItem>
                                </asp:RadioButtonList><br />
                                <br />
                                <asp:Panel ID="pnlPreferredTime" runat="server" Visible="false">
                                    <div style="margin-left: 1.5em;">
                                        <asp:RadioButtonList ID="rblTimeOptions" runat="server" Width="301px" CssClass="radioBtnList" Font-Bold="false" RepeatLayout="Flow" AutoPostBack="true">
                                            <asp:ListItem>A.M.</asp:ListItem>
                                            <asp:ListItem>P.M.</asp:ListItem>
                                            <asp:ListItem>Earliest</asp:ListItem>
                                            <asp:ListItem>Latest</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </asp:Panel>
                                <p>
                                </p>
                                <asp:Panel ID="pnlCoordinateParticipants" runat="server" Visible="false">
                                   <label for="Coordinate" style="font-weight:bold; width: 100%; text-align: left">Coordinating Students</label>
                                    <br />
                                    <asp:Table ID="tblCoordinates" runat="server">
                                        <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
                                            <asp:TableHeaderCell Scope="Column" Text="Id" />
                                            <asp:TableHeaderCell Scope="Column" Text="First Name" />
                                            <asp:TableHeaderCell Scope="Column" Text="Last Name" />
                                            <asp:TableHeaderCell Scope="Column" Text="Reason" />
                                        </asp:TableHeaderRow>
                                    </asp:Table>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </fieldset>
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                        <ContentTemplate>
                             <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                                <p>&nbsp;View Additional Information<asp:CheckBox ID="chkAdditionalInfo" runat="server" CssClass="checkbox" AutoPostBack="true" OnCheckedChanged="chkAdditionalInfo_CheckedChanged" /></p>
                            </div>
                            <p></p>
                            <asp:Panel ID="pnlAdditionalInfo" runat="server" Visible="false">
                                <fieldset>
                                    <legend>Audition Information</legend>
                                    <div style="font-weight: bold">
                                        <label for="Instrument">Instrument:</label><asp:Label ID="lblInstrument" runat="server" Text=""></asp:Label>
                                        <br />
                                        <br />
                                        <label for="Accompanist">Accompanist:</label><asp:Label ID="lblAccompanist" runat="server" Text=""></asp:Label><br />
                                        <br />
                                        <label for="AuditionType">Audition Type:</label><asp:Label ID="lblAuditionType" runat="server" Text=""></asp:Label><br />
                                        <br />
                                    </div>
                                </fieldset>
                                <p></p>
                                <fieldset>
                                    <legend>Compositions To Perform</legend>
                                    <asp:Table ID="tblCompositions" runat="server">
                                        <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" BorderStyle="Solid">
                                            <asp:TableHeaderCell Scope="Column" Text="" />
                                            <asp:TableHeaderCell Scope="Column" Text="Id" Visible="false" />
                                            <asp:TableHeaderCell Scope="Column" Text="Composition" />
                                            <asp:TableHeaderCell Scope="Column" Text="Composer" />
                                            <asp:TableHeaderCell Scope="Column" Text="Style" />
                                            <asp:TableHeaderCell Scope="Column" Text="Level" />
                                            <asp:TableHeaderCell Scope="Column" Text="Time" />
                                        </asp:TableHeaderRow>
                                    </asp:Table>
                                </fieldset>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                    <ContentTemplate>
                        <p>
                            <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Visible="false"></asp:Label><br />
                            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="button" OnClick="btnRegister_Click" />
                        </p>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server">
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The audition was successfully created." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create New" Value="Create New"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Edit Existing" Value="Edit Existing"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Delete Audition" Value="Delete Existing"></asp:ListItem>
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
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:Label ID="lblWarning" runat="server" Text="Are you sure that you want to permanently delete this audition and all associated information?  This action cannot be undone." CssClass="labelSuccess" ForeColor="Red" Width="300px"></asp:Label><br />
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
