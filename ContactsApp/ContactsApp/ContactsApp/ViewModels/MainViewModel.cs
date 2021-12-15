using ContactsApp.ItemViewModels;
using ContactsApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ContactsApp.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly IContactsRepository _contactsRepo;

        public ObservableCollection<ContactItemViewModel> Contacts { get; set; }

        private DateTime _lastSyncDate;
        public DateTime LastSyncDate
        {
            get => _lastSyncDate;
            set
            {
                _lastSyncDate = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IContactsRepository contactsRepo)
        {
            _contactsRepo = contactsRepo;

            var contactModels = contactsRepo.GetContacts();
            var contactIvms = contactModels.Select(x => new ContactItemViewModel(x));

            Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
        }
    }
}
