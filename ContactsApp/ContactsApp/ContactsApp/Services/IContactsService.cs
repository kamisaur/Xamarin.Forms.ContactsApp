using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    internal interface IContactsService
    {
        Task<List<ContactModel>> GetAllContactsAsync();
    }
}
