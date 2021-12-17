using ContactsApp.ItemViewModels;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
            SyncContactsCommand = new Command(() => SyncContatctsAsync());
            ClearCacheCommand = new Command(() => ClearCacheAsync());
            RequestPermissionCommand = new Command(() => RequestPermission());
            GoBackToContactsCommand = new Command(() => GoBackToContacts());
            ErrorCommand = new Command(() => HandleErrorState());
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
            RunInBackground(async () => await InitializeData());
        }

        private async Task InitializeData()
        {
            var syncInfo = await _contactsRepo.GetSyncInfo();
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

            CurrentSyncState = SyncState.Completed;

        }

        private async void SyncContatctsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var dialogResult = await _dialogService.DisplayPromptAsync(
                contactsString,
                importNoticeString,
                yesString,
                noString);

            if (!dialogResult)
            {
                IsBusy = false;
                return;
            }

            var permissionStatus = await _permissionService.GetContactsPermissionStatusAsync();
            if (permissionStatus != PermissionStatus.Granted)
            {
                CurrentState = ViewModelState.PermissionDenied;
                CurrentSyncState = SyncState.Completed;
                IsBusy = false;
                return;
            }
           
            await RunInBackground(async () =>
            {
                var contacts = await _contactsRepo.SyncContacts();

                var newSync = DateTime.Now;
                await _contactsRepo.SaveSyncInfoAsync(new SyncInfoModel(newSync));
                LastSyncDateString = newSync.ToString(dateTimeFormat);

                var contactIvms = contacts.Select(x => new ContactItemViewModel(x));
                if (contactIvms.Any())
                {
                    Contacts = new ObservableCollection<ContactItemViewModel>(contactIvms);
                }
                    
                CurrentSyncState = SyncState.Completed;
                IsBusy = false;
            });
        }

        private void ClearCacheAsync()
        {
            RunInBackground(async () =>
            {
                Contacts.Clear();
                await _contactsRepo.DeleteAllContactsAsync();

                CurrentSyncState = SyncState.Completed;
                CurrentState = ViewModelState.Empty;
            });
        }

        private async void RequestPermission()
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
                    CurrentState = ViewModelState.Normal;
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current);
        }
    }
}
