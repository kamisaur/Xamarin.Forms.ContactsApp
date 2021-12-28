using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contacts;
using ContactsApp.iOS.Services;
using ContactsApp.Models;
using ContactsApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(ContactsApp.iOS.Services.ContactsService))]
namespace ContactsApp.iOS.Services
{
	public class ContactsService : IContactsService
    {
        public async Task<List<Contact>> GetAllContactsAsync()
        {
            var contacts = await PlatformGetAllAsync();
            return contacts.ToList();
        }

        private Task<IEnumerable<Contact>> PlatformGetAllAsync(CancellationToken cancellationToken = default)
        {
            var keys = new[]
            {
                CNContactKey.Identifier,
                CNContactKey.NamePrefix,
                CNContactKey.GivenName,
                CNContactKey.MiddleName,
                CNContactKey.FamilyName,
                CNContactKey.NameSuffix,
                CNContactKey.EmailAddresses,
                CNContactKey.PhoneNumbers,
                CNContactKey.Type
            };

            var store = new CNContactStore();
            var containers = store.GetContainers(null, out _);
            if (containers == null)
                return Task.FromResult<IEnumerable<Contact>>(Array.Empty<Contact>());

            return Task.FromResult(GetEnumerable());

            IEnumerable<Contact> GetEnumerable()
            {
                foreach (var container in containers)
                {
                    using var pred = CNContact.GetPredicateForContactsInContainer(container.Identifier);
                    var contacts = store.GetUnifiedContacts(pred, keys, out var error);
                    if (contacts == null)
                        continue;

                    foreach (var contact in contacts)
                    {
                        yield return ConvertContact(contact);
                    }
                }
            }
        }

        internal static Contact ConvertContact(CNContact contact)
        {
            if (contact == null)
                return default;

            var phones = contact.PhoneNumbers?.Select(
                item => new ContactPhone(item?.Value?.StringValue));
            var emails = contact.EmailAddresses?.Select(
                item => new ContactEmail(item?.Value?.ToString()));

            return new Contact(
                contact.Identifier,
                contact.NamePrefix,
                contact.GivenName,
                contact.MiddleName,
                contact.FamilyName,
                contact.NameSuffix,
                phones,
                emails);
        }
    }
}

