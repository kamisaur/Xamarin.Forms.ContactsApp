﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    internal class ContactModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public ContactModel(
            string id,
            string displayName, 
            string firstName,
            string lastName, 
            string phoneNumber)
        {
            Id = id;
            DisplayName = displayName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }
    }
}
