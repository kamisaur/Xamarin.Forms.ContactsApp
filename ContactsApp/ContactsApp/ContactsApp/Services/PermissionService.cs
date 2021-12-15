using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContactsApp.Services
{
    internal class PermissionService : IPermissionService
    {
        public async  Task<PermissionStatus> GetContactsPermissionStatusAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            return status;
        }

        public async Task<PermissionStatus> RequestContactsPermissionAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            return status;
        }
    }
}
