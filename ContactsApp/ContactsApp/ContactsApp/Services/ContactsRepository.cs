using ContactsApp.Models;
using System.Collections.Generic;
using System.Linq;
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
            var contactModels = contacts
                .Select(x => ConvertToContactModel(x))
                .ToList();

            await _databaseService.UpsertContactsAsync(contactModels);

            return contactModels;
        }

        private ContactModel ConvertToContactModel(Contact contact)
        {
            var phoneNumber = GetPhoneNumber(contact.Phones);

            return new ContactModel(
                contact.Id,
                contact.GivenName,
                contact.FamilyName,
                phoneNumber);
        }

        private string GetPhoneNumber(List<ContactPhone> contactPhones)
        {
            var phoneNumber = contactPhones != null && contactPhones.Any()
                    ? contactPhones.First().PhoneNumber
                    : string.Empty;

            return phoneNumber;
        }

        public Task DeleteAllContactsAsync()
        {
             return _databaseService.DeleteAllContactsAsync();
        }

        public Task<SyncInfoModel> GetSyncInfoAsync()
        {
            return _databaseService.GetSyncInfoAsync();
        }

        public Task<int> SaveSyncInfoAsync(SyncInfoModel syncInfo)
        {
            return _databaseService.SaveSyncInfoAsync(syncInfo);
        }
    }
}
