using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContactsApp.Services
{
    public class PermissionService : IPermissionService
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
        public async Task<bool> HandleContactsPermission()
        {
            var status = await GetContactsPermissionStatusAsync();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else if (status == PermissionStatus.Denied)
            {
                var requestStatus = await RequestContactsPermissionAsync();
                if (requestStatus == PermissionStatus.Granted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
