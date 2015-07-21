<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageRepertoire.aspx.cs" Inherits="WMTA.CompositionTools.ManageRepertoire" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="well bs-component col-md-6 main-div center">
            <section id="repertoireForm">
                <asp:UpdatePanel ID="upFullPage" runat="server">
                    <ContentTemplate>
                        <div class="form-horizontal">
                            <%-- Start of form --%>
                            <fieldset>
                                <legend runat="server" id="legend">Add Composition</legend>
                                <%-- Composition search --%>
                                <asp:UpdatePanel ID="upSearch" runat="server">
                                    <ContentTemplate>
                                        <div>
                                            <h4>Composition Search</h4>
                                            <br />
                                            <label runat="server" id="lblSearchNote" visible="false" class="instruction-label">This section is for verifying that the composition you are about to enter does not already exist. Please confirm the composition is not in the system before you proceed with adding it.</label>
                                            <asp:Panel runat="server" ID="pnlCompositionId">
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtId" CssClass="col-md-3 control-label float-left">Composition Id</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox runat="server" ID="txtId" CssClass="form-control" />
                                                    </div>
                                                    <asp:Button ID="btnSearchId" runat="server" Text="Search Id" CssClass="btn btn-primary btn-min-width-72" OnClick="btnSearchId_Click" CausesValidation="false" />
                                                </div>
                                            </asp:Panel>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlComposerSearch" CssClass="col-md-3 control-label float-left">Composer</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlComposerSearch" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource3" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlComposerSearch_SelectedIndexChanged">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlComposition" CssClass="col-md-3 control-label float-left">Title</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlComposition" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource6" DataTextField="CompositionName" DataValueField="CompositionId" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlComposition_SelectedIndexChanged" AutoPostBack="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="WmtaDataSource6" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposition" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                            </div>
                                            <label id="txtCompositionId" runat="server" visible="false" />
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="ddlStyleSearch" CssClass="col-md-3 control-label float-left">Period</asp:Label>
                                            <div class="col-md-6">
                                                <asp:DropDownList ID="ddlStyleSearch" runat="server" CssClass="dropdown-list form-control" DataSourceID="SqlDataSource1" DataTextField="Style" DataValueField="Style" AppendDataBoundItems="true" OnSelectedIndexChanged="cboStyle_SelectedIndexChanged" AutoPostBack="True">
                                                    <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                            </div>
                                            <asp:Button ID="btnClearCompSearch" runat="server" Text="Clear" CssClass="btn btn-default btn-min-width-72" CausesValidation="false" OnClick="btnClearCompSearch_Click" />
                                        </div>
                                        <hr />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <%-- End Composition Search --%>
                                <%-- Composition Information --%>
                                <asp:Panel DefaultButton="btnSubmit" runat="server">
                                    <asp:UpdatePanel ID="upCompositionInfo" runat="server">
                                        <ContentTemplate>
                                            <h4>Composition Information</h4>
                                            <div style="text-align: center; margin: 20px 10px 20px 10px; font-weight: bold">
                                                <hr />
                                                Before creating ‘New’ composition titles MAKE SURE to verify that the title does not already exist in the database.  Double check all titles using the <a href="../CompositionTools/TitleLookup.aspx">Composition Title Finder</a>. 
                                                <hr />
                                            </div>
                                            <asp:Panel ID="pnlTitleEdit" runat="server" Visible="true">
                                                <label for="CompositionInstructions" class="instruction-label">Title, Key, Movement, Tempo, Catalog No.</label><br />
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtComposition" CssClass="col-md-3 control-label float-left">Title</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtComposition" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtComposition" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Title is required" ValidationGroup="EditingDeleting" />
                                                </div>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlTitleNew" runat="server" Visible="false">
                                                <label for="CompositionInstructions" class="instruction-label">Please include all known information, especially for Masterworks.</label>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtTitleNew" CssClass="col-md-3 control-label float-left">Title</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtTitleNew" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitleNew" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Title is required" ValidationGroup="Adding" />
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlKeyLetter" CssClass="col-md-3 control-label float-left">Key</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlKeyLetter" runat="server" CssClass="dropdown-list form-control float-left" Width="25%">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="A" Value="A"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="B" Value="B"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="C" Value="C"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="D" Value="D"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="E" Value="E"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="F" Value="F"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="G" Value="G"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddlKeyFS" runat="server" CssClass="dropdown-list form-control float-left margin-left-1percent" Width="36.5%">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Flat" Value="Flat"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Sharp" Value="Sharp"></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddlKeyMM" runat="server" CssClass="dropdown-list form-control float-left margin-left-1percent" Width="36.5%">
                                                            <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Major" Value="Major"></asp:ListItem>
                                                            <asp:ListItem Selected="False" Text="Minor" Value="Minor"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtMvmt" CssClass="col-md-3 control-label float-left">Movement</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtMvmt" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="txtTempo" CssClass="col-md-3 control-label float-left">Tempo</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:TextBox ID="txtTempo" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label runat="server" AssociatedControlID="ddlPrefix" CssClass="col-md-3 control-label float-left">Catalog No.</asp:Label>
                                                    <div class="col-md-6">
                                                        <asp:DropDownList ID="ddlPrefix" runat="server" CssClass="dropdown-list form-control float-left" Width="45%" Enabled="False">
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
                                                        <asp:TextBox ID="txtCatalogNo" runat="server" CssClass="form-control float-left margin-left-1percent" OnTextChanged="txtCatalogNo_TextChanged" Width="54%" AutoPostBack="true"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlComposer" CssClass="col-md-3 control-label float-left">Composer</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlComposer" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource5" DataTextField="Composer" DataValueField="Composer" AppendDataBoundItems="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:CheckBox ID="chkNewComposer1" runat="server" CssClass="checkbox float-left" Text="New" TextAlign="Left" OnCheckedChanged="chkNewComposer_CheckedChanged" AutoPostBack="true" />
                                                <div>
                                                    <asp:RequiredFieldValidator ID="rfvComposer" runat="server" ControlToValidate="ddlComposer" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Composer is required" ValidationGroup="Composer" /><br />
                                                </div>
                                                <asp:SqlDataSource ID="WmtaDataSource5" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownComposer" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                            </div>
                                            <asp:Panel ID="pnlComposer" runat="server" Visible="false">
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
                                                <asp:Label runat="server" CssClass="col-md-3 control-label float-left" Font-Bold="true">Playing Time</asp:Label>
                                            </div>
                                            <div class="form-group">
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
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMinutes" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Time is required" /><br />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlStyle" CssClass="col-md-3 control-label float-left">Period</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlStyle" runat="server" CssClass="dropdown-list form-control" AppendDataBoundItems="true" DataSourceID="WmtaDataSource2" DataTextField="Style" DataValueField="Style">
                                                        <asp:ListItem Selected="True" Text="" Value="" />
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="WmtaDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="SELECT [Style] FROM [ConfigStyles] ORDER BY [Style]"></asp:SqlDataSource>
                                                </div>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStyle" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Style is required" />
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="ddlCompLevel" CssClass="col-md-3 control-label float-left">Level</asp:Label>
                                                <div class="col-md-6">
                                                    <asp:DropDownList ID="ddlCompLevel" runat="server" CssClass="dropdown-list form-control" DataSourceID="WmtaDataSource7" DataTextField="Description" DataValueField="CompLevelId" AppendDataBoundItems="true">
                                                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="WmtaDataSource7" runat="server" ConnectionString="<%$ ConnectionStrings:WmtaConnectionString %>" SelectCommand="sp_DropDownCompLevel" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
                                                </div>
                                                <div>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCompLevel" CssClass="text-danger vertical-center font-size-12" ErrorMessage="Level is required" /><br />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="checkbox float-left col-md-3-margin">
                                                    <label>
                                                        16 measures, not including repeats?<input id="chkConfirmMeasures" type="checkbox" runat="server" class="float-left" />
                                                    </label>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </asp:Panel>
                                <%-- End Composition Information --%>
                                <hr />
                                <asp:Panel runat="server" ID="pnlButtons">
                                    <div style="text-align: center; margin: 0px 10px 20px 10px; font-weight: bold">
                                        For assistance email WMTACompositionHotline@gmail.com.
                                    </div>
                                    <div class="form-group">
                                        <div class="col-lg-10 col-lg-offset-2 float-right">
                                            <asp:Button ID="btnClear" Text="Clear" runat="server" CssClass="btn btn-default float-right" OnClick="btnClear_Click" CausesValidation="false" />
                                            <asp:Button ID="btnSubmit" Text="Submit" runat="server" CssClass="btn btn-primary float-right margin-right-5px" OnClick="btnSubmit_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </fieldset>
                        </div>
                        <label id="lblErrorMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblWarningMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblSuccessMessage" runat="server" style="color: transparent">.</label>
                        <label id="lblInfoMessage" runat="server" style="color: transparent">.</label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </section>
        </div>
    </div>
    <script>
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

        function showInfoMessage() {
            var message = $('#MainContent_lblInfoMessage').text();

            $.notify(message.toString(), { position: 'left-top', className: 'info' });
        }

        //show a success message
        function showSuccessMessage() {
            var message = $('#MainContent_lblSuccessMessage').text();

            $.notify(message.toString(), { position: "left-top", className: "success" });
        };
    </script>
</asp:Content>
