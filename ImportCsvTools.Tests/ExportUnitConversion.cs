using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase.ToolDataBase;
using ImportCsvTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImportCsvTools.Tests
{
    [TestClass]
    public class ExportUnitConversion
    {
        // Get path to the project root
        private const string TestDataDirectory = @"..\..\test-data";
        private const string OutputDirectory = @"..\..\test-output";

        [ClassInitialize]
        public static void SetupTestData(TestContext context)
        {
            Console.WriteLine("=== Setting up Export Unit Conversion Tests ===");

            var outputDir = Path.GetFullPath(OutputDirectory);

            // Ensure output directory exists
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Console.WriteLine("=== Setup Complete ===\n");
        }

        [TestMethod]
        public void TestExportConversionMetricToInches()
        {
            Console.Write("Testing export conversion metric to inches... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var testOutputPath = Path.Combine(outputDir, "test_metric_to_inches.csv");

            // Load the default mapping and set to inches output
            var mapping = CsvMappingConfig.Load(mappingPath);
            mapping.CsvInputUnits = "in";
            mapping.AllowInvalidToolImport = true;

            // Create database with tool in metric units
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool = new Tool(true);
            tool.Number = 1;
            tool.Library = "Test Library";
            tool.Comment = "Metric Tool";
            tool.Diameter = 25.4; // 25.4 mm = 1 inch
            tool.Diameter_m = true; // Metric
            tool.Flute_Len = 50.8; // 50.8 mm = 2 inches
            tool.Flute_len_m = true;
            tool.Shank_Dia = 12.7; // 12.7 mm = 0.5 inches
            tool.Shank_Dia_m = true;
            tool.Tool_type_id = (Enums.ToolTypes)1; // End mill
            tool.Tool_material_id = (Enums.ToolMaterials)1; // Carbide
            testDb.Tools.Add(tool);

            // Export to CSV with inches mapping
            CsvToolImporter.ExportToFile(testDb, testOutputPath, mapping);

            // Read exported CSV using CsvFileHandler
            var csvData = CsvFileHandler.ReadCsvFile(testOutputPath);
            if (csvData.Rows.Count < 1)
            {
                throw new Exception("Exported file has insufficient data");
            }

            var headers = csvData.Headers.ToArray();
            var values = csvData.Rows[0];

            // Find column indices
            int diameterIndex = FindColumnIndex(headers, "(D) - Cutting Dia");
            int fluteLenIndex = FindColumnIndex(headers, "(WL)  - Working Length");
            int shankDiaIndex = FindColumnIndex(headers, "(DS) Shank Dia");

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column '(D) - Cutting Dia' not found in CSV");
            }

            // Parse and verify diameter conversion
            var exportedDiameter = double.Parse(values[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var expectedDiameter = 1.0; // 25.4mm converted to inches

            if (Math.Abs(exportedDiameter - expectedDiameter) > 0.0001)
            {
                throw new Exception($"Diameter not converted correctly. Expected: {expectedDiameter:F4} inches, Got: {exportedDiameter:F4} inches");
            }

            // Verify flute length conversion if column exists
            if (fluteLenIndex != -1)
            {
                var exportedFluteLen = double.Parse(values[fluteLenIndex], System.Globalization.CultureInfo.InvariantCulture);
                var expectedFluteLen = 2.0; // 50.8mm converted to inches

                if (Math.Abs(exportedFluteLen - expectedFluteLen) > 0.0001)
                {
                    throw new Exception($"Flute length not converted correctly. Expected: {expectedFluteLen:F4} inches, Got: {exportedFluteLen:F4} inches");
                }
            }

            // Verify shank diameter conversion if column exists
            if (shankDiaIndex != -1)
            {
                var exportedShankDia = double.Parse(values[shankDiaIndex], System.Globalization.CultureInfo.InvariantCulture);
                var expectedShankDia = 0.5; // 12.7mm converted to inches

                if (Math.Abs(exportedShankDia - expectedShankDia) > 0.0001)
                {
                    throw new Exception($"Shank diameter not converted correctly. Expected: {expectedShankDia:F4} inches, Got: {exportedShankDia:F4} inches");
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportConversionInchesToMetric()
        {
            Console.Write("Testing export conversion inches to metric... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var testOutputPath = Path.Combine(outputDir, "test_inches_to_metric.csv");

            // Load the default mapping and set to mm output
            var mapping = CsvMappingConfig.Load(mappingPath);
            mapping.CsvInputUnits = "mm";
            mapping.AllowInvalidToolImport = true;

            // Create database with tool in inch units
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool = new Tool(true);
            tool.Number = 1;
            tool.Library = "Test Library";
            tool.Comment = "Inch Tool";
            tool.Diameter = 1.0; // 1 inch = 25.4 mm
            tool.Diameter_m = false; // Inches
            tool.Flute_Len = 2.0; // 2 inches = 50.8 mm
            tool.Flute_len_m = false;
            tool.Shank_Dia = 0.5; // 0.5 inches = 12.7 mm
            tool.Shank_Dia_m = false;
            tool.Tool_type_id = (Enums.ToolTypes)1; // End mill
            tool.Tool_material_id = (Enums.ToolMaterials)1; // Carbide
            testDb.Tools.Add(tool);

            // Export to CSV with mm mapping
            CsvToolImporter.ExportToFile(testDb, testOutputPath, mapping);

            // Read exported CSV using CsvFileHandler
            var csvData = CsvFileHandler.ReadCsvFile(testOutputPath);
            if (csvData.Rows.Count < 1)
            {
                throw new Exception("Exported file has insufficient data");
            }

            var headers = csvData.Headers.ToArray();
            var values = csvData.Rows[0];

            // Find column indices
            int diameterIndex = FindColumnIndex(headers, "(D) - Cutting Dia");
            int fluteLenIndex = FindColumnIndex(headers, "(WL)  - Working Length");
            int shankDiaIndex = FindColumnIndex(headers, "(DS) Shank Dia");

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column '(D) - Cutting Dia' not found in CSV");
            }

            // Parse and verify diameter conversion
            var exportedDiameter = double.Parse(values[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var expectedDiameter = 25.4; // 1 inch converted to mm

            if (Math.Abs(exportedDiameter - expectedDiameter) > 0.01)
            {
                throw new Exception($"Diameter not converted correctly. Expected: {expectedDiameter:F2} mm, Got: {exportedDiameter:F2} mm");
            }

            // Verify flute length conversion if column exists
            if (fluteLenIndex != -1)
            {
                var exportedFluteLen = double.Parse(values[fluteLenIndex], System.Globalization.CultureInfo.InvariantCulture);
                var expectedFluteLen = 50.8; // 2 inches converted to mm

                if (Math.Abs(exportedFluteLen - expectedFluteLen) > 0.01)
                {
                    throw new Exception($"Flute length not converted correctly. Expected: {expectedFluteLen:F2} mm, Got: {exportedFluteLen:F2} mm");
                }
            }

            // Verify shank diameter conversion if column exists
            if (shankDiaIndex != -1)
            {
                var exportedShankDia = double.Parse(values[shankDiaIndex], System.Globalization.CultureInfo.InvariantCulture);
                var expectedShankDia = 12.7; // 0.5 inches converted to mm

                if (Math.Abs(exportedShankDia - expectedShankDia) > 0.01)
                {
                    throw new Exception($"Shank diameter not converted correctly. Expected: {expectedShankDia:F2} mm, Got: {exportedShankDia:F2} mm");
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportConversionMixedUnitsMode()
        {
            Console.Write("Testing export conversion mixed units mode... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var testOutputPath = Path.Combine(outputDir, "test_mixed_units.csv");

            // Load the default mapping and set to mixed units mode
            var mapping = CsvMappingConfig.Load(mappingPath);
            mapping.CsvInputUnits = "mixed";
            mapping.AllowInvalidToolImport = true;

            // Add mapping for Input_units_m field (required for mixed mode)
            mapping.Mappings.Add(new CsvMapping
            {
                CsvColumn = "Input_units_m",
                ToolField = "_Input_units_m"
            });

            // Create database with tools in different units
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            // Tool 1: Metric
            var tool1 = new Tool(true);
            tool1.Number = 1;
            tool1.Library = "Test Library";
            tool1.Comment = "Metric Tool";
            tool1.Diameter = 25.4; // mm (should NOT be converted)
            tool1.Diameter_m = true;
            tool1.Flute_Len = 50.8; // mm
            tool1.Flute_len_m = true;
            tool1._Input_units_m = "true";
            tool1.Tool_type_id = (Enums.ToolTypes)1;
            tool1.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool1);

            // Tool 2: Inches
            var tool2 = new Tool(true);
            tool2.Number = 2;
            tool2.Library = "Test Library";
            tool2.Comment = "Inch Tool";
            tool2.Diameter = 0.5; // inches (should NOT be converted)
            tool2.Diameter_m = false;
            tool2.Flute_Len = 2.0; // inches
            tool2.Flute_len_m = false;
            tool2._Input_units_m = "false";
            tool2.Tool_type_id = (Enums.ToolTypes)1;
            tool2.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool2);

            // Export to CSV with mixed units mapping
            CsvToolImporter.ExportToFile(testDb, testOutputPath, mapping);

            // Read exported CSV using CsvFileHandler
            var csvData = CsvFileHandler.ReadCsvFile(testOutputPath);
            if (csvData.Rows.Count < 2)
            {
                throw new Exception($"Exported file has insufficient data. Expected 2 tools, got {csvData.Rows.Count}");
            }

            var headers = csvData.Headers.ToArray();
            var values1 = csvData.Rows[0];  // First tool
            var values2 = csvData.Rows[1];  // Second tool

            // Find column indices
            int diameterIndex = FindColumnIndex(headers, "(D) - Cutting Dia");
            int fluteLenIndex = FindColumnIndex(headers, "(WL)  - Working Length");
            int unitsIndex = FindColumnIndex(headers, "Input_units_m");

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column '(D) - Cutting Dia' not found in CSV");
            }

            if (unitsIndex == -1)
            {
                throw new Exception("Input_units_m column not found - required for mixed units mode");
            }

            // Parse Tool 1 (metric) values
            var diameter1 = double.Parse(values1[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var units1 = values1[unitsIndex].Trim().ToLower();

            // Verify no conversion occurred for metric tool
            if (Math.Abs(diameter1 - 25.4) > 0.01)
            {
                throw new Exception($"Tool 1 diameter should NOT be converted in mixed mode. Expected: 25.4, Got: {diameter1}");
            }

            if (fluteLenIndex != -1)
            {
                var fluteLen1 = double.Parse(values1[fluteLenIndex], System.Globalization.CultureInfo.InvariantCulture);
                if (Math.Abs(fluteLen1 - 50.8) > 0.01)
                {
                    throw new Exception($"Tool 1 flute length should NOT be converted in mixed mode. Expected: 50.8, Got: {fluteLen1}");
                }
            }

            // Verify Input_units_m flag
            if (units1 != "true" && units1 != "1")
            {
                throw new Exception($"Tool 1 Input_units_m should be true. Got: {units1}");
            }

            // Parse Tool 2 (inches) values
            var diameter2 = double.Parse(values2[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var units2 = values2[unitsIndex].Trim().ToLower();

            // Verify no conversion occurred for inch tool
            if (Math.Abs(diameter2 - 0.5) > 0.0001)
            {
                throw new Exception($"Tool 2 diameter should NOT be converted in mixed mode. Expected: 0.5, Got: {diameter2}");
            }

            if (fluteLenIndex != -1)
            {
                var fluteLen2 = double.Parse(values2[fluteLenIndex], System.Globalization.CultureInfo.InvariantCulture);
                if (Math.Abs(fluteLen2 - 2.0) > 0.0001)
                {
                    throw new Exception($"Tool 2 flute length should NOT be converted in mixed mode. Expected: 2.0, Got: {fluteLen2}");
                }
            }

            // Verify Input_units_m flag
            if (units2 != "false" && units2 != "0")
            {
                throw new Exception($"Tool 2 Input_units_m should be false. Got: {units2}");
            }

            Console.WriteLine("PASS");
        }

        /// <summary>
        /// Helper method to find column index by name (case-insensitive)
        /// </summary>
        private static int FindColumnIndex(string[] headers, string columnName)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (string.Equals(headers[i].Trim(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
