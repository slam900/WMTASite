using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Contacts
{
    public partial class TransferStudents : System.Web.UI.Page
    {
        /* session variables */
        private string teacher1Search = "Teacher1Data";
        private string teacher2Search = "Teacher2Data";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[teacher1Search] = null;
                Session[teacher2Search] = null;
            }
        }
        
        /*
         * Pre:
         * Post: If the user is not logged in they will be redirected to the welcome screen
         */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else
            {
                User user = (User)Session[Utility.userRole];

                //system admins only
                if (!user.permissionLevel.Contains('A'))
                    Response.Redirect("/Default.aspx");
            }
        }

        /*
         * Pre:
         * Post: If the entered data is valid, the students of the From teacher are associated with the To teacher
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            bool success = true;

            if (dataIsValid())
            {
                int fromTeacherId = Convert.ToInt32(ddlFrom.SelectedValue);
                int toTeacherId = Convert.ToInt32(ddlTo.SelectedValue);

                //TODO -- do student transfer



                //display message depending on whether or not the operation was successful
                if (success)
                {
                    showSuccessMessage("The students were successfully transferred.");
                    clearPage();
                }
                else
                {
                    showErrorMessage("Error: An error occurred while transferring the students.");
                }
            }
        }

        /*
         * Pre:
         * Post: Returns true if the entered data is valid and false otherwise.
         *       Both teachers must be selected and a reason must be chosen
         */
        private bool dataIsValid()
        {
            bool valid = true;

            //make sure the teachers are different
            if (ddlFrom.SelectedIndex == ddlTo.SelectedIndex)
            {
                showWarningMessage("The two teachers you have selected are the same.  Please choose a different teacher.");
                valid = false;
            }

            return valid;
        }

        //Show search for the From teacher
        protected void btnFromSearch_Click(object sender, EventArgs e)
        {
            pnlFromSearch.Visible = true;
            pnlToSearch.Visible = false;
        }

        //Show search for the To teacher
        protected void btnToSearch_Click(object sender, EventArgs e)
        {
            pnlFromSearch.Visible = false;
            pnlToSearch.Visible = true;
        }

        /*
         * Pre:   
         * Post:  The information for the selected teacher is loaded to the page
         */
        protected void gvFromSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvFromSearch.SelectedIndex;

            if (index >= 0 && index < gvFromSearch.Rows.Count)
            {
                string id = gvFromSearch.Rows[index].Cells[1].Text;
                string firstName = gvFromSearch.Rows[index].Cells[2].Text;
                string lastName = gvFromSearch.Rows[index].Cells[3].Text;

                //todo - change this to teacher
                //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (student != null)
                {
                    firstName = student.firstName;
                    lastName = student.lastName;
                }

                //load search fields
                txtFromId.Text = id;
                txtFromFirstName.Text =
                txtFromLastName.Text =
                lblFromId.Text = id;

                //select student from dropdown
                ddlFrom.SelectedValue = id;

                //hide search area
                pnlFromSearch.Visible = false;
                clearFromSearch();
                btnFromSearch.Visible = true;
            }
        }

        /*
         * Pre:   
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvToSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvToSearch.SelectedIndex;

            if (index >= 0 && index < gvToSearch.Rows.Count)
            {
                string id = gvToSearch.Rows[index].Cells[1].Text;
                string firstName = gvToSearch.Rows[index].Cells[2].Text;
                string lastName = gvToSearch.Rows[index].Cells[3].Text;

                //load student data to avoid the bug where ' shows up as &#39; if the data is just taken from the gridview
                Student student = DbInterfaceStudent.LoadStudentData(Convert.ToInt32(id));
                if (student != null)
                {
                    firstName = student.firstName;
                    lastName = student.lastName;
                }

                //load search fields
                txtToId.Text = id;
                txtToFirstName.Text = firstName;
                txtToLastName.Text = lastName;
                lblToId.Text = id;

                //select student from dropdown
                ddlTo.SelectedValue = txtToId.Text;

                //hide search area
                pnlToSearch.Visible = false;
                clearToSearch();
                btnToSearch.Visible = true;
            }
        }

        /*
         * Pre:   
         * Post:  The page of gvFromSearch is changed
         */
        protected void gvFromSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFromSearch.PageIndex = e.NewPageIndex;
            BindFromSessionData();
        }

        /*
         * Pre:   
         * Post:  The page of gvToSearch is changed
         */
        protected void gvToSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvToSearch.PageIndex = e.NewPageIndex;
            BindToSessionData();
        }

        /*
         * Pre:   The student 1 search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindFromSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[teacher1Search];
                gvFromSearch.DataSource = data;
                gvFromSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Transfer Students", "BindFromSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:   The student 2 search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindToSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[teacher2Search];
                gvToSearch.DataSource = data;
                gvToSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Coordinate Students", "BindToSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvFromSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvFromSearch, e);
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvToSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvToSearch, e);
        }

        /*
         * Pre:  The input must be a gridview that exists on the current page
         * Post: The background of the header row is set
         * @param gv is the gridView that will have its header row color changed
         * @param e are the event args for the event fired by the row being bound to data
         */
        private void setHeaderRowColor(GridView gv, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in gv.HeaderRow.Cells)
                {
                    cell.BackColor = Color.Black;
                    cell.ForeColor = Color.White;
                }
            }
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnSearchFrom_Click(object sender, EventArgs e)
        {
            string id = txtFromId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                //if the search does not return any result, display a message saying so
                if (!searchTeachers(gvFromSearch, id, txtFromFirstName.Text, txtFromLastName.Text, teacher1Search))
                {
                    showInfoMessage("The search did not return any results.");
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvFromSearch);
                showWarningMessage("A Contact Id must be numeric");
            }
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students matching the search criteria are displayed (student id, first name, 
         *        and last name). The error message is also reset.
         */
        protected void btnSearchTo_Click(object sender, EventArgs e)
        {
            string id = txtToId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {
                //if the search does not return any result, display a message saying so
                if (!searchTeachers(gvToSearch, id, txtToFirstName.Text, txtToLastName.Text, teacher2Search))
                {
                    showInfoMessage("The search did not return any results.");
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvToSearch);
                showWarningMessage("A Contact Id must be numeric.");
            }
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing contacts.  Matching contact 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @returns true if results were found and false otherwise
         */
        private bool searchTeachers(GridView gridView, string id, string firstName, string lastName, string session)
        {
            DataTable table;
            bool result = true;

            try
            {
                if (!id.Equals(""))
                {
                    table = DbInterfaceContact.TeacherSearch(id, firstName, lastName);
                }
                else
                {
                    User user = (User)Session[Utility.userRole];

                    table = DbInterfaceContact.TeacherSearch(user.contactId.ToString(), firstName, lastName);
                }

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridView.DataSource = table;
                    gridView.DataBind();
                    gridView.HeaderRow.BackColor = Color.Black;

                    //save the data for quick re-binding upon paging
                    Session[session] = table;
                }
                else if (table != null && table.Rows.Count == 0)
                {
                    clearGridView(gridView);
                    result = false;
                }
                else if (table == null)
                {
                    showErrorMessage("Error: An error occurred during the search. Please make sure all entered data is valid.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred during the search. Please make sure all entered data is valid.");

                //log issue in database
                Utility.LogError("Transfer Students", "searchTeachers", gridView.ID + ", " + id + ", " + firstName + ", " + lastName + ", " + session, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }

        /*
        * Clear page
        */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Clears the data on the page
         */
        private void clearPage()
        {
            clearFromSearch();
            clearToSearch();
            ddlFrom.SelectedIndex = 0;
            ddlTo.SelectedIndex = 0;
        }

        /*
         * Clear Student 1 Search section
         */
        protected void btnClearFromSearch_Click(object sender, EventArgs e)
        {
            clearFromSearch();
        }

        /*
         * Clear Student 2 Search section
         */
        protected void btnClearToSearch_Click(object sender, EventArgs e)
        {
            clearToSearch();
        }

        /*
         * Clear From Teacher search section
         */
        private void clearFromSearch()
        {
            txtFromId.Text = "";
            txtFromFirstName.Text = "";
            txtFromLastName.Text = "";

            clearGridView(gvFromSearch);
        }

        /*
         * Clear To Teacher Search section
         */
        private void clearToSearch()
        {
            txtToId.Text = "";
            txtToFirstName.Text = "";
            txtToLastName.Text = "";

            clearGridView(gvToSearch);
        }

        /*
         * Pre: The GridView gv must exist on the current form
         * Post:  The data binding of the GridView is cleared, causing the table to be cleared
         * @param gv is the GridView to be cleared
         */
        private void clearGridView(GridView gv)
        {
            gv.DataSource = null;
            gv.DataBind();
        }

        /*
         * Pre:
         * Post: Update the hidden student id based on the selected student
         */
        protected void ddlFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblFromId.Text = ddlFrom.SelectedValue;
        }
        protected void ddlTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblToId.Text = ddlTo.SelectedValue;
        }

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

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Transfer Students", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}