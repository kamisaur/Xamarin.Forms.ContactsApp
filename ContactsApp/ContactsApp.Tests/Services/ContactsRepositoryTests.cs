using AutoFixture;
using ContactsApp.Models;
using ContactsApp.Services;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Tests.Services
{
    [TestFixture]
    public class ContactsRepositoryTests
    {
        [Test]
        public void GetContacts_CallDatabaseService_RecievesOneCall()
        {
            // Arrange
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            cUT.GetContacts();

            // Assert
            databaseService.Received(1).GetContactsAsync();
        }

        [Test]
        public void GetContacts_DatabaseReturnsEmpty_RecievesEmpty()
        {
            // Arrange
            var emptyList = new List<ContactModel>();
            var taskResult = Task.FromResult(emptyList);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.GetContactsAsync().Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var contacts = cUT.GetContacts();

            // Assert
            contacts.Should().Be(taskResult);
        }

        [Test]
        public void GetContacts_DatabaseReturnsData_RecievesData()
        {
            // Arrange
            var dataList = new List<ContactModel>
            {
                new ContactModel("0", "Jack", "Black", "111222333"),
                new ContactModel("1", "Anthony", "Bright", "222333444"),
                new ContactModel("2", "Mark", "Anthony", "333444555"),
            };
            var taskResult = Task.FromResult(dataList);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.GetContactsAsync().Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var contacts = cUT.GetContacts();

            // Assert
            contacts.Should().Be(taskResult);
        }

        [Test]
        public void DeleteAllContactsAsync_CallDatabaseService_RecievesOneCall()
        {
            // Arrange
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            cUT.DeleteAllContactsAsync();

            // Assert
            databaseService.Received(1).DeleteAllContactsAsync();
        }

        [Test]
        public void DeleteAllContactsAsync_ItemsDeleted_RecievesValue()
        {
            // Arrange
            var taskResult = Task.FromResult(10);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.DeleteAllContactsAsync().Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var deletedQty = cUT.DeleteAllContactsAsync();

            // Assert
            deletedQty.Should().Be(taskResult);
        }

        [Test]
        public void GetSyncInfoAsync_CallDatabaseService_RecievesOneCall()
        {
            // Arrange
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            cUT.GetSyncInfoAsync();

            // Assert
            databaseService.Received(1).GetSyncInfoAsync();
        }

        [Test]
        public void GetSyncInfoAsync_DatabaseReturnsNull_RecievesNull()
        {
            // Arrange
            var taskResult = Task.FromResult<SyncInfoModel>(null);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.GetSyncInfoAsync().Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var syncInfo = cUT.GetSyncInfoAsync();

            // Assert
            syncInfo.Should().Be(taskResult);
        }

        [Test]
        public void GetSyncInfoAsync_DatabaseReturnsData_RecievesData()
        {
            // Arrange
            var syncInfoData = new SyncInfoModel(DateTime.Now);
            var taskResult = Task.FromResult<SyncInfoModel>(syncInfoData);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.GetSyncInfoAsync().Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var syncInfo = cUT.GetSyncInfoAsync();

            // Assert
            syncInfo.Should().Be(taskResult);
        }

        [Test]
        public void SaveSyncInfoAsync_DatabaseReturnsData_RecievesData()
        {
            // Arrange
            var syncInfoData = new SyncInfoModel(DateTime.Now);
            var savedItemQty = 1;
            var taskResult = Task.FromResult(savedItemQty);
            var databaseService = Substitute.For<IDatabaseService>();
            var contactsService = Substitute.For<IContactsService>();
            databaseService.SaveSyncInfoAsync(Arg.Any<SyncInfoModel>()).Returns(taskResult);

            var fixture = new Fixture();
            fixture.Register<IDatabaseService>(() => databaseService);
            fixture.Register<IContactsService>(() => contactsService);
            var cUT = fixture.Build<ContactsRepository>().OmitAutoProperties().Create();

            // Act
            var quantity = cUT.SaveSyncInfoAsync(syncInfoData);

            // Assert
            quantity.Should().Be(taskResult);
        }
    }
}
