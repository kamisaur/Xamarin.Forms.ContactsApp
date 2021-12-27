using ContactsApp.Services;
using ContactsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ContactsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var permissionService = DependencyService.Get<IPermissionService>();
            var contactsService = new ContactsService();
            var databaseService = new DatabaseService();
            var dialogService = new DialogService();
            var contactsRepo = new ContactsRepository(contactsService, databaseService);
            var vm = new MainViewModel(contactsRepo, permissionService, dialogService);
            vm.Initialize();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
