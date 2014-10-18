using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.CompositionTools
{
    public partial class TitleLookup : System.Web.UI.Page
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

        #region Lookup

        /*
         * Pre:
         * Post: The selected composer name is changed to the input composer name
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //check for compositions containing string
            string queryString = txtComposition.Text;
            string composer = ddlComposer.SelectedValue.ToString();

            List<Tuple<Composition, int>> qryResults = DbInterfaceComposition.CompositionTitleQuery(queryString, composer);

            if (qryResults != null)
            {
                loadResults(qryResults);

                if (qryResults.Count > 0)
                {
                    tblCompositions.Visible = true;
                }
                else
                {
                    tblCompositions.Visible = false;
                    showInfoMessage("No compositions were found with titles matching the input search string.");
                }
            }
            else
            {
                showErrorMessage("Error: There was an error finding all titles containing the entered characters.");
            }
        }

        /*
         * Pre:
         * Post: Loads the query results to the table
         */
        private void loadResults(List<Tuple<Composition, int>> qryResults)
        {
            clearTable();

            //load results
            foreach (Tuple<Composition, int> comps in qryResults)
                addComposition(comps.Item1, comps.Item2);
        }

        /*
         * Pre:
         * Post: The data of the input composition is added to the
         *       composition table 
         * @param composition is the composition to be entered into the table
         * @param timesUsed is the number of times that composition has been used in an event
         */
        private void addComposition(Composition composition, int numTimesUsed)
        {
            TableRow row = new TableRow();
            TableCell compId = new TableCell();
            TableCell comp = new TableCell();
            TableCell composer = new TableCell();
            TableCell style = new TableCell();
            TableCell level = new TableCell();
            TableCell time = new TableCell();
            TableCell timesUsed = new TableCell();

            //set cell text
            compId.Text = composition.compositionId.ToString();
            comp.Text = composition.title;
            composer.Text = composition.composer;
            style.Text = composition.style;
            level.Text = composition.compLevel;
            time.Text = composition.playingTime.ToString();
            timesUsed.Text = numTimesUsed.ToString();

            //add cells to new row
            row.Cells.Add(compId);
            row.Cells.Add(comp);
            row.Cells.Add(composer);
            row.Cells.Add(style);
            row.Cells.Add(level);
            row.Cells.Add(time);
            row.Cells.Add(timesUsed);

            //add new row to table
            tblCompositions.Rows.Add(row);
        }

        #endregion Lookup

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
            txtComposition.Text = "";
            tblCompositions.Visible = false;
            clearTable();
        }

        /*
         * Pre:
         * Post: Removes all rows except the header
         */
        private void clearTable()
        {
            while (tblCompositions.Rows.Count > 1)
            {
                tblCompositions.Rows.RemoveAt(1);
            }
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

        #endregion Messages
    }
}