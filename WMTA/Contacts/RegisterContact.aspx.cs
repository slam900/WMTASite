using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA.Contacts
{
    public partial class RegisterContact : System.Web.UI.Page
    {
        Contact contact;
        //Session variables
        private string contactSearch = "ContactData";
        private string contactVar = "Contact";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables and set state to WI
            if (!Page.IsPostBack)
            {
                Session[contactSearch] = null;
                Session[contactVar] = null;

                loadYearDropdown();
            }

            //if a contact object has been instantiated, reload
            if (Page.IsPostBack && Session[contactVar] != null)
                contact = (Contact)Session[contactVar];
        }

        /*
        * Pre:
        * Post: If the user is not logged or they are not an administrator
         *      they will be redirected to the welcome screen
        */
        private void checkPermissions()
        {
            //if the user is not logged in, send them to login screen
            if (Session[Utility.userRole] == null)
                Response.Redirect("/Default.aspx");
            else //if the user is not an administrator, send them to welcome screen
            {
                User user = (User)Session[Utility.userRole];

                if (!user.permissionLevel.Contains("A"))
                    Response.Redirect("/Default.aspx");
            }
        }

        /*
         * Pre:
         * Post: Loads the current year and next year to the year dropdown
         */
        private void loadYearDropdown()
        {
            ddlYear.Items.Add(new ListItem("", ""));
            ddlYear.Items.Add(new ListItem(DateTime.Today.Year.ToString(), DateTime.Today.Year.ToString()));
            ddlYear.Items.Add(new ListItem((DateTime.Today.Year + 1).ToString(), (DateTime.Today.Year + 1).ToString()));
        }

        /*
        * Pre:
        * Post: Clicking this button will attempt to register the contact for the
        *       selected year.
        */
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Register();
        }

        /*
         * Pre:
         * Post: If the user has entered valid information, the new contact information is added
         *       to the database.  
         *       If the add is successful, the new contacts's id number will be displayed to the user,
         *       otherwise an error message will take its place.
         */
        private void Register()
        {
            try
            {
                //if the entered information is valid, add the new contact
                if (verifyInformation())
                {
                    int contactId, year;
                    string mtnaId;

                    //get input information
                    contactId = Convert.ToInt32(lblId.Text);
                    year = Convert.ToInt32(ddlYear.SelectedValue);
                    mtnaId = txtMtnaId.Text;

                    if (DbInterfaceContact.RegisterContact(contactId, year, mtnaId)) //show success message
                    {
                        showSuccessMessage("The contact was successfully registered.");

                        pnlButtons.Visible = false;
                        pnlFullPage.Visible = false;
                        pnlContactSearch.Visible = true;

                        clearData();
                        clearContactSearch();
                    }
                    else //show error message
                    {
                        showErrorMessage("Error: There was an error registering the contact.");
                    }
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: There was an error registering the contact.");

                Utility.LogError("Register Contact", "Register", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Verifies that the information entered by the user 
         *       is in the correct format.  Need contact id, MTNA id, and year
         */
        private bool verifyInformation()
        {
            bool valid = true;
            int num;

            if (lblId.Text.Equals("") || !Int32.TryParse(lblId.Text, out num))
            {
                showWarningMessage("Please select a contact to register.");
                valid = false;
            }

            return valid;
        }

        /*
         * Pre:   The ContactId field must be empty or contain an integer
         * Post:  Contacts matching the search criteria are displayed. The error message is also reset.
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string id = txtContactId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            //if the id is an integer or empty, do the search
            if (isNum || id.Equals(""))
            {

                //if the search does not return any result, display a message saying so
                if (!searchContacts(gvSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, contactSearch, -1))
                {
                    showInfoMessage("The search did not return any results.");
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvSearch);
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
         * @param districtId is the district of contacts that should be searched
         * @returns true if results were found and false otherwise
         */
        private bool searchContacts(GridView gridView, string id, string firstName, string lastName, string session, int districtId)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceContact.GetContactSearchResults(id, firstName, lastName, districtId);

                //If there are results in the table, display them.  Otherwise clear current
                //results and return false
                if (table != null && table.Rows.Count > 0)
                {
                    gridView.DataSource = table;
                    gridView.DataBind();

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
                    showErrorMessage("Error: An error occurred.  Please make sure all entered data is valid.");
                }
            }
            catch (Exception e)
            {
                showErrorMessage("Error: An error occurred.  Please make sure all entered data is valid.");

                //log issue in database
                Utility.LogError("Register Contacts", "searchContacts", gridView.ID + ", " + id + ", " + firstName + ", " + lastName + ", " + session, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
        }
        /*
         * Pre:   
         * Post:  The information for the selected contact is loaded to the page
         */
        protected void gvSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlFullPage.Visible = true;
            pnlButtons.Visible = true;
            int index = gvSearch.SelectedIndex;
            clearData();

            if (index >= 0 && index < gvSearch.Rows.Count)
            {
                txtContactId.Text = gvSearch.Rows[index].Cells[1].Text;

                //get contact information using id
                loadContact(Convert.ToInt32(gvSearch.Rows[index].Cells[1].Text));
            }
        }

        /*
         * Pre:  There must exist a contact with the input contact id
         * Post: The informatin of the contact with the input contact id is loaded to the page
         * @param contactId is the contact id of the contact to load
         */
        private void loadContact(int contactId)
        {
            Contact contact = new Contact(contactId, true);

            if (contact.id != -1)
            {
                Session[contactVar] = contact;

                //load contact data to form
                lblId.Text = contact.id.ToString();
                txtFirstNameSearch.Text = contact.firstName;
                txtLastNameSearch.Text = contact.lastName;
                lblName.Text = contact.firstName + " " + contact.middleInitial + " " + contact.lastName;
                lblDistrict.Text = DbInterfaceStudent.GetStudentDistrict(contact.districtId);
                lblContactType.Text = contact.contactTypeId;

                string mtnaId = DbInterfaceContact.GetMtnaId(contact.id);

                if (!mtnaId.Equals(""))
                {
                    txtMtnaId.Text = mtnaId;
                    txtMtnaId.Enabled = false;
                }
            }
            else
            {
                showErrorMessage("Error: An error occurred during the search.");
            }
        }

        /*
         * Pre:   
         * Post:  The page of gvSearch is changed
         */
        protected void gvSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:   The contact search session variable must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session[contactSearch];
                gvSearch.DataSource = data;
                gvSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("Register Contacts", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            setHeaderRowColor(gvSearch, e);
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
         * Pre:
         * Post:  Clears entered data, error messages, and error
         *        highlighting on the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearData();
            clearContactSearch();

            pnlFullPage.Visible = false;
            pnlButtons.Visible = false;
        }

        /*
         * Pre:
         * Post: Clears data in the contact search section
         */
        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            clearContactSearch();
        }

        /*
         * Pre:
         * Post:  Clears entered data, error messages, and error
         *        highlighting on the page
         */
        private void clearData()
        {
            //clear text and selections
            lblName.Text = "";
            lblId.Text = "";
            lblDistrict.Text = "";
            lblContactType.Text = "";
            txtMtnaId.Text = "";
            ddlYear.SelectedIndex = 0;

            Session[contactSearch] = null;
            Session[contactVar] = null;
        }

        /*
         * Pre:
         * Post: Clears the Contact Search section
         */
        private void clearContactSearch()
        {
            txtContactId.Text = "";
            txtFirstNameSearch.Text = "";
            txtLastNameSearch.Text = "";
            gvSearch.DataSource = null;
            gvSearch.DataBind();
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

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Register Contacts", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}