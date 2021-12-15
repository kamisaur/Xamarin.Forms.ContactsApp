using ContactsApp.Models;
using ContactsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.ItemViewModels
{
    internal class ContactItemViewModel : BaseViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public ContactItemViewModel(
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

        public ContactItemViewModel(ContactModel contact)
        {
            Id = contact.Id;
            DisplayName = contact.DisplayName;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            PhoneNumber = contact.PhoneNumber;
        }
    }
}
