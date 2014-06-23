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
                                <legend>District Registration</legend>
                                <%-- Student search --%>
                                <asp:UpdatePanel ID="upStudentSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Student Search</h4>
                                            <br />
                                            <asp:PlaceHolder runat="server" ID="phStudentSearchError" Visible="false">
                                                <p class="text-danger">
                                                    <asp:Literal runat="server" ID="lblStudentSearchError" />
                                                </p>
                                            </asp:PlaceHolder>
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
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtGrade" CssClass="col-md-3 control-label">Grade</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtGrade" CssClass="form-control small-txtbx-width" />
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
                                                    <asp:PlaceHolder runat="server" ID="phAuditionError" Visible="false">
                                                        <p class="text-danger">
                                                            <asp:Literal runat="server" ID="lblAuditionError" Text="This student has no editable auditions for the current year" />
                                                        </p>
                                                    </asp:PlaceHolder>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" AssociatedControlID="cboAudition" CssClass="col-md-3 control-label float-left">Select Audition</asp:Label>
                                                        <div class="col-md-6">
                                                            <asp:DropDownList ID="cboAudition" runat="server" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlDataSource1" DataTextField="DropDownInfo" DataValueField="AuditionId" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged">
                                                                <asp:ListItem Selected="True" Text="" Value="" />
                                                            </asp:DropDownList>
                                                            <%-- add required data validation --%>
                                                        </div>
                                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrictAuditionOptions" SelectCommandType="StoredProcedure">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="txtStudentId" Name="studentId" PropertyName="Text" Type="Int32" />
                                                            </SelectParameters>
                                                        </asp:SqlDataSource>
                                                    </div>
                                                </asp:Panel>
                                                <asp:PlaceHolder runat="server" ID="phAuditionErrors" Visible="false">
                                                    <p class="text-danger">
                                                        <asp:Literal runat="server" ID="lblAuditionErrors" />
                                                    </p>
                                                    <%-- lblAuditionSiteError and lblFreezeDatePassed --%>
                                                </asp:PlaceHolder>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlSite" CssClass="col-md-3 control-label float-left">Audition Site</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlSite" runat="server" DataSourceID="WmtaDataSource" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlSite_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <%-- required field validator --%>
                                                        <asp:SqlDataSource ID="WmtaDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT GeoName, GeoId FROM ConfigDistrict WHERE (AuditionLevel = @AuditionLevel) ORDER BY GeoName">
                                                            <SelectParameters>
                                                                <asp:Parameter DefaultValue="District" Name="AuditionLevel" Type="String" />
                                                            </SelectParameters>
                                                        </asp:SqlDataSource>
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
                                                    <asp:DropDownList ID="ddlInstrument" runat="server" DataSourceID="WmtaDataSource1" DataTextField="Instrument" DataValueField="Instrument" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlInstrument_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="WmtaDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Instrument] FROM [ConfigInstrument] ORDER BY [Instrument]"></asp:SqlDataSource>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtAccompanist" CssClass="col-md-3 control-label float-left">Accompanist</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtAccompanist" CssClass="form-control" />
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlAuditionType" CssClass="col-md-3 control-label float-left">Audtion Type</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlAuditionType" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlAuditionType_SelectedIndexChanged">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Solo" Value="Solo"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Duet" Value="Duet"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <%-- required field validator --%>
                                                    <asp:Label ID="lblDuetPartner" runat="server" CssClass="instruction-label" Visible="false" />
                                                    <asp:LinkButton ID="lnkChangePartner" runat="server" Font-Size="Smaller" OnClick="lnkChangePartner_Click" Visible="false">Edit Partner</asp:LinkButton>
                                                </div>
                                                <asp:Panel ID="pnlDuetPartner" runat="server" Visible="false">
                                                    <h5>Choose a Duet Partner</h5>
                                                    <label class="instruction-label-smaller-margin text-align-center" style="margin-left: 4.5em">
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
                                                </asp:Panel>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlAuditionTrack" CssClass="col-md-3 control-label float-left">Audtion Track</asp:Label>
                                                    <asp:DropDownList ID="ddlAuditionTrack" runat="server" AppendDataBoundItems="true" DataSourceID="WmtaDataSource3" DataTextField="Track" DataValueField="Track" OnSelectedIndexChanged="ddlAuditionTrack_SelectedIndexChanged" AutoPostBack="True">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <%-- required field validation --%>
                                                    <asp:SqlDataSource ID="WmtaDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownTrack" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlTheoryLevel" CssClass="col-md-3 control-label float-left">Theory Level</asp:Label>
                                                    <asp:DropDownList ID="ddlTheoryLevel" runat="server" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlTheoryLevel_SelectedIndexChanged" Width="70px">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="ddlTheoryLevelType" runat="server" Visible="false" Width="80px">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                        <asp:ListItem Text="Keybrd" Value="Keybrd" />
                                                        <asp:ListItem Text="Treble" Value="Treble" />
                                                        <asp:ListItem Text="Alto" Value="Alto" />
                                                        <asp:ListItem Text="Bass" Value="Bass" />
                                                    </asp:DropDownList>
                                                    <%-- theory level required field validator and lblTheoryLvl --%>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <hr />
                                    <%-- End Audition Information --%>


                                </asp:Panel>
                            </fieldset>
                        </div>
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
                content: 'Fill in one or more of the search fields and click "Search" to find students.  First and last names do not need to be complete in order to search.  Ex: entering "sch" in the Last Name field would find all students with last names containing "sch"."',
            });
        });
    </script>
</asp:Content>
