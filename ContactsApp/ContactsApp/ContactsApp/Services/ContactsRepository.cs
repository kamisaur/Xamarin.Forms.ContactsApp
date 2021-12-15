using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Services
{
    internal class ContactsRepository : IContactsRepository
    {
        public IList<ContactModel> GetContacts()
        {
            var contacts = new List<ContactModel>
            {
                new ContactModel(0, "Jack", "Nicholson", "123123123"),
                new ContactModel(1, "Johnny", "Depp", "222333111"),
                new ContactModel(2, "Armand", "Third", "999888777"),
                new ContactModel(3, "Clint", "Eastwood", "777111777"),
                new ContactModel(4, "Will", "Smith", "666111666"),
            };

            return contacts;
        }
    }
}
