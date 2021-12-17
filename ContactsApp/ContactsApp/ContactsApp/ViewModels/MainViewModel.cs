using ContactsApp.ItemViewModels;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContactsApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IPermissionService _permissionService;
        private readonly IDialogService _dialogService;

        private const string dateTimeFormat = "MM/dd/yyyy HH:mm";

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

        private ICommand _requestPermissionCommand;
        public ICommand RequestPermissionCommand
        {
            get => _requestPermissionCommand;
            set
            {
                _requestPermissionCommand = value;
                OnPropertyChanged();
            }
        }

        private ICommand _goBackToContactsCommand;
        public ICommand GoBackToContactsCommand
        {
            get => _goBackToContactsCommand;
            set
            {
                _goBackToContactsCommand = value;
                OnPropertyChanged();
            }
        }

        private string _lastSyncDateString;
        public string LastSyncDateString
        {
            get => _lastSyncDateString;
            set
            {
                _lastSyncDateString = value;
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

        public MainViewModel(
            IContactsRepository contactsRepo,
            IPermissionService permissionService,
            IDialogService dialogService)
        {
            _contactsRepo = contactsRepo;
            _permissionService = permissionService;
            _dialogService = dialogService;

            SyncContactsCommand = new Command(() => SyncContatctsAsync());
            ClearCacheCommand = new Command(() => ClearCacheAsync());
            RequestPermissionCommand = new Command(() => RequestPermission());
            GoBackToContactsCommand = new Command(() => GoBackToContacts());

            Task.Run(async () =>
            {
                CurrentState = ViewModelState.Normal;
                IsLoading = true;

                var syncInfo = await _contactsRepo.GetSyncInfo();
                if(syncInfo != null)
                {
                    LastSyncDateString = syncInfo.SyncDateTime.ToString(dateTimeFormat);
                }

                var contactModels = await _contactsRepo.GetContacts();
                var contactIvms = contactModels.Select(x => new ContactItemViewModel(x));

                if (contactIvms.Any())
                {
                    Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
                }
                else
                {
                    CurrentState = ViewModelState.Empty;
                }

                IsLoading = false;
            });
        }

        public async void SyncContatctsAsync()
        {
            var dialogResult = await _dialogService.DisplayPromptAsync(
                "Contacts",
                "Are you sure you want to import contacts?",
                "Yes",
                "No");

            if (!dialogResult)
            {
                return;
            }

            await Task.Run(async () =>
            {
                IsLoading = true;
                CurrentState = ViewModelState.Normal;

                var permissionStatus = await _permissionService.GetContactsPermissionStatusAsync();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    CurrentState = ViewModelState.PermissionDenied;
                    IsLoading = false;
                    return;
                }

                var contacts = await _contactsRepo.SyncContacts();

                var newSync = DateTime.Now;
                await _contactsRepo.SaveSyncInfoAsync(new SyncInfoModel(newSync));
                LastSyncDateString = newSync.ToString(dateTimeFormat);

                var contactIvms = contacts.Select(x => new ContactItemViewModel(x));
                if (contactIvms.Any())
                {
                    Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
                }

                IsLoading = false;
            });
        }

        public void ClearCacheAsync()
        {
            Task.Run(async () =>
            {
                IsLoading = true;
                CurrentState = ViewModelState.Normal;

                Contacts.Clear();
                await _contactsRepo.DeleteAllContactsAsync();

                IsLoading = false;
                CurrentState = ViewModelState.Empty;
            });
        }

        public async void RequestPermission()
        {
            var permissionStatus = await _permissionService.RequestContactsPermissionAsync();
            if(permissionStatus == PermissionStatus.Granted)
            {
                SyncContatctsAsync();
            }
            else
            {
                CurrentState = ViewModelState.PermissionDenied;
            }
        }

        public void GoBackToContacts()
        {
            if(Contacts != null && Contacts.Any())
            {
                CurrentState = ViewModelState.Normal;
            }
            else
            {
                CurrentState = ViewModelState.Empty;
            }
        }
    }
}
