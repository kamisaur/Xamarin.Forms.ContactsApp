using ContactsApp.ItemViewModels;
using ContactsApp.Services;
using ContactsApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContactsApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IPermissionService _permissionService;

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

        private ICommand _clearCacheCommand;
        public ICommand ClearCacheCommand
        {
            get => _clearCacheCommand;
            set
            {
                _clearCacheCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _syncContactsCommand;
        public ICommand SyncContactsCommand
        {
            get => _syncContactsCommand;
            set
            {
                _syncContactsCommand = value;
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

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private ViewModelState _currentState;
        public ViewModelState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IContactsRepository contactsRepo, IPermissionService permissionService)
        {
            _contactsRepo = contactsRepo;
            _permissionService = permissionService;

            SyncContactsCommand = new Command(() => SyncContatctsAsync());
            ClearCacheCommand = new Command(() => ClearCacheAsync());

            Task.Run(async () =>
            {
                IsLoading = true;
                CurrentState = ViewModelState.Loading;

                var permissionStatus = await _permissionService.HandleContactsPermission();
                if (!permissionStatus)
                {
                    CurrentState = ViewModelState.PermissionDenied;
                    return;
                }

                var contactModels = await _contactsRepo.GetContacts();
                var contactIvms = contactModels.Select(x => new ContactItemViewModel(x));

                if (contactIvms.Any())
                {
                    Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
                    CurrentState = ViewModelState.Normal;
                }
                else
                {
                    CurrentState = ViewModelState.Empty;
                }

                IsLoading = false;
            });
        }

        public void SyncContatctsAsync()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                CurrentState = ViewModelState.Loading;

                var contacts = await _contactsRepo.SyncContacts();
                var contactIvms = contacts.Select(x => new ContactItemViewModel(x));
                if (contactIvms.Any())
                {
                    Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
                }

                IsLoading = false;
                CurrentState = ViewModelState.Normal;
            });
        }

        public void ClearCacheAsync()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                CurrentState = ViewModelState.Loading;

                Contacts.Clear();
                await _contactsRepo.DeleteAllContactsAsync();

                IsLoading = false;
                CurrentState = ViewModelState.Empty;
            });
        }
    }
}
