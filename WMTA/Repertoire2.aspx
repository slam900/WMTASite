<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Repertoire2.aspx.cs" Inherits="WMTA.Repertoire2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="Styles/ControlsStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cpMainContent" runat="Server">
    <asp:ScriptManager ID="scriptManager" runat="server" />
    <h1>Repertoire</h1>
    <asp:UpdatePanel ID="upFullPage" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnlFullPage" runat="server" Visible="false">
                <asp:UpdatePanel ID="upSearch" runat="server" Visible="false">
                    <ContentTemplate>
                        <fieldset>
                            <legend>Select Composition</legend>
                            <asp:Label ID="lblSearchError" runat="server" Text="Please select a composition" CssClass="labelError" Visible="false"></asp:Label>
                            <p>
                                <label for="Style" style="font-weight: bold">Style</label>
                                <asp:DropDownList ID="ddlStyleSearch" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource2" DataTextField="Style" DataValueField="Style" AppendDataBoundItems="true" OnSelectedIndexChanged="cboStyle_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                <asp:Button ID="btnClearCompSearch" runat="server" Text="Clear" CssClass="button" OnClick="btnClearCompSearch_Click" />
                            </p>
                            <p>
                                <label for="Level">Level</label>
                                <asp:DropDownList ID="ddlCompLevelSearch" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource7" DataTextField="Description" DataValueField="CompLevelId" AppendDataBoundItems="true" OnSelectedIndexChanged="cboCompLevel_SelectedIndexChanged" AutoPostBack="True">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                            </p>
                            <p>
                                <label for="Composer">Composer</label>
                                <asp:DropDownList ID="ddlComposerSearch" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlComposerSearch_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                            </p>
                            <p>
                                <label for="Composition">Title</label>
                                <asp:DropDownList ID="ddlComposition" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlComposition_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                <label id="txtCompositionId" runat="server" visible="false" />
                            </p>
                        </fieldset>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <p></p>
                <asp:UpdatePanel ID="upCompositionInfo" runat="server">
                    <ContentTemplate>
                         <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                             <asp:Panel ID="pnlTitleEdit" runat="server" Visible="true" >
                            <label for="CompositionInstructions" class="labelInstruction">Title, Key, Movement, Tempo, Catalog No.</label><br />
                            <label for="ExistingComposition" style="font-weight: bold">Title</label><asp:TextBox ID="txtComposition" runat="server" AutoPostBack="True" OnTextChanged="txtComposition_TextChanged"></asp:TextBox>
                            <asp:Label ID="lblCompositionError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label><br />
                            </asp:Panel>
                             <asp:Panel ID="pnlTitleNew" runat="server" Visible="false" >
                                 <label for="CompositionInstructions" class="labelInstruction">The Title is required. Please include all known information, especially for Masterworks.</label>
                                 <label for="NewComposition" style="font-weight: bold">Title</label><asp:TextBox ID="txtTitleNew" runat="server" AutoPostBack="true" OnTextChanged="txtTitleNew_TextChanged"></asp:TextBox>
                                 <asp:Label ID="lblTitleNewError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label><br />
                                 <label style="font-weight: bold">Key</label>
                                 <asp:DropDownList ID="ddlKeyLetter" runat="server" CssClass="dropDownList" Width="40">
                                     <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="A" Value="A"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="B" Value="B"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="C" Value="C"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="D" Value="D"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="E" Value="E"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="F" Value="F"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="G" Value="G"></asp:ListItem>
                                 </asp:DropDownList>
                                 <asp:DropDownList ID="ddlKeyFS" runat="server" CssClass="dropDownList" Width="60">
                                     <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Flat" Value="Flat"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Sharp" Value="Sharp"></asp:ListItem>
                                 </asp:DropDownList>
                                 <asp:DropDownList ID="ddlKeyMM" runat="server" CssClass="dropDownList" Width="60">
                                     <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Major" Value="Major"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Minor" Value="Minor"></asp:ListItem>
                                 </asp:DropDownList><br />
                                 <label style="font-weight: bold">Movement</label><asp:TextBox ID="txtMvmt" runat="server"></asp:TextBox><br />
                                 <label style="font-weight: bold">Tempo</label><asp:TextBox ID="txtTempo" runat="server"></asp:TextBox><br />
                                 <label style="font-weight: bold">Catalog No.</label>
                                 <asp:DropDownList ID="ddlPrefix" runat="server" CssClass="dropDownList" Width="60" Enabled="False">
                                     <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="BWV" Value="BWV"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Opus/No." Value="Opus/No."></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="BuxWV" Value="BuxWV"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="Hob" Value="Hob"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="K" Value="K"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="L" Value="L"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="S" Value="S"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="P" Value="P"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="D" Value="D"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="KV" Value="KV"></asp:ListItem>
                                     <asp:ListItem Selected="False" Text="WoO" Value="WoO"></asp:ListItem>
                                 </asp:DropDownList>
                                 <asp:TextBox ID="txtCatalogNo" runat="server" OnTextChanged="txtCatalogNo_TextChanged" Width="87px" AutoPostBack="true"></asp:TextBox>
                             </asp:Panel>
                            <p>
                                <label for="Composer">Composer</label>
                                <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropDownList" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" OnSelectedIndexChanged="cboComposer_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                <asp:Label ID="lblComposerError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label>
                                <asp:Label ID="lblNewComposerError" runat="server" Text="*" CssClass="labelStarError" Visible="false" ForeColor="Red"></asp:Label>
                                &nbsp; New<asp:CheckBox ID="chkNewComposer" runat="server" AutoPostBack="true" OnCheckedChanged="chkNewComposer_CheckedChanged" />
                                <asp:Panel ID="pnlComposer" runat="server" Visible="false">
                                    <label for="Composer" class="labelSmallInstruction">Last Name</label><asp:TextBox ID="txtComposerLast" runat="server" Width="150px" CssClass="input" OnTextChanged="txtComposerLast_TextChanged"></asp:TextBox><br />
                                    <label for="Composer" class="labelSmallInstruction">F.I.</label><asp:TextBox ID="txtComposerFI" runat="server" CssClass="input" Width="30px" MaxLength="1"></asp:TextBox><br />
                                    <label for="Composer" class="labelSmallInstruction">M.I.</label><asp:TextBox ID="txtComposerMI" runat="server" CssClass="input" Width="30px" MaxLength="1"></asp:TextBox>
                                </asp:Panel>
                                <p>
                                </p>
                                <p>
                                </p>
                                <label for="CompositionTime" style="font-weight: bold">
                                    Playing Time</label><br />
                                <div style="margin-left: 1.5em">
                                    <asp:Label ID="lblMinutesErrorMsg" runat="server" CssClass="labelError" Text="Minutes must be a positive integer" Visible="false"></asp:Label>
                                    <label for="Minutes" style="font-weight: bold; font-size: 14px; vertical-align: bottom">
                                        Minutes</label><asp:TextBox ID="txtMinutes" runat="server" AutoPostBack="True" CssClass="textBox2" OnTextChanged="txtMinutes_TextChanged" TextMode="Number"></asp:TextBox>
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
                                <p>
                                </p>
                                <label for="Style" style="font-weight: bold">
                                    Style</label>
                                <asp:DropDownList ID="ddlStyle" runat="server" AppendDataBoundItems="true" AutoPostBack="True" CssClass="dropDownList" DataSourceID="WmtaDataSource2" DataTextField="Style" DataValueField="Style" OnSelectedIndexChanged="ddlStyle_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="lblStyleError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                <asp:SqlDataSource ID="WmtaDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                <p>
                                </p>
                                <label for="Level" style="font-weight: bold">
                                    Level</label>
                                <asp:DropDownList ID="ddlCompLevel" runat="server" AppendDataBoundItems="true" AutoPostBack="True" CssClass="dropDownList" DataSourceID="WmtaDataSource7" DataTextField="Description" DataValueField="CompLevelId" OnSelectedIndexChanged="ddlCompLevel_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="lblCompLevelError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                <asp:SqlDataSource ID="WmtaDataSource7" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                <p>
                                    <asp:CheckBox ID="chkConfirmMeasures" runat="server" TextAlign="Left" AutoPostBack="true" OnCheckedChanged="chkConfirmMeasures_CheckedChanged"/>
                                    <label style="width:250px">16 measures, not including repeats?</label>
                                    <asp:Label ID="lblMeasuresError" runat="server" CssClass="labelStarError" ForeColor="Red" Text="*" Visible="false"></asp:Label>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                            </p>
                        </div>
                        <p>
                        </p>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="upButtons" runat="server">
                    <ContentTemplate>
                        <p>
                             <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                                <asp:Label ID="lblErrorMsg" runat="server" Text="**Errors on page**" CssClass="labelMainError" Width="350px" Visible="false"></asp:Label><br />
                            </div>
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
            <asp:Panel ID="pnlSuccess" runat="server">
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto; text-align:center">
                    <asp:Label ID="lblSuccess" runat="server" Text="The composition was successfully created." CssClass="labelSuccess" Width="300px" Visible="false"></asp:Label><br />
                </div>
                 <div style="width: 23.8em; margin-left: auto; margin-right: auto">
                    <label for="UserOption" style="font-weight: bold">Options</label>
                    <asp:DropDownList ID="ddlUserOptions" runat="server" CssClass="dropDownList">
                        <asp:ListItem Selected="True" Text="Create Composition" Value="Create New"></asp:ListItem>
                        <asp:ListItem Selected="False" Text="Edit Composition" Value="Edit Existing"></asp:ListItem>
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
