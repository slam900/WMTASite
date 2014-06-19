<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="DistrictRegistration.aspx.cs" Inherits="WMTA.DistrictRegistration" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <br />
    
    <h1>District Audition Registration</h1>
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server">
                <asp:UpdatePanel ID="upStudentSearch" runat="server">
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
                            <asp:Panel ID="pnlMyStudents" runat="server" Visible="false">
                                &nbsp; My Students Only<asp:CheckBox ID="chkMyStudentsOnly" runat="server" />
                            </asp:Panel>
                            <asp:GridView ID="gvStudentSearch" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <p></p>
                <asp:Panel ID="pnlInfo" runat="server">
                    <asp:UpdatePanel ID="upStudentInfo" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Student Information</legend>
                                <div style="font-weight: bold">
                                    <label for="StudentId">Student Id:</label><asp:Label ID="lblStudentId" runat="server" Text=""></asp:Label><br />
                                    <br />
                                    <label for="Name">Name:</label><asp:Label ID="lblName" runat="server" Text=""></asp:Label><br />
                                    <br />
                                    <label for="Grade">Grade:</label><asp:TextBox ID="txtGrade" runat="server" Width="30px" OnTextChanged="txtGrade_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:Label ID="lblGradeError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label><br />
                                    <br />
                                    <label for="District">District:</label><asp:Label ID="lblDistrict" runat="server" Text=""></asp:Label><br />
                                    <br />
                                    <label for="Teacher">Teacher:</label><asp:Label ID="lblTeacher" runat="server" Text=""></asp:Label><br />
                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <p></p>
                    <asp:UpdatePanel ID="upAuditionInfo" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Audition Information</legend>
                                <p>
                                    <asp:Panel ID="pnlChooseAudition" runat="server" Visible="false">
                                        <label id="lblAuditionError" runat="server" class="labelError" visible="false">This student has no editable auditions for the current year.</label>
                                        <div style="font-weight: bold">
                                            <label for="ChooseAudition">Choose Audition</label>
                                        </div>
                                        <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropDownList" AutoPostBack="True" AppendDataBoundItems="True" DataSourceID="SqlDataSource1" DataTextField="DropDownInfo" DataValueField="AuditionId" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblAuditionError2" runat="server" Text="*" CssClass="labelStarError" Visible="False" ForeColor="Red"></asp:Label>
                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictAuditionOptions" SelectCommandType="StoredProcedure">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="txtStudentId" Name="studentId" PropertyName="Text" Type="Int32" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                        <br />
                                    </asp:Panel>
                                    <p>
                                        <label id="lblAuditionSiteError" runat="server" class="labelError" visible="false">No audition has been created for the chosen site</label>
                                        <label id="lblFreezeDatePassed" runat="server" class="labelError" visible="false">The freeze date has passed.  No auditions may be added or modified.</label>
                                        <div style="font-weight: bold">
                                            <label for="AuditionSite">Audition Site</label>
                                        </div>
                                        <asp:DropDownList ID="ddlSite" runat="server" class="dropDownList" DataSourceID="WmtaDataSource" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblSiteError" runat="server" Text="*" CssClass="labelStarError" Visible="False" ForeColor="Red"></asp:Label>
                                        <asp:SqlDataSource ID="WmtaDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT GeoName, GeoId FROM ConfigDistrict WHERE (AuditionLevel = @AuditionLevel) ORDER BY GeoName">
                                            <SelectParameters>
                                                <asp:Parameter DefaultValue="District" Name="AuditionLevel" Type="String" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                        <p>
                                        </p>
                                        <div style="font-weight: bold">
                                            <label for="AuditionDate">Audition Date</label>
                                        </div>
                                        <asp:Label ID="lblAuditionDate" runat="server" Text=""></asp:Label><br />
                                        <p>
                                        </p>
                                        <div style="font-weight: bold">
                                            <label for="Instrument">Instrument</label>
                                        </div>
                                        <asp:DropDownList ID="ddlInstrument" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource1" DataTextField="Instrument" DataValueField="Instrument" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Label ID="lblInstrumentError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label>
                                        <asp:SqlDataSource ID="WmtaDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Instrument] FROM [ConfigInstrument] ORDER BY [Instrument]"></asp:SqlDataSource>
                                        <p>
                                        </p>
                                        <p>
                                            <label for="Accompanist">
                                                Accompanist</label>
                                            <asp:TextBox ID="txtAccompanist" runat="server" AutoPostBack="True"></asp:TextBox>
                                        </p>
                                        <p>
                                            <label for="AuditionType">
                                                Audition Type</label>
                                            <asp:DropDownList ID="ddlAuditionType" runat="server" AppendDataBoundItems="true" AutoPostBack="true" CssClass="dropDownList" OnSelectedIndexChanged="ddlAuditionType_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Selected="False" Text="Solo" Value="Solo"></asp:ListItem>
                                                <asp:ListItem Selected="False" Text="Duet" Value="Duet"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="lblAuditionTypeError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                            <label id="lblDuetPartner" runat="server" class="labelInstruction" for="duetPartnerName" visible="false">
                                            </label>
                                            <asp:LinkButton ID="lnkChangePartner" runat="server" Font-Size="Smaller" OnClick="lnkChangePartner_Click" Visible="false">Edit Partner</asp:LinkButton>
                                            <br />
                                            <asp:Panel ID="pnlDuetPartner" runat="server" Visible=" false">
                                                <div style="width: 80%; margin-left: 10%; margin-right: 10%">
                                                    <label class="labelSubtitle">
                                                        Choose A Duet Partner</label><br />
                                                </div>
                                                <label class="labelInstructionSmallerMargin" style="text-align: center; margin-left: 4.5em">
                                                    An audition will be automatically generated for the selected student.</label>
                                                <asp:Label ID="lblPartnerError" runat="server" CssClass="labelError" Text=""></asp:Label>
                                                <br />
                                                <label for="StudentId" style="font-weight: bold">
                                                    Student Id</label>
                                                <asp:TextBox ID="txtPartnerId" runat="server" CssClass="input"></asp:TextBox>
                                                <asp:Button ID="btnPartnerSearch" runat="server" CssClass="button" OnClick="btnPartnerSearch_Click" Text="Search" />
                                                <p>
                                                    <label for="FirstName">
                                                        First Name</label>
                                                    <asp:TextBox ID="txtPartnerFirstName" runat="server" CssClass="input"></asp:TextBox>
                                                    <asp:Button ID="btnPartnerClear" runat="server" CssClass="button" OnClick="btnPartnerClear_Click" Text="Clear" />
                                                </p>
                                                <p>
                                                    <label for="LastName">
                                                        Last Name</label>
                                                    <asp:TextBox ID="txtPartnerLastName" runat="server" CssClass="input"></asp:TextBox>
                                                </p>
                                                <asp:GridView ID="gvDuetPartner" runat="server" AllowPaging="True" AutoGenerateSelectButton="True" CssClass="gridview" Font-Size="14px" OnPageIndexChanging="gvDuetPartner_PageIndexChanging" OnRowDataBound="gvDuetPartner_RowDataBound" OnSelectedIndexChanged="gvDuetPartner_SelectedIndexChanged">
                                                </asp:GridView>
                                            </asp:Panel>
                                            <p>
                                            </p>
                                            <p>
                                                <label for="AuditionTrack">
                                                    Audition Track</label>
                                                <asp:DropDownList ID="ddlAuditionTrack" runat="server" AppendDataBoundItems="true" CssClass="dropDownList" DataSourceID="WmtaDataSource3" DataTextField="Track" DataValueField="Track" OnSelectedIndexChanged="ddlAuditionTrack_SelectedIndexChanged" AutoPostBack="True">
                                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblAuditionTrackError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                                <asp:SqlDataSource ID="WmtaDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTrack" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                            </p>
                                            <asp:Label ID="lblTheoryLvl" runat="server" Text="" CssClass="labelExtendedInstruction" ForeColor="Red" Visible="false">The student has gotten a 5 on this theory level or a higher level more than one time.  Please choose a higher level.</asp:Label>
                                            <div style="font-weight: bold">
                                                <label for="TheoryLevel">Theory Level</label>
                                            </div>
                                            <asp:DropDownList ID="ddlTheoryLevel" runat="server" CssClass="dropDownList" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlTheoryLevel_SelectedIndexChanged" Width="70px">
                                                <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                                <asp:DropDownList ID="ddlTheoryLevelType" runat="server" CssClass="dropDownList" Visible="false" Width="80px">
                                                    <asp:ListItem Selected="True" Text="" Value="" />
                                                    <asp:ListItem Text="Keybrd" Value="Keybrd" />
                                                    <asp:ListItem Text="Treble" Value="Treble" />
                                                    <asp:ListItem Text="Alto" Value="Alto" />
                                                    <asp:ListItem Text="Bass" Value="Bass" />
                                                </asp:DropDownList>
                                            <asp:Label ID="lblTheoryLevelError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label>
                                            <p></p>
                                        </p>
                                    </p>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <p></p>
                    <asp:UpdatePanel ID="upCompositions" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Compositions To Perform</legend>
                                <asp:Label ID="lblCompositionError" runat="server" Text="" CssClass="labelExtendedInstruction" ForeColor="Red" Visible="false"></asp:Label>
                                <p>
                                    <label for="Style">Style</label>
                                    <asp:DropDownList ID="ddlStyle" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource2" DataTextField="Style" DataValueField="Style" AppendDataBoundItems="true" OnSelectedIndexChanged="cboStyle_SelectedIndexChanged" AutoPostBack="True">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="WmtaDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                    <asp:Button ID="btnClearCompSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearCompSearch_Click" />
                                </p>
                                <p>
                                    <label for="Level">Level</label>
                                    <asp:DropDownList ID="ddlCompLevel" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource7" DataTextField="Description" DataValueField="CompLevelId" AppendDataBoundItems="true" OnSelectedIndexChanged="cboCompLevel_SelectedIndexChanged" AutoPostBack="True">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="WmtaDataSource7" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                </p>
                                <p>
                                    <label for="Composer">Composer</label>
                                    <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" OnSelectedIndexChanged="cboComposer_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                    &nbsp; New<asp:CheckBox ID="chkNewComposer" runat="server" AutoPostBack="true" OnCheckedChanged="chkNewComposer_CheckedChanged" />
                                    <asp:Panel ID="pnlComposer" runat="server" Visible="false">
                                        <label for="Composer" class="labelSmallInstruction">Last Name</label><asp:TextBox ID="txtComposerLast" runat="server" Width="150px" CssClass="input"></asp:TextBox><br />
                                        <label for="Composer" class="labelSmallInstruction">F.I.</label><asp:TextBox ID="txtComposerFI" runat="server" CssClass="input" Width="30px" MaxLength="1"></asp:TextBox><br />
                                        <label for="Composer" class="labelSmallInstruction">M.I.</label><asp:TextBox ID="txtComposerMI" runat="server" CssClass="input" Width="30px" MaxLength="1"></asp:TextBox>
                                    </asp:Panel>
                                    <p>
                                    </p>
                                </p>
                                <p>
                                    <label id="lblCompositionInstruction" runat="server" class="labelInstruction" for="CompositionInstructions" visible="false">
                                        Title, Key, Movement, Catalog No.</label>
                                    <label for="Composition">Title</label>
                                    <asp:DropDownList ID="ddlComposition" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlComposition_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                    <asp:TextBox ID="txtComposition" runat="server" CssClass="input" Visible="false"></asp:TextBox>
                                    &nbsp; New<asp:CheckBox ID="chkNewTitle" runat="server" AutoPostBack="true" OnCheckedChanged="chkNewTitle_CheckedChanged" />
                                </p>
                                <p>
                                    <label for="CompositionTime">Playing Time</label><br />
                                    <div style="margin-left: 1.5em">
                                        <asp:Label ID="lblMinutesErrorMsg" runat="server" CssClass="labelError" Text="Minutes must be a positive integer" Visible="false"></asp:Label>
                                        <label for="Minutes" style="font-weight: bold; font-size: 14px; vertical-align: bottom">
                                            Minutes</label><asp:TextBox ID="txtMinutes" runat="server" CssClass="textBox2" TextMode="Number"></asp:TextBox>
                                        <asp:Label ID="lblMinutesError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                        <br />
                                        <label for="Seconds" style="font-weight: bold; font-size: 14px">
                                            Seconds</label>
                                        <asp:DropDownList ID="ddlSeconds" runat="server" CssClass="dropDownList" Width="65px">
                                            <asp:ListItem Selected="True" Text="0" Value="0" />
                                            <asp:ListItem Text="15" Value="0.25" />
                                            <asp:ListItem Text="30" Value="0.5" />
                                            <asp:ListItem Text="45" Value="0.75" />
                                        </asp:DropDownList>
                                    </div>
                                    <asp:Button ID="btnAddComposition" runat="server" Text="Add" CssClass="button2" OnClick="btnAddComposition_Click" />
                                    <asp:Button ID="btnRemoveComposition" runat="server" Text="Remove" CssClass="button2" OnClick="btnRemoveComposition_Click" /><br />
                                    <asp:Label ID="lblRemoveError" runat="server" CssClass="labelErrorSmallerMargin" Text="Please select a composition to remove from the audition." Visible="false"></asp:Label><br />
                                    <asp:Table ID="tblCompositions" runat="server">
                                        <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
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
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <p></p>
                    <asp:UpdatePanel ID="upTimeConstraints" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <legend>Time Constraints</legend>
                                <p>
                                    <label id="lblTimePrefError" runat="server" class="labelError" style="width: 90%; margin-left: 5%; margin-right: 5%" visible="false">Please choose the preferred time or select "No Preference".</label>
                                    <asp:RadioButtonList ID="rblTimePreference" runat="server" Width="301px" CssClass="radioBtnList" RepeatLayout="Flow" OnSelectedIndexChanged="rblTimePreference_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Selected="True">No Preference</asp:ListItem>
                                        <asp:ListItem>Preferred Time</asp:ListItem>
                                    </asp:RadioButtonList><br />
                                    <br />
                                    <asp:Panel ID="pnlPreferredTime" runat="server" Visible="false">
                                        <div style="margin-left: 1.5em;">
                                            <asp:RadioButtonList ID="rblTimeOptions" runat="server" Width="301px" CssClass="radioBtnList" Font-Bold="false" RepeatLayout="Flow">
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
                                        <label for="Coordinate" style="font-weight: bold; width: 100%; text-align: left">Coordinating Students</label>
                                        <br />
                                        <asp:Table ID="tblCoordinates" runat="server">
                                            <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" BorderStyle="Solid">
                                                <asp:TableHeaderCell Scope="Column" Text="Id" />
                                                <asp:TableHeaderCell Scope="Column" Text="First Name" />
                                                <asp:TableHeaderCell Scope="Column" Text="Last Name" />
                                                <asp:TableHeaderCell Scope="Column" Text="Reason" />
                                            </asp:TableHeaderRow>
                                        </asp:Table>
                                    </asp:Panel>
                                    <p>
                                    </p>
                                </p>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
                <asp:UpdatePanel ID="upButtons" runat="server">
                    <ContentTemplate>
                        <p>
                            <div style="margin-left: 15px">
                                <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Width="350px" Visible="false"></asp:Label><br />
                            </div>
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
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align: center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The audition was successfully created." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create New Audition" Value="Create New"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Edit Audition" Value="Edit Existing"></asp:ListItem>
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
