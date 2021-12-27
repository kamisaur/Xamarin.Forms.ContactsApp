using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using ContactsApp.Droid.Services;
using ContactsApp.Services;
using ContactsApp.Utils;
using Java.Security;
using Xamarin.Forms;

[assembly: Dependency(typeof(PermissionService))]
namespace ContactsApp.Droid.Services
{
	public class PermissionService : IPermissionService
	{
        static readonly object locker = new object();

        static int requestCode;

        private const int requestCodeStart = 12000;

        private int nextRequestCode = requestCodeStart;

        private int NextRequestCode()
        {
            if (++nextRequestCode >= 12999)
                nextRequestCode = requestCodeStart;

            return nextRequestCode;
        }

        public (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.ReadContacts, true) };

        public Task<PermissionStatus> GetContactsPermissionStatusAsync()
        {
            var permission = ContextCompat.CheckSelfPermission(MainActivity.Context, Manifest.Permission.ReadContacts);
            var isGranted = permission == (int)Android.Content.PM.Permission.Granted;

            if (isGranted)
            {
                return Task.FromResult(PermissionStatus.Granted);
            }
            else
            {
                return Task.FromResult(PermissionStatus.Denied);
            }
        }


        static readonly Dictionary<string, (int requestCode, TaskCompletionSource<PermissionStatus> tcs)> requests =
               new Dictionary<string, (int, TaskCompletionSource<PermissionStatus>)>();


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

        internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            lock (locker)
            {
                // Check our pending requests for one with a matching request code
                foreach (var kvp in requests)
                {
                    if (kvp.Value.requestCode == requestCode)
                    {
                        var tcs = kvp.Value.tcs;

                        // Look for any denied requests, and deny the whole request if so
                        // Remember, each PermissionType is tied to 1 or more android permissions
                        // so if any android permissions denied the whole PermissionType is considered denied
                        if (grantResults.Any(g => g == Android.Content.PM.Permission.Denied))
                            tcs.TrySetResult(PermissionStatus.Denied);
                        else
                            tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    }
                }
            }
        }

        public async Task<PermissionStatus> RequestContactsPermissionAsync()
        {
            TaskCompletionSource<PermissionStatus> tcs;
            var doRequest = true;

            var runtimePermissions = RequiredPermissions.Where(p => p.isRuntime)
                ?.Select(p => p.androidPermission)?.ToArray();

            // We may have no runtime permissions required, in this case
            // knowing they all exist in the manifest from the Check call above is sufficient
            if (runtimePermissions == null || !runtimePermissions.Any())
                return PermissionStatus.Granted;

            var permissionId = string.Join(';', runtimePermissions);

            lock (locker)
            {
                if (requests.ContainsKey(permissionId))
                {
                    tcs = requests[permissionId].tcs;
                    doRequest = false;
                }
                else
                {
                    tcs = new TaskCompletionSource<PermissionStatus>();

                    requestCode = NextRequestCode();

                    requests.Add(permissionId, (requestCode, tcs));
                }
            }

            if (!doRequest)
                return await tcs.Task;

            if (!IsMainThread)
                throw new Exception("Permission request must be invoked on main thread.");

            ActivityCompat.RequestPermissions((Activity)MainActivity.Context, runtimePermissions.ToArray(), requestCode);

            var result = await tcs.Task;

            if (requests.ContainsKey(permissionId))
                requests.Remove(permissionId);

            return result;
        }


        private bool IsMainThread
        {
            get
            {
                if (HasApiLevel(BuildVersionCodes.M))
                    return Looper.MainLooper.IsCurrentThread;

                return Looper.MyLooper() == Looper.MainLooper;
            }
        }

        private int? sdkInt;

        private int SdkInt
            => sdkInt ??= (int)Build.VERSION.SdkInt;

        private bool HasApiLevel(BuildVersionCodes versionCode) =>
            SdkInt >= (int)versionCode;

    }
}

