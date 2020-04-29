using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibrary.Helpers;
using MetadataInformationUpdater;

namespace MateDataInfoUpdater.DescriptionUpdatesLogic
{
    public class DescriptionUpdater
    {
        private static IdProvider _idProvider;
        private static List<DDRecord> _inputRecords;
        private static List<DDRecord> _resultDescriptions;

        public DescriptionUpdater(List<DDRecord> inputRecords, List<DDRecord> resultDescriptions)
        {
            _idProvider = new IdProvider("ui.EnDtFieldDescription");
            _inputRecords = inputRecords;
            _resultDescriptions = resultDescriptions;
        }

        public void UpdateBusinessDescriptionAndFieldBehaviours(List<DDRecord> _ddRecordsWithUpdates)
        {
            foreach (var update in _ddRecordsWithUpdates)
            {
                var recordsToUpdate = FindMatchingRecordsForGivenUpdate(update);
                if (recordsToUpdate.Count == 0 &&
                    (
                        ComponenUniqueNameMap.GridNameMap.ContainsKey(update.Section)
                     || !string.IsNullOrEmpty(update.DataKey))
                    )
                {
                    recordsToUpdate.Add(new DDRecord()
                    {
                        Id = _idProvider.GetNextAvailableId(),
                        ContextName = update.ContextName,
                        DataKey = string.IsNullOrEmpty(update.DataKey) ? null : update.DataKey,
                        ComponentUniqueName = string.IsNullOrEmpty(update.DataKey) ? ComponenUniqueNameMap.GridNameMap[update.Section] : string.Empty
                    });
                }
                foreach (var foundRecord in recordsToUpdate)
                {
                    if (string.IsNullOrEmpty(foundRecord.BusinessDescription))
                    {
                        foundRecord.BusinessDescription = update.BusinessDescription;
                    }
                    if (string.IsNullOrEmpty(foundRecord.FieldBehaviour))
                    {
                        foundRecord.FieldBehaviour = update.FieldBehaviour;
                    }
                    _resultDescriptions.AddRange(recordsToUpdate);
                }

            }
        }

        public List<DDRecord> FindMatchingRecordsForGivenUpdate(DDRecord update)
        {
            List<DDRecord> result;
            if (string.IsNullOrEmpty(update.DataKey))
            {
                result = GetSubflowsDDRecords(_inputRecords, update);
                RemoveRecords(_inputRecords, result.Select(el => el.Id));
                return result;
            }

            result = GetDDRecordUsingDataKeyAndContextName(_inputRecords, update);
            RemoveRecords(_inputRecords, result.Select(el => el.Id));
            return result.Any() ? new List<DDRecord>() { result.First() } : result;
        }

        public List<DDRecord> GetSubflowsDDRecords(List<DDRecord> records, DDRecord update)
        {
            var result = new List<DDRecord>();
            Console.WriteLine(string.Format("Searching for {0} context; {1} data key, {2} component", update.ContextName, update.DataKey, update.Section));
            foreach (var metadataDescriptionRecord in records)
            {
                if (metadataDescriptionRecord.ContextName.Trim().Equals(update.ContextName)
                    && (string.IsNullOrEmpty(metadataDescriptionRecord.DataKey))
                    && update.FieldType.Equals(Constants.GridComponentName)
                    && metadataDescriptionRecord.ComponentUniqueName.Equals(ComponenUniqueNameMap.GridNameMap[update.Section]))
                {
                    result.Add(metadataDescriptionRecord);
                }

            }
            result = result.Where(el => !string.IsNullOrEmpty(el.BusinessDescription)).ToList();
            if (result.Count > 1 && result.All(el => (!string.IsNullOrEmpty(el.DataKey))))
            {
                throw new Exception(string.Format("There are duplicated insert commands for context name: {0} dataKey:{1} component unique name {2}", result[0].ContextName, result[0].DataKey, result[0].ComponentUniqueName));
            }

            return result;
        }

        public List<DDRecord> GetDDRecordUsingDataKeyAndContextName(List<DDRecord> records, DDRecord update)
        {
            var result = new List<DDRecord>();
            foreach (var metadataDescriptionRecord in records)
            {
                if (metadataDescriptionRecord.ContextName.Trim().Equals(update.ContextName) &&
                    !string.IsNullOrEmpty(update.DataKey) &&
                    update.DataKey.Equals(metadataDescriptionRecord.DataKey.Trim()))
                {
                    result.Add(metadataDescriptionRecord);
                }
            }

            return result;
        }

        public void RemoveRecords(List<DDRecord> records, IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                records.RemoveAll(el => el.Id == id);
            }
        }
    }
}
