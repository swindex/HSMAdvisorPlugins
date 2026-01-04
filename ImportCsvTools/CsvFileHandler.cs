using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImportCsvTools
{
    /// <summary>
    /// Represents CSV file data with headers and rows
    /// </summary>
    public class CsvData
    {
        /// <summary>
        /// CSV column headers
        /// </summary>
        public List<string> Headers { get; set; }

        /// <summary>
        /// CSV data rows (each row is an array of field values)
        /// </summary>
        public List<string[]> Rows { get; set; }

        public CsvData()
        {
            Headers = new List<string>();
            Rows = new List<string[]>();
        }
    }

    /// <summary>
    /// Provides reusable CSV file reading and writing utilities
    /// </summary>
    public static class CsvFileHandler
    {
        /// <summary>
        /// Reads a CSV file and returns structured data with headers and rows
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <returns>CsvData containing headers and rows</returns>
        /// <exception cref="ArgumentException">If file path is null or empty</exception>
        /// <exception cref="FileNotFoundException">If file does not exist</exception>
        /// <exception cref="IOException">If file cannot be read</exception>
        public static CsvData ReadCsvFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path must be provided.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}", filePath);
            }

            var csvData = new CsvData();

            using (var parser = new TextFieldParser(filePath))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                // Read first row as headers
                if (!parser.EndOfData)
                {
                    var headers = parser.ReadFields();
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            var trimmedHeader = header?.Trim();
                            csvData.Headers.Add(trimmedHeader ?? string.Empty);
                        }
                    }
                }

                // Read all data rows
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields != null && fields.Length > 0)
                    {
                        // Unescape newline sequences in each field
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (fields[i] != null)
                            {
                                fields[i] = UnescapeCsvValue(fields[i]);
                            }
                        }
                        csvData.Rows.Add(fields);
                    }
                }
            }

            return csvData;
        }

        /// <summary>
        /// Reads a CSV file and analyzes columns, returning column information with unique values
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <returns>List of CsvImportColumnInfo with unique values for each column</returns>
        /// <exception cref="ArgumentException">If file path is null or empty</exception>
        /// <exception cref="FileNotFoundException">If file does not exist</exception>
        /// <exception cref="IOException">If file cannot be read</exception>
        public static List<CsvImportColumnInfo> ReadCsvColumns(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path must be provided.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}", filePath);
            }

            var columns = new List<CsvImportColumnInfo>();

            using (var parser = new TextFieldParser(filePath))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                // Read first row as headers
                if (parser.EndOfData)
                {
                    return columns;
                }

                var headers = parser.ReadFields();
                if (headers == null || headers.Length == 0)
                {
                    return columns;
                }

                // Initialize columns with headers and HashSets for unique values
                var uniqueValueSets = new List<HashSet<string>>();
                for (int i = 0; i < headers.Length; i++)
                {
                    var trimmedHeader = headers[i]?.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedHeader))
                    {
                        columns.Add(new CsvImportColumnInfo(trimmedHeader));
                        uniqueValueSets.Add(new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                    }
                }

                // Read all data rows and collect unique values
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < fields.Length && i < uniqueValueSets.Count; i++)
                    {
                        var value = fields[i]?.Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            uniqueValueSets[i].Add(value);
                        }
                    }
                }

                // Convert HashSets to sorted lists
                for (int i = 0; i < columns.Count; i++)
                {
                    columns[i].UniqueValues = new List<string>(uniqueValueSets[i]);
                    columns[i].UniqueValues.Sort(StringComparer.OrdinalIgnoreCase);
                }
            }

            return columns;
        }

        /// <summary>
        /// Writes data to a CSV file with headers and rows
        /// </summary>
        /// <param name="filePath">Path to the output CSV file</param>
        /// <param name="headers">List of column headers</param>
        /// <param name="rows">List of data rows (each row is a list of field values)</param>
        /// <exception cref="ArgumentException">If file path is null or empty</exception>
        /// <exception cref="ArgumentNullException">If headers or rows are null</exception>
        /// <exception cref="IOException">If file cannot be written</exception>
        public static void WriteCsvFile(string filePath, List<string> headers, List<List<string>> rows)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path must be provided.", nameof(filePath));
            }

            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Write header row
                var escapedHeaders = new List<string>();
                foreach (var header in headers)
                {
                    escapedHeaders.Add(EscapeCsvValue(header));
                }
                writer.WriteLine(string.Join(",", escapedHeaders));

                // Write data rows
                foreach (var row in rows)
                {
                    var escapedValues = new List<string>();
                    foreach (var value in row)
                    {
                        escapedValues.Add(EscapeCsvValue(value));
                    }
                    writer.WriteLine(string.Join(",", escapedValues));
                }
            }
        }

        /// <summary>
        /// Escapes a CSV value by handling special characters (commas, quotes, newlines)
        /// </summary>
        /// <param name="value">The value to escape</param>
        /// <returns>Escaped CSV value ready for output</returns>
        public static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Replace newlines and carriage returns with escaped sequences for better compatibility
            var escapedValue = value.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\n");

            // If value contains comma or quote, wrap in quotes and escape internal quotes
            if (escapedValue.Contains(",") || escapedValue.Contains("\""))
            {
                return "\"" + escapedValue.Replace("\"", "\"\"") + "\"";
            }

            return escapedValue;
        }

        /// <summary>
        /// Unescapes a CSV value by converting escaped newline sequences back to actual newlines
        /// </summary>
        /// <param name="value">The value to unescape</param>
        /// <returns>Unescaped value with actual newline characters</returns>
        public static string UnescapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Unescape newline sequences that were escaped during export
            return value.Replace("\\n", "\n");
        }
    }
}
