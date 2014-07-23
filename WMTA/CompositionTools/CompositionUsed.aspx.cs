using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class CompositionUsed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();
        }

        /*
         * Pre:
         * Post: If the user is not logged in or has invalid credentials they will be redirected to the welcome screen
         *       System administrators and those with composition rights can use this page
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

        #region Find Usage

        /*
         * Pre:
         * Post: The selected composer name is changed to the input composer name
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                //check usage and display appropriate message
                Composition comp = new Composition(Convert.ToInt32(ddlComposition.SelectedValue));

                if (comp.compositionId >= 0)
                {
                    int timesUsed = comp.getTimesUsedCount();

                    if (timesUsed > 0) 
                    {
                        pUsed.Visible = true;
                        pNotUsed.Visible = false;
                    } 
                    else 
                    {
                        pUsed.Visible = false;
                        pNotUsed.Visible = true;
                    }
                }
                else
                {
                    showErrorMessage("Error: There was an error determining the usage of the selected composition.");
                }
            }
            else //show error message if required data is missing
            {
                showWarningMessage("Please fill in all required fields.");
            }
        }

        #endregion Find Usage

        #region Composition Filter

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns 
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer. (in the select composition section)
         */
        protected void cboStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtId.Text = "";

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
            txtId.Text = "";

            searchCompositions(ddlStyleSearch.Text, "", ddlComposerSearch.Text);
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

                Utility.LogError("CompositionUsed", "searchComposers", "style: " + style + ", compLevelId: " + compLevelId,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Retrieve the id associated with the input id
         */
        protected void btnId_Click(object sender, EventArgs e)
        {
            int num;

            if (Int32.TryParse(txtId.Text, out num))
            {
                ddlStyleSearch.SelectedIndex = -1;
                ddlComposerSearch.SelectedIndex = -1;
                searchComposers("", "");
                searchCompositions("", "", "");

                ListItem item = ddlComposition.Items.FindByValue(num.ToString());

                if (item != null)
                {
                    ddlComposition.SelectedValue = num.ToString();
                }
                else
                {
                    showWarningMessage("No composition exists with with the entered id.");
                }
            }
            else
            {
                showWarningMessage("The id must be a number.");
            }
        }

        #endregion Composition Filter

        #region Clear Functions

        /*
         * Pre:
         * Post: All data on the page is cleared
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: All data on the page is cleared
         */
        private void clearPage()
        {
            txtId.Text = "";
            ddlStyleSearch.SelectedIndex = 0;
            ddlComposerSearch.SelectedIndex = 0;
            ddlComposition.SelectedIndex = 0;
            pUsed.Visible = false;
            pNotUsed.Visible = false;
        }

        #endregion Clear Functions

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

        #endregion Messages
    }
}