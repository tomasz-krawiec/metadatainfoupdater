using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using MetadataInformationUpdater;

namespace MateDataInfoUpdater.DescriptionUpdatesLogic
{
    public class MissingTableNameAndColumnNameResolver
    {
        private static readonly string _path = @"F:\Fenergo One Drive\OneDrive - Fenergo\Data dictionary\MissingColumnNameAndTableName\mapContextTableMetadata.txt";
        private static readonly string dbConnString =@"Data Source=localhost;Initial Catalog=FenergoDev; User Id=sa; Password=ergo.1234";
        private readonly Dictionary<string, List<string>> _contextDataKeysMap = new Dictionary<string,List<string>>();
        private List<string> resultInsertQuery=new List<string>();
        private static StreamWriter _streamWriter;
        public void Run()
        {
            var dtKeysAndContexts = new List<DDRecord>();// FileReaderHelper.ReadColumnNameTableNameConfig(_path);
            _streamWriter = new System.IO.StreamWriter(@"F:\Fenergo One Drive\OneDrive - Fenergo\Data dictionary\mergeCommands.txt");
            foreach (var dtKeyAndContext in dtKeysAndContexts)
            {
                var mappingRecord = string.Format(
                    @"MERGE INTO tools.DDGenerationMapping gm USING (SELECT Id ContextId, '{0}' DataKey, '{1}' TableName,'{2}' ColumnName FROM ui.Context WHERE ContextName = '{3}') c ON(gm.ContextId = c.ContextId AND gm.DataKey = c.DataKey) WHEN MATCHED THEN UPDATE SET gm.TableName = c.TableName, gm.ColumnName = c.ColumnName WHEN NOT MATCHED THEN INSERT(ContextId, DataKey, TableName, ColumnName) VALUES(c.ContextId, c.DataKey, c.TableName, c.ColumnName);",
                    dtKeyAndContext.DataKey, dtKeyAndContext.TableName, dtKeyAndContext.ColumnName, dtKeyAndContext.ContextName);

                _streamWriter.WriteLine(mappingRecord);
            }
            _streamWriter.Close();
            
        }

        public void ResolveMissingColNamesTableNames()
        {
            CreateContextNameDtKeysListMap();
            foreach (var contextDataKeysEntry in _contextDataKeysMap)
            {
                var dsNames = GetDataSourcesNamesLinkedToContext(contextDataKeysEntry.Key);
                if (dsNames.Count > 0)
                {
                    var dsNamesConcatenaed = "";
                    dsNames.ForEach(el => dsNamesConcatenaed += "'" + el + "',");
                    var dataSourcesList = GetDataSourcesLinkedToContext(dsNamesConcatenaed.TrimEnd(','));
                    foreach (var dataKey in contextDataKeysEntry.Value)
                    {
                        foreach (var dsConfig in dataSourcesList)
                        {
                            var sourceTableName = dsConfig.SelectSingleNode("/dataSource")
                                .Attributes["sourceObjectName"].Value;
                            var matchingNode =
                                dsConfig.SelectSingleNode(
                                    string.Format("/dataSource/dataFields/dataField[@key=\"{0}\"]", dataKey));
                            if (matchingNode != null)
                            {
                                var sourceFieldName = matchingNode.Attributes["sourceFieldName"].Value;
                                /* MERGE INTO tools.DDGenerationMapping gm USING
                                     (SELECT Id ContextId, 'dbo_LECompany_LookupSicCodeId_RiskFactorId' DataKey, 'N/A - Not persisted in DB' TableName,
                                 'N/A - Not persisted in DB' ColumnName FROM ui.Context WHERE ContextName = 'completeRiskAssessment-Company')
                                 c ON(gm.ContextId = c.ContextId AND gm.DataKey = c.DataKey) WHEN MATCHED THEN UPDATE SET gm.TableName = c.TableName, 
                                 gm.ColumnName = c.ColumnName WHEN NOT MATCHED THEN INSERT(ContextId, DataKey, TableName, ColumnName)
                                 VALUES(c.ContextId, c.DataKey, c.TableName, c.ColumnName);*/


                                var mappingRecord = string.Format(
                                    @"MERGE INTO tools.DDGenerationMapping gm USING (SELECT Id ContextId, '{0}' DataKey, '{1}' TableName,'{2}' ColumnName FROM ui.Context WHERE ContextName = '{3}') c ON(gm.ContextId = c.ContextId AND gm.DataKey = c.DataKey) WHEN MATCHED THEN UPDATE SET gm.TableName = c.TableName, gm.ColumnName = c.ColumnName WHEN NOT MATCHED THEN INSERT(ContextId, DataKey, TableName, ColumnName) VALUES(c.ContextId, c.DataKey, c.TableName, c.ColumnName);",
                                    dataKey, sourceTableName, sourceFieldName, contextDataKeysEntry.Key);

                                resultInsertQuery.Add(mappingRecord);
                            }
                        }
                    }
                }
            }
        }

        public void CreateContextNameDtKeysListMap()
        {
            var dtKeysAndContexts = new List<DDRecord>();// FileReaderHelper.ReadLinesFromFileRaw2(_path);
            var contextName = string.Empty;
            foreach (var dtKeysAndContext in dtKeysAndContexts)
            {
                if (!dtKeysAndContext.ContextName.Equals(contextName))
                {
                    contextName = dtKeysAndContext.ContextName;
                    _contextDataKeysMap.Add(contextName, new List<string>());
                }
                _contextDataKeysMap[contextName].Add(dtKeysAndContext.DataKey);
            }
        }

        public List<string> GetDataSourcesNamesLinkedToContext(string contextName)
        {
            var dataSourcesList = new List<string>();
            using (SqlConnection conn = new SqlConnection(dbConnString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("select EntityDataDataSourceName from ui.LinkEnDtSourceCx where ContextName='{0}'",contextName);
                SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.Default);
                while (sdr.Read())
                {
                    dataSourcesList.Add(sdr[0].ToString());
                }
                sdr.Close();
                conn.Close();

            }

            return dataSourcesList;
        }

        public List<XmlDocument> GetDataSourcesLinkedToContext(string dsNames)
        {
            var dataSourcesList = new List<XmlDocument>();
            using (SqlConnection conn = new SqlConnection(dbConnString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("select MetaData from ui.EntityDataSource where [Name] in ({0})", dsNames);
                SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.Default);
                while (sdr.Read())
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(sdr[0].ToString());
                    dataSourcesList.Add(xmlDoc);
                }
                sdr.Close();
                conn.Close();

            }

            return dataSourcesList;
        }
    }
}
