<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="DistrictPointEntry.aspx.cs" Inherits="WMTA.DistrictPointEntry" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <h1>District Point Entry</h1>
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
                <asp:Panel ID="pnlInfo" runat="server">
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <p>
                                <label id="lblAuditionError" runat="server" class="labelError" visible="false">This student has no eligible auditions</label><br /><br />
                                <label id="lblStudentError" runat="server" class="labelError" visible="false">Please select a student</label>
                                <label id="StudentInfo">Student</label>
                                <asp:Label ID="lblStudent" runat="server" style="font-size:11px"></asp:Label>
                                <label id="lblStudId" runat="server" visible="false"></label>
                            </p>
                            <br />
                            <p>
                                <label for="AuditionYear">Audition Year</label>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="dropDownList" AutoPostBack="True" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" />
                                <br />
                                <br />
                                <label id="lblChooseAudition" runat="server" class="labelError" visible="false">Please select an audition</label>
                                <label for="ChooseAudition">Choose Audition</label>
                                <asp:DropDownList ID="cboAudition" runat="server" CssClass="dropDownList" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="cboAudition_SelectedIndexChanged" />
                                <label id="lblDuetPartnerInfo" runat="server" class="labelInstruction" visible="false">The composition points of the student's duet partner will also be updated.</label><br />
                            </p><br />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Button ID="btnUpdatePoints" runat="server" Text="Update Total" Font-Bold="True" Font-Size="Small" OnClick="btnUpdatePoints_Click" Width="84px" />
                            <label id="lblPointError" runat="server" class="labelError" visible="false">Point values must be in the range from 0-5</label>
                            <asp:Table ID="tblCompositions" runat="server">
                                <asp:TableHeaderRow ID="TableHeaderRow1" runat="server" BorderStyle="Solid">
                                    <asp:TableHeaderCell Scope="Column" Text="CompId" Visible="false" />
                                    <asp:TableHeaderCell Scope="Column" Text="Composition" />
                                    <asp:TableHeaderCell Scope="Column" Text="Points" />
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <p>
                                <label id="lblTheoryPointsError" runat="server" class="labelError" visible="false">Theory test points must be in the range from 0-5</label>
                                <label id="lblTheoryPoints">Theory Points</label>
                                <asp:TextBox ID="txtTheoryPoints" runat="server" TextMode="Number" OnTextChanged="txtTheoryPoints_TextChanged" AutoPostBack="True"></asp:TextBox>
                            </p>
                            <p>
                                <label for="PointTotal">Point Total</label>
                                <label for="Points" id="lblPoints" runat="server" style="font-weight: normal; text-align: left">0</label>
                            </p>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </div><br />
                    </asp:Panel>
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
                 <div style="width: 14em; margin-left: auto; margin-right: auto">
                    <asp:Button ID="btnNew" runat="server" Text="Enter Points" CssClass="buttonLong" Font-Bold="true" OnClick="btnNew_Click" />
                    <asp:Button ID="btnBackOption" runat="server" Text="Back" CssClass="buttonLong" Font-Bold="true" Width="100px" OnClick="btnBackOption_Click" />
                </div>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
