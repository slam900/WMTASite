<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="WMTA.Resources.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row"  style="height: 1000px">
        <div class="well bs-component col-md-8 main-div center">
            <section id="helpForm">
                <div class="form-horizontal">
                    <h2>Help</h2>
                    <hr />
                    <div class="help-site-group">
                        <h4>*More Help Content Coming Soon!*</h4>
                    </div>
                    <hr />
                    <div class="help-site-group">
                        <h4>Send Feedback</h4>
                        <label>Click <a runat="server" href="~/Resources/SendFeedback.aspx">here</a> to send us feedback, including problems you have experienced with the site, comments, or questions!</label>
                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
