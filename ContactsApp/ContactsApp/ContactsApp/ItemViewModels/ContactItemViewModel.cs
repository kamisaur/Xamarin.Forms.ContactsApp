using ContactsApp.Models;
using ContactsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.ItemViewModels
{
    internal class ContactItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public ContactItemViewModel(
            int id, 
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
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            PhoneNumber = contact.PhoneNumber;
        }
    }
}
