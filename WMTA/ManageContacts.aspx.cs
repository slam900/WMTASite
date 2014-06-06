using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class ManageContacts : System.Web.UI.Page
    {
        Contact contact;
        Judge judge;
        //Session variables
        private string creatingNew = "CreatingNew"; //tracks whether a contact is being created or edited
        private string deleting = "Deleting";       //tracks whether a contact is being deleted
        private string contactSearch = "ContactData";
        private string contactVar = "Contact", judgeVar = "Judge";

        protected void Page_Load(object sender, EventArgs e)
        {
            //clear session variables and set state to WI
            if (!Page.IsPostBack)
            {
                Session[creatingNew] = null;
                Session[deleting] = null;
                Session[contactSearch] = null;
                Session[contactVar] = null;
                Session[judgeVar] = null;

                checkPermissions();
            }

            //if a contact object has been instantiated, reload
            if (Page.IsPostBack && Session[contactVar] != null)
                contact = (Contact)Session[contactVar];
            //if a judge object has been instantiated, reload
            if (Page.IsPostBack && Session[judgeVar] != null)
                judge = (Judge)Session[judgeVar];
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

                //add create and delete contact options if user is a system admin
                if (user.permissionLevel.Contains("A"))
                {
                    ddlUserOptions.Items.Clear();
                    ddlUserOptions.Items.Add(new ListItem("Create New Contact", "Create New"));
                    ddlUserOptions.Items.Add(new ListItem("Edit Existing Contact", "Edit Existing"));
                    ddlUserOptions.Items.Add(new ListItem("Delete Existing Contact", "Delete Existing"));
                }
            }
        }

        /*
         * Pre:
         * Post: If the user has not yet added a new contact, clicking this 
         *       button will attempt to add a new contact to the database
         *       using the supplied information.  If the user added a new
         *       contact this button will clear the form and allow the user
         *       to add another new contact.
         */
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            clearErrors();

            //add new contact
            if (btnAdd.Text.Equals("Add"))
            {
                AddNewContact();

                if (!lblError.Visible)
                {
                    clearData();
                    clearContactSearch();
                    pnlFullPage.Visible = false;
                    pnlContactSearch.Visible = false;
                    pnlButtons.Visible = false;
                    pnlSuccess.Visible = true;
                    lblSuccess.Text = "The contact information was successfully added";
                    lblSuccess.Visible = true;
                }
            }
            //edit existing contact
            else if (btnAdd.Text.Equals("Submit") && !(bool)Session[creatingNew] && !(bool)Session[deleting])
            {
                EditExistingContact();

                if (!lblError.Visible)
                {
                    clearData();
                    clearContactSearch();
                    pnlFullPage.Visible = false;
                    pnlContactSearch.Visible = false;
                    pnlButtons.Visible = false;
                    pnlSuccess.Visible = true;
                    lblSuccess.Text = "The contact information was successfully updated";
                    lblSuccess.Visible = true;
                }
            }
            //delete contact
            else if (btnAdd.Text.Equals("Submit") && (bool)Session[deleting])
            {
                if (!txtFirstName.Text.Equals("") && !txtLastName.Text.Equals("") && !lblId.Text.Equals(""))
                    DeleteContact();

                if (!lblError.Visible)
                {
                    clearData();
                    pnlFullPage.Visible = false;
                    pnlContactSearch.Visible = false;
                    pnlButtons.Visible = false;
                    pnlAreYouSure.Visible = false;
                    pnlSuccess.Visible = true;
                    lblSuccess.Text = "The contact information was successfully deleted.";
                    lblSuccess.Visible = true;
                }
            }
        }

        /*
         * Pre:
         * Post: If the user has entered valid information, the new contact information is added
         *       to the database.  
         *       If the add is successful, the new contacts's id number will be displayed to the user,
         *       otherwise an error message will take its place.
         */
        private void AddNewContact()
        {
            try
            {
                //if the entered information is valid, add the new contact
                if (verifyInformation())
                {
                    string firstName, mi, lastName, street, city, state;
                    string email, phone, contactType;
                    int zip = -1, districtId;

                    //get input information
                    firstName = txtFirstName.Text;
                    mi = txtMiddleInitial.Text;
                    lastName = txtLastName.Text;
                    street = txtStreet.Text;
                    city = txtCity.Text;
                    state = ddlState.SelectedValue;
                    if (!txtZip.Text.Equals("")) zip = Convert.ToInt32(txtZip.Text);
                    email = txtEmail.Text;
                    phone = txtPhone.Text;
                    districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                    contactType = ddlContactType.SelectedValue;

                    //if the contact does not already exist, add it
                    if (!DbInterfaceContact.ContactExists(firstName, lastName, email))
                    {
                        contact = new Contact(firstName, mi, lastName, email, phone, districtId, contactType);
                        contact.setAddress(street, city, state, zip);
                        contact.addToDatabase();

                        Session[contactVar] = contact;

                        //add preferences
                        if (contact.id != -1 && pnlJudges.Visible)
                        {
                            judge = new Judge(contact.id, firstName, mi, lastName, email, phone,
                                              districtId, contactType, null, false);

                            if (judge.preferences != null)
                                UpdateJudgePreferences();
                        }

                        if (contact.id == -1)
                        {
                            lblError.Text = "There was an error adding the new contact";
                            lblError.Visible = true;
                        }
                    }
                    else
                    {
                        lblError.Text = "This contact already exists.  You may edit their information.";
                        lblError.Visible = true;
                    }
                }
            }
            catch (Exception e)
            {
                lblError.Text = "An error occurred while adding the contact.";
                lblError.Visible = true;

                Utility.LogError("Manage Contacts", "AddNewContact", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: If the user has entered valid information, the  contact information is edited
         *       in the database. 
         */
        private void EditExistingContact()
        {
            try
            {
                //if the entered information is valid, edit the contact information
                if (verifyInformation())
                {
                    int zip = -1;

                    if (contact == null)
                        contact = (Contact)Session[contactVar];

                    contact.firstName = txtFirstName.Text;
                    contact.middleInitial = txtMiddleInitial.Text;
                    contact.lastName = txtLastName.Text;
                    contact.email = txtEmail.Text;
                    contact.phone = txtPhone.Text;
                    contact.districtId = Convert.ToInt32(ddlDistrict.SelectedValue);
                    contact.contactTypeId = ddlContactType.SelectedValue;

                    //set address
                    if (!txtZip.Text.Equals("")) zip = Convert.ToInt32(txtZip.Text);
                    contact.setAddress(txtStreet.Text, txtCity.Text, ddlState.SelectedValue, zip);

                    contact.updateInDatabase();

                    if (pnlJudges.Visible && contact != null)
                    {
                        judge = new Judge(contact.id, contact.firstName,
                                          contact.middleInitial, contact.lastName,
                                          contact.email, contact.phone,
                                          contact.districtId, contact.contactTypeId,
                                          null, true);

                        if (judge.preferences != null)
                            UpdateJudgePreferences();
                        else
                        {
                            lblError.Text = "An error occurred while updating the judge's preferences.";
                            lblError.Visible = true;
                        }
                    }
                    else if (contact == null)
                    {
                        lblError.Text = "An error occurred while editing the contact.";
                        lblError.Visible = true;
                    }
                }
            }
            catch (Exception e)
            {
                lblError.Text = "An error occurred while editing the contact.";
                lblError.Visible = true;

                Utility.LogError("Manage Contacts", "EditExistingContact", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:  The contact must have judge as part of their contact type
         * Post: The preferences for the judge are updated
         */
        private void UpdateJudgePreferences()
        {
            JudgePreference[] prefArr = null;
            bool[] foundArr = null;

            try
            {
                //make arrays to track which preferences should be kept for the judge
                //and which ones should be deleted
                if (judge.preferences != null && judge.preferences.Count > 0)
                {
                    prefArr = new JudgePreference[judge.preferences.Count];
                    foundArr = new bool[judge.preferences.Count];

                    //add preferences to prefArr
                    prefArr = judge.preferences.ToArray();

                    //initialize found array to false
                    for (int i = 0; i < foundArr.Length; i++)
                        foundArr[i] = false;
                }

                //add audition track preferences
                foreach (ListItem item in chkLstTrack.Items)
                    HandleSpecificPreference(prefArr, foundArr, Utility.JudgePreferences.AuditionLevel, item);

                //add audition type preferences
                foreach (ListItem item in chkLstType.Items)
                    HandleSpecificPreference(prefArr, foundArr, Utility.JudgePreferences.AuditionType, item);

                //add composition level preferences
                foreach (ListItem item in chkLstCompLevel.Items)
                    HandleSpecificPreference(prefArr, foundArr, Utility.JudgePreferences.CompositionLevel, item);

                //add instrument preferences
                foreach (ListItem item in chkLstInstrument.Items)
                    HandleSpecificPreference(prefArr, foundArr, Utility.JudgePreferences.Instrument, item);

                //delete preferences that were not found
                if (prefArr != null)
                {
                    for (int i = 0; i < prefArr.Length; i++)
                    {
                        if (!foundArr[i])
                        {
                            //delete the preference, display an error if it isn't successful
                            if (!judge.deletePreference(prefArr[i].preferenceType, prefArr[i].preference))
                            {
                                lblError.Text = "An error occurred while removing a judge preference";
                                lblError.Visible = true;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                lblError.Text = "An error occurred while updating the judge preferences.";
                lblError.Visible = true;

                Utility.LogError("Manage Contacts", "UpdateJudgePreferences", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Either adds the input preference data to the current judge or marks
         *       it as being found in the list of the judge's current preferences
         * @param prefArr is an array containing the judges current preferences
         * @param foundArr is a boolean array with indexes matching the data in prefArr.
         */
        private void HandleSpecificPreference(JudgePreference[] prefArr, bool[] foundArr,
                                              Utility.JudgePreferences prefType, ListItem item)
        {
            string preference = item.Value;

            //if the item is selected and doesn't already exist, add it
            if (item.Selected)
            {
                JudgePreference pref = new JudgePreference(-1, prefType, preference);

                //if the preference doesn't exist, add it
                if (judge.preferences == null || !judge.preferences.Contains(pref))
                {
                    //add the new preference and display an error if it is not successful
                    if (!judge.addPreference(prefType, preference))
                    {
                        lblError.Text = "An error occurred while adding a judge preference.";
                        lblError.Visible = true;
                    }
                }
                //otherwise mark it as found in the prefArr
                else
                {
                    int idx = 0;
                    bool found = false;

                    //search for the preference and mark true if found
                    while (idx < prefArr.Length && !found)
                    {
                        if (prefArr[idx].Equals(pref))
                            foundArr[idx] = true;

                        idx = idx + 1;
                    }
                }
            }
        }

        /*
         * Pre:
         * Post: If a contact has been chosen the user is asked to confirm if they wish 
         *       to delete the contact and all associated information
         */
        private void DeleteContact()
        {
            //TODO

            try
            {
                pnlFullPage.Visible = false;
                pnlButtons.Visible = false;
                pnlAreYouSure.Visible = true;
                lblWarning.Text = "Are you sure that you want to permanently delete the data of " +
                                  txtFirstName.Text + " " + txtLastName.Text + "?  All of the contacts's " +
                                  "associated information will also be deleted.  This action cannot " +
                                  "be undone.";
            }
            catch (Exception e)
            {
                lblError.Text = "There was an error deleting the contact.";
                lblError.Visible = true;

                Utility.LogError("Manage Contacts", "DeleteContact", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:  A contact must have been selected previously
         * Post: The contact and all associated data is deleted from the system
         */
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string firstName, middleInitial, lastName;

            firstName = txtFirstName.Text;
            middleInitial = txtMiddleInitial.Text;

            /*if (!contact.deleteDatabaseInformation())
            {
                lblError.Text = "There was an error deleting the student's information";
                lblError.Visible = true;
            }*/
        }

        /*
         * Pre:
         * Post: Verifies that the information entered by the user 
         *       is in the correct format
         */
        private bool verifyInformation()
        {
            bool valid = true;
            int num;
            clearErrors();

            //make sure a first name was entered
            if (txtFirstName.Text.Equals(""))
            {
                lblFirstNameError.Visible = true;
                valid = false;
            }

            //make sure the middle name is not more than 2 characters
            if (txtMiddleInitial.Text.Length > 2)
            {
                lblMiddleNameError.Visible = true;
                valid = false;
            }

            //make sure a last name was entered
            if (txtLastName.Text.Equals(""))
            {
                lblLastNameError.Visible = true;
                valid = false;
            }

            //make sure the zipcode was left empty or is numeric
            if (!txtZip.Text.Equals(""))
            {
                if (txtZip.Text.Length == 5)
                {
                    if (!Int32.TryParse(txtZip.Text, out num))
                        lblZipErrorDesc.Visible = true;
                }
                else
                {
                    lblZipErrorDesc.Visible = true;
                }
            }

            //make sure a valid email address was entered 
            if (txtEmail.Text.Equals(""))
            {
                lblEmailError.Visible = true;
                valid = false;
            }
            else
            {
                char[] arr = txtEmail.Text.ToCharArray();

                int atPosition = 0, dotPosition = 0;

                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Equals('@'))
                        atPosition = i;
                    else if (arr[i].Equals('.'))
                        dotPosition = i;
                }

                if (atPosition == 0 || dotPosition == 0 || atPosition > dotPosition)
                {
                    lblEmailError.Visible = true;
                    valid = false;
                }
            }

            //make sure a phone number was entered 
            if (txtPhone.Text.Equals("") || txtPhone.Text.Length > 12)
            {
                lblPhoneError.Visible = true;
                valid = false;
            }

            //make sure a district is selected
            if (ddlDistrict.SelectedValue.ToString().Equals(""))
            {
                lblDistrictError.Visible = true;
                valid = false;
            }

            //make sure a contact type is selected
            if (ddlContactType.SelectedValue.ToString().Equals(""))
            {
                lblContactTypeError.Visible = true;
                valid = false;
            }

            if (!valid) lblError.Visible = true;

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
                if (userIsStateAdmin())  //if the user is a state admin, get all judges and self
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchJudgeContacts(gvSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, contactSearch))
                    {
                        lblContactSearchError.Visible = true;
                        lblContactSearchError.ForeColor = Color.DarkBlue;
                        lblContactSearchError.Text = "The search did not return any results";
                    }
                }
                else
                {
                    //if the search does not return any result, display a message saying so
                    if (!searchContacts(gvSearch, id, txtFirstNameSearch.Text, txtLastNameSearch.Text, contactSearch, getDistrictIdForPermissionLevel()))
                    {
                        lblContactSearchError.Visible = true;
                        lblContactSearchError.ForeColor = Color.DarkBlue;
                        lblContactSearchError.Text = "The search did not return any results";
                    }
                }
            }
            //if the id is not numeric, display a message
            else
            {
                clearGridView(gvSearch);
                lblContactSearchError.Visible = true;
                lblContactSearchError.ForeColor = Color.Red;
                lblContactSearchError.Text = "A Contact Id must be numeric";
            }
        }

        /*
         * Pre:
         * Post: If the current user's permission level should only allow them to edit/view contacts
         *       of a certain district, that district id is returned.  Otherwise -1 is returned.
         * @returns the district id that should be viewable by the current user or -1 for all districts
         */
        private int getDistrictIdForPermissionLevel()
        {
            User user = (User)Session[Utility.userRole];
            int districtId = -1;

            if (user.permissionLevel.Contains('D'))
                districtId = user.districtId;

            return districtId;
        }

        /*
         * Pre:
         * Post: Determines whether or not the current user is a state administrator
         * @returns true if the current user is a state admin and false otherwise
         */
        private bool userIsStateAdmin()
        {
            User user = (User)Session[Utility.userRole];
            bool result = false;

            if (user.permissionLevel.Contains('S'))
                result = true;

            return result;
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
                    lblContactSearchError.Text = "An error occurred.  Please make sure all entered data is valid.";
                    lblContactSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblContactSearchError.Text = "An error occurred.  Please make sure all entered data is valid.";
                lblContactSearchError.Visible = true;

                //log issue in database
                Utility.LogError("Manage Contacts", "searchContacts", gridView.ID + ", " + id + ", " + firstName + ", " + lastName + ", " + session, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
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
        private bool searchJudgeContacts(GridView gridView, string id, string firstName, string lastName, string session)
        {
            DataTable table;
            bool result = true;

            try
            {
                if (!id.Equals(""))
                    table = DbInterfaceContact.GetJudgeAndSelfSearchResults(id, firstName, lastName, false);
                else
                {
                    User user = (User)Session[Utility.userRole];

                    table = DbInterfaceContact.GetJudgeAndSelfSearchResults(user.contactId.ToString(), firstName, lastName, true);
                }

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
                    lblContactSearchError.Text = "An error occurred.  Please make sure all entered data is valid.";
                    lblContactSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblContactSearchError.Text = "An error occurred.  Please make sure all entered data is valid.";
                lblContactSearchError.Visible = true;

                //log issue in database
                Utility.LogError("Manage Contacts", "searchJudgeContacts", gridView.ID + ", " + id + ", " + firstName + ", " + lastName + ", " + session, "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
            lblContactSearchError.Visible = false;
            int index = gvSearch.SelectedIndex;
            clearData();

            if (index >= 0 && index < gvSearch.Rows.Count)
            {
                txtContactId.Text = gvSearch.Rows[index].Cells[1].Text;

                //get contact information using id
                loadContact(Convert.ToInt32(gvSearch.Rows[index].Cells[1].Text));

                //add empty option to contact type dropdown and make it selected value
                refreshContactTypeDropdown();
            }
        }

        private void refreshContactTypeDropdown()
        {
            User user = (User)Session[Utility.userRole];

            if ((user.permissionLevel.Contains('S') || user.permissionLevel.Contains('D')) && !user.permissionLevel.Contains('A'))
            {
                if (!ddlContactType.SelectedValue.Equals("J")) //if editing themselves, they can add or remove J from their contact type
                {
                    ListItem currPermission = ddlContactType.SelectedItem;
                    ListItem otherPermission = null;

                    if (ddlContactType.SelectedItem.Value.Contains('J'))
                        otherPermission = ddlContactType.Items.FindByValue(ddlContactType.SelectedItem.Value.Replace("J", ""));
                    else
                        otherPermission = ddlContactType.Items.FindByValue(ddlContactType.SelectedItem.Value + "J");

                    ddlContactType.Items.Clear();
                    ddlContactType.Items.Add(currPermission);
                    if (otherPermission != null) ddlContactType.Items.Add(otherPermission);
                }
                else //if editing a judge, cannot edit the contact type
                {
                    ListItem item = ddlContactType.Items.FindByValue("J");

                    ddlContactType.Items.Clear();
                    ddlContactType.Items.Add(item);
                }
            }
        }

        /*
         * Pre:  There must exist a contact with the input contact id
         * Post: The informatin of the contact with the input contact id is loaded to the page
         * @param contactId is the contact id of the contact to load
         */
        private void loadContact(int contactId)
        {
            Contact contact = new Contact(contactId);

            if (contact.id != -1)
            {
                Session[contactVar] = contact;

                //load contact data to form
                lblId.Text = contact.id.ToString();
                txtFirstName.Text = contact.firstName;
                txtFirstNameSearch.Text = contact.firstName;
                txtMiddleInitial.Text = contact.middleInitial;
                txtLastName.Text = contact.lastName;
                txtLastNameSearch.Text = contact.lastName;
                txtStreet.Text = contact.street;
                txtCity.Text = contact.city;

                if (!contact.state.Equals(""))
                    ddlState.SelectedValue = contact.state;
                else
                    ddlState.SelectedValue = "WI";

                if (contact.zip != -1) txtZip.Text = contact.zip.ToString();
                txtEmail.Text = contact.email;
                txtPhone.Text = contact.phone;

                //find district
                ListItem currItem = ddlDistrict.Items.FindByValue(contact.districtId.ToString());
                if (currItem != null)
                    ddlDistrict.SelectedIndex = ddlDistrict.Items.IndexOf(currItem);


                //rebind to all contact types
                ddlContactType.DataSourceID = null;
                ddlContactType.DataSource = SqlDataSource2;
                ddlContactType.DataBind();
                //find contact type
                currItem = ddlContactType.Items.FindByValue(contact.contactTypeId);
                if (currItem != null)
                    ddlContactType.SelectedIndex = ddlContactType.Items.IndexOf(currItem);

                //show or hide the judges panel based on whether or not the contact is a judge
                //if they are a judge, load their preferences
                if (contact.contactTypeId.Contains('J'))
                {
                    Judge judge = new Judge(contact.id, contact.firstName, contact.middleInitial,
                                            contact.lastName, contact.email, contact.phone,
                                            contact.districtId, contact.contactTypeId, null, true);
                    Session[judgeVar] = judge;
                    Session[contactVar] = (Contact)judge;

                    //load each preference
                    if (judge.preferences != null)
                    {
                        foreach (JudgePreference pref in judge.preferences)
                        {
                            int idx = -1;

                            //audition level
                            if (pref.preferenceType == Utility.JudgePreferences.AuditionLevel)
                            {
                                idx = chkLstTrack.Items.IndexOf(new ListItem(pref.preference));

                                if (idx >= 0) chkLstTrack.Items.FindByValue(pref.preference).Selected = true;
                            }
                            //audition type
                            else if (pref.preferenceType == Utility.JudgePreferences.AuditionType)
                            {
                                idx = chkLstType.Items.IndexOf(new ListItem(pref.preference));

                                if (idx >= 0) chkLstType.Items.FindByValue(pref.preference).Selected = true;
                            }
                            //composition level
                            else if (pref.preferenceType == Utility.JudgePreferences.CompositionLevel)
                            {
                                if (chkLstCompLevel.Items.Count == 0) chkLstCompLevel.DataBind();

                                ListItem temp = chkLstCompLevel.Items.FindByValue(pref.preference.Trim());

                                if (temp != null)
                                    chkLstCompLevel.Items.FindByValue(temp.Value).Selected = true;
                            }
                            //instrument
                            else if (pref.preferenceType == Utility.JudgePreferences.Instrument)
                            {
                                if (chkLstInstrument.Items.Count == 0) chkLstInstrument.DataBind();

                                idx = chkLstInstrument.Items.IndexOf(new ListItem(pref.preference));

                                if (idx >= 0) chkLstInstrument.Items.FindByValue(pref.preference).Selected = true;
                            }
                        }
                    }
                    else
                    {
                        lblError.Text = "An error occurred while loading the judge's preferences.";
                        lblError.Visible = true;
                    }

                    pnlJudges.Visible = true;
                }
                else
                    pnlJudges.Visible = false;
            }
            else
            {
                lblContactSearchError.Text = "An error occurred during the search";
                lblContactSearchError.Visible = true;
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
                Utility.LogError("Manage Contacts", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
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
                    cell.BackColor = Color.FromArgb(204, 204, 255);
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
            txtFirstName.Text = "";
            txtMiddleInitial.Text = "";
            txtLastName.Text = "";
            txtStreet.Text = "";
            txtCity.Text = "";
            ddlState.SelectedValue = "WI";
            txtZip.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            ddlDistrict.SelectedIndex = 0;
            ddlContactType.SelectedIndex = 0;

            lblId.Text = "";

            btnClear.Visible = true;
            clearErrors();

            //clear preferences and hide judge panel
            pnlJudges.Visible = false;

            foreach (ListItem item in chkLstType.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstTrack.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstCompLevel.Items)
                item.Selected = false;
            foreach (ListItem item in chkLstInstrument.Items)
                item.Selected = false;

            Session[contactSearch] = null;
            Session[contactVar] = null;
            Session[judgeVar] = null;
        }

        /*
         * Pre:
         * Post: Clears error messages and highlighting on the page
         */
        private void clearErrors()
        {
            lblError.Text = "";
            lblContactSearchError.Visible = false;
            lblFirstNameError.Visible = false;
            lblMiddleNameError.Visible = false;
            lblLastNameError.Visible = false;
            lblZipErrorDesc.Visible = false;
            lblEmailError.Visible = false;
            lblEmailErrorDesc.Visible = false;
            lblPhoneError.Visible = false;
            lblPhoneErrorDesc.Visible = false;
            lblDistrictError.Visible = false;
            lblContactTypeError.Visible = false;
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
            lblContactSearchError.Visible = false;
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
         * Post: Initialize the page for adding, editing, or deleting
         *       based on user selection
         */
        protected void btnGo_Click(object sender, EventArgs e)
        {
            pnlSuccess.Visible = false;
            pnlButtons.Visible = true;

            if (ddlUserOptions.SelectedValue.Equals("Create New"))
            {
                Session[creatingNew] = true;
                Session[deleting] = false;
                pnlFullPage.Visible = true;
                pnlContactSearch.Visible = false;
                header.InnerText = "Add New Contact";
                btnAdd.Text = "Add";
                enableControls();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Edit Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = false;
                pnlContactSearch.Visible = true;
                pnlFullPage.Visible = true;
                header.InnerText = "Edit Existing Contact";
                btnAdd.Text = "Submit";
                enableControls();
                setPermissionFunctionalities();
            }
            else if (ddlUserOptions.SelectedValue.Equals("Delete Existing"))
            {
                Session[creatingNew] = false;
                Session[deleting] = true;
                pnlContactSearch.Visible = true;
                pnlFullPage.Visible = true;
                header.InnerText = "Delete Existing Contact";
                btnAdd.Text = "Submit";
                disableControls();
            }
        }

        /*
         * Pre:
         * Post:  If the user is a teacher, load their data only.  They should only be able to add or remove
         *        the J from their permission level and they may not change their dsitrict
         */
        private void setPermissionFunctionalities()
        {
            //if the user does not have permissions higher than Teacher, load their data and hide search 
            User user = (User)Session[Utility.userRole];
            if (!(user.permissionLevel.Contains('A') || user.permissionLevel.Contains("S") || user.permissionLevel.Contains("D")) && user.permissionLevel.Contains("T"))
            {
                pnlContactSearch.Visible = false;

                ddlContactType.DataBind();
                ddlDistrict.DataBind();
                loadContact(user.contactId);
                updateAvailableContactTypes();

                //only show current user's district
                ListItem districtItem = ddlDistrict.Items.FindByValue(user.districtId.ToString());
                ddlDistrict.DataSource = null;
                ddlDistrict.DataBind();
                ddlDistrict.Items.Clear();
                ddlDistrict.Items.Add(districtItem);

            }
            //if the user is a district admin, they can edit anyone in their district
            else if (user.permissionLevel.Contains('D') && !(user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
            {
                ddlDistrict.DataBind();

                //only show current user's district
                ListItem districtItem = ddlDistrict.Items.FindByValue(user.districtId.ToString());
                ddlDistrict.DataSource = null;
                ddlDistrict.DataBind();
                ddlDistrict.Items.Clear();
                ddlDistrict.Items.Add(districtItem);
            }
        }

        /* Pre:  
         * Post: The contact types that can be changed to are avaiable in the dropdown and
         *       all others are removed
         */
        private void updateAvailableContactTypes()
        {
            User user = (User)Session[Utility.userRole];

            //if not permissions higher than teacher, include current permission level with and without judge option
            if (user.permissionLevel.Contains('T') && !(user.permissionLevel.Contains('D') || user.permissionLevel.Contains('S') || user.permissionLevel.Contains('A')))
            {
                ListItem currPermission = ddlContactType.Items.FindByValue(user.permissionLevel);
                ListItem otherPermission = null;

                //if current contact type contains judge, get corresponding type without judge
                if (user.permissionLevel.Contains('J'))
                    otherPermission = ddlContactType.Items.FindByValue(user.permissionLevel.Replace("J", ""));
                //if current contact type doesn't contain judge, get corresponding type with judge
                else
                    otherPermission = ddlContactType.Items.FindByValue(user.permissionLevel + "J");

                //empty contact type dropdown
                ddlContactType.DataSource = null;
                ddlContactType.DataBind();
                ddlContactType.Items.Clear();

                //add available types
                ddlContactType.Items.Add(currPermission);
                if (otherPermission != null) ddlContactType.Items.Add(otherPermission);
            }
            else if (user.permissionLevel.Contains("S") && !user.permissionLevel.Contains('A'))
            {
                //remove system administrator option
                ddlContactType.DataBind();
                ddlContactType.Items.Remove(ddlContactType.Items.FindByValue("A"));
            }
            //if the user is a district admin, they can edit anyone in their district
            else if (user.permissionLevel.Contains("D") && !user.permissionLevel.Contains('A'))
            {
                ddlContactType.DataBind();

                //remove all system admin and state admin permission types
                int i = 0;
                while (i < ddlContactType.Items.Count)
                {
                    if (ddlContactType.Items[i].Value.Contains('A') || ddlContactType.Items[i].Value.Contains('S'))
                        ddlContactType.Items.RemoveAt(i);
                    else
                        i++;
                }
            }
        }

        /*
         * Pre:
         * Post: All controls on the page are disabled
         */
        private void disableControls()
        {
            txtFirstName.Enabled = false;
            txtMiddleInitial.Enabled = false;
            txtLastName.Enabled = false;
            txtStreet.Enabled = false;
            txtCity.Enabled = false;
            ddlState.Enabled = false;
            txtZip.Enabled = false;
            txtEmail.Enabled = false;
            txtPhone.Enabled = false;
        }

        /*
         * Pre:
         * Post: All controls on the page are enabled
         */
        private void enableControls()
        {
            txtFirstName.Enabled = true;
            txtMiddleInitial.Enabled = true;
            txtLastName.Enabled = true;
            txtStreet.Enabled = true;
            txtCity.Enabled = true;
            ddlState.Enabled = true;
            txtZip.Enabled = true;
            txtEmail.Enabled = true;
            txtPhone.Enabled = true;
        }

        /*
         * Pre:
         * Post: The user is redirected to the Welcome Screen
         */
        protected void btnMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: No contact information is changed and the user is brought back
         *       to the main screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: No additional changes are made and the user is brought 
         *       back to the welcome/option screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /** clear errors **/
        protected void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            lblFirstNameError.Visible = false;
        }
        protected void txtMiddleInitial_TextChanged(object sender, EventArgs e)
        {
            lblMiddleNameError.Visible = false;
        }
        protected void txtLastName_TextChanged(object sender, EventArgs e)
        {
            lblLastNameError.Visible = false;
        }
        protected void ddlDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDistrictError.Visible = false;
        }

        /*
         * Show the contact preferences if a type containing a J (for Judge) is chosen
         * and hide the preferences otherwise
         */
        protected void ddlContactType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblContactTypeError.Visible = false;

            if (ddlContactType.SelectedValue.Contains('J'))
                pnlJudges.Visible = true;
            else
                pnlJudges.Visible = false;
        }

        /*
         * Make sure the zip code is empty or valid
         */
        protected void txtZip_TextChanged(object sender, EventArgs e)
        {
            int num;

            lblZipErrorDesc.Visible = false;

            if (!txtZip.Text.Equals(""))
            {
                if (txtZip.Text.Length == 5)
                {
                    if (!Int32.TryParse(txtZip.Text, out num)) lblZipErrorDesc.Visible = true;
                }
                else
                    lblZipErrorDesc.Visible = true;
            }
        }

        /*
         * Make sure email address includes @ and .
         */
        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            lblEmailError.Visible = false;
            lblEmailErrorDesc.Visible = false;

            if (!txtEmail.Text.Equals("") && (!txtEmail.Text.Contains('@') || !txtEmail.Text.Contains('.')))
            {
                lblEmailErrorDesc.Text = "Please enter a valid email address.";
                lblEmailErrorDesc.Visible = true;
            }
        }

        /*
         * change the phone number to XXX-XXX-XXXX format
         */
        protected void txtPhone_TextChanged(object sender, EventArgs e)
        {
            string phone = txtPhone.Text, newPhone;
            long num;

            //exit if phone number is empty
            if (phone.Equals("")) return;

            lblPhoneError.Visible = false;
            lblPhoneErrorDesc.Visible = false;

            //get rid of all white-space, parenthesis, and dashes
            phone = phone.Trim();
            phone = phone.Replace("(", "");
            phone = phone.Replace(")", "");
            phone = phone.Replace(" ", "");
            phone = phone.Replace("-", "");

            //make sure there are 10 integers remaining
            if (phone.Length != 10 || !long.TryParse(phone, out num))
            {
                lblPhoneErrorDesc.Text = "Please enter a phone number with area code.";
                lblPhoneErrorDesc.Visible = true;
            }
            //add dashes and display reformatted phone number
            else
            {
                newPhone = phone.Substring(0, 3) + "-" + phone.Substring(3, 3) + "-" +
                           phone.Substring(6, 4);
                txtPhone.Text = newPhone;
            }
        }

        /*
         * Pre:
         * Post: The contact is not deleted and the confirmation panel is hidden
         */
        protected void btnCancelDelete_Click(object sender, EventArgs e)
        {
            pnlFullPage.Visible = true;
            pnlButtons.Visible = true;
            pnlAreYouSure.Visible = false;
        }

        protected void ddlState_DataBound(object sender, EventArgs e)
        {
            ddlState.SelectedValue = "WI";
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("Manage Contacts", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblError.Text = "An error occurred";
            lblError.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}