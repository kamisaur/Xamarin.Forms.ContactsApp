using AutoFixture;
using ContactsApp.ItemViewModels;
using ContactsApp.Services;
using ContactsApp.Utils;
using ContactsApp.ViewModels;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContactsApp.Tests.ViewModels
{
    [TestFixture]
    public class MainViewModelTests : BaseTestContext
    {
        [Test]
        public void ClearCacheCommand_ContactsEmptyBeforeClearing_ContactsEmptyAfterClearing()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            cUT.ClearCacheCommand.Execute(null);

            // Assert
            cUT.Contacts.Should().BeEmpty();
        }

        [Test]
        public void ClearCacheCommand_HaveContactsBeforeClearing_ContactsEmptyAfterClearing()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>
            {
                new ContactItemViewModel("0", "Jack", "Black", "111222333"),
                new ContactItemViewModel("1", "Anthony", "Bright", "222333444"),
                new ContactItemViewModel("2", "Mark", "Anthony", "333444555"),
            };

            // Act
            RunInSingleThread(() =>
            {
                cUT.ClearCacheCommand.Execute(null);
            });

            // Assert
            cUT.Contacts.Should().BeEmpty();
        }

        [Test]
        public void ClearCacheCommand_DatabaseCalled_DatabaseReceivesCall()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            cUT.ClearCacheCommand.Execute(null);

            // Assert
            databaseService.Received(1).DeleteAllContactsAsync();
        }

        [Test]
        public async Task ClearCacheCommand_CurrentSyncStateChanged_ChangedToCompleted()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            await RunInSingleThread(() =>
            {
                cUT.ClearCacheCommand.Execute(null);
            });

            // Assert
            cUT.CurrentSyncState.Should().Be(SyncState.Completed);
        }
        
        [Test]
        public async Task ClearCacheCommand_CurrentStateChanged_ChangeToEmpty()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            await RunInSingleThread(() =>
            {
                cUT.ClearCacheCommand.Execute(null);
            });

            // Assert
            cUT.CurrentState.Should().Be(ViewModelState.Empty);
        }
        
        [Test]
        public void GoBackToContactsCommand_ContactsEmpty_CurrentStatCehangeToEmpty()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            cUT.GoBackToContactsCommand.Execute(null);

            // Assert
            cUT.CurrentState.Should().Be(ViewModelState.Empty);
        }

        [Test]
        public void GoBackToContactsCommand_ContactsNotEmpty_CurrentStatehangeToNormal()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>
            {
                new ContactItemViewModel("0", "Jack", "Black", "111222333"),
                new ContactItemViewModel("1", "Anthony", "Bright", "222333444"),
                new ContactItemViewModel("2", "Mark", "Anthony", "333444555"),
            };

            // Act
            cUT.GoBackToContactsCommand.Execute(null);

            // Assert
            cUT.CurrentState.Should().Be(ViewModelState.Normal);
        }

        [Test]
        public async Task ErrorCommand_ThrowsException_SyncStateChangedToError()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();
            cUT.Contacts = new ObservableCollection<ContactItemViewModel>();

            // Act
            await RunInSingleThread(() =>
            {
                cUT.ErrorCommand.Execute(null);
            });

            // Assert
            cUT.CurrentSyncState.Should().Be(SyncState.Error);
        }

        [Test]
        public void RequestPermission_PermissionDenied_StateChangesToPermissionDenied()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            permissionService.RequestContactsPermissionAsync().Returns(PermissionStatus.Denied);

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();

            // Act
            cUT.RequestPermissionCommand.Execute(null);

            // Assert
            cUT.CurrentState.Should().Be(ViewModelState.PermissionDenied);
        }

        [Test]
        public void RequestPermission_PermissionGranted_Sync()
        {
            // Arrange
            var databaseService = Substitute.For<IContactsRepository>();
            var permissionService = Substitute.For<IPermissionService>();
            var dialogService = Substitute.For<IDialogService>();

            permissionService.RequestContactsPermissionAsync().Returns(PermissionStatus.Denied);

            var fixture = new Fixture();
            fixture.Register<IContactsRepository>(() => databaseService);
            fixture.Register<IPermissionService>(() => permissionService);
            fixture.Register<IDialogService>(() => dialogService);

            var cUT = fixture.Build<MainViewModel>().OmitAutoProperties().Create();

            // Act
            cUT.RequestPermissionCommand.Execute(null);

            // Assert
            cUT.CurrentState.Should().Be(ViewModelState.PermissionDenied);
        }
    }
}
