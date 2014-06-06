using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WMTA
{
    public partial class DistrictPointEntry : System.Web.UI.Page
    {
        DistrictAudition audition;
        //session variables
        private string auditionVar = "Audition";
        private string studentSearch = "StudentData";
        private string compositionTable = "CompositionTable";

        protected void Page_Load(object sender, EventArgs e)
        {
            checkPermissions();

            //clear session variables
            if (!Page.IsPostBack)
            {
                Session[auditionVar] = null;
                Session[studentSearch] = null;
                Session[compositionTable] = null;
                loadYearDropdown();
            }

            //if an audition object has been instantiated, reload
            if (Page.IsPostBack && Session[auditionVar] != null)
            {
                audition = (DistrictAudition)Session[auditionVar];
            }

            //if there were compositions selected before the postback, add them 
            //back to the table
            if (Page.IsPostBack && Session[compositionTable] != null)
            {
                TableRow[] rowArray = (TableRow[])Session[compositionTable];

                for (int i = 1; i < rowArray.Length; i++)
                    tblCompositions.Rows.Add(rowArray[i]);
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

                if (!(user.permissionLevel.Contains("D") || user.permissionLevel.Contains("A")))
                    Response.Redirect("/WelcomeScreen.aspx");
            }
        }

        /*
         * Pre:
         * Post: Loads the appropriate years in the dropdown
         */
        private void loadYearDropdown()
        {
            int firstYear = DbInterfaceStudentAudition.GetFirstAuditionYear();

            for (int i = DateTime.Now.Year; i >= firstYear; i--)
                ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        /*
         * Pre:   The StudentId field must be empty or contain an integer
         * Post:  Students are displayed that match the search criteria (student id, first name, and last name).
         *        The error message is also reset.
         */
        protected void btnStudentSearch_Click(object sender, EventArgs e)
        {
            string id = txtStudentId.Text;
            int num;
            bool isNum = int.TryParse(id, out num);

            lblStudentSearchError.Text = "";

            if (isNum || txtStudentId.Text.Equals(""))
            {
                User user = (User)Session[Utility.userRole];
                int districtId = -1;

                //if user is district admin get their district
                if (!user.permissionLevel.Contains('A'))
                    districtId = user.districtId;

                searchStudents(gvStudentSearch, txtStudentId.Text, txtFirstName.Text, txtLastName.Text, studentSearch, districtId);
            }
            else
            {
                clearGridView(gvStudentSearch);
                lblStudentSearchError.ForeColor = Color.Red;
                lblStudentSearchError.Text = "A Student Id must be numeric";
            }

            cboAudition.Items.Clear();
            cboAudition.Items.Add(new ListItem("", ""));
        }

        /*
         * Pre:  id must be an integer or the empty string
         * Post:  The input parameters are used to search for existing students.  Matching student 
         *        information is displayed in the input gridview.
         * @param gridView is the gridView in which the search results will be displayed
         * @param id is the id being searched for - must be an integer or the empty string
         * @param firstName is all or part of the first name being searched for
         * @param lastName is all or part of the last name being searched for
         * @param session is the name of the session variable storing the student search table data
         * @districtId is the district in which to search for students, -1 indicates all districts
         * @returns true if results were found and false otherwise
         */
        private bool searchStudents(GridView gridView, string id, string firstName, string lastName, string session, int districtId)
        {
            bool result = true;

            try
            {
                DataTable table = DbInterfaceStudent.GetStudentSearchResults(id, firstName, lastName, districtId);

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
                    lblStudentSearchError.Text = "An error occurred during the search";
                    lblStudentSearchError.Visible = true;
                }
            }
            catch (Exception e)
            {
                lblStudentSearchError.Text = "An error occurred during the search";
                lblStudentSearchError.Visible = true;

                Utility.LogError("District Point Entry", "searchStudents", "gridView: " + gridView.ID + ", id: " + id +
                                 ", firstName: " + firstName + ", lastName: " + lastName + ", session: " + session,
                                 "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }

            return result;
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
         * Post: The table in the input is saved to a session variable
         * @table is the table being saved
         * @session is the name of the session variable
         */
        private void saveTableToSession(Table table, string session)
        {
            TableRow[] rowArray = new TableRow[table.Rows.Count];
            table.Rows.CopyTo(rowArray, 0);
            Session[session] = rowArray;
        }

        /*
         * Pre:   The selected index must be a positive number less than the number of rows
         *        in the gridView
         * Post:  The information for the selected student is loaded to the page
         */
        protected void gvStudentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlInfo.Visible = true;
            lblAuditionError.Visible = false;
            clearAllExceptSearch();

            int index = gvStudentSearch.SelectedIndex;

            if (index >= 0 && index < gvStudentSearch.Rows.Count)
            {
                string id = gvStudentSearch.Rows[index].Cells[1].Text;

                txtStudentId.Text = id;
                lblStudId.InnerText = id;

                loadStudentData(Convert.ToInt32(gvStudentSearch.Rows[index].Cells[1].Text));
            }
        }

        /*
         * Pre:  studentId must exist as a StudentId in the system
         * Post: The existing data for the student associated to the studentId 
         *       is loaded to the page.
         * @param studentId is the StudentId of the student being registered
         */
        private Student loadStudentData(int studentId)
        {
            Student student = DbInterfaceStudent.LoadStudentData(studentId);

            //get eligible auditions
            if (student != null)
            {
                DataTable table = DbInterfaceStudentAudition.GetDistrictAuditionsForDropdownByYear(student, Convert.ToInt32(ddlYear.SelectedValue));
                cboAudition.DataSource = null;
                cboAudition.Items.Clear();
                cboAudition.DataSourceID = "";

                //load student name
                txtFirstName.Text = student.firstName;
                txtLastName.Text = student.lastName;
                lblStudent.Text = student.firstName + " " + student.lastName;

                if (table.Rows.Count > 0)
                {
                    cboAudition.DataSource = table;
                    cboAudition.DataTextField = "DropDownInfo";
                    cboAudition.DataValueField = "AuditionId";
                    cboAudition.Items.Add(new ListItem(""));
                    cboAudition.DataBind();
                }
                else
                {
                    lblAuditionError.InnerText = "This student has no district auditions to award points to for the selected year";
                    lblAuditionError.Visible = true;
                }
            }
            else
            {
                lblErrorMsg.Text = "An error occurred while loading the student's audition data";
                lblErrorMsg.Visible = true;
            }

            return student;
        }

        /*
         * Pre: 
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        protected void btnClearStudentSearch_Click(object sender, EventArgs e)
        {
            clearStudentSearch();
        }

        /*
         * Pre: 
         * Post:  The three text boxes in the Student Search section and the
         *        search result in the gridview are cleared
         */
        private void clearStudentSearch()
        {
            txtStudentId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gvStudentSearch.DataSource = null;
            gvStudentSearch.DataBind();
            lblStudentSearchError.Text = "";
        }

        /*
         * Pre:
         * Post: Clears all data except student search
         */
        private void clearAllExceptSearch()
        {
            clearErrorMessages();
            ddlYear.SelectedIndex = 0;

            lblStudent.Text = "";
            lblStudId.InnerText = "";
            lblDuetPartnerInfo.Visible = false;
            txtTheoryPoints.Text = "";
            clearCompositions();

            if (cboAudition.Items.Count > 0)
                cboAudition.SelectedIndex = 0;

            lblPoints.InnerText = "0";
        }

        /*
         * Pre:
         * Post: Clears the error messages on the page
         */
        private void clearErrorMessages()
        {
            lblStudentSearchError.Visible = false;
            lblChooseAudition.Visible = false;
            lblStudentError.Visible = false;
            lblErrorMsg.Visible = false;
            lblErrorMsg.Text = "**Errors on page**";
            lblAuditionError.Visible = false;
            lblTheoryPointsError.Visible = false;
            lblPointError.Visible = false;
        }

        /*
         * Pre:
         * Post: Clears the compositions and points table
         */
        private void clearCompositions()
        {
            //clear the compositions saved in the table
            while (tblCompositions.Rows.Count > 1)
                tblCompositions.Rows.Remove(tblCompositions.Rows[tblCompositions.Rows.Count - 1]);


            Session[compositionTable] = null;
        }

        /*
         * Pre:   gvStudentSearch must contain more than one page
         * Post:  The page of gvStudentSearch is changed
         */
        protected void gvStudentSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStudentSearch.PageIndex = e.NewPageIndex;
            BindSessionData();
        }

        /*
         * Pre:
         * Post:  The color of the header row is set
         */
        protected void gvStudentSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in gvStudentSearch.HeaderRow.Cells)
                    cell.BackColor = Color.FromArgb(204, 204, 255);
            }
        }

        /*
         * Pre:   The StudentData table must have been previously defined
         * Post:  The stored data is bound to the gridView
         */
        protected void BindSessionData()
        {
            try
            {
                DataTable data = (DataTable)Session["StudentData"];
                gvStudentSearch.DataSource = data;
                gvStudentSearch.DataBind();
            }
            catch (Exception e)
            {
                Utility.LogError("District Point Entry", "BindSessionData", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre: 
         * Post: If a student is selected, the auditions are retrieve for that student
         *       for the selected year
         */
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!lblStudId.InnerText.Equals(""))
                loadStudentData(Convert.ToInt32(lblStudId.InnerText));
        }

        /*
         * Pre:
         * Post: Load the audition information associated with the selected audition
         */
        protected void cboAudition_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAuditionError.Visible = false;
            lblTheoryPointsError.Visible = false;

            if (!cboAudition.SelectedValue.ToString().Equals("") && !lblStudId.InnerText.Equals(""))
                resetAuditionVar();
        }

        /*
         * Pre:
         * Post: Loads the information of the selected audition and saves it to a session variable
         */
        private void resetAuditionVar()
        {
            try
            {
                int auditionId = Convert.ToInt32(cboAudition.SelectedValue);
                int studentId = Convert.ToInt32(Convert.ToInt32(txtStudentId.Text));
                Student student = DbInterfaceStudent.LoadStudentData(studentId);

                //get all audition info associated with audition id and save as session variable
                if (student != null)
                {
                    audition = DbInterfaceStudentAudition.GetStudentDistrictAudition(auditionId, student);

                    if (audition != null)
                    {
                        Session[auditionVar] = audition;

                        //if the audition was a duet, show label to inform user that the points for the
                        //partner will also be updated
                        if (audition.auditionType.ToUpper().Equals("DUET"))
                            lblDuetPartnerInfo.Visible = true;
                        else
                            lblDuetPartnerInfo.Visible = false;

                        loadCompositions();
                        txtTheoryPoints.Text = audition.theoryPoints.ToString();
                        calculatePointTotal();
                    }
                }
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred";
                lblErrorMsg.Visible = true;

                Utility.LogError("District Point Entry", "resetAuditionVar", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Sets the points and point control values for the selected audition
         */
        private void loadCompositions()
        {
            //clear current compositions
            while (tblCompositions.Rows.Count > 1)
                tblCompositions.Rows.RemoveAt(1);

            Session[compositionTable] = null;

            foreach (AuditionCompositions currComp in audition.compositions)
            {
                TableRow row = new TableRow();
                TableCell idCell = new TableCell();
                TableCell compCell = new TableCell();
                TableCell pointCell = new TableCell();
                TextBox txtBox = new TextBox();

                //set text box to accept only numbers
                txtBox.TextMode = TextBoxMode.Number;
                txtBox.Text = currComp.points.ToString();
                txtBox.Width = 45;

                //add text box to point cell
                pointCell.Controls.Add(txtBox);

                //save the composition id in an invisible cell
                idCell.Text = currComp.composition.compositionId.ToString();
                idCell.Visible = false;

                //set composition title 
                compCell.Text = currComp.composition.title;

                //add new row to table
                row.Cells.Add(idCell);
                row.Cells.Add(compCell);
                row.Cells.Add(pointCell);
                tblCompositions.Rows.Add(row);
            }

            //save table to session variable as an array
            saveTableToSession(tblCompositions, compositionTable);
        }

        /*
         * Pre:  The point fields must contain integers
         * Post: The audition point total is calculated and displayed
         */
        private void calculatePointTotal()
        {
            int total = 0;
            string points = "0";

            //add composition points
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                points = ((TextBox)tblCompositions.Rows[i].Cells[2].Controls[0]).Text;

                if (!points.Equals(""))
                    total = total + Convert.ToInt32(points);
            }

            //add theory points
            if (!txtTheoryPoints.Text.Equals(""))
                total = total + Convert.ToInt32(txtTheoryPoints.Text);

            //display total
            lblPoints.InnerText = total.ToString();
        }

        /*
         * Pre:
         * Post: If all data is valid, the point data is updated
         */
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            clearErrorMessages();

            if (dataIsValid())
            {
                if (audition == null) resetAuditionVar();

                setAuditionPoints();

                if (audition.submitPoints())
                    displaySuccessMessageAndOptions();
                else
                {
                    lblErrorMsg.Text = "An error occurred";
                    lblErrorMsg.Visible = true;
                }
            }
        }

        /*
         * Pre:
         * Post: The audition's points are set
         */
        private void setAuditionPoints()
        {
            try
            {
                int compId, points;

                //update points for each composition
                for (int i = 1; i < tblCompositions.Rows.Count; i++)
                {
                    compId = Convert.ToInt32(tblCompositions.Rows[i].Cells[0].Text);
                    points = Convert.ToInt32(((TextBox)tblCompositions.Rows[i].Cells[2].Controls[0]).Text);

                    audition.setCompositionPoints(compId, points);
                }

                //update theory points
                audition.theoryPoints = Convert.ToInt32(txtTheoryPoints.Text);
            }
            catch (Exception e)
            {
                lblErrorMsg.Text = "An error occurred";
                lblErrorMsg.Visible = false;

                Utility.LogError("District Point Entry", "setAuditionPoints", "", "Message: " + e.Message + "   Stack Trace: " + e.StackTrace, -1);
            }
        }

        /*
         * Pre:
         * Post: Indicates whether or not the data on the page is
         *       valid and complete
         * @returns true if the data is complete and valid and false otherwise
         */
        private bool dataIsValid()
        {
            bool isValid = true, isInt = true;
            int num;

            //make sure student is chosen
            if (lblStudId.InnerText.Equals(""))
            {
                isValid = false;
                lblStudentError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //make sure audition is chosen
            if (cboAudition.SelectedIndex == 0)
            {
                isValid = false;
                lblChooseAudition.Visible = true;
                lblErrorMsg.Visible = true;
            }

            //make sure each composition has a value from 0-5
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                isInt = Int32.TryParse(((TextBox)tblCompositions.Rows[i].Cells[2].Controls[0]).Text, out num);

                if (!isInt || num < 0 || num > 5)
                {
                    isValid = false;
                    lblPointError.Visible = true;
                    lblErrorMsg.Visible = true;
                }
            }

            //make sure theory points are in the range from 0-5
            isInt = Int32.TryParse(txtTheoryPoints.Text, out num);
            if (!isInt || num < 0 || num > 5)
            {
                isValid = false;
                lblTheoryPointsError.Visible = true;
                lblErrorMsg.Visible = true;
            }

            return isValid;
        }

        /*
         * Pre:
         * Post: All controls are hidden, the user is told that the points were entered,
         *       and is given the options to add additional points or go back to
         *       the menu/welcome page
         */
        private void displaySuccessMessageAndOptions()
        {
            pnlSuccess.Visible = true;
            pnlFullPage.Visible = false;

            if (audition.auditionType.ToUpper().Equals("DUET"))
                lblSuccess.Text = "The points of the student and their duet partner were successfully entered";
            else
                lblSuccess.Text = "The student's points were successfully entered";
        }

        /*
         * Pre:
         * Post: Clears the screen to allow a user to add additional points
         */
        protected void btnNew_Click(object sender, EventArgs e)
        {
            clearPage();
            pnlSuccess.Visible = false;
            pnlFullPage.Visible = true;
            btnSubmit.Text = "Submit";
        }

        /*
         * Pre:
         * Post: Clears data on the page
         */
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clearPage();
        }

        /*
         * Pre:
         * Post: Clears data on the page
         */
        private void clearPage()
        {
            clearErrorMessages();
            clearStudentSearch();
            ddlYear.SelectedIndex = 0;

            //pnlInfo.Visible = false;
            lblStudent.Text = "";
            lblStudId.InnerText = "";
            lblDuetPartnerInfo.Visible = false;
            clearCompositions();

            //clear audition dropdown
            if (cboAudition.Items.Count > 0)
            {
                cboAudition.Items.Clear();
                cboAudition.Items.Add(new ListItem("", ""));
            }

            txtTheoryPoints.Text = "0";
            lblPoints.InnerText = "0";
        }
        /*
         * Pre:
         * Post: No new changes are submitted and the user is redirected
         *       to the welcome screen
         */
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:
         * Post: Redirect the user to the welcome screen
         */
        protected void btnBackOption_Click(object sender, EventArgs e)
        {
            Response.Redirect("/WelcomeScreen.aspx");
        }

        /*
         * Pre:  The field must contain an integer
         * Post: The new point total is displayed
         */
        protected void txtTheoryPoints_TextChanged(object sender, EventArgs e)
        {
            int points;

            if (Int32.TryParse(txtTheoryPoints.Text, out points))
            {
                int test = tblCompositions.Rows.Count;
                lblTheoryPointsError.Visible = false;

                //make sure value is in valid range
                if (points < 0 || points > 5)
                {
                    txtTheoryPoints.Text = "";
                    lblTheoryPointsError.Visible = true;
                }

                calculatePointTotal();
            }
            else
            {
                txtTheoryPoints.Text = "";
                lblTheoryPointsError.Visible = true;
            }
        }

        /*
         * Pre:  Each point field must contain an integer
         * Post: The new point total is displayed
         */
        protected void btnUpdatePoints_Click(object sender, EventArgs e)
        {
            int tempPoints = 0;
            string value = "";
            bool valid = true;

            lblPointError.Visible = false;

            //make sure all values are valid
            for (int i = 1; i < tblCompositions.Rows.Count; i++)
            {
                value = ((TextBox)tblCompositions.Rows[i].Cells[2].Controls[0]).Text;

                if (!Int32.TryParse(value, out tempPoints) || (tempPoints < 0 || tempPoints > 5))
                {
                    lblPointError.Visible = true;
                    valid = false;
                }
            }

            //if all entered values are valid, update the point total
            if (valid)
                calculatePointTotal();

            //save composition table to session variable
            saveTableToSession(tblCompositions, compositionTable);
        }

        /*
         * Catch unhandled exceptions, add information to error log
         */
        protected override void OnError(EventArgs e)
        {
            //Get last error from the server
            Exception exc = Server.GetLastError();

            //log exception
            Utility.LogError("District Point Entry", "OnError", "", "Message: " + exc.Message + "   Stack Trace: " + exc.StackTrace, -1);

            //show error label
            lblErrorMsg.Text = "An error occurred";
            lblErrorMsg.Visible = true;

            //Pass error on to error page
            Server.Transfer("ErrorPage.aspx", true);
        }
    }
}