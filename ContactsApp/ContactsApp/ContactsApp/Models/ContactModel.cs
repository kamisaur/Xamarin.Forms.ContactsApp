using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    public class ContactModel
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public ContactModel(
            string id,
            string firstName,
            string lastName, 
            string phoneNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public ContactModel()
        {
        }
    }
}
