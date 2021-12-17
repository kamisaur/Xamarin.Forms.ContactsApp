using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public interface IDialogService
    {
        Task<bool> DisplayPromptAsync(string title, string message, string accept = "Ok", string cancel = "Cancel");
    }
}
