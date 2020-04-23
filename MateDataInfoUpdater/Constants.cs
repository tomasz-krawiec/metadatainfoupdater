namespace MateDataInfoUpdater
{
    public static class Constants
    {
        public static string InsertPrefix =
            "INSERT INTO ui.EnDtFieldDescription(ContextName, DataKey, BusinessDescription, FieldBehaviour, ComponentUniqueName) VALUES (";

        public static string DDSeparator = "', N'";
        public static string DDSeparator2 = "', NULL";
        public static string DDSeparator3 = "N'";
        public static string DDSeparator4 = "', '";
        public static string DDSeparator5 = ", N''";
        public static string TempSeparator = "^";
        public static string NULLFragment = "NULL";
        public static string NvarcharPrefix = "N'";
        public static string CommandEnding = ");";

        public static string DDInsertCommandTemplate =
            "Insert.IntoTable(nameof(EnDtFieldDescription)).InSchema(\"ui\").Row(new EnDtFieldDescription(){ContextName = {0}, DataKey = {1}, BusinessDescription ={2}, FieldBehaviour = {3}})";
    }
}
