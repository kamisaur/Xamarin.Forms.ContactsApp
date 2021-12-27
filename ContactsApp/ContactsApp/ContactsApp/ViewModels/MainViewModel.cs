using ContactsApp.ItemViewModels;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContactsApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IContactsRepository _contactsRepo;
        private readonly IPermissionService _permissionService;
        private readonly IDialogService _dialogService;

        private const string dateTimeFormat = "MM/dd/yyyy HH:mm";
        private const string contactsString = "Contacts";
        private const string importNoticeString = "Are you sure you want to import contacts?";
        private const string yesString = "Yes";
        private const string noString = "No";
        private const string notAvailableString = "N/A";

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

        private ICommand _errorCommande;
        public ICommand ErrorCommand
        {
            get => _errorCommande;
            set
            {
                _errorCommande = value;
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

        private SyncState _currentSyncState;
        public SyncState CurrentSyncState
        {
            get => _currentSyncState;
            set
            {
                _currentSyncState = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
     
        private void InitCommands()
        {
            SyncContactsCommand = new Command(() => SyncContatcts());
            ClearCacheCommand = new Command(() => ClearCacheAsync());
            RequestPermissionCommand = new Command(() => RequestPermission());
            GoBackToContactsCommand = new Command(() => GoBackToContacts());
            ErrorCommand = new Command(() => HandleErrorState());
        }

        public Task Initialize()
        {
            RunInBackground(async () => await InitializeSyncInfo());
            RunInBackground(async () => await InitializeContacts());

            return Task.CompletedTask;
        }

        public MainViewModel(
            IContactsRepository contactsRepo,
            IPermissionService permissionService,
            IDialogService dialogService)
        {
            _contactsRepo = contactsRepo;
            _permissionService = permissionService;
            _dialogService = dialogService;

            InitCommands();
        }

        /// <summary>
        /// Converts ContactModel collection into ItemViewModels. Updates Contacts and State properties.
        /// </summary>
        /// <param name="contactModels">Items to convert.</param>
        private void UpdateContacts(List<ContactModel> contactModels)
        {
            if (contactModels == null)
                return;

            var contactIvms = contactModels
                .Select(x => new ContactItemViewModel(x))
                .OrderBy(x => x.FirstName);

            if (contactIvms.Any())
            {
                Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
            }
            else
            {
                CurrentState = ViewModelState.Empty;
            }

            CurrentSyncState = SyncState.Completed;
            CurrentState = ViewModelState.Normal;
            IsBusy = false;
        }

        /// <summary>
        /// Sets initial value to LastSyncDateString based on data in db.
        /// </summary>
        private async Task InitializeSyncInfo()
        {
            var syncInfo = await _contactsRepo.GetSyncInfoAsync();
            if (syncInfo != null)
            {
                if (syncInfo.SyncDateTime == DateTime.MinValue)
                {
                    LastSyncDateString = notAvailableString;
                }
                else
                {
                    LastSyncDateString = syncInfo.SyncDateTime.ToString(dateTimeFormat);
                }
            }
        }

        /// <summary>
        /// Retrieves existing contacts from repository and updates the current state. 
        /// </summary>
        private async Task InitializeContacts()
        {
            var contactModels = await _contactsRepo.GetContacts();
            UpdateContacts(contactModels);
        }

        /// <summary>
        /// Synchronizes contacts. Prompts user to confirm sync and checks the permission status
        /// before permorming sync.
        /// </summary>
        private async void SyncContatcts(bool skipPrompts = false)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!skipPrompts)
            {
                var canSync = await CanSyncContacts();
                if (!canSync)
                {
                    IsBusy = false;
                    return;
                }
            }

            await RunInBackground(async () => await UpdateContacts());
            await RunInBackground(async () => await UpdateSyncInfo());

            IsBusy = false;
        }

        private async Task<bool> CanSyncContacts()
        {
            var dialogResult = await _dialogService.DisplayPromptAsync(
                contactsString,
                importNoticeString,
                yesString,
                noString);

            if (!dialogResult)
            {
                return false;
            }

            var permissionStatus = await _permissionService.GetContactsPermissionStatusAsync();
            if (permissionStatus != PermissionStatus.Granted)
            {
                CurrentState = ViewModelState.PermissionDenied;
                CurrentSyncState = SyncState.Completed;
                return false;
            }

            return true;
        }

        private async Task UpdateContacts()
        {
            var contacts = await _contactsRepo.SyncContacts();
            UpdateContacts(contacts);
        }

        private async Task UpdateSyncInfo()
        {
            var newSync = DateTime.Now;
            await _contactsRepo.SaveSyncInfoAsync(new SyncInfoModel(newSync));
            LastSyncDateString = newSync.ToString(dateTimeFormat);
        }

        private async void ClearCacheAsync()
        {
            await RunInBackground(async () =>
            {
                await _contactsRepo.DeleteAllContactsAsync();
            });

            Contacts.Clear();
            CurrentSyncState = SyncState.Completed;
            CurrentState = ViewModelState.Empty;

        }

        private async void RequestPermission()
        {
            var permissionStatus = await _permissionService.RequestContactsPermissionAsync();
            if(permissionStatus == PermissionStatus.Granted)
            {
                SyncContatcts(skipPrompts: true);
            }
            else
            {
                CurrentState = ViewModelState.PermissionDenied;
            }
        }

        private void GoBackToContacts()
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

        private void HandleErrorState()
        {
            RunInBackground(() =>
            {
                throw new NotImplementedException("Something went wrong:(");
            });
        }

        /// <summary>
        /// Helper method to run task compatible with unit tests.
        /// </summary>
        /// <param name="action">Action to be performed in the Task</param>
        public Task RunInBackground(Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    CurrentSyncState = SyncState.Loading;
                    CurrentState = ViewModelState.Normal;

                    action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                    CurrentSyncState = SyncState.Error;
                }
                finally
                {
                    IsBusy = false;
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current);
        }
    }
}
