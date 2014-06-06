using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   12/16/2013
 * This class represents a user of the system
 */
public class User
{
    public string mtnaId { get; private set; }
    public string firstInitial { get; private set; }
    public string lastName { get; private set; }
    public int contactId { get; set; }
    public string permissionLevel { get; set; }
    public int districtId { get; set; }

	public User(string firstInitial, string lastName, string mtnaId)
	{
        this.firstInitial = firstInitial;
        this.lastName = lastName;
        this.mtnaId = mtnaId;

        login();
	}

    /*
     * Pre:
     * Post: The permission information is retrieved for the user with
     *       the current first initial, last name, and MTNA Id
     */
    private void login()
    {
        DbInterfaceContact.ContactLogin(this);
    }
}