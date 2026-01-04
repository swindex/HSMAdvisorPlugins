using System.Collections.Generic;

namespace ImportCsvTools
{
    /// <summary>
    /// Represents a CSV column with its name and unique values
    /// </summary>
    public class CsvImportColumnInfo
    {
        /// <summary>
        /// The name/header of the CSV column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// List of unique values found in this column
        /// </summary>
        public List<string> UniqueValues { get; set; }

        public CsvImportColumnInfo()
        {
            UniqueValues = new List<string>();
        }

        public CsvImportColumnInfo(string columnName)
        {
            ColumnName = columnName;
            UniqueValues = new List<string>();
        }

        public override string ToString()
        {
            return ColumnName ?? "(unnamed column)";
        }
    }
}
