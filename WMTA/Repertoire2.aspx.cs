using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class Repertoire2 : System.Web.UI.Page
    {
        private string creatingNew = "CreatingNew"; //tracks whether a composition is being created 

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
                Session[creatingNew] = null;
        }

        /*
         * Pre:
         * Post: If the user is not logged in or does not have sufficient permissions they will be redirected to the welcome screen
         *       Users must have system admin or composition permissions to use this page
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                if (!(user.permissionLevel.Contains("A") || user.permissionLevel.Contains("C")))
                {
                    Response.Redirect("/Default.aspx");
                }
            }
        }

        /*
         * Pre:
         * Post: Make sure the data is valid and submit the composition information
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            clearErrors();

            if (dataIsValid())
            {
                //Add new
                if (lblSearchError.Text.Equals("Searching purposes only") || (bool)Session[creatingNew])
                {
                    Composition newComp;
                    string composer, title;

                    //construct title
                    if (pnlTitleNew.Visible)
                    {
                        title = txtTitleNew.Text;

                        if (ddlKeyLetter.SelectedIndex > 0) //get letter of key
                            title = title + ", " + ddlKeyLetter.SelectedValue;
                        if (ddlKeyFS.SelectedIndex > 0 && ddlKeyLetter.SelectedIndex > 0) //get flat or sharp
                            title = title + "-" + ddlKeyFS.SelectedValue;
                        if (ddlKeyMM.SelectedIndex > 0) //get major or minor
                            title = title + " " + ddlKeyMM.SelectedValue;
                        if (!txtMvmt.Text.Equals("")) //get movement
                            title = title + ", " + txtMvmt.Text;
                        if (!txtTempo.Text.Equals("")) //get tempo
                            title = title + ", " + txtTempo.Text;
                        if (!txtCatalogNo.Text.Equals("")) //get catalog number
                        {
                            if (ddlPrefix.SelectedIndex > 0) //get catalog prefix
                                title = title + ", " + ddlPrefix.SelectedValue + " " + txtCatalogNo.Text;
                            else
                                title = title + ", " + txtCatalogNo.Text;
                        }
                    }
                    else
                        title = txtComposition.Text;

                    //get composition length
                    double time = Convert.ToDouble(txtMinutes.Text) + Convert.ToDouble(ddlSeconds.SelectedValue);

                    //if an existing composer is being used
                    if (!chkNewComposer.Checked)
                    {
                        if (!compositionExists())
                        {
                            newComp = new Composition(title, ddlComposer.SelectedValue,
                                                    ddlStyle.SelectedValue, ddlCompLevel.SelectedValue, time);

                            if (newComp.compositionId == -1)
                            {
                                lblErrorMsg.Text = "The composition could not be added";
                                lblErrorMsg.Visible = true;
                            }
                        }
                        else
                        {
                            lblErrorMsg.Text = "The composition already exists.";
                            lblErrorMsg.Visible = true;
                        }
                    }
                    else
                    {
                        composer = txtComposerLast.Text;

                        //get first and middle initials, if entered
                        if (!txtComposerFI.Text.Equals(""))
                        {
                            composer = composer + ", " + txtComposerFI.Text + ".";

                            if (!txtComposerMI.Text.Equals(""))
                                composer = composer + txtComposerMI.Text + ".";
                        }
                        else if (!txtComposerMI.Text.Equals(""))
                            composer = composer + ", " + txtComposerMI.Text + ".";

                        if (!compositionExists())
                            newComp = new Composition(title, composer, ddlStyle.SelectedValue,
                                                  ddlCompLevel.SelectedValue, time);
                        else
                        {
                            lblErrorMsg.Text = "The composition already exists.";
                            lblErrorMsg.Visible = true;
                        }

                        ddlComposer.DataBind();
                    }

                    //error - if not error, display success message and options
                    if (!lblErrorMsg.Visible)
                        displaySuccessMessageAndOptions();
                }
                //Edit existing
                else
                {
                    int id = Convert.ToInt32(txtCompositionId.InnerText);
                    string composer;
                    double time = Convert.ToDouble(txtMinutes.Text) + Convert.ToDouble(ddlSeconds.SelectedValue);

                    if (!chkNewComposer.Checked)
                        composer = ddlComposer.SelectedValue;
                    else
                    {
                        composer = txtComposerLast.Text;

                        //get first and middle initials, if entered
                        if (!txtComposerFI.Text.Equals(""))
                        {
                            composer = composer + ", " + txtComposerFI.Text + ".";

                            if (!txtComposerMI.Text.Equals(""))
                                composer = composer + txtComposerMI.Text + ".";
                        }
                        else if (!txtComposerMI.Text.Equals(""))
                            composer = composer + ", " + txtComposerMI.Text + ".";
                    }

                    Composition comp = new Composition(id, txtComposition.Text, composer, ddlStyle.SelectedValue,
                                                       ddlCompLevel.SelectedValue, time);
                    if (comp.updateInDatabase())
                    {
                        ddlComposer.DataBind();
                        displaySuccessMessageAndOptions();
                    }
                    else
                    {
                        lblErrorMsg.Text = "The composition could not be updated";
                        lblErrorMsg.Visible = true;
                    }
                }
            }
        }

        /*
         * Pre:
         * Post: Determines whether all required information is entered and all
         *       data is in a valid format.  
         * @returns true if all data is valid and false otherwise
         */
        private bool dataIsValid()
        {
            bool valid = true;

            //make sure user verified that there are 16 measures
            if (!chkConfirmMeasures.Checked)
            {
                valid = false;
                lblMeasuresError.Visible = true;
            }

            //make sure a composition was selected
            if (Session[creatingNew] != null && !(bool)Session[creatingNew])
            {
                if (txtCompositionId.InnerText.Equals(""))
                {
                    valid = false;
                    lblSearchError.Visible = true;
                }
            }

            //make sure a composition is entered
            if (txtComposition.Text.Equals("") && txtTitleNew.Text.Equals(""))
            {
                valid = false;
                lblCompositionError.Visible = true;
                lblTitleNewError.Visible = true;
            }

            //make sure a composer is selected/entered
            if (!chkNewComposer.Checked)
            {
                if (ddlComposer.SelectedIndex == 0)
                {
                    valid = false;
                    lblComposerError.Visible = true;
                }
            }
            else
            {
                if (txtComposerLast.Text.Equals(""))
                {
                    valid = false;
                    lblNewComposerError.Visible = true;
                }
            }

            //make sure minutes are entered and greater than 0
            if (!txtMinutes.Text.Equals(""))
            {
                int num;
                bool isNum = Int32.TryParse(txtMinutes.Text, out num);

                if (!isNum || (isNum && num < 0))
                {
                    valid = false;
                    lblMinutesErrorMsg.Visible = true;
                    lblMinutesError.Visible = true;
                }
            }
            else
            {
                valid = false;
                lblMinutesError.Visible = true;
            }

            //make sure a style is selected
            if (ddlStyle.SelectedIndex == 0)
            {
                valid = false;
                lblStyleError.Visible = true;
            }

            //make sure a composition level is selected
            if (ddlCompLevel.SelectedIndex == 0)
            {
                valid = false;
                lblCompLevelError.Visible = true;
            }

            //if any data is invalid, display the main error message
            if (!valid)
                lblErrorMsg.Visible = true;

            return valid;
        }

        /*
         * Pre:
         * Post: Determines whether or not a composition exists with the input 
         *       title and composer name
         * @returns true if the composition exists and false otherwise
         */
        private bool compositionExists()
        {
            bool exists = false;

            if (!chkNewComposer.Checked)
                exists = DbInterfaceComposition.CompositionExists(txtComposition.Text, ddlComposer.SelectedValue);
            else
            {
                string composer = txtComposerLast.Text;

                //get first and middle initials, if entered
                if (!txtComposerFI.Text.Equals(""))
                {
                    composer = composer + ", " + txtComposerFI.Text + ".";

                    if (!txtComposerMI.Text.Equals(""))
                        composer = composer + txtComposerMI.Text + ".";
                }
                else if (!txtComposerMI.Text.Equals(""))
                    composer = composer + ", " + txtComposerMI.Text + ".";

                exists = DbInterfaceComposition.CompositionExists(txtComposition.Text, composer);
            }

            return exists;
        }

        /*
         * Pre:
         * Post: All controls are hidden, the user is told that the composition 
         *       was entered.  They are given the options to add an additional 
         *       composition or go back to the menu/welcome page
         */
        private void displaySuccessMessageAndOptions()
        {
            if (upSearch.Visible)
                lblSuccess.Text = "The composition was successfully updated";
            else if (!upSearch.Visible)
                lblSuccess.Text = "The composition was successfully added";

            lblSuccess.Visible = true;
            pnlSuccess.Visible = true;
            pnlFullPage.Visible = false;

            clearPage();
        }

        protected void chkNewComposer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNewComposer.Checked)
            {
                ddlComposer.Visible = false;
                pnlComposer.Visible = true;
            }
            else
            {
                pnlComposer.Visible = false;
                ddlComposer.Visible = true;
            }
        }

        /*
         * Pre:
         * Post: The following methods hide error messages associated with the
         *       specific control
         */
        protected void txtMinutes_TextChanged(object sender, EventArgs e)
        {
            lblMinutesError.Visible = false;
            lblMinutesErrorMsg.Visible = false;
        }
        protected void ddlStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblStyleError.Visible = false;
        }
        protected void ddlCompLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCompLevelError.Visible = false;
        }
        protected void txtComposition_TextChanged(object sender, EventArgs e)
        {
            lblCompositionError.Visible = false;
        }
        protected void txtComposerLast_TextChanged(object sender, EventArgs e)
        {
            lblNewComposerError.Visible = false;
        }
        protected void cboComposer_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblComposerError.Visible = false;
        }

        /*
         * Pre:
         * Post: The user is taken back to the main welcome screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: The user is taken back to the main welcome screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: The repertoire screen is cleared for another entry
         */
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            clearPage();

            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;
        }

        /*
         * Pre:
         * Post: Clears all data on the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: Clears all data on the page
         */
        private void clearPage()
        {
            clearErrors();
            clearCompSelect();

            txtComposition.Text = "";
            txtTitleNew.Text = "";
            ddlKeyLetter.SelectedIndex = 0;
            ddlKeyFS.SelectedIndex = 0;
            ddlKeyMM.SelectedIndex = 0;
            txtMvmt.Text = "";
            txtTempo.Text = "";
            txtCatalogNo.Text = "";
            ddlComposer.SelectedIndex = 0;
            txtComposerLast.Text = "";
            txtComposerFI.Text = "";
            txtComposerMI.Text = "";
            txtMinutes.Text = "";
            ddlSeconds.SelectedIndex = 0;
            ddlStyle.SelectedIndex = 0;
            ddlCompLevel.SelectedIndex = 0;
            chkNewComposer.Checked = false;
            pnlComposer.Visible = false;
            ddlComposer.Visible = true;
            txtCompositionId.InnerText = "";
        }

        /*
         * Pre:
         * Post: Clears the composition search section
         */
        private void clearCompSelect()
        {
            ddlStyleSearch.SelectedIndex = -1;
            ddlCompLevelSearch.SelectedIndex = -1;
            ddlComposerSearch.SelectedIndex = -1;
            ddlComposition.SelectedIndex = -1;

            //reset the dropdowns with all data
            searchComposers("", "");
            searchCompositions("", "", "");
        }

        /*
         * Pre:
         * Post: The error messages on the page are hidden
         */
        private void clearErrors()
        {
            lblSearchError.Visible = false;
            lblSearchError.Text = "Please select a composition";
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
            lblCompositionError.Visible = false;
            lblTitleNewError.Visible = false;
            lblComposerError.Visible = false;
            lblNewComposerError.Visible = false;
            lblMinutesError.Visible = false;
            lblMinutesErrorMsg.Visible = false;
            lblStyleError.Visible = false;
            lblCompLevelError.Visible = false;
            lblMeasuresError.Visible = false;
        }

        protected void btnClearCompSearch_Click(object sender, EventArgs e)
        {
            clearCompSelect();
        }

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns 
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer. (in the select composition section)
         */
        protected void cboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyleSearch.Text, ddlCompLevelSearch.Text, ddlComposerSearch.Text);
            searchComposers(ddlStyleSearch.Text, ddlCompLevelSearch.Text);
        }
        /*
             * Pre:
             * Post: The options in the "Composer" and "Composition" dropdowns
             *       will be filtered based on the selected Style and Level.
             *       Compositions will also be filtered based on the selected
             *       composer. (in the select composition section)
             */
        protected void cboCompLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyleSearch.Text, ddlCompLevelSearch.Text, ddlComposerSearch.Text);
            searchComposers(ddlStyleSearch.Text, ddlCompLevelSearch.Text);
        }

        /*
         * Pre:
         * Post: The options in the "Composition" dropdown will be filtered based
         *       on the selected Style, Level, and Composer
         */
        protected void ddlComposerSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyleSearch.Text, ddlCompLevelSearch.Text, ddlComposerSearch.Text);
        }

        protected void ddlComposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            double length, seconds;
            int minutes;

            if (!(bool)Session[creatingNew] && !ddlComposition.SelectedValue.ToString().Equals(""))
            {
                int selectedCompId = Convert.ToInt32(ddlComposition.SelectedValue);
                Composition composition = DbInterfaceComposition.GetComposition(selectedCompId);

                if (composition != null)
                {
                    txtCompositionId.InnerText = composition.compositionId.ToString();
                    txtComposition.Text = composition.title;
                    ddlStyle.Text = composition.style;
                    ddlStyleSearch.Text = composition.style;
                    ddlCompLevel.SelectedValue = composition.compLevel;
                    ddlCompLevelSearch.SelectedValue = composition.compLevel;
                    ddlComposer.SelectedValue = composition.composer;
                    ddlComposerSearch.SelectedValue = composition.composer;
                    chkConfirmMeasures.Checked = true;

                    //get minutes and seconds
                    length = composition.playingTime;
                    minutes = (int)length;
                    seconds = length - (double)minutes;
                    txtMinutes.Text = minutes.ToString();
                    ddlSeconds.SelectedValue = seconds.ToString();
                }
                else
                {
                    lblErrorMsg.Text = "The composition could not be loaded";
                    lblErrorMsg.Visible = true;
                }
            }
        }

        /*
         * Pre:   The input style and competition level must exist in the system
         * Post:  The input parameters are used to search for existing compositions.  
         *        Matching compositions are loaded to the corresponding drop downs
         * @param style is the style of compositions being loaded
         * @param compLevel is the competition level of compositions being loaded
         */
        private void searchCompositions(string style, string compLevelId, string composer)
        {
            try
            {
                DataTable tableComposition = DbInterfaceComposition.GetCompositionSearchResults(style, compLevelId, composer);

                if (tableComposition != null)
                {
                    //clear current contents
                    ddlComposition.DataSource = null;
                    ddlComposition.Items.Clear();
                    ddlComposition.DataSourceID = "";

                    //update tables
                    ddlComposition.DataSource = tableComposition;
                    ddlComposition.DataTextField = "CompositionName";
                    ddlComposition.DataValueField = "CompositionId";

                    //add blank item
                    ddlComposition.Items.Add(new ListItem(""));

                    //bind new data
                    ddlComposition.DataBind();
                }
                else
                {
                    lblSearchError.Text = "An error occurred during the search";
                    lblSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblSearchError.Text = "An error occurred during the search";
                lblSearchError.Visible = true;

                Utility.LogError("Repertoire", "searchCompositions", "style: " + style + ", compLevelId: " + compLevelId +
                                 ", composer: " + composer, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The input style and competition level must exist in the system
         * Post:  The input parameters are used to search for existing composers.  
         *        Matching composers are loaded to the Composer dropdown
         * @param style is the style of compositions by composers being loaded
         * @param compLevel is the competition level of compositions by composers being loaded
         */
        private void searchComposers(string style, string compLevelId)
        {
            try
            {
                DataTable tableComposer = DbInterfaceComposition.GetComposerSearchResults(style, compLevelId);

                if (tableComposer != null)
                {
                    //Load the search results in the dropdowns. 
                    ddlComposerSearch.DataSource = null;

                    //clear current contents
                    ddlComposerSearch.Items.Clear();
                    ddlComposerSearch.DataSourceID = "";

                    //update tables
                    ddlComposerSearch.DataSource = tableComposer;
                    ddlComposerSearch.DataTextField = "Composer";
                    ddlComposerSearch.DataValueField = "Composer";

                    //add blank item
                    ddlComposerSearch.Items.Add(new ListItem(""));

                    //bind new data
                    ddlComposerSearch.DataBind();
                }
                else
                {
                    lblSearchError.Text = "An error occurred during the search";
                    lblSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblSearchError.Text = "An error occurred during the search";
                lblSearchError.Visible = true;

                Utility.LogError("Repertoire", "searchComposers", "style: " + style + ", compLevelId: " + compLevelId,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post Initialize the page based on the user's selected action
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                Session[creatingNew] = true;
                upSearch.Visible = true;
                pnlTitleEdit.Visible = false;
                pnlTitleNew.Visible = true;
                lblSearchError.ForeColor = System.Drawing.Color.DarkBlue;
                lblSearchError.Text = "Searching purposes only";
                lblSearchError.Visible = true;
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                Session[creatingNew] = false;
                upSearch.Visible = true;
                pnlTitleEdit.Visible = true;
                pnlTitleNew.Visible = false;
                lblSearchError.ForeColor = System.Drawing.Color.Red;
            }
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Repertoire", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
        protected void txtTitleNew_TextChanged(object sender, EventArgs e)
        {
            lblTitleNewError.Visible = false;
        }
        protected void chkConfirmMeasures_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmMeasures.Checked)
                lblMeasuresError.Visible = false;
        }
        /* 
         * When no Catalog No is entered, disable the Catalog Prefix dropdown
         */
        protected void txtCatalogNo_TextChanged(object sender, EventArgs e)
        {
            if (!txtCatalogNo.Text.Equals(""))
                ddlPrefix.Enabled = true;
            else
            {
                ddlPrefix.SelectedIndex = -1;
                ddlPrefix.Enabled = false;
            }
        }
    }
}