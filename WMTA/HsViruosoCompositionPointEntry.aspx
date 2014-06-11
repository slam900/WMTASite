<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="HsViruosoCompositionPointEntry.aspx.cs" Inherits="WMTA.HsViruosoCompositionPointEntry" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <h1>HS Virtuoso & Composition Point Entry</h1>
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
                            <asp:GridView ID="gvStudentSearch" runat="server" CssClass="gridview" Font-Size="14px" AllowPaging="True" AutoGenerateSelectButton="True" OnPageIndexChanging="gvStudentSearch_PageIndexChanging" OnRowDataBound="gvStudentSearch_RowDataBound" OnSelectedIndexChanged="gvStudentSearch_SelectedIndexChanged"></asp:GridView>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <p>
                </p>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                         <div style="width: 23.8em; margin-left: auto; margin-right: auto;">
                            <p>
                                <label id="lblStudentError" runat="server" class="labelError" visible="false">Please select a student</label>
                                <label id="StudentInfo">Student</label>
                                <label id="lblStudent" runat="server" style="font-size:11px"></label>
                                <label id="lblStudId" runat="server" visible="false"></label>
                            </p><br />
                            <p>
                                <br />
                                <label for="AuditionYear">Competition Year</label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropDownList" AutoPostBack="True" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" />
                                <label id="lblAuditionYearError" runat="server" class="labelStarError" visible="false">*</label>
                                <br />
                                <br />
                                <label for="ChooseAudition">Competition Type</label>
                                <asp:DropDownList ID="ddlAuditionType" runat="server" CssClass="dropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlAuditionType_SelectedIndexChanged" >
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Selected="False" Text="HS Virtuoso" Value="HS Virtuoso"></asp:ListItem>
                                    <asp:ListItem Selected="False" Text="Composition" Value="Composition"></asp:ListItem>
                                </asp:DropDownList>
                                <label id="lblChooseAudition" runat="server" class="labelStarError" visible="false">*</label>
                            </p>
                            <p>
                                <label id="lblAttendanceError" runat="server" class="labelError" style="width: 90%; margin-left: 5%; margin-right: 5%" visible="false">If the student earned a Room Award please indicate that they attended the audition</label>
                                <div style="font-weight:bold">
                                <asp:RadioButtonList ID="rblAttendance" runat="server" Width="302px" CssClass="radioWithProperWrap" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblAttendance_SelectedIndexChanged">
                                    <asp:ListItem Selected="True">Participated</asp:ListItem>
                                    <asp:ListItem>No Show</asp:ListItem>
                                </asp:RadioButtonList>
                                </div>
                            </p>
                            <p>
                                <label for="RoomAward">Award</label>
                                <asp:DropDownList ID="ddlRoomAward" runat="server" CssClass="dropDownList" AutoPostBack="true" OnSelectedIndexChanged="ddlRoomAward_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Text="None" Value="0"></asp:ListItem>
                                    <asp:ListItem Selected="False" Text="Honorable Mention" Value="3"></asp:ListItem>
                                    <asp:ListItem Selected="False" Text="Runner Up" Value="4"></asp:ListItem>
                                    <asp:ListItem Selected="False" Text="Room Winner" Value="5"></asp:ListItem>
                                </asp:DropDownList>
                            </p>
                            <p>
                                <label for="PointTotal">Point Total</label>
                                <label for="Points" id="lblPoints" runat="server" style="font-weight: normal; text-align: left">10</label><br />
                            </p>
                            </div>
                    </ContentTemplate>
                </asp:UpdatePanel><p></p>
                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                    <ContentTemplate>
                        <p>
                            <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Visible="false"></asp:Label>
                            <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="button" OnClick="btnBack_Click" />
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" OnClick="btnSubmit_Click" />
                        </p>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSuccess" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The student's points were successfully entered." Width="300px" CssClass="labelSuccess"></asp:Label><br />
                    <br />
                </div>
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto;">
                    <asp:Button ID="btnNew" runat="server" Text="Enter Points" CssClass="buttonLong" Font-Bold="true" OnClick="btnNew_Click" />
                    <asp:Button ID="btnBackOption" runat="server" Text="Back" CssClass="buttonLong" Font-Bold="true" Width="100px" OnClick="btnBackOption_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

