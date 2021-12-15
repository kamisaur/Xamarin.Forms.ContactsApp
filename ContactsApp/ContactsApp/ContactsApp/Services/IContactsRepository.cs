using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Services
{
    internal interface IContactsRepository
    {
        IList<ContactModel> GetContacts();
    }
}
