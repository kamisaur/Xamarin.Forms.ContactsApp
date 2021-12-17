using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly IContactsService _contactsService;
        private readonly IDatabaseService _databaseService;

        public ContactsRepository(IContactsService contactsService, IDatabaseService databaseService)
        {
            _contactsService = contactsService;
            _databaseService = databaseService;
        }

        public Task<List<ContactModel>> GetContacts()
        {
            var contacts = _databaseService.GetContactsAsync();
            return contacts;
        }

        public async Task<List<ContactModel>> SyncContacts()
        {
            await DeleteAllContactsAsync();
            var contacts = await _contactsService.GetAllContactsAsync();
            await _databaseService.UpsertContactsAsync(contacts);

            return contacts;
        }

        public Task DeleteAllContactsAsync()
        {
             return _databaseService.DeleteAllContactsAsync();
        }

        public Task<SyncInfoModel> GetSyncInfo()
        {
            return _databaseService.GetSyncInfoAsync();
        }

        public Task<int> SaveSyncInfoAsync(SyncInfoModel syncInfo)
        {
            return _databaseService.SaveSyncInfoAsync(syncInfo);
        }
    }
}
