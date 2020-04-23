using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonLibrary.Helpers;
using MateDataInfoUpdater;
using MateDataInfoUpdater.IO;
using Newtonsoft.Json;

namespace MetadataInformationUpdater
{
    class Program
    {
        //Put path to EnDtFieldDescription folder from branch here
        private static readonly string MetadataFilesPath = "D:\\DevResources\\DD\\8.8\\input.txt";
        private static readonly string ResultPathFile = "D:\\DevResources\\DD\\8.8\\output.txt";

        private static List<DDRecord> _metadataDescriptionRecords;
        private static List<DDRecord> _resultDescriptions;
        private static List<DDRecord> _recordsWithUpdates;
        private static StreamWriter _streamWriter;
        private static IdProvider _idProvider;
        private static string _gridComponenetName = "Grid";
        private static IFileReaderHelper excelFileHelper;
        private static IFileReaderHelper txtFileHelper;

        [STAThread]
        static void Main(string[] args)
        {
            _resultDescriptions = new List<DDRecord>();
            _idProvider = new IdProvider("ui.EnDtFieldDescription");
            excelFileHelper = new ExcelFileHelper();
            txtFileHelper = new TxtFileHelper();
            RunDescriptionsUpdater();
        }

        public static void RunDescriptionsUpdater()
        {
            _metadataDescriptionRecords = txtFileHelper.ReadLines(MetadataFilesPath);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _recordsWithUpdates = excelFileHelper.ReadLines(openFileDialog.FileName);

            }
          
            UpdateBusinessDescriptionAndFieldBehaviours(_recordsWithUpdates);
            _resultDescriptions.AddRange(_metadataDescriptionRecords);
            //_resultDescriptions = RemoveDuplications(_resultDescriptions);
            _resultDescriptions = _resultDescriptions.OrderBy(el => el.ContextName).ThenBy(el => el.DataKey).ToList();


            using (StreamWriter outputFile = new System.IO.StreamWriter(ResultPathFile))
            {
                foreach (var resultDescription in _resultDescriptions)
                {
                    outputFile.Write(
                        $"{Constants.InsertPrefix}{TextUtility.GetFormattedText(resultDescription.ContextName)}" +
                        $", {TextUtility.GetFormattedText(resultDescription.DataKey)}" +
                        $", {TextUtility.GetFormattedText(resultDescription.BusinessDescription)}" +
                        $", {TextUtility.GetFormattedText(resultDescription.FieldBehaviour)}" +
                        $", {TextUtility.GetFormattedText(resultDescription.ComponentUniqueName)}{Constants.CommandEnding}" + System.Environment.NewLine);
                }
            }
        }

        public static void UpdateBusinessDescriptionAndFieldBehaviours(List<DDRecord> _ddRecordsWithUpdates)
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

        public static List<DDRecord> FindMatchingRecordsForGivenUpdate(DDRecord update)
        {
            List<DDRecord> result;
            if (string.IsNullOrEmpty(update.DataKey))
            {
                result = GetSubflowsDDRecords(_metadataDescriptionRecords, update);
                RemoveRecords(_metadataDescriptionRecords, result.Select(el => el.Id));
                return result;
            }
           
            result = GetDDRecordUsingDataKeyAndContextName(_metadataDescriptionRecords, update);
            RemoveRecords(_metadataDescriptionRecords, result.Select(el => el.Id));
            return result.Any() ? new List<DDRecord>() { result.First() } : result;
        }

        public static List<DDRecord> GetSubflowsDDRecords(List<DDRecord> records, DDRecord update)
        {
            var result = new List<DDRecord>();
            Console.WriteLine(string.Format("Searching for {0} context; {1} data key, {2} component", update.ContextName, update.DataKey, update.Section));
            foreach (var metadataDescriptionRecord in records)
            {
                if (metadataDescriptionRecord.ContextName.Trim().Equals(update.ContextName)
                    && (string.IsNullOrEmpty(metadataDescriptionRecord.DataKey))
                    && update.FieldType.Equals(_gridComponenetName)
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

        public static List<DDRecord> GetDDRecordUsingDataKeyAndContextName(List<DDRecord> records, DDRecord update)
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
        private static bool SectionNamePartiallyMatchesComponentUniqueName(DDRecord metadataDescriptionRecord, DDRecord update)
        {
            return metadataDescriptionRecord.ComponentUniqueName.Substring(0, 3).Contains(update.Section.Substring(0, 3));
        }
        public static void RemoveRecords(List<DDRecord> records, IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                records.RemoveAll(el => el.Id == id);
            }
        }

        public static List<DDRecord> RemoveDuplications(List<DDRecord> records)
        {
            var result = new List<DDRecord>();
            foreach (var record in records)
            {
                var allMatchingDuplications = records.Where(el =>
                    (el.ContextName.Equals(record.ContextName) && string.IsNullOrEmpty(el.DataKey) && el.ComponentUniqueName.Equals(record.ComponentUniqueName)) ||
                     ((!string.IsNullOrEmpty(el.DataKey) && el.DataKey.Equals(record.DataKey) && el.ContextName.Equals(record.ContextName))));
                if (allMatchingDuplications.Any())
                {
                    if (result.Any(el =>
                        (string.IsNullOrEmpty(el.DataKey) || el.DataKey.Equals(record.DataKey))
                        && el.ContextName.Equals(record.ContextName)
                        && el.ComponentUniqueName.Equals(record.ComponentUniqueName)))
                    {
                        continue;
                    }
                    else
                    {
                        var mostDescriptiveRecord = allMatchingDuplications.FirstOrDefault(el =>
                            !string.IsNullOrEmpty(el.BusinessDescription) && !string.IsNullOrEmpty(el.FieldBehaviour));
                        result.Add(mostDescriptiveRecord ?? allMatchingDuplications.First());
                    }
                }
            }

            return result;
        }


        private static bool RecordWasModified(DDRecord record, DDRecord update)
        {
            var result = false;
            if (string.IsNullOrEmpty(record.BusinessDescription) && !string.IsNullOrEmpty(update.BusinessDescription))
            {
                record.BusinessDescription = update.BusinessDescription;
                result = true;
            }

            if (string.IsNullOrEmpty(record.FieldBehaviour) && !string.IsNullOrEmpty(update.FieldBehaviour))
            {
                record.FieldBehaviour = update.FieldBehaviour;
                result = true;
            }

            return result;
        }
    }
}
