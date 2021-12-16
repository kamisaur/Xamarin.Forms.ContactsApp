using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContactsApp.Services
{
    public interface IPermissionService
    {
        Task<PermissionStatus> GetContactsPermissionStatusAsync();
        Task<PermissionStatus> RequestContactsPermissionAsync();
        Task<bool> HandleContactsPermission();
    }
}
