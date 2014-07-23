using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class ReplaceComposition : System.Web.UI.Page
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
         * Post: The selected composition to replace is replaced by the replacement composition.
         *       The composition to replace is then deleted from the system.
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                //replace first composition with second
                int idToReplace = Convert.ToInt32(ddlCompositionToReplace.SelectedValue);
                int replacementId = Convert.ToInt32(ddlReplacement.SelectedValue);
                bool success = DbInterfaceComposition.ReplaceComposition(idToReplace, replacementId);

                if (success)
                {
                    clearPage();
                    showSuccessMessage("The composition was successfully replaced.");
                }
                else
                {
                    showErrorMessage("Error: There was an error replacing the selected composition.");
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
            txtIdReplace.Text = "";

            searchCompositions(ddlCompositionToReplace, ddlStyleSearch.Text, "", ddlComposerSearch.Text);
            searchComposers(ddlComposerSearch, ddlStyleSearch.Text, "");
        }

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns 
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer. (in the select composition section)
         */
        protected void cboStyle2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIdReplacement.Text = "";
            searchCompositions(ddlReplacement, ddlStyleSearch2.Text, "", ddlComposerSearch2.Text);
            searchComposers(ddlComposerSearch2, ddlStyleSearch2.Text, "");
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
            txtIdReplace.Text = "";

            searchCompositions(ddlCompositionToReplace, ddlStyleSearch.Text, "", ddlComposerSearch.Text);
            searchComposers(ddlComposerSearch, ddlStyleSearch.Text, "");
        }

        /*
         * Pre:
         * Post: The options in the "Composer" and "Composition" dropdowns
         *       will be filtered based on the selected Style and Level.
         *       Compositions will also be filtered based on the selected
         *       composer. (in the select composition section)
         */
        protected void cboCompLevel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIdReplacement.Text = "";

            searchCompositions(ddlReplacement, ddlStyleSearch2.Text, "", ddlComposerSearch2.Text);
            searchComposers(ddlComposerSearch2, ddlStyleSearch2.Text, "");
        }

        /*
         * Pre:
         * Post: The options in the "Composition" dropdown will be filtered based
         *       on the selected Style, Level, and Composer
         */
        protected void ddlComposerSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIdReplace.Text = "";

            searchCompositions(ddlCompositionToReplace, ddlStyleSearch.Text, "", ddlComposerSearch.Text);
        }

        /*
         * Pre:
         * Post: The options in the "Composition" dropdown will be filtered based
         *       on the selected Style, Level, and Composer
         */
        protected void ddlComposerSearch2_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtIdReplacement.Text = "";

            searchCompositions(ddlReplacement, ddlStyleSearch2.Text, "", ddlComposerSearch2.Text);
        }

        /*
         * Pre:   The input style and competition level must exist in the system
         * Post:  The input parameters are used to search for existing compositions.  
         *        Matching compositions are loaded to the corresponding drop downs
         * @param ddl is the dropdown list to load with compositions
         * @param style is the style of compositions being loaded
         * @param compLevel is the competition level of compositions being loaded
         */
        private void searchCompositions(DropDownList ddl, string style, string compLevelId, string composer)
        {
            try
            {
                DataTable tableComposition = DbInterfaceComposition.GetCompositionSearchResults(style, compLevelId, composer);

                if (tableComposition != null)
                {
                    //clear current contents
                    ddl.DataSource = null;
                    ddl.Items.Clear();
                    ddl.DataSourceID = "";

                    //update tables
                    ddl.DataSource = tableComposition;
                    ddl.DataTextField = "CompositionName";
                    ddl.DataValueField = "CompositionId";

                    //add blank item
                    ddl.Items.Add(new ListItem(""));

                    //bind new data
                    ddl.DataBind();
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
         * @param ddl is the dropdown list to load with composers
         * @param style is the style of compositions by composers being loaded
         * @param compLevel is the competition level of compositions by composers being loaded
         */
        private void searchComposers(DropDownList ddl, string style, string compLevelId)
        {
            try
            {
                DataTable tableComposer = DbInterfaceComposition.GetComposerSearchResults(style, compLevelId);

                if (tableComposer != null)
                {
                    //Load the search results in the dropdowns. 
                    ddl.DataSource = null;

                    //clear current contents
                    ddl.Items.Clear();
                    ddl.DataSourceID = "";

                    //update tables
                    ddl.DataSource = tableComposer;
                    ddl.DataTextField = "Composer";
                    ddl.DataValueField = "Composer";

                    //add blank item
                    ddl.Items.Add(new ListItem(""));

                    //bind new data
                    ddl.DataBind();
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
        protected void btnIdReplace_Click(object sender, EventArgs e)
        {
            int num;

            if (Int32.TryParse(txtIdReplace.Text, out num))
            {
                ddlStyleSearch.SelectedIndex = -1;
                ddlComposerSearch.SelectedIndex = -1;
                searchComposers(ddlComposerSearch, "", "");
                searchCompositions(ddlCompositionToReplace, "", "", "");

                ListItem item = ddlCompositionToReplace.Items.FindByValue(num.ToString());

                if (item != null)
                {
                    ddlCompositionToReplace.SelectedValue = num.ToString();
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

        /*
         * Pre:
         * Post: Retrieve the id associated with the input id
         */
        protected void btnIdReplacement_Click(object sender, EventArgs e)
        {
            int num;

            if (Int32.TryParse(txtIdReplacement.Text, out num))
            {
                ddlStyleSearch2.SelectedIndex = -1;
                ddlComposerSearch2.SelectedIndex = -1;
                searchComposers(ddlComposerSearch2, "", "");
                searchCompositions(ddlReplacement, "", "", "");

                ListItem item = ddlReplacement.Items.FindByValue(num.ToString());

                if (item != null)
                {
                    ddlReplacement.SelectedValue = num.ToString();
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
            txtIdReplace.Text = "";
            txtIdReplacement.Text = "";
            ddlCompositionToReplace.SelectedIndex = -1;
            ddlReplacement.SelectedIndex = -1;
            ddlStyleSearch.SelectedIndex = -1;
            ddlStyleSearch2.SelectedIndex = -1;
            ddlComposerSearch.SelectedIndex = -1;
            ddlComposerSearch2.SelectedIndex = -1;

            searchComposers(ddlComposerSearch, "", "");
            searchComposers(ddlComposerSearch2, "", "");
            searchCompositions(ddlReplacement, "", "", "");
            searchCompositions(ddlCompositionToReplace, "", "", "");
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

    }
}