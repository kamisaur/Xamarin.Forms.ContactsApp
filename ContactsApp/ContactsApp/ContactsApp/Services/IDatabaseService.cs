using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IDatabaseService
    {
        Task<List<ContactModel>> GetContactsAsync();

        Task<int> UpsertContactsAsync(List<ContactModel> contactModels);

        public Task<int> DeleteAllContactsAsync();
    }
}
