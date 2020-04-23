using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataInformationUpdater;
using Newtonsoft.Json;

namespace MateDataInfoUpdater.IO
{
    public class JsonFileHelper : IFileReaderHelper
    {
        private readonly string SearchPattern = "*.json"; //you can leave it like this or put phrase e.g. *EMIR.json than you will only work on emir json files

        public List<DDRecord> ReadLines(string path)
        {
            var result = new List<DDRecord>();
            foreach (string file in Directory.EnumerateFiles(path, SearchPattern))
            {
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    result.Add(JsonConvert.DeserializeObject<DDRecord>(json));
                }
            }
            return result;
        }

        public void WriteChangesToFiles(List<DDRecord> records,string path)
        {
            foreach (var resultDescription in records)
            {
                using (StreamWriter outputFile = new System.IO.StreamWriter(string.Format(@"{0}\{1}-{2}.json", path, resultDescription.Id, resultDescription.ContextName)))
                {
                    if (!string.IsNullOrEmpty(resultDescription.ContextName) ||
                        !string.IsNullOrEmpty(resultDescription.DataKey))
                    {
                        outputFile.Write(JsonConvert.SerializeObject(resultDescription, Formatting.Indented));
                    }
                }
            }
        }
    }
}
