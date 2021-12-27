using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AddressBook;
using ContactsApp.Services;
using Foundation;
using Xamarin.Essentials;
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

        protected Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSContactsUsageDescription" };

        public Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

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

        internal static PermissionStatus GetAddressBookPermissionStatus()
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

        internal static Task<PermissionStatus> RequestAddressBookPermission()
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

        internal void EnsureMainThread()
        {
            if (!MainThread.IsMainThread)
                throw new PermissionException("Permission request must be invoked on main thread.");
        }

        public void EnsureDeclared()
        {
            if (RequiredInfoPlistKeys == null)
                return;

            var plistKeys = RequiredInfoPlistKeys?.Invoke();

            if (plistKeys == null)
                return;

            foreach (var requiredInfoPlistKey in plistKeys)
            {
                if (!IsKeyDeclaredInInfoPlist(requiredInfoPlistKey))
                    throw new PermissionException($"You must set `{requiredInfoPlistKey}` in your Info.plist file to use the Permission: {GetType().Name}.");
            }
        }

        public static bool IsKeyDeclaredInInfoPlist(string usageKey) =>
            NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));
    }
}

