using ContactsApp.Services;
using ContactsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContactsApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var contactsRepo = new ContactsRepository();
            BindingContext = new MainViewModel(contactsRepo);
        }
    }
}
