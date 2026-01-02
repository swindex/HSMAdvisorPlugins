using HSMAdvisorPlugin;
using HSMAdvisorDatabase;
using HSMAdvisorDatabase.ToolDataBase;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;
using ImportCsvTools.Forms;

namespace ImportCsvTools
{
    public class CsvToolImporter : ToolsPluginInterface
    {
        public override DataBase ImportTools()
        {
            // First, select the CSV file to import
            var csvFileName = ShowOpenFileDialog(
                "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                "Select a CSV tool list to import");

            if (csvFileName == null)
            {
                return null;
            }

            // Read CSV columns with unique values
            List<CsvImportColumnInfo> csvColumns = ReadCsvColumnsWithData(csvFileName);

            // Show mapping selection form with CSV column data
            string mappingFileName;
            using (var selectionForm = new MappingSelectionForm())
            {
                selectionForm.CsvColumns = csvColumns;
                
                if (selectionForm.ShowDialog() != DialogResult.OK)
                {
                    return null;
                }

                mappingFileName = selectionForm.SelectedMappingFile;
            }

            if (string.IsNullOrWhiteSpace(mappingFileName))
            {
                return null;
            }

            return ImportFromFiles(csvFileName, mappingFileName);
        }

        public override void ExportTools(DataBase src)
        {
            throw new NotImplementedException();
        }

        public override void ModifyTools(DataBase db)
        {
            throw new NotImplementedException();
        }

        public override List<Capability> GetCapabilities()
        {
            var caps = new List<Capability>
            {
                new Capability("Import CSV Tool Database", (int)ToolsPluginCapabilityMethod.ImportTools)
            };

            return caps;
        }

        private static List<CsvImportColumnInfo> ReadCsvColumnsWithData(string csvFileName)
        {
            var columns = new List<CsvImportColumnInfo>();
            
            try
            {
                using (var parser = new TextFieldParser(csvFileName))
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to read CSV data:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return columns;
        }

        public static DataBase ImportFromFiles(string csvFileName, string mappingFileName)
        {
            if (string.IsNullOrWhiteSpace(csvFileName))
            {
                throw new ArgumentException("CSV file path must be provided.", nameof(csvFileName));
            }

            if (string.IsNullOrWhiteSpace(mappingFileName))
            {
                throw new ArgumentException("Mapping file path must be provided.", nameof(mappingFileName));
            }

            var mapping = CsvMappingConfig.Load(mappingFileName);

            var targetDb = new DataBase();
            var libraryName = string.IsNullOrWhiteSpace(mapping.LibraryName)
                ? "CSV Import"
                : mapping.LibraryName;

            if (!string.IsNullOrWhiteSpace(libraryName))
            {
                targetDb.AddLibrary(libraryName);
            }

            var headerIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var skippedToolsCount = 0;
            var totalToolsCount = 0;

            using (var parser = new TextFieldParser(csvFileName))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                // Always read the first row as headers
                var headers = parser.ReadFields();
                if (headers != null)
                {
                    for (var i = 0; i < headers.Length; i += 1)
                    {
                        var header = headers[i]?.Trim();
                        if (!string.IsNullOrWhiteSpace(header) && !headerIndex.ContainsKey(header))
                        {
                            headerIndex[header] = i;
                        }
                    }
                }

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields == null || fields.Length == 0)
                    {
                        continue;
                    }

                    totalToolsCount++;
                    var tool = new Tool(true);

                    foreach (var map in mapping.Mappings)
                    {
                        var rawValue = GetCsvValue(map, fields, headerIndex);
                        if (string.IsNullOrWhiteSpace(rawValue))
                        {
                            rawValue = map.DefaultValue;
                        }

                        if (string.IsNullOrWhiteSpace(rawValue))
                        {
                            continue;
                        }
                        
                        if (!TryApplyMapping(tool, map, rawValue, out var error))
                        {
                            Debug.WriteLine(error);
                        }
                    }

                    // Set metric flags if needed
                    if (tool._Input_units_m != null && Parse.ToBoolean(tool._Input_units_m))
                    {
                        // If Input_units_m is true, set metric flags
                        SetMetricFlags(tool);
                    }
                    else if (string.Equals(mapping.CsvInputUnits, "mm", StringComparison.OrdinalIgnoreCase))
                    {
                        // If mapping specifies CSV input units as mm, set metric flags
                        SetMetricFlags(tool);
                    }
                

                    if (string.IsNullOrWhiteSpace(tool.Library))
                    {
                        tool.Library = libraryName;
                    }

                    if (!string.IsNullOrWhiteSpace(tool.Library))
                    {
                        targetDb.AddLibrary(tool.Library);
                    }

                    // Validate tool has required fields if AllowInvalidToolImport is false
                    if (!mapping.AllowInvalidToolImport)
                    {
                        if (!ReflectionHelpers.ValidateToolHasRequiredFields(tool))
                        {
                            skippedToolsCount++;
                            Debug.WriteLine($"Skipped tool due to missing required fields. Tool: {tool.Comment ?? "(no comment)"}");
                            continue;
                        }
                    }

                    targetDb.Tools.Add(tool);
                }
            }

            // Show summary if tools were skipped
            if (skippedToolsCount > 0)
            {
                MessageBox.Show(
                    $"Import completed.\n\n" +
                    $"Total rows processed: {totalToolsCount}\n" +
                    $"Tools imported: {totalToolsCount - skippedToolsCount}\n" +
                    $"Tools skipped (missing required fields): {skippedToolsCount}\n\n" +
                    $"To import tools with missing required fields, enable 'Allow Invalid Tool Import' in the mapping configuration.",
                    "Import Summary",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            return targetDb;
        }

        private static string GetCsvValue(CsvMapping map, string[] fields, Dictionary<string, int> headerIndex)
        {
            if (!string.IsNullOrWhiteSpace(map.CsvColumn) && headerIndex.TryGetValue(map.CsvColumn, out var headerPos))
            {
                if (headerPos >= 0 && headerPos < fields.Length)
                {
                    return fields[headerPos]?.Trim();
                }
            }

            return null;
        }

        private static bool TryApplyMapping(Tool tool, CsvMapping map, string rawValue, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(map.ToolField))
            {
                error = "ToolField is not defined in mapping entry.";
                return false;
            }

            var property = typeof(Tool).GetProperty(
                map.ToolField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property == null)
            {
                error = $"Tool field '{map.ToolField}' was not found on Tool.";
                return false;
            }

            var value = ApplyValueMap(map, rawValue);
            if (value == null)
            {
                return true;
            }

            if (!TryConvertValue(map, property.PropertyType, value, out var converted))
            {
                error = $"Unable to convert value '{value}' for field '{map.ToolField}'.";
                return false;
            }
                        
            property.SetValue(tool, converted);
            return true;
        }

        private static string ApplyValueMap(CsvMapping map, string rawValue)
        {
            if (map.ValueMap == null || map.ValueMap.Count == 0)
            {
                return rawValue;
            }

            foreach (var pair in map.ValueMap)
            {
                if (string.Equals(pair.Key, rawValue, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Value;
                }
            }

            return rawValue;
        }

        private static bool TryConvertValue(CsvMapping map, Type targetType, string rawValue, out object converted)
        {
            converted = null;

            var originalRawValue = rawValue;

            // Evaluate expression if defined. The result is converted to string for further processing.
            rawValue = ExpressionEvaluator.EvaluateExpression(map.Expression, rawValue).ToString();

            if (targetType == typeof(string))
            {
                converted = rawValue;
                return true;
            }

            if (targetType == typeof(bool))
            {
                if (TryParseBool(rawValue, out var boolValue))
                {
                    converted = boolValue;
                    return true;
                }

                return false;
            }

            if (targetType.IsEnum)
            {
                try
                {
                    converted = Enum.Parse(targetType, originalRawValue);
                    return true;
                }
                catch
                {
                    return false;
                }
               
            }

            if (targetType == typeof(int))
            {
                if (int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                {
                    converted = intValue;
                    return true;
                }

                return false;
            }

            if (targetType == typeof(double))
            {
                if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
                {
                    converted = doubleValue;
                    return true;
                }

                return false;
            }

            if (targetType == typeof(float))
            {
                if (float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
                {
                    converted = floatValue;
                    return true;
                }

                return false;
            }

            if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var decimalValue))
                {
                    converted = decimalValue;
                    return true;
                }

                return false;
            }

            return false;
        }
                
        private static bool TryParseBool(string rawValue, out bool value)
        {
            if (bool.TryParse(rawValue, out value))
            {
                return true;
            }

            if (int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            {
                value = intValue != 0;
                return true;
            }

            if (string.Equals(rawValue, "yes", StringComparison.OrdinalIgnoreCase))
            {
                value = true;
                return true;
            }

            if (string.Equals(rawValue, "no", StringComparison.OrdinalIgnoreCase))
            {
                value = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets all boolean properties ending with "_m" to true to indicate metric units
        /// </summary>
        private static void SetMetricFlags(Tool tool)
        {
            var properties = typeof(Tool).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            
            foreach (var property in properties)
            {
                // Check if property is a boolean and ends with "_m"
                if (property.PropertyType == typeof(bool) && 
                    property.CanWrite && 
                    property.Name.EndsWith("_m", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        property.SetValue(tool, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to set metric flag '{property.Name}': {ex.Message}");
                    }
                }
            }
        }

        private string ShowOpenFileDialog(string filter, string title)
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = string.Empty,
                Title = title,
                Filter = filter,
                AddExtension = true,
                SupportMultiDottedExtensions = true,
                CheckFileExists = true
            };

            var ret = openFileDialog.ShowDialog();
            if (ret == DialogResult.OK && File.Exists(openFileDialog.FileName))
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }    
}
