using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IContactsRepository
    {
        Task<List<ContactModel>> GetContacts();

        Task<List<ContactModel>> SyncContacts();

        Task DeleteAllContactsAsync();

        Task<SyncInfoModel> GetSyncInfo();

        Task<int> SaveSyncInfoAsync(SyncInfoModel syncInfo);
    }
}
