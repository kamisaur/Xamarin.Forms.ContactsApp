using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Models
{
    public class SyncInfoModel
    {
        [PrimaryKey]
        public int Id { get; set; } = 0;
        public DateTime SyncDateTime { get; set; }

        public SyncInfoModel()
        {
        }

        public SyncInfoModel(DateTime syncDateTime)
        {
            SyncDateTime = syncDateTime;
        }
    }
}
