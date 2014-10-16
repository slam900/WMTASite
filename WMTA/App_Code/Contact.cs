using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date:   August 2013
 * This class represents a Contact (teach, judge, DA, etc.) in the system
 */
public class Contact : Person
{
    public string contactTypeId { get; set; }
    public string street { get; protected set; }
    public string city { get; protected set; }
    public string state { get; protected set; }
    public string phone { get; set; }
    public string email { get; set; }
    public int districtId { get; set; }
    public int zip { get; protected set; }

    /*
     * Constructor for creating a new contact
     */
	public Contact(string firstName, string mi, string lastName, string email, string phone,
                   int districtId, string contactTypeId) : 
                   base(-1, firstName, mi, lastName)
	{
        this.email = email;
        this.phone = phone;
        this.districtId = districtId;
        this.contactTypeId = contactTypeId;
	}

    /*
     * Constructor for an existing contact
     */
    public Contact(int id,string firstName, string mi, string lastName, string email,
                   string phone, int districtId, string contactTypeId) :
                   base(id, firstName, mi, lastName)
    {
        this.email = email;
        this.phone = phone;
        this.districtId = districtId;
        this.contactTypeId = contactTypeId;
    }

    /*
     * Constructor for loading the data of an existing contact
     */
    public Contact(int id, bool needInfo) : base(id)
    {
        Contact temp = null;

        if (needInfo)
            temp = DbInterfaceContact.GetContact(id);

        if (temp != null)
        {
            this.id = id;
            firstName = temp.firstName;
            middleInitial = temp.middleInitial;
            lastName = temp.lastName;
            contactTypeId = temp.contactTypeId;
            phone = temp.phone;
            email = temp.email;
            setAddress(temp.street, temp.city, temp.state, temp.zip);
            districtId = temp.districtId;
        }
        else if (needInfo)
        {
            this.id = -1;
        }
        else if (!needInfo)
        {
            this.id = id;
        }
    }

    /*
     * Pre:  The contact must have a first and last name, contact type, phone
     *       number, and email address
     * Post: The new contact is added to the database
     */
    public void addToDatabase()
    {
        id = DbInterfaceContact.CreateContact(this);
    }

    /*
     * Pre:  The contact information must have been previously entered
     *       into the database, meaning that there is a unique id
     * Post: The contact's information is updated
     */
    public void updateInDatabase()
    {
        DbInterfaceContact.UpdateContact(this);
    }

    /*
     * Pre: 
     * Post: The contact and all associated information is deleted
     *       from the database
     * @returns true if the contact was deleted and false otherwise
     */
    public bool deleteInDatabase()
    {
        return DbInterfaceContact.DeleteContact(id);
    }

    /*
     * Pre:
     * Post: Determines whether or not the contact is associated with
     *       any students
     * @returns true if there are associated students and false otherwise
     */
    public bool hasStudents()
    {
        return DbInterfaceContact.ContactHasStudents(id);
    }

    /*
     * Pre:
     * Post: Set the contact's address
     */
    public void setAddress(string street, string city, string state, int zip)
    {
        this.street = street;
        this.city = city;
        this.state = state;
        this.zip = zip;
    }

    public override bool Equals(object obj)
    {
        return ((Contact)obj).id == id;
    }
}