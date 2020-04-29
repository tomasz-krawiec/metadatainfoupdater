using System;
using System.Collections.Generic;
using System.IO;
using MateDataInfoUpdater.Helper;
using MetadataInformationUpdater;

namespace MateDataInfoUpdater.IO
{
    public class TxtFileHelper : IFileReaderHelper, IFileWriterHelper
    {
        public List<DDRecord> ReadLines(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var counter = 0;
            int i = 0;
            var result = new List<DDRecord>();
            if (lines.Length < 1)
                return new List<DDRecord>();
            do
            {
                var ddLine = lines[i++];
                while (!ddLine.Contains(Constants.CommandEnding))
                {
                    ddLine += '\n' + lines[i++];
                }

                ddLine = ddLine.Trim().Replace(Constants.InsertPrefix, string.Empty);
                ddLine = ddLine.Replace(Constants.CommandEnding, string.Empty);
                ddLine = ddLine.Replace(Constants.DDSeparator, Constants.TempSeparator);
                ddLine = ddLine.Replace(Constants.DDSeparator4, Constants.TempSeparator);
                ddLine = ddLine.Replace(Constants.DDSeparator2, Constants.TempSeparator + Constants.NULLFragment);
                ddLine = ddLine.Replace(Constants.DDSeparator5, Constants.TempSeparator + Constants.NULLFragment);
                ddLine = ddLine.Replace(", N'", Constants.TempSeparator);
                ddLine = ddLine.Replace(Constants.DDSeparator3, string.Empty).Trim('\'').Trim();
                var tokens = ddLine.Split(Constants.TempSeparator.ToCharArray());
                result.Add(new DDRecord()
                {
                    Id = counter++,
                    ContextName = TextUtility.CleanToken(tokens[0]),
                    DataKey = TextUtility.CleanToken(tokens[1]),
                    BusinessDescription = TextUtility.CleanToken(tokens[2]),
                    FieldBehaviour = tokens.Length > 3 ? TextUtility.CleanToken(tokens[3]) : String.Empty,
                    ComponentUniqueName = tokens.Length > 4 ? TextUtility.CleanToken(tokens[4]) : string.Empty
                });
            } while (i < lines.Length);

            return result;
        }

        public void WriteToFile(string path, List<DDRecord> records)
        {
            using (StreamWriter outputFile = new System.IO.StreamWriter(path))
            {
                foreach (var resultDescription in records)
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
    }
}
