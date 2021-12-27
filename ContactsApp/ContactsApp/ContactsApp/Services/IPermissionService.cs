using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContactsApp.Utils;

namespace ContactsApp.Services
{
    public interface IPermissionService
    {
        Task<PermissionStatus> GetContactsPermissionStatusAsync();
        Task<PermissionStatus> RequestContactsPermissionAsync();
        Task<bool> HandleContactsPermission();
    }
}
