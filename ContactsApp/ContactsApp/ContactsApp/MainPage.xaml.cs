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

            var permissionService = new PermissionService();
            var contactsService = new ContactsService();
            var contactsRepo = new ContactsRepository(contactsService);
            BindingContext = new MainViewModel(contactsRepo, permissionService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
