using ContactsApp.Models;
using ContactsApp.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApp.Services
{
    public class DatabaseService : IDatabaseService
    {
        static SQLiteAsyncConnection Database;

        public DatabaseService()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            Database.CreateTableAsync<ContactModel>();
            Database.CreateTableAsync<SyncInfoModel>();
        }

        public Task<List<ContactModel>> GetContactsAsync()
        {
            return Database.Table<ContactModel>().ToListAsync();
        }

        public Task<int> UpsertContactsAsync(List<ContactModel> contactModels)
        {
            DeleteAllContactsAsync();
            return Database.InsertAllAsync(contactModels);
        }

        public Task<int> DeleteAllContactsAsync()
        {
            return Database.DeleteAllAsync<ContactModel>();
        }

        public Task<SyncInfoModel> GetSyncInfoAsync()
        {
            return Database.Table<SyncInfoModel>().FirstOrDefaultAsync();
        }

        public Task<int> SaveSyncInfoAsync(SyncInfoModel syncInfo)
        {
            return Database.InsertOrReplaceAsync(syncInfo);
        }
    }
}
