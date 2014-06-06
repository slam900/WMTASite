using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * Author: Krista Schultz
 * Date: February 2013
 * This is an abstract class representing a person in the system, such as a Student, 
 * Teacher, or Judge.
 */ 
public abstract class Person
{
    public int id { get; set; }
    public string firstName { get; set; }
    public string middleInitial { get; set; }
    public string lastName { get; set; }

	public Person(int id)
	{
        this.id = id;
        firstName = "";
        lastName = "";
        middleInitial = "";
	}

    public Person(int id, string firstName, string middleInitial, string lastName)
    {
        this.id = id;
        this.firstName = firstName;
        this.middleInitial = middleInitial;
        this.lastName = lastName;
    }
}