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
            if (src == null || src.Tools == null || src.Tools.Count == 0)
            {
                MessageBox.Show("No tools to export.", "Export", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Step 1: Select mapping configuration
            string mappingFileName;
            using (var selectionForm = new MappingSelectionForm())
            {
                // Note: For export, we don't have CSV columns to display
                // The form will just show available mapping files
                if (selectionForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                mappingFileName = selectionForm.SelectedMappingFile;
            }

            if (string.IsNullOrWhiteSpace(mappingFileName))
            {
                return;
            }

            // Step 2: Select output CSV file path
            var csvFileName = ShowSaveFileDialog(
                "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                "Export tools to CSV file");

            if (csvFileName == null)
            {
                return;
            }

            // Step 3: Perform export
            try
            {
                ExportToFile(src, csvFileName, mappingFileName);
                
                MessageBox.Show(
                    $"Successfully exported {src.Tools.Count} tool(s) to:\n{csvFileName}",
                    "Export Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to export tools:\n{ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public override void ModifyTools(DataBase db)
        {
            throw new NotImplementedException();
        }

        public override List<Capability> GetCapabilities()
        {
            var caps = new List<Capability>
            {
                new Capability("Import CSV Tool Database", (int)ToolsPluginCapabilityMethod.ImportTools),
                new Capability("Export CSV Tool Database", (int)ToolsPluginCapabilityMethod.ExportTools)
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

        public static DataBase ImportFromFiles(string csvFileName, string mappingFileName, MessageFlags msgFlag = MessageFlags.Error)
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

            if (skippedToolsCount > 0 && msgFlag > 0)
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

        public static void ExportToFile(DataBase database, string csvFileName, string mappingFileName)
        {
            if (string.IsNullOrWhiteSpace(csvFileName))
            {
                throw new ArgumentException("CSV file path must be provided.", nameof(csvFileName));
            }

            if (string.IsNullOrWhiteSpace(mappingFileName))
            {
                throw new ArgumentException("Mapping file path must be provided.", nameof(mappingFileName));
            }

            if (database == null || database.Tools == null || database.Tools.Count == 0)
            {
                throw new ArgumentException("Database must contain tools to export.", nameof(database));
            }

            var mapping = CsvMappingConfig.Load(mappingFileName);

            using (var writer = new StreamWriter(csvFileName, false, System.Text.Encoding.UTF8))
            {
                // Write header row
                var headers = new List<string>();
                foreach (var map in mapping.Mappings)
                {
                    if (!string.IsNullOrWhiteSpace(map.CsvColumn))
                    {
                        headers.Add(EscapeCsvValue(map.CsvColumn));
                    }
                }
                writer.WriteLine(string.Join(",", headers));

                // Write data rows
                foreach (var tool in database.Tools)
                {
                    var values = new List<string>();
                    foreach (var map in mapping.Mappings)
                    {
                        var value = GetExportValue(tool, map, mapping.CsvInputUnits);
                        values.Add(EscapeCsvValue(value));
                    }
                    writer.WriteLine(string.Join(",", values));
                }
            }
        }

        private static string GetExportValue(Tool tool, CsvMapping map, string csvUnits)
        {
            if (string.IsNullOrWhiteSpace(map.ToolField))
            {
                return string.Empty;
            }

            var property = typeof(Tool).GetProperty(
                map.ToolField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (property == null)
            {
                return string.Empty;
            }

            var rawValue = property.GetValue(tool);
            if (rawValue == null)
            {
                return map.DefaultValue ?? string.Empty;
            }

            string stringValue;

            // Handle enum values - convert to enum name
            if (property.PropertyType.IsEnum)
            {
                stringValue = rawValue.ToString();
            }
            // Handle numeric values with potential unit conversion
            else if (rawValue is double || rawValue is float || rawValue is decimal)
            {
                double numericValue = Convert.ToDouble(rawValue);
                numericValue = ConvertForExport(tool, map.ToolField, numericValue, csvUnits);
                stringValue = numericValue.ToString(CultureInfo.InvariantCulture);
            }
            // Handle integer values (including enum underlying values)
            else if (rawValue is int || rawValue is long || rawValue is short)
            {
                int intValue = Convert.ToInt32(rawValue);
                
                // Check if this is an enum field by looking at the EnumType or field name
                if (!string.IsNullOrWhiteSpace(map.EnumType))
                {
                    // Convert int enum value to enum name
                    var enumType = typeof(Enums).GetNestedType(map.EnumType, BindingFlags.Public);
                    if (enumType != null && enumType.IsEnum)
                    {
                        try
                        {
                            stringValue = Enum.GetName(enumType, intValue) ?? intValue.ToString();
                        }
                        catch
                        {
                            stringValue = intValue.ToString();
                        }
                    }
                    else
                    {
                        stringValue = intValue.ToString();
                    }
                }
                else
                {
                    stringValue = intValue.ToString();
                }
            }
            else
            {
                stringValue = rawValue.ToString();
            }

            // Apply reverse value map (Tool value → CSV value)
            stringValue = ApplyReverseValueMap(map, stringValue);

            // Apply export expression if defined (overrides everything else)
            if (!string.IsNullOrWhiteSpace(map.ExportExpression))
            {
                try
                {
                    stringValue = ExpressionEvaluator.EvaluateExpression(map.ExportExpression, stringValue).ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to evaluate export expression for field '{map.ToolField}': {ex.Message}");
                    // Keep the original value if expression fails
                }
            }

            return stringValue ?? string.Empty;
        }

        private static string ApplyReverseValueMap(CsvMapping map, string toolValue)
        {
            if (map.ValueMap == null || map.ValueMap.Count == 0)
            {
                return toolValue;
            }

            // Find the CSV key that maps to this tool value
            foreach (var pair in map.ValueMap)
            {
                if (string.Equals(pair.Value, toolValue, StringComparison.OrdinalIgnoreCase))
                {
                    return pair.Key;  // Return the CSV value (key)
                }
            }

            return toolValue;  // Return as-is if no mapping found
        }

        private static double ConvertForExport(Tool tool, string fieldName, double toolValue, string csvUnits)
        {
            // If mixed units, export as-is (no conversion)
            if (string.Equals(csvUnits, "mixed", StringComparison.OrdinalIgnoreCase))
            {
                return toolValue;
            }

            // Check if this field is dimensional (has corresponding _m flag)
            var metricFlagName = fieldName + "_m";
            var metricProperty = typeof(Tool).GetProperty(
                metricFlagName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (metricProperty == null || metricProperty.PropertyType != typeof(bool))
            {
                // Not a dimensional field, return as-is
                return toolValue;
            }

            // Get the tool's unit flag
            bool toolIsMetric = (bool)metricProperty.GetValue(tool);
            bool csvIsMetric = string.Equals(csvUnits, "mm", StringComparison.OrdinalIgnoreCase);

            // Convert if units don't match
            if (toolIsMetric && !csvIsMetric)
            {
                // Tool is metric, CSV wants inches
                return toolValue * 0.03937007874;  // mm → inches
            }
            else if (!toolIsMetric && csvIsMetric)
            {
                // Tool is inches, CSV wants metric
                return toolValue * 25.4;  // inches → mm
            }

            return toolValue;  // Units match, no conversion
        }

        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // If value contains comma, quote, or newline, wrap in quotes and escape internal quotes
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
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

        private string ShowSaveFileDialog(string filter, string title)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = string.Empty,
                Title = title,
                Filter = filter,
                AddExtension = true,
                SupportMultiDottedExtensions = true,
                OverwritePrompt = true
            };

            var ret = saveFileDialog.ShowDialog();
            if (ret == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }
    }    
}
