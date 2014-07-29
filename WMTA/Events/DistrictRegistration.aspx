<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="DistrictRegistration.aspx.cs" Inherits="WMTA.Events.DistrictRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend id="legend" runat="server">Add District Registration</legend>
                                <%-- Student search --%>
                                <asp:UpdatePanel ID="upStudentSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Student Search</h4>
                                            <br />
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtStudentId" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" ID="txtStudentId" CssClass="form-control" />
                                                </div>
                                                <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnStudentSearch_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtFirstName" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" ID="txtFirstName" CssClass="form-control" />
                                                </div>
                                                <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudentSearch_Click" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtLastName" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox runat="server" ID="txtLastName" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="col-md-3-margin popover-font">
                                                <a href="#" id="searchHint">Search Tip</a>
                                            </div>
                                            <div class="form-group">
                                                <asp:Panel ID="pnlMyStudents" runat="server" Visible="false">
                                                    <div class="checkbox">
                                                        <label>
                                                            <input type="checkbox" id="chkMyStudentsOnly" runat="server" />
                                                            Only My Students
                                                        </label>
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                            <div class="form-group">
                                                <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                    OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Student Search --%>
                                <%-- Student Information --%>
                                <asp:Panel ID="pnlInfo" runat="server" Visible="false">
                                    <asp:UpdatePanel ID="upStudentInfo" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <h4>Student Information</h4>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblStudentId" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                    <div class="col-md-6 label-top-margin">
                                                        <asp:Label runat="server" ID="lblStudentId" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblName" CssClass="col-md-3 control-label float-left">Name</asp:Label>
                                                    <div class="col-md-6 label-top-margin">
                                                        <asp:Label runat="server" ID="lblName" />
                                                    </div>
                                                </div>
                                                <div id="divGrade" class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtGrade" CssClass="col-md-3 control-label">Grade</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtGrade" CssClass="form-control small-txtbx-width" OnTextChanged="txtGrade_TextChanged" AutoPostBack="true" />
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGrade" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Grade is required" ValidationGroup="Required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblDistrict" CssClass="col-md-3 control-label float-left">District</asp:Label>
                                                    <div class="col-md-6 label-top-margin">
                                                        <asp:Label runat="server" ID="lblDistrict" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblTeacher" CssClass="col-md-3 control-label float-left">Teacher</asp:Label>
                                                    <div class="col-md-6 label-top-margin">
                                                        <asp:Label runat="server" ID="lblTeacher" />
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <%-- End Student Information --%>
                                    <%-- Audition Information --%>
                                    <hr />
                                    <asp:UpdatePanel ID="upAuditionInfo" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <h4>Audition Information</h4>
                                                <asp:Panel ID="pnlChooseAudition" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="cboAudition" CssClass="col-md-3 control-label float-left">Select Audition</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropdown-list form-control" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlDataSource1" DataTextField="DropDownInfo" DataValueField="AuditionId" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True" Text="" Value="" />
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div>
                                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="cboAudition" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition is required" ValidationGroup="RequiredForEditOrDelete" />
                                                        </div>
                                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictAuditionOptions" SelectCommandType="StoredProcedure">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="txtStudentId" Name="studentId" PropertyName="Text" Type="Int32" />
                                                            </SelectParameters>
                                                        </asp:SqlDataSource>
                                                    </div>
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlSite" CssClass="col-md-3 control-label float-left">Audition Site</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlSite" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="WmtaDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT GeoName, GeoId FROM ConfigDistrict WHERE (AuditionLevel = @AuditionLevel) ORDER BY GeoName">
                                                            <SelectParameters>
                                                                <asp:Parameter DefaultValue="District" Name="AuditionLevel" Type="String" />
                                                            </SelectParameters>
                                                        </asp:SqlDataSource>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlSite" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition Site is required" ValidationGroup="Required" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="lblAuditionDate" CssClass="col-md-3 control-label float-left">Audition Date</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:Label runat="server" ID="lblAuditionDate" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlInstrument" CssClass="col-md-3 control-label float-left">Instrument</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlInstrument" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource1" DataTextField="Instrument" DataValueField="Instrument" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlInstrument" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Instrument is required" ValidationGroup="Required" />
                                                    </div>
                                                    <asp:SqlDataSource ID="WmtaDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Instrument] FROM [ConfigInstrument] ORDER BY [Instrument]"></asp:SqlDataSource>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtAccompanist" CssClass="col-md-3 control-label float-left">Accompanist</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtAccompanist" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlAuditionType" CssClass="col-md-3 control-label float-left">Audition Type</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlAuditionType_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Solo" Value="Solo"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Duet" Value="Duet"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlAuditionType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition Type is required" ValidationGroup="Required" />
                                                    </div>
                                                    <asp:Label ID="lblDuetPartner" runat="server" CssClass="instruction-label" Visible="false" />
                                                    <asp:LinkButton ID="lnkChangePartner" runat="server" Font-Size="Smaller" OnClick="lnkChangePartner_Click" Visible="false">Edit Partner</asp:LinkButton>
                                                </div>
                                                <asp:Panel ID="pnlDuetPartner" runat="server" Visible="false">
                                                    <hr />
                                                    <h5>Choose a Duet Partner</h5>
                                                    <label class="instruction-label-centered">
                                                        An audition will be automatically generated for the selected student.
                                                    </label>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtPartnerId" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtPartnerId" CssClass="form-control" />
                                                        </div>
                                                        <asp:Button ID="btnPartnerSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnPartnerSearch_Click" />
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtPartnerFirstName" CssClass="col-md-3 control-label">First Name</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtPartnerFirstName" CssClass="form-control" />
                                                        </div>
                                                        <asp:Button ID="btnPartnerClear" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnPartnerClear_Click" />
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="txtPartnerLastName" CssClass="col-md-3 control-label">Last Name</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtPartnerLastName" CssClass="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:GridView ID="gvDuetPartner" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" PagerStyle-CssClass="bs-pagination"
                                                            OnPageIndexChanging="gvDuetPartner_PageIndexChanging" OnRowDataBound="gvDuetPartner_RowDataBound" OnSelectedIndexChanged="gvDuetPartner_SelectedIndexChanged" />
                                                    </div>
                                                    <hr />
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlAuditionTrack" CssClass="col-md-3 control-label float-left">Audition Track</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlAuditionTrack" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" DataSourceID="WmtaDataSource3" DataTextField="Track" DataValueField="Track" OnSelectedIndexChanged="ddlAuditionTrack_SelectedIndexChanged" AutoPostBack="True">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlAuditionTrack" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition Track is required" ValidationGroup="Required" />
                                                    </div>
                                                    <asp:SqlDataSource ID="WmtaDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTrack" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlTheoryLevel" CssClass="col-md-3 control-label float-left">Theory Level</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlTheoryLevelType" runat="server" CssClass="dropdown-list form-control float-right" Visible="false" Width="65%">
                                                            <asp:ListItem Selected="True" Text="" Value="" />
                                                            <asp:ListItem Text="Keybrd" Value="Keybrd" />
                                                            <asp:ListItem Text="Treble" Value="Treble" />
                                                            <asp:ListItem Text="Alto" Value="Alto" />
                                                            <asp:ListItem Text="Bass" Value="Bass" />
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddlTheoryLevel" runat="server" CssClass="dropdown-list form-control float-left" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlTheoryLevel_SelectedIndexChanged" Width="30%">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTheoryLevel" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Theory Level is required" ValidationGroup="Required" />
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTheoryLevelType" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Theory Type is required" ValidationGroup="TheoryLevelExtra" />
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <hr />
                                    <%-- End Audition Information --%>
                                    <%-- Composition Information --%>
                                    <asp:UpdatePanel ID="upCompositions" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <h4>Compositions To Perform</h4>
                                                <div class="form-group">
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStyle" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Style is required" ValidationGroup="NewComposition" /><br />
                                                    </div>
                                                    <asp:Label runat="server" AssociatedControlID="ddlStyle" CssClass="col-md-3 control-label float-left">Style</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlStyle" runat="server" CssClass="dropdown-list form-control" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="WmtaDataSource2" DataTextField="Style" DataValueField="Style" OnSelectedIndexChanged="cboStyle_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value="" />
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="WmtaDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                                    </div>
                                                    <asp:Button ID="btnClearCompSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearCompSearch_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlComposer" CssClass="col-md-3 control-label float-left">Composer</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" OnSelectedIndexChanged="cboComposer_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <asp:CheckBox ID="chkNewComposer" runat="server" CssClass="checkbox float-left" Text="New" TextAlign="Left" OnCheckedChanged="chkNewComposer_CheckedChanged" AutoPostBack="true" />
                                                    <div>
                                                        <asp:RequiredFieldValidator ID="rfvComposer" runat="server" ControlToValidate="ddlComposer" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composer is required" ValidationGroup="NewComposition" /><br />
                                                    </div>
                                                    <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                                <asp:Panel ID="pnlComposer" runat="server" CssClass="display-none">
                                                    <%--Visible="false"--%>
                                                    <div class="form-group" style="font-size: smaller">
                                                        <asp:Label runat="server" AssociatedControlID="txtComposerLast" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtComposerLast" CssClass="form-control" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group" style="font-size: smaller">
                                                        <asp:Label runat="server" AssociatedControlID="txtComposerFI" CssClass="col-md-3 control-label float-left">F.I.</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtComposerFI" CssClass="form-control small-txtbx-width" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group" style="font-size: smaller">
                                                        <asp:Label runat="server" AssociatedControlID="txtComposerMI" CssClass="col-md-3 control-label float-left">M.I.</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:TextBox runat="server" ID="txtComposerMI" CssClass="form-control small-txtbx-width" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" CssClass="instruction-label">Title, Key, Movement, Catalog No</asp:Label><br />
                                                    <asp:Label runat="server" AssociatedControlID="ddlComposition" CssClass="col-md-3 control-label float-left">Title</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlComposition" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlComposition_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                    </div>
                                                    <asp:TextBox runat="server" ID="txtComposition" CssClass="form-control display-none" />
                                                    <%--Visible="false"--%>
                                                    <asp:CheckBox ID="chkNewTitle" runat="server" CssClass="checkbox float-left" Text="New" TextAlign="Left" OnCheckedChanged="chkNewTitle_CheckedChanged" AutoPostBack="true" />
                                                    <div>
                                                        <asp:RequiredFieldValidator ID="rfvComposition" runat="server" ControlToValidate="ddlComposition" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composition is required" ValidationGroup="NewComposition" /><br />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtCompLevel" CssClass="col-md-3 control-label float-left">Level</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtCompLevel" CssClass="form-control small-txtbx-width" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" CssClass="col-md-3 control-label float-left" Font-Bold="true">Playing Time</asp:Label>
                                                </div>
                                                <div class="form-group">
                                                    <div>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMinutes" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Time is required" ValidationGroup="Composition" /><br />
                                                    </div>
                                                    <asp:Label runat="server" AssociatedControlID="txtMinutes" CssClass="col-md-3 control-label smaller-font">Minutes</asp:Label>
                                                    <div class="col-md-2">
                                                        <asp:TextBox runat="server" ID="txtMinutes" CssClass="form-control small-txtbx-width float-left" TextMode="Number" />
                                                    </div>
                                                    <asp:Label runat="server" AssociatedControlID="ddlSeconds" CssClass="col-md-2 control-label float-left smaller-font">Seconds</asp:Label>
                                                    <div class="col-md-2">
                                                        <asp:DropDownList ID="ddlSeconds" runat="server" CssClass="dropdown-list form-control float-left" Width="70px">
                                                            <asp:ListItem Selected="True" Text="0" Value="0" />
                                                            <asp:ListItem Text="15" Value="0.25" />
                                                            <asp:ListItem Text="30" Value="0.5" />
                                                            <asp:ListItem Text="45" Value="0.75" />
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Button ID="btnAddComposition" runat="server" Text="Add" CssClass="btn btn-default btn-min-width-72" OnClick="btnAddComposition_Click" />
                                                    <asp:Button ID="btnRemoveComposition" runat="server" Text="Remove" CssClass="btn btn-default btn-min-width-72" OnClick="btnRemoveComposition_Click" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Table ID="tblCompositions" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
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
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <hr />
                                    <%-- End Composition Information --%>
                                    <%-- Time Constraints --%>
                                    <asp:UpdatePanel ID="upTimeConstraints" runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <h4>Time Constraints</h4>
                                                <div class="form-group">
                                                    <div class="col-lg-10">
                                                        <asp:RadioButtonList ID="rblTimePreference" runat="server" CssClass="radio" RepeatLayout="Flow" OnSelectedIndexChanged="rblTimePreference_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True">No Preference</asp:ListItem>
                                                            <asp:ListItem>Preferred Time</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <asp:Panel ID="pnlPreferredTime" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <label class="col-lg-2 control-label">Preference</label>
                                                        <div class="col-lg-10">
                                                            <div class="radio">
                                                                <label>
                                                                    <input type="radio" name="timePrefOptions" id="opAM" runat="server" value="A.M." />A.M.
                                                                </label>
                                                            </div>
                                                            <div class="radio">
                                                                <label>
                                                                    <input type="radio" name="timePrefOptions" id="opPM" runat="server" value="P.M." />P.M.
                                                                </label>
                                                            </div>
                                                            <div class="radio">
                                                                <label>
                                                                    <input type="radio" name="timePrefOptions" id="opEarly" runat="server" value="Earliest" />Earliest
                                                                </label>
                                                            </div>
                                                            <div class="radio">
                                                                <label>
                                                                    <input type="radio" name="timePrefOptions" id="opLate" runat="server" value="Latset" />Latest
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <hr />
                                                <asp:Panel ID="pnlCoordinateParticipants" runat="server" Visible="false">
                                                    <h4>Coordinating Students</h4>
                                                        <div class="form-group">
                                                            <asp:Table ID="tblCoordinates" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
                                                                <asp:TableHeaderRow ID="TableHeaderRow2" runat="server" BorderStyle="Solid">
                                                                    <asp:TableHeaderCell Scope="Column" Text="Id" />
                                                                    <asp:TableHeaderCell Scope="Column" Text="First Name" />
                                                                    <asp:TableHeaderCell Scope="Column" Text="Last Name" />
                                                                    <asp:TableHeaderCell Scope="Column" Text="Reason" />
                                                                </asp:TableHeaderRow>
                                                            </asp:Table>
                                                        </div>
                                                </asp:Panel>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <%-- End Time Constraints --%>
                                </asp:Panel>
                                <hr />
                                <asp:Panel runat="server" ID="pnlButtons" Visible="false">
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </fieldset>
                        </div>
                        <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('#searchHint').popover(
            {
                trigger: 'hover',
                html: true,
                placement: 'right',
                content: 'Fill in any number of the search fields and click "Search" to find students. Clicking "Search" without filling in any fields will return all students linked to you. First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all students with last names containing "sch"."',
            });

            $('#MainContent_pnlComposer').hide();
            $('#MainContent_txtComposition').hide();
        });

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

        //show an informational message
        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "info" });
        };
    </script>
</asp:Content>
