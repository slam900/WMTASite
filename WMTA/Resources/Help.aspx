<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="WMTA.Resources.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row" style="height: 1000px">
        <div class="well bs-component col-md-8 main-div center">
            <section id="helpForm">
                <div class="form-horizontal">
                    <h2>Help</h2>
                    <hr />
                    <div class="help-site-group">
                        <h4>Resources</h4>
                        <div class="resource">
                            <a href="../Resource Files/Ovation Users Guide - 2015 Teacher-Judge.pdf" target="_blank">Teacher/Judge User Guide</a>
                        </div>
                        <div class="resource">
                            <a href="../Resource Files/Keyboard Repertoire Leveling Guideline.pdf" target="_blank">Keyboard Repertoire Leveling Guideline</a>
                        </div>
                        <div class="resource">
                            <a href="../Resource Files/2016 Ineligible Pieces.pdf" target="_blank">Ineligible Compositions For Piano Solo - Examples</a>
                        </div>
                        <div class="resource">
                            <a href="../Resource Files/ComposersTableLastName2014.pdf" target="_blank">Composer List</a>
                            <div>
                                <label class="resource-note"><b>Note:</b> Some composers may be considered to be in more than one time period.  Based on historical acceptance and decisions made by the composition committee, please refer to what is designated in the above composer list; or to what is already assigned to a specific composition in the Ovation database.</label>
                            </div>
                        </div>
                        <div class="resource">
                            <a href="http://www.wisconsinmusicteachers.com/event-information-and-rules-handbook" target="_blank">District Auditions Handbook & Event Rules</a>
                        </div>
                        <div class="resource">
                            <a href="../Resource Files/2016ChairsAndEvents.pdf" target="_blank">2016 WMTA District Chairs And Event Locations</a>
                        </div>
                        <div class="resource">
                            <a href="../Resource Files/WMTAChairsAddresses2016.pdf" target="_blank">2016 WMTA District Chair Addresses</a>
                        </div>
                        <div style="margin-top: 30px">
                            <h4>Blank Judging Forms</h4>
                            <div class="resource">
                                <a href="../Resource Files/District Judging Form - KEYBOARD PIANO - blank.pdf" target="_blank">Keyboard - Piano</a>
                            </div>
                            <div class="resource">
                                <a href="../Resource Files/District Judging Form - KEYBOARD ORGAN - blank.pdf" target="_blank">Keyboard - Organ</a>
                            </div>
                            <div class="resource">
                                <a href="../Resource Files/District Judging Form - INSTRUMENTAL - blank.pdf" target="_blank">Instrumental</a>
                            </div>
                            <div class="resource">
                                <a href="../Resource Files/District Judging Form - STRINGS - blank.pdf" target="_blank">Strings</a>
                            </div>
                            <div class="resource">
                                <a href="../Resource Files/District Judging Form - VOCAL - blank.pdf" target="_blank">Vocal</a>
                            </div>
                        </div>
                    </div>
                    <hr />
                    <div class="help-site-group">
                        <h4>Send Feedback</h4>
                        <label>Click <a runat="server" href="~/Resources/SendFeedback.aspx">here</a> to send us feedback, including problems you have experienced with the site, comments, or questions!</label>
                    </div>
                    <div>
                        Email the Composition Hotline at <a href="mailto:wmtacompositionhotline@gmail.com" runat="server">wmtacompositionhotline@gmail.com</a>
                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
