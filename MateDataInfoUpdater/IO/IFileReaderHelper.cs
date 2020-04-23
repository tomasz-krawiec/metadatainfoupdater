using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Newtonsoft.Json;

namespace MetadataInformationUpdater
{
    public interface IFileReaderHelper
    {
        List<DDRecord> ReadLines(string path);
/*
        public static List<DDRecord> ReadLinesFromFileRaw(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var counter = 0;
            var result = new List<DDRecord>();
            foreach (var line in lines)
            {
                var tokens = line.Split(_separator);
                // Console.WriteLine(line);
                result.Add(new DDRecord()
                {
                    Id = counter++,
                    ContextName = tokens[0].Trim(),
                    DataKey = tokens[1].Trim(),
                    BusinessDescription = tokens[2].Trim(),
                    FieldBehaviour = tokens.Length > 3 ? tokens[3].Trim() : String.Empty,
                    ComponentUniqueName = tokens.Length > 4 ? tokens[4].Trim() : string.Empty
                });
            }

            return result;
        }

        public static List<DDRecord> ReadLinesFromFileRaw2(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var counter = 0;
            var result = new List<DDRecord>();
            foreach (var line in lines)
            {
                var tokens = line.Split(_separator);
                //  Console.WriteLine(line);
                result.Add(new DDRecord()
                {
                    Id = counter++,
                    DataKey = tokens[0].Trim(),
                    ContextName = tokens[1].Trim(),
                    BusinessDescription = tokens.Length>2?tokens[2].Trim():string.Empty,
                    FieldBehaviour = tokens.Length > 3 ? tokens[3].Trim() : String.Empty,
                    //ComponentUniqueName = tokens[4]
                });
            }

            return result;
        }

        public static List<DDRecord> ReadFileConsistedOf3Tokens(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var counter = 0;
            var result = new List<DDRecord>();
            foreach (var line in lines)
            {
                var tokens = line.Split(_separator);
                // Console.WriteLine(line);
                result.Add(new DDRecord()
                {
                    Id = counter++,
                    ContextName = tokens[0].Trim(),
                    ComponentUniqueName = tokens[1].Trim(),
                    BusinessDescription = tokens[2].Trim()
                });
            }

            return result;
        }

        public static List<DDRecord> ReadColumnNameTableNameConfig(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var counter = 0;
            var result = new List<DDRecord>();
            foreach (var line in lines)
            {
                var tokens = line.Split(_separator);
                // Console.WriteLine(line);
                result.Add(new DDRecord()
                {
                    Id = counter++,
                    DataKey = tokens[0].Trim(),
                    TableName = tokens[1].Trim(),
                    ColumnName = tokens[2].Trim(),
                    ContextName = tokens[3].Trim(),
                });
            }

            return result;
        }*/
    }
}
