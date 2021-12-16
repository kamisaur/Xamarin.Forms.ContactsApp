using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContactsApp.Services
{
    public class ContactsService : IContactsService
    {
        public async Task<List<ContactModel>> GetAllContactsAsync()
        {
            try
            {
                var contacts = await Contacts.GetAllAsync();
                var contactModels = new List<ContactModel>();
                contactModels = contactModels ?? new List<ContactModel>();

                foreach (var contact in contacts)
                {
                    var phoneNumber = GetPhoneNumber(contact.Phones);
                    contactModels.Add(new ContactModel(
                        contact.Id,
                        contact.GivenName,
                        contact.FamilyName,
                        phoneNumber));
                }

                return contactModels;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetPhoneNumber(List<ContactPhone> contactPhones)
        {
            var phoneNumber = contactPhones != null && contactPhones.Any()
                    ? contactPhones.First().PhoneNumber
                    : string.Empty;

            return phoneNumber;
        }
    }
}
