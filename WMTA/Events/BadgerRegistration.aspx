<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="BadgerRegistration.aspx.cs" Inherits="WMTA.Events.BadgerRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="registrationForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Add Badger Registration</legend>
                                <asp:UpdatePanel ID="upStudentSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Student Search</h4>
                                            <br />
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="txtStudentId" runat="server" CssClass="col-md-3 control-label float-left">Student Id</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtStudentId" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                                <asp:Button ID="btnStudentSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-min-width-72" OnClick="btnStudentSearch_Click" CausesValidation="false" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="txtFirstName" runat="server" CssClass="col-md-3 control-label float-left">First Name</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                                <asp:Button ID="btnClearStudentSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" OnClick="btnClearStudentSearch_Click" CausesValidation="false" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label AssociatedControlID="txtLastName" runat="server" CssClass="col-md-3 control-label float-left">Last Name</asp:Label>
                                            <div class="col-md-6">
                                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:GridView ID="gvStudentSearch" runat="server" CssClass="td table table-hover table-striped smaller-font width-80 center" AllowPaging="true" AutoGenerateSelectButton="true" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Panel runat="server" ID="pnlFullPage" Visible="false">
                                    <asp:Panel ID="pnlInfo" runat="server" Visible="false">
                                        <div>
                                            <h4>Student Information</h4>
                                            <br />
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="lblStudentId" runat="server" CssClass="col-md-3 control-label float-left">Student Id:</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label ID="lblStudentId" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="lblName" runat="server" CssClass="col-md-3 control-label float-left">Name:</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label ID="lblName" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="lblGrade" runat="server" CssClass="col-md-3 control-label float-left">Grade:</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label ID="lblGrade" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="lblDistrict" CssClass="col-md-3 control-label float-left">District:</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label ID="lblDistrict" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="lblTeacher" CssClass="col-md-3 control-label float-left">Teacher</asp:Label>
                                                <div class="col-md-6 label-top-margin">
                                                    <asp:Label ID="lblTeacher" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <hr />
                                            <h4>Competition Information</h4>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="cboAudition" runat="server" CssClass="col-md-3 control-label float-left">Choose Audition</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropdown-list form-control" AutoPostBack="true" AppendDataBoundItems="true" DataSourceID="SqlDataSource1" DataTextField="DropDownInfo" DataValueField="AuditionId" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="cboAudition" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition is required" />
                                                </div>
                                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownStateCompOptions" SelectCommandType="StoredProcedure">
                                                    <SelectParameters>
                                                        <asp:ControlParameter ControlID="lblStudentId" Name="studentId" PropertyName="Text" Type="Int32" />
                                                    </SelectParameters>
                                                </asp:SqlDataSource>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <div class="form-group">
                                                <asp:Label AssociatedControlID="cboSite" runat="server" CssClass="col-md-3 control-label float-left">Regional Site</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="cboSite" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" OnSelectedIndexChanged="cboSite_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="cboSite" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Audition Site is required" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="lblAuditionDate" CssClass="col-md-3 control-label float-left">Audition Date</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:Label runat="server" ID="lblAuditionDate" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="txtDriveTime" CssClass="col-md-3 control-label float-left">Drive Time</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:TextBox ID="txtDriveTime" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDriveTime" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Drive Time is required" />
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <%-- Start Time Constraints --%>
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <hr />
                                            <div>
                                                <h4>Time Constraints</h4>
                                                <div class="form-group">
                                                    <%-- lblTimePrefError goes here --%>
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
                                                                    <input type="radio" name="timePrefOptions" id="opLate" runat="server" value="Latest" />Latest
                                                                </label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                                <asp:Panel ID="pnlCoordinateParticipants" runat="server" Visible="false">
                                                    <h4>Coordinating Students</h4>
                                                    <div class="form-group">
                                                        <asp:Table ID="tblCoordinates" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
                                                            <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
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
                                    <asp:UpdatePanel runat="server">
                                        <ContentTemplate>
                                            <div>
                                                <hr />
                                                <asp:Label AssociatedControlID="chkAdditionalInfo" runat="server" CssClass="col-md-5 control-label float-left">View Additional Information</asp:Label>
                                                <asp:CheckBox ID="chkAdditionalInfo" runat="server" CssClass="checkbox float-left" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="chkAdditionalInfo_CheckedChanged" />
                                            </div>
                                            <br />
                                            <asp:Panel ID="pnlAdditionalInfo" runat="server" Visible="false">
                                                <hr />
                                                <div>
                                                    <h4>Audition Information</h4>
                                                    <div class="form-group">
                                                        <asp:Label AssociatedControlID="lblInstrument" runat="server" CssClass="col-md-3 control-label float-left">Instrument:</asp:Label>
                                                        <div class="col-md-6 label-top-margin">
                                                            <asp:Label ID="lblInstrument" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label AssociatedControlID="lblAccompanist" runat="server" CssClass="col-md-3 control-label float-left">Accompanist:</asp:Label>
                                                        <div class="col-md-6 label-top-margin">
                                                            <asp:Label ID="lblAccompanist" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:Label AssociatedControlID="lblAuditionType" runat="server" CssClass="col-md-3 control-label float-left">Audition Type</asp:Label>
                                                        <div class="col-md-6 label-top-margin">
                                                            <asp:Label ID="lblAuditionType" runat="server" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <hr />
                                                <div>
                                                    <h4>Compositions To Perform</h4>
                                                    <div class="form-group">
                                                        <asp:Table ID="tblCompositions" runat="server" CssClass="table table-striped table-bordered table-hover text-align-center">
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
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:UpdatePanel ID="UPdatePanel8" runat="server">
                                        <ContentTemplate>
                                            <div class="form-group">
                                                <div class="col-lg-10 col-lg-offset-2 float-right">
                                                    <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
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

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };
    </script>
</asp:Content>
