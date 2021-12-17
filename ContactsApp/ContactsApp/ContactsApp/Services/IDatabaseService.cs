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

        Task<int> DeleteAllContactsAsync();

        Task<SyncInfoModel> GetSyncInfoAsync();

        Task<int> SaveSyncInfoAsync(SyncInfoModel syncInfo);

    }
}
