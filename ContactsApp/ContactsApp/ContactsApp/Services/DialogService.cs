using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ContactsApp.Services
{
    public class DialogService : IDialogService
    {
        private Page GetCurrentPage()
        {
            var currentPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            return currentPage;
        }

        public async Task<bool> DisplayPromptAsync(string title, string message, string accept = "Ok", string cancel = "Cancel")
        {
            var page = GetCurrentPage();
            return await page.DisplayAlert(title, message, accept, cancel);
        }

        public Task DisplayPromptAsync(string title, string message, string accept = "Ok")
        {
            var page = GetCurrentPage();
            return page.DisplayAlert(title, message, accept);
        }
    }
}
