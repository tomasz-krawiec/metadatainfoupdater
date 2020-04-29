using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonLibrary.Helpers;
using MateDataInfoUpdater;
using MateDataInfoUpdater.DescriptionUpdatesLogic;
using MateDataInfoUpdater.IO;

namespace MetadataInformationUpdater
{
    class Program
    {
        private static List<DDRecord> _inputRecords;
        private static List<DDRecord> _resultDescriptions;
        private static List<DDRecord> _recordsWithUpdates;
        private static DescriptionUpdater _descriptionUpdater;
        private static IFileReaderHelper excelFileHelper;
        private static IFileWriterHelper txtFileWriter;

        [STAThread]
        static void Main(string[] args)
        {
            _resultDescriptions = new List<DDRecord>();
            excelFileHelper = new ExcelFileHelper();
            txtFileWriter = new TxtFileHelper();
            Program p=new Program();
            p.RunDescriptionsUpdater();
        }

        public void RunDescriptionsUpdater()
        {
            _inputRecords = new List<DDRecord>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _recordsWithUpdates = excelFileHelper.ReadLines(openFileDialog.FileName);
            }
            _descriptionUpdater = new DescriptionUpdater(_inputRecords, _resultDescriptions);
            _descriptionUpdater.UpdateBusinessDescriptionAndFieldBehaviours(_recordsWithUpdates);
            _resultDescriptions.AddRange(_inputRecords);
            _resultDescriptions = _resultDescriptions.OrderBy(el => el.ContextName).ThenBy(el => el.DataKey).ToList();
            SaveFileDialog saveFileDialog=new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileWriter.WriteToFile(saveFileDialog.FileName+".txt", _resultDescriptions);
            }
            
        }
    }
}
