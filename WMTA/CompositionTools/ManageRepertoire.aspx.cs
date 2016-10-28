using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class ManageRepertoire : System.Web.UI.Page
    {
        private Utility.Action action = Utility.Action.Add;

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
            initializePage();
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
        }

        /*
         * Pre:
         * Post Initialize the page based on the user's selected action
         */
        protected void initializePage()
        {
            //get requested action - default to adding
            string actionIndicator = Request.QueryString["action"];
            User user = (User)Session[Utility.userRole];

            if (actionIndicator == null || actionIndicator.Equals(""))
                action = Utility.Action.Add;
            else
                action = (Utility.Action)Convert.ToInt32(actionIndicator);

            //initialize page based on action
            if (action == Utility.Action.Add)
            {
                legend.InnerText = "Add Composition";
                pnlTitleEdit.Visible = false;
                pnlTitleNew.Visible = true;
                lblSearchNote.Visible = true;
                pnlCompositionId.Visible = false;
            }
            else if (action == Utility.Action.Edit && (user.permissionLevel.Contains("A") || user.permissionLevel.Contains("C")))
            {
                legend.InnerText = "Edit Composition";
                pnlTitleEdit.Visible = true;
                pnlTitleNew.Visible = false;
                lblSearchNote.Visible = false;
            }
            else if (action == Utility.Action.Delete && (user.permissionLevel.Contains("A") || user.permissionLevel.Contains("C")))
            {
                legend.InnerText = "Delete Composition - not implemented";
                pnlTitleEdit.Visible = true;
                pnlTitleNew.Visible = false;
                lblSearchNote.Visible = false;
                disableControls();
            }
            else
            {
                Response.Redirect("/Default.aspx");
            }
        }

        #region Submit Info

        /*
         * Pre:
         * Post: Make sure the data is valid and submit the composition information
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dataIsValid())
            {
                //Add new
                if (action == Utility.Action.Add)
                {
                    addNewComposition();
                }
                //Edit existing
                else if (action == Utility.Action.Edit)
                {
                    editComposition();
                }
                else if (action == Utility.Action.Delete)
                {
                    deleteComposition();
                }
            }
        }

        /*
         * Pre:
         * Post: If the composition doesn't already exist, add it to the database
         */
        private void addNewComposition()
        {
            Composition newComp;
            string title;

            User user = (User)Session[Utility.userRole];
            string name = user.contactId + ": " + user.lastName + ", " + user.firstInitial.ToUpper();

            //construct title
            if (pnlTitleNew.Visible)
                title = createTitle();
            else
                title = txtComposition.Text.Trim();

            //get composition length
            double time = Convert.ToDouble(txtMinutes.Text) + Convert.ToDouble(ddlSeconds.SelectedValue);

            //if an existing composer is being used
            if (!chkNewComposer1.Checked)
            {
                if (!compositionExists())
                {
                    newComp = new Composition(title, ddlComposer.SelectedValue,
                                            ddlStyle.SelectedValue, ddlCompLevel.SelectedValue, time, name);

                    if (newComp.compositionId == -1)
                    {
                        showErrorMessage("Error: There was an error adding the composition.");
                    }
                    else  //show success message and clear page
                    {
                        showSuccessMessage("The composition has been added successfully.");
                        clearPage();
                        ddlComposition.DataBind();
                        ddlComposer.DataBind();
                    }
                }
                else
                {
                    showWarningMessage("The composition already exists.");
                }
            }
            else
            {
                string composer = createComposerName();

                if (!compositionExists())
                {
                    newComp = new Composition(title, composer, ddlStyle.SelectedValue,
                                          ddlCompLevel.SelectedValue, time, name);

                    //if successful, show message and clear page
                    if (newComp.compositionId != -1)
                    {
                        showSuccessMessage("The composition has been added successfully.");
                        clearPage();
                        ddlComposition.DataBind();
                        ddlComposer.DataBind();
                    }
                    else
                    {
                        showErrorMessage("Error: There was an error adding the composition.");
                    }
                }
                else
                {
                    showWarningMessage("The composition already exists.");
                }

                ddlComposer.DataBind();
            }
        }

        /*
         * Pre: 
         * Post: Edit the selected composition with the input information
         */
        private void editComposition()
        {
            string composer = "";
            int id = Convert.ToInt32(txtCompositionId.InnerText);
            double time = Convert.ToDouble(txtMinutes.Text) + Convert.ToDouble(ddlSeconds.SelectedValue);

            if (!chkNewComposer1.Checked)
            {
                composer = ddlComposer.SelectedValue;
            }
            else
            {
                composer = createComposerName();
            }

            Composition comp = new Composition(id, txtComposition.Text.Trim(), composer, ddlStyle.SelectedValue,
                                               ddlCompLevel.SelectedValue, time);
            if (comp.updateInDatabase())
            {
                //update dropdown
                ddlComposer.DataBind();

                //display success message and clear page
                showSuccessMessage("The composition has been updated successfully.");
                clearPage();
            }
            else
            {
                showErrorMessage("Error: There was an error adding the composition.");
            }
        }

        /*
         * Pre: 
         * Post: Delete the selected composition
         */
        private void deleteComposition()
        {
            int id = Convert.ToInt32(txtCompositionId.InnerText);

            Composition comp = new Composition(id);
            bool result = comp.deleteFromDatabase();

            //display success message and clear page
            if (result)
            {
                showSuccessMessage("The composition has been deleted successfully.");
                clearPage();
            }
            else
            {
                showErrorMessage("Error: There was an error deleting the composition.  Ensure the " +
                                 "composition is not used in any student events using the 'Composition Usage' page.");
            }
        }


        /*
         * Pre:
         * Post: Construct the composition's title
         * @returns the composition title
         */
        private string createTitle()
        {
            string title = txtTitleNew.Text.Trim();

            if (ddlKeyLetter.SelectedIndex > 0) //get letter of key
                title = title + ", " + ddlKeyLetter.SelectedValue;

            if (ddlKeyFS.SelectedIndex > 0 && ddlKeyLetter.SelectedIndex > 0) //get flat or sharp
                title = title + "-" + ddlKeyFS.SelectedValue;

            if (ddlKeyMM.SelectedIndex > 0) //get major or minor
                title = title + " " + ddlKeyMM.SelectedValue;

            if (!txtMvmt.Text.Equals("")) //get movement
                title = title + ", " + txtMvmt.Text.Trim();

            if (!txtTempo.Text.Equals("")) //get tempo
                title = title + ", " + txtTempo.Text.Trim();

            if (!txtCatalogNo.Text.Equals("")) //get catalog number
            {
                if (ddlPrefix.SelectedIndex > 0) //get catalog prefix
                    title = title + ", " + ddlPrefix.SelectedValue + " " + txtCatalogNo.Text.Trim();
                else
                    title = title + ", " + txtCatalogNo.Text.Trim();
            }

            return title;
        }

        /*
         * Pre:
         * Post: Constructs the composer name
         * @returns the composer's name
         */
        private string createComposerName()
        {
            string composer = txtComposerLast.Text.Trim();

            //get first and middle initials, if entered
            if (!txtComposerFI.Text.Equals(""))
            {
                composer = composer + ", " + txtComposerFI.Text.Trim() + ".";

                if (!txtComposerMI.Text.Equals(""))
                {
                    composer = composer + txtComposerMI.Text.Trim() + ".";
                }
            }
            else if (!txtComposerMI.Text.Equals(""))
            {
                composer = composer + ", " + txtComposerMI.Text.Trim() + ".";
            }

            return composer;
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
            if (!chkConfirmMeasures.Checked && action != Utility.Action.Delete)
            {
                valid = false;
                showWarningMessage("The composition must contain 16 measures, not including repeats.");
            }

            //make sure a composition was selected if editing or deleting
            if (action != Utility.Action.Add && txtCompositionId.InnerText.Equals(""))
            {
                valid = false;
                showWarningMessage("Please select a composition.");
            }

            if (action == Utility.Action.Add)
            {
                Page.Validate("Adding");
                valid = valid && Page.IsValid;
            }
            else
            {
                Page.Validate("EditingDeleting");
                valid = valid && Page.IsValid;
            }

            //set which composer option should be validated and then validate
            if (chkNewComposer1.Checked)
                rfvComposer.ControlToValidate = txtComposerLast.ID;
            else
                rfvComposer.ControlToValidate = ddlComposer.ID;

            Page.Validate("Composer");
            valid = valid && Page.IsValid;

            //make sure minutes are entered and greater than 0
            if (!txtMinutes.Text.Equals(""))
            {
                int num;
                bool isNum = Int32.TryParse(txtMinutes.Text, out num);

                if (!isNum || (isNum && num < 0))
                {
                    valid = false;
                    showWarningMessage("Minutes must be a positive integer.");
                }
            }

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

            if (!chkNewComposer1.Checked)
                exists = DbInterfaceComposition.CompositionExists(txtComposition.Text.Trim(), ddlComposer.SelectedValue);
            else
            {
                string composer = txtComposerLast.Text.Trim();

                //get first and middle initials, if entered
                if (!txtComposerFI.Text.Equals(""))
                {
                    composer = composer + ", " + txtComposerFI.Text.Trim() + ".";

                    if (!txtComposerMI.Text.Equals(""))
                        composer = composer + txtComposerMI.Text.Trim() + ".";
                }
                else if (!txtComposerMI.Text.Equals(""))
                    composer = composer + ", " + txtComposerMI.Text.Trim() + ".";

                exists = DbInterfaceComposition.CompositionExists(txtComposition.Text.Trim(), composer);
            }

            return exists;
        }

        /*
         * Pre:
         * Post: Show or hide the controls to create a new composer
         */
        protected void chkNewComposer_CheckedChanged(object sender, EventArgs e)
        {
            //show controls to create new composer
            if (chkNewComposer1.Checked)
            {
                ddlComposer.Visible = false;
                pnlComposer.Visible = true;
            }
            //show controls to use existing composer
            else
            {
                pnlComposer.Visible = false;
                ddlComposer.Visible = true;
            }
        }

        /* 
         * Pre:
         * Post: When no Catalog No is entered, disable the Catalog Prefix dropdown
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

        #endregion Submit Info

        #region Search


        /*
         * Pre:
         * Post: Get the information of the composition with the input id
         */
        protected void btnSearchId_Click(object sender, EventArgs e)
        {
            int compId = 0;

            if (Int32.TryParse(txtId.Text, out compId))
            {
                ListItem item = ddlComposition.Items.FindByValue(compId.ToString());

                if (item != null)
                {
                    ddlComposition.SelectedValue = compId.ToString();
                    LoadComposition();
                }
                else
                {
                    showInfoMessage("The entered composition id was not found.");
                }
            }
            else
            {
                showInfoMessage("Please enter a numeric composition id to search.");
            }
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
            searchCompositions(ddlStyleSearch.Text, "", ddlComposerSearch.Text);
            searchComposers(ddlStyleSearch.Text, "");
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
            searchCompositions(ddlStyleSearch.Text, "", ddlComposerSearch.Text);
            searchComposers(ddlStyleSearch.Text, "");
        }

        /*
         * Pre:
         * Post: The options in the "Composition" dropdown will be filtered based
         *       on the selected Style, Level, and Composer
         */
        protected void ddlComposerSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCompositions(ddlStyleSearch.Text, "", ddlComposerSearch.Text);
        }

        protected void ddlComposition_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComposition();            
        }

        /*
         * Pre:
         * Post: Load the data of the selected composition
         */
        private void LoadComposition()
        {
            if (action != Utility.Action.Add && !ddlComposition.SelectedValue.ToString().Equals(""))
            {
                int selectedCompId = Convert.ToInt32(ddlComposition.SelectedValue);
                Composition composition = DbInterfaceComposition.GetComposition(selectedCompId);

                if (composition != null)
                {
                    double length, seconds;
                    int minutes;

                    txtCompositionId.InnerText = composition.compositionId.ToString();
                    txtComposition.Text = composition.title;
                    ddlStyle.Text = composition.style;
                    ddlStyleSearch.Text = composition.style;
                    ddlCompLevel.SelectedValue = composition.compLevel;
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
                    showErrorMessage("Error: There was an error loading the composition.");
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
                    showErrorMessage("Error: An error occurred during the search.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

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
                    showErrorMessage("Error: An error occurred during the search.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search.");

                Utility.LogError("Repertoire", "searchComposers", "style: " + style + ", compLevelId: " + compLevelId,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        #endregion Search

        #region Clear Page

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
            chkNewComposer1.Checked = false;
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
            txtId.Text = "";
            ddlStyleSearch.SelectedIndex = -1;
            ddlComposerSearch.SelectedIndex = -1;
            ddlComposition.SelectedIndex = -1;

            //reset the dropdowns with all data
            searchComposers("", "");
            searchCompositions("", "", "");
        }

        protected void btnClearCompSearch_Click(object sender, EventArgs e)
        {
            clearCompSelect();
        }

        /*
         * Pre:
         * Post:
         */
        private void disableControls()
        {
            txtComposition.Enabled = false;
            ddlComposer.Enabled = false;
            txtMinutes.Enabled = false;
            ddlSeconds.Enabled = false;
            ddlStyle.Enabled = false;
            ddlCompLevel.Enabled = false;
            chkConfirmMeasures.Disabled = true;
        }

        #endregion Clear Page

        #region Messages

        /*
         * Pre:
         * Post: Displays the input error message in the top-left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showErrorMessage(string message)
        {
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowError", "showMainError(" + message + ")", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowMainError", "showMainError(" + message + ")", true);
            lblErrorMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowError", "showMainError()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input warning message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showWarningMessage(string message)
        {
            lblWarningMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowWarning", "showWarningMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input informational message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showInfoMessage(string message)
        {
            lblInfoMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowInfo", "showInfoMessage()", true);
        }

        /*
         * Pre: 
         * Post: Displays the input success message in the top left corner of the screen
         * @param message is the message text to be displayed
         */
        private void showSuccessMessage(string message)
        {
            lblSuccessMessage.InnerText = message;

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ShowSuccess", "showSuccessMessage()", true);
        }

        #endregion Messages

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
            showErrorMessage("Error: An error occurred.");

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}