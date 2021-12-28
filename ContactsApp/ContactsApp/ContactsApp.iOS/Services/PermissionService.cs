using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook;
using ContactsApp.iOS.Services;
using ContactsApp.Services;
using ContactsApp.Utils;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(PermissionService))]
namespace ContactsApp.iOS.Services
{
	public class PermissionService : IPermissionService
	{
        public Task<PermissionStatus> GetContactsPermissionStatusAsync()
        {
            return Task.FromResult(GetAddressBookPermissionStatus());
        }

        public Task<PermissionStatus> RequestContactsPermissionAsync()
        {
            EnsureDeclared();

            var status = GetAddressBookPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return RequestAddressBookPermission();
        }

        private Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSContactsUsageDescription" };

        private PermissionStatus GetAddressBookPermissionStatus()
        {
            var status = ABAddressBook.GetAuthorizationStatus();
            return status switch
            {
                ABAuthorizationStatus.Authorized => PermissionStatus.Granted,
                ABAuthorizationStatus.Denied => PermissionStatus.Denied,
                ABAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        private Task<PermissionStatus> RequestAddressBookPermission()
        {
            var addressBook = ABAddressBook.Create(out var createError);

            // if the permission was denied, then we can't create the object
            if (createError?.Code == (int)ABAddressBookError.OperationNotPermittedByUserError)
                return Task.FromResult(PermissionStatus.Denied);

            var tcs = new TaskCompletionSource<PermissionStatus>();

            addressBook.RequestAccess((success, error) =>
            {
                tcs.TrySetResult(success ? PermissionStatus.Granted : PermissionStatus.Denied);

                addressBook?.Dispose();
                addressBook = null;
            });

            return tcs.Task;
        }

        private bool PlatformIsMainThread =>
            NSThread.Current.IsMainThread;

        private void EnsureMainThread()
        {
            if (!PlatformIsMainThread)
                throw new Exception("Permission request must be invoked on main thread.");
        }

        private void EnsureDeclared()
        {
            if (RequiredInfoPlistKeys == null)
                return;

            var plistKeys = RequiredInfoPlistKeys?.Invoke();

            if (plistKeys == null)
                return;

            foreach (var requiredInfoPlistKey in plistKeys)
            {
                if (!IsKeyDeclaredInInfoPlist(requiredInfoPlistKey))
                    throw new Exception($"You must set `{requiredInfoPlistKey}` in your Info.plist file to use the Permission: {GetType().Name}.");
            }
        }

        private bool IsKeyDeclaredInInfoPlist(string usageKey) =>
            NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));
    }
}

