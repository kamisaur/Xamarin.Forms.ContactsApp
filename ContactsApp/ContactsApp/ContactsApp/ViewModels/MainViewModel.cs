using ContactsApp.ItemViewModels;
using ContactsApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IPermissionService _permissionService;

        public MainViewModel(IContactsRepository contactsRepo, IPermissionService permissionService)
        {
            _contactsRepo = contactsRepo;
            _permissionService = permissionService;


            Task.Run(async () =>
            {
                var permissionStatus = await _permissionService.HandleContactsPermission();
                if (!permissionStatus)
                    return;

                var contactModels = await _contactsRepo.GetContacts();
                var contactIvms = contactModels.Select(x => new ContactItemViewModel(x));

                Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
            });
        }

        private ObservableCollection<ContactItemViewModel> _contacts;
        public ObservableCollection<ContactItemViewModel> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged();
            }
        }

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

    }
}
