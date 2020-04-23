using MetadataInformationUpdater;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace MateDataInfoUpdater.IO
{
    public class ExcelFileHelper : IFileReaderHelper
    {
        public List<DDRecord> ReadLines(string path)
        {
            var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", path);

            //var adapter = new OleDbDataAdapter("SELECT * FROM [Page Definition$]", connectionString);
            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, "ddUpdates");

            DataTable data = ds.Tables["ddUpdates"];
            var result = new List<DDRecord>();
            int i = 0;
            foreach (DataRow row in data.Rows)
            {
                result.Add(new DDRecord()
                {
                    Id = 0,
                    ContextName = row["Context Name"].ToString(),
                    DataKey = row["DataKey"].ToString(),
                    BusinessDescription = row["Business Description"].ToString().Replace("'", "''"),
                    FieldBehaviour = row["Field Behaviour"].ToString().Replace("'","''"),
                    Section = row["Section"].ToString(),
                    FieldType = row["Field Type"].ToString(),
                });
            }
            return result;
        }
    }
}
