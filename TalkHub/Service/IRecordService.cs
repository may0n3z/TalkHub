using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkHub
{
    public interface IRecordService
    {
        void AddRecord(string tableName);
        void SaveRecord(string tableName, DataTable data);
        void DeleteRecord(string tableName, DataRowView selectedRow);
    }

}
