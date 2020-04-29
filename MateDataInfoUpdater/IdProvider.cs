using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CommonLibrary.Helpers
{
    public class IdProvider 
    {
        private readonly List<int> _alreadyUsedIds = new List<int>();
        private readonly int _minId, _maxId;
        private readonly SqlDataBaseReader _sqlDbReader;
        private readonly string _tableName;

        public IdProvider(string tableName) 
        {
            _sqlDbReader=new SqlDataBaseReader();
            _tableName = tableName;
            _alreadyUsedIds.AddRange(ConvertDataTable(_sqlDbReader.ExecuteCommand($"SELECT Id FROM {tableName};")));
            _alreadyUsedIds.Sort();
        }
        public int GetNextAvailableId()
        {
            var result = 0;
            for (int i = 1; i < _alreadyUsedIds.Count; i++)
            {
                if (_alreadyUsedIds[i] - 1 != _alreadyUsedIds[i - 1])
                {
                    result = _alreadyUsedIds[i - 1] + 1;
                    _alreadyUsedIds.Add(result);
                    _alreadyUsedIds.Sort();
                    return result;
                }
            }
            result = _alreadyUsedIds.Count == 0 ? 1 : _alreadyUsedIds.Max(el => el) + 1;

            _alreadyUsedIds.Add(result);
            _alreadyUsedIds.Sort();

            return result;
        }

        public int GetLastAvailableId()
        {
            int maxId;

            if (_alreadyUsedIds.Count == 0)
                maxId = 0;
            else
                maxId = _alreadyUsedIds.Max();

            var nextMaxId = maxId + 1;

            _alreadyUsedIds.Add(nextMaxId);
            _alreadyUsedIds.Sort();

            return nextMaxId;
        }

        private static List<int> ConvertDataTable(DataTable dt)
        {
            var result = new List<int>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(int.Parse(row.ItemArray[0].ToString()));
            }

            return result;
        }

        private static void FillWithIDs(List<int> result, int startingId)
        {
            var firstId = startingId + 1;
            for (int i = 0; i < result.Capacity; i++)
            {
                result.Add(firstId + i);
            }
        }
    }
}