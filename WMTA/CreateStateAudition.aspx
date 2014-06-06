<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateStateAudition.aspx.cs" Inherits="WMTA.CreateStateAudition" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/ControlsStyle.css" rel="stylesheet" type="text/css" />
    <link href="Styles/DatePicker.css" rel="stylesheet" />
    <link href="Styles/jquery.timepicker.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/jquery.timepicker.js"></script>
    <script>
        function pageLoad() {
            $('#cpMainContent_txtDate').datepicker();
            $('#cpMainContent_txtFreezeDate').datepicker();

            $(".ui-timepicker").timepicker();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <h1 id="header" runat="server">Manage Badger Competition</h1>
    <asp:UpdatePanel ID="upAuditionSearch" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlAuditionSearch" runat="server" Visible="false">
                <fieldset>
                    <legend>Audition Search</legend>
                    <p>
                        <label for="District">Region</label>
                        <asp:DropDownList ID="ddlDistrictSearch" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <label id="Label1" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownDistrict" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                        <asp:Button ID="btnAuditionSearch" runat="server" Text="Search" CssClass="button" OnClick="btnAuditionSearch_Click" />
                    </p>
                    <p>
                        <label for="Year">Audition Year</label>
                        <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropDownList" />
                        <asp:Button ID="btnClearAuditionSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearAuditionSearch_Click" />
                    </p>
                    <asp:GridView ID="gvAuditionSearch" runat="server" CssClass="gridview" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvAuditionSearch_PageIndexChanging" OnRowDataBound="gvAuditionSearch_RowDataBound" OnSelectedIndexChanged="gvAuditionSearch_SelectedIndexChanged"></asp:GridView>
                </fieldset>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upMain" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlMain" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:TextBox ID="txtIdHidden" runat="server" Visible="false"></asp:TextBox>
                    <p>
                        <label for="District" style="width: 8em">Region</label>
                        <asp:DropDownList ID="ddlDistrict" runat="server" CssClass="dropDownList" DataSourceID="SqlDataSource1" DataTextField="GeoName" DataValueField="GeoId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlDistrict_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <label id="lblDistrictError" runat="server" class="labelStarError" visible="false">*</label>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownStateDistricts" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                    </p>
                    <p>
                        <label for="Venue" style="width: 8em">Venue</label>
                        <asp:TextBox ID="txtVenue" runat="server" OnTextChanged="txtVenue_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <label id="lblVenueError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="Duets" style="width: 8em">Duets</label>
                        <asp:DropDownList ID="ddlDuets" runat="server" CssClass="dropDownList" >
                            <asp:ListItem Selected="True" Text="No" Value="No"></asp:ListItem>
                            <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                        </asp:DropDownList>
                    </p>
                    <p>
                        <label for="NumJudges" style="width: 8em">Number of Judges</label>
                        <asp:TextBox ID="txtNumJudges" runat="server" TextMode="Number" Width="90px" OnTextChanged="txtNumJudges_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <label id="lblNumJudgesError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="Chairperson" style="width: 8em">Chairperson</label>
                        <asp:DropDownList ID="ddlChairPerson" runat="server" CssClass="dropDownList" AppendDataBoundItems="true" DataSourceID="SqlDataSource2" DataTextField="ComboName" DataValueField="ContactId" OnSelectedIndexChanged="ddlChairPerson_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownChairperson" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                        <label id="lblChairpersonError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="Date" style="width: 8em">Audition Date</label>
                        <input runat="server" type="text" id="txtDate" class="ui-datepicker" style="width: 160px" />
                        <label id="lblDateError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label id="lblTimeError" runat="server" class="labelExtendedInstruction" style="color: red; text-align:center" visible="false">The Start Time must be before the End Time</label><br />
                        <label for="StartTime" style="width: 8em">Start Time</label>
                        <input id="txtStartTime" runat="server" type="text" class="ui-timepicker" style="width: 160px" />
                        <label id="lblStartTimeError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label for="EndTime" style="width: 8em">End Time</label>
                        <input id="txtEndTime" runat="server" type="text" class="ui-timepicker" style="width: 160px" />
                        <label id="lblEndTimeError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                    <p>
                        <label id="lblFreezeDateError2" runat="server" class="labelExtendedInstruction" style="color: red; text-align:center" visible="false">The Freeze Date must be before the Audition Date</label><br />
                        <label for="FreezeDate" style="width: 8em">Freeze Date</label>
                        <input runat="server" type="text" id="txtFreezeDate" class="ui-datepicker" style="width: 160px" />
                        <label id="lblFreezeDateError" runat="server" class="labelStarError" visible="false">*</label>
                    </p>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upButtons" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlButtons" runat="server" Visible="false">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Width="350px" Visible="false"></asp:Label><br />
                </div>
                <p>
                    <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" OnClick="btnSubmit_Click" />
                </p>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="true">
                <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The audition was successfully created" CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                    <br />
                </div>
                <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create New" Value="Create New"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Edit Existing" Value="Edit Existing"></asp:ListItem>
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
