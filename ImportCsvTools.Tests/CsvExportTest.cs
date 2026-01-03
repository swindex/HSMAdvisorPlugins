using System;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase.ToolDataBase;
using ImportCsvTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImportCsvTools.Tests
{
    [TestClass]
    public class CsvExportTest
    {
        // Get path to the project root
        private const string TestDataDirectory = @"..\..\test-data";
        private const string OutputDirectory = @"..\..\test-output";

        // Shared test data - imported/exported once for all tests
        private static DataBase originalDatabase;
        private static DataBase reimportedDatabase;
        private static string exportedFilePath;

        [ClassInitialize]
        public static void SetupTestData(TestContext context)
        {
            SetupTestDataInternal();
        }

        // Internal setup method that can be called from both MSTest and manual test runner
        private static void SetupTestDataInternal()
        {
            // Only setup once
            if (originalDatabase != null)
                return;

            Console.WriteLine("=== Setting up CSV Export Tests ===");
            Console.WriteLine("Importing source CSV once...");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);

            // Ensure output directory exists
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            exportedFilePath = Path.Combine(outputDir, "test_export.csv");

            // Import original CSV once
            originalDatabase = CsvToolImporter.ImportFromFiles(csvPath, mappingPath, MessageFlags.None);
            if (originalDatabase == null || originalDatabase.Tools.Count == 0)
            {
                throw new Exception("Failed to import original tools");
            }
            Console.WriteLine($"  Imported {originalDatabase.Tools.Count} tools");

            // Delete old export file if it exists
            if (File.Exists(exportedFilePath))
            {
                File.Delete(exportedFilePath);
            }

            // Export once
            Console.WriteLine("Exporting to CSV once...");
            CsvToolImporter.ExportToFile(originalDatabase, exportedFilePath, mappingPath);
            Console.WriteLine($"  Exported to: {exportedFilePath}");

            // Re-import exported file once
            Console.WriteLine("Re-importing exported CSV once...");
            reimportedDatabase = CsvToolImporter.ImportFromFiles(exportedFilePath, mappingPath, MessageFlags.None);
            if (reimportedDatabase == null || reimportedDatabase.Tools.Count == 0)
            {
                throw new Exception("Failed to re-import exported tools");
            }
            Console.WriteLine($"  Re-imported {reimportedDatabase.Tools.Count} tools");
            Console.WriteLine("=== Setup Complete ===\n");
        }

        // Manual test runner method (for Program.cs)
        public void RunAllTests()
        {
            // Setup once
            SetupTestDataInternal();

            // Run all export tests
            TestExportCreatesFile();
            TestExportImportRoundTrip();
            TestExportToolCount();
            TestExportToolProperties();
            TestExportValueMaps();
            TestExportUnitConversion();
            TestExportEnumValues();
            TestExportWithMissingValues();

            // Priority 1: Critical functionality tests
            TestExportCsvSpecialCharacterEscaping();
            TestExportUnitConversionInchesToMm();
            TestExportUnitConversionMmToInches();
            TestExportMixedUnitsMode();
        }

        [TestMethod]
        public void TestExportCreatesFile()
        {
            Console.Write("Testing export creates CSV file... ");

            // Verify file was created
            if (!File.Exists(exportedFilePath))
            {
                throw new Exception("Export file was not created");
            }

            // Verify file has content
            var fileInfo = new FileInfo(exportedFilePath);
            if (fileInfo.Length == 0)
            {
                throw new Exception("Export file is empty");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportImportRoundTrip()
        {
            Console.Write("Testing export/import round trip... ");

            // Compare tool counts
            if (originalDatabase.Tools.Count != reimportedDatabase.Tools.Count)
            {
                throw new Exception(
                    $"Tool count mismatch: Original={originalDatabase.Tools.Count}, " +
                    $"Reimported={reimportedDatabase.Tools.Count}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportToolCount()
        {
            Console.Write("Testing exported tool count... ");

            // Count lines in exported file (excluding header)
            var lineCount = File.ReadAllLines(exportedFilePath).Length - 1; // Subtract header row

            if (lineCount != originalDatabase.Tools.Count)
            {
                throw new Exception(
                    $"Exported line count ({lineCount}) does not match tool count ({originalDatabase.Tools.Count})");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportToolProperties()
        {
            Console.Write("Testing exported tool properties... ");

            // Find same tool in both databases (Tool Number 10)
            var originalTool = originalDatabase.Tools.FirstOrDefault(t => t.Number == 10);
            var reimportedTool = reimportedDatabase.Tools.FirstOrDefault(t => t.Number == 10);

            if (originalTool == null || reimportedTool == null)
            {
                throw new Exception("Could not find tool number 10 in both databases");
            }

            // Compare properties
            if (originalTool.Number != reimportedTool.Number)
            {
                throw new Exception(
                    $"Tool number mismatch: {originalTool.Number} != {reimportedTool.Number}");
            }

            if (originalTool.Comment != reimportedTool.Comment)
            {
                throw new Exception(
                    $"Tool comment mismatch: '{originalTool.Comment}' != '{reimportedTool.Comment}'");
            }

            // Compare diameter with tolerance
            if (Math.Abs(originalTool.Diameter - reimportedTool.Diameter) > 0.0001)
            {
                throw new Exception(
                    $"Tool diameter mismatch: {originalTool.Diameter} != {reimportedTool.Diameter}");
            }

            // Compare tool material
            if (originalTool.Tool_material_id != reimportedTool.Tool_material_id)
            {
                throw new Exception(
                    $"Tool material mismatch: {originalTool.Tool_material_id} != {reimportedTool.Tool_material_id}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportValueMaps()
        {
            Console.Write("Testing export value map reversal... ");

            // Read the exported CSV and check if value maps were reversed correctly
            var lines = File.ReadAllLines(exportedFilePath);

            if (lines.Length < 2)
            {
                throw new Exception("Exported file has insufficient data");
            }

            // Compare tool materials for all tools
            for (int i = 0; i < originalDatabase.Tools.Count; i++)
            {
                var original = originalDatabase.Tools[i];
                var reimported = reimportedDatabase.Tools.FirstOrDefault(t => t.Number == original.Number);

                if (reimported == null)
                {
                    throw new Exception($"Tool {original.Number} not found in reimported database");
                }

                if (original.Tool_material_id != reimported.Tool_material_id)
                {
                    throw new Exception(
                        $"Tool {original.Number} material mismatch: " +
                        $"{original.Tool_material_id} != {reimported.Tool_material_id}");
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportUnitConversion()
        {
            Console.Write("Testing export unit conversion... ");

            // Get a tool before and after round trip
            var originalTool = originalDatabase.Tools.FirstOrDefault(t => t.Number == 10);
            if (originalTool == null)
            {
                throw new Exception("Could not find tool number 10");
            }

            var reimportedTool = reimportedDatabase.Tools.FirstOrDefault(t => t.Number == 10);
            if (reimportedTool == null)
            {
                throw new Exception("Could not find tool number 10 in reimported database");
            }

            // Verify diameter is preserved (within tolerance)
            var diameterDiff = Math.Abs(originalTool.Diameter - reimportedTool.Diameter);
            if (diameterDiff > 0.0001)
            {
                throw new Exception(
                    $"Diameter not preserved through export/import: " +
                    $"Original={originalTool.Diameter}, Reimported={reimportedTool.Diameter}, " +
                    $"Difference={diameterDiff}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportEnumValues()
        {
            Console.Write("Testing export enum value handling... ");

            // Compare enum values for all tools
            foreach (var originalTool in originalDatabase.Tools)
            {
                var reimportedTool = reimportedDatabase.Tools.FirstOrDefault(t => t.Number == originalTool.Number);

                if (reimportedTool == null)
                {
                    throw new Exception($"Tool {originalTool.Number} not found in reimported database");
                }

                // Check tool type
                if (originalTool.Tool_type_id != reimportedTool.Tool_type_id)
                {
                    throw new Exception(
                        $"Tool {originalTool.Number} type mismatch: " +
                        $"{originalTool.Tool_type_id} != {reimportedTool.Tool_type_id}");
                }

                // Check tool material
                if (originalTool.Tool_material_id != reimportedTool.Tool_material_id)
                {
                    throw new Exception(
                        $"Tool {originalTool.Number} material mismatch: " +
                        $"{originalTool.Tool_material_id} != {reimportedTool.Tool_material_id}");
                }

                // Check coating (if mapped)
                if (originalTool.Coating_id != reimportedTool.Coating_id)
                {
                    throw new Exception(
                        $"Tool {originalTool.Number} coating mismatch: " +
                        $"{originalTool.Coating_id} != {reimportedTool.Coating_id}");
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportWithMissingValues()
        {
            Console.Write("Testing export with missing/default values... ");

            // Verify the file was created and has content
            if (!File.Exists(exportedFilePath))
            {
                throw new Exception("Export file was not created");
            }

            var lines = File.ReadAllLines(exportedFilePath);
            if (lines.Length < 2)
            {
                throw new Exception("Export file has insufficient data");
            }

            // Re-import should work even with missing values (using defaults)
            if (reimportedDatabase.Tools.Count == 0)
            {
                throw new Exception("Failed to re-import tools with missing values");
            }

            Console.WriteLine("PASS");
        }

        // ==================== PRIORITY 1: CRITICAL FUNCTIONALITY TESTS ====================

        [TestMethod]
        public void TestExportCsvSpecialCharacterEscaping()
        {
            Console.Write("Testing CSV special character escaping... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var testOutputPath = Path.Combine(outputDir, "test_special_chars.csv");

            // Create a simple mapping with just the fields we need to test
            var mapping = new CsvMappingConfig();
            mapping.LibraryName = "Test Library";
            mapping.CsvInputUnits = "in";
            mapping.AllowInvalidToolImport = true; // Allow import of tools without all required fields
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Number", ToolField = "Number" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Comment", ToolField = "Comment" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Diameter", ToolField = "Diameter" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Tool Type", ToolField = "Tool_type_id", EnumType = "ToolTypes" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Tool Material", ToolField = "Tool_material_id", EnumType = "ToolMaterials" });

            var simpleMappingPath = Path.Combine(outputDir, "test_simple_mapping.json");
            using (var stream = new FileStream(simpleMappingPath, FileMode.Create))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CsvMappingConfig));
                serializer.WriteObject(stream, mapping);
            }

            // Create database with tools containing special characters
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool1 = new Tool(true);
            tool1.Number = 1;
            tool1.Library = "Test Library";
            tool1.Comment = "End Mill, 4 Flute"; // Contains comma
            tool1.Diameter = 0.25;
            tool1.Tool_type_id = (Enums.ToolTypes)1; // End mill
            tool1.Tool_material_id = (Enums.ToolMaterials)1; // Carbide
            testDb.Tools.Add(tool1);

            var tool2 = new Tool(true);
            tool2.Number = 2;
            tool2.Library = "Test Library";
            tool2.Comment = "End Mill 1/4\" Diameter"; // Contains quote
            tool2.Diameter = 0.125;
            tool2.Tool_type_id = (Enums.ToolTypes)1; // End mill
            tool2.Tool_material_id = (Enums.ToolMaterials)1; // Carbide
            testDb.Tools.Add(tool2);

            var tool3 = new Tool(true);
            tool3.Number = 3;
            tool3.Library = "Test Library";
            tool3.Comment = "End Mill\nMulti-line"; // Contains actual newline
            tool3.Diameter = 0.5;
            tool3.Tool_type_id = (Enums.ToolTypes)1; // End mill
            tool3.Tool_material_id = (Enums.ToolMaterials)1; // Carbide
            testDb.Tools.Add(tool3);

            // Export to CSV
            CsvToolImporter.ExportToFile(testDb, testOutputPath, simpleMappingPath);

            // Read raw CSV and verify escaping
            var csvLines = File.ReadAllLines(testOutputPath);
            if (csvLines.Length < 4)
            {
                throw new Exception("Expected at least 4 lines (header + 3 tools)");
            }

            // Check that values with commas are quoted
            bool foundCommaEscaped = false;
            bool foundQuoteEscaped = false;
            bool foundNewlineEscaped = false;

            foreach (var line in csvLines)
            {
                if (line.Contains("\"End Mill, 4 Flute\""))
                {
                    foundCommaEscaped = true;
                }
                if (line.Contains("\"End Mill 1/4\"\" Diameter\"")) // Quotes should be doubled
                {
                    foundQuoteEscaped = true;
                }
                // Newline should be converted to \\n escape sequence
                if (line.Contains("End Mill\\nMulti-line"))
                {
                    foundNewlineEscaped = true;
                }
            }

            if (!foundCommaEscaped)
            {
                throw new Exception("Comma in comment was not properly escaped with quotes");
            }
            if (!foundQuoteEscaped)
            {
                throw new Exception("Quote in comment was not properly escaped (doubled)");
            }
            if (!foundNewlineEscaped)
            {
                throw new Exception("Newline in comment was not properly escaped with quotes");
            }

            // Re-import and verify strings are identical
            var reimported = CsvToolImporter.ImportFromFiles(testOutputPath, simpleMappingPath, MessageFlags.None);

            var reimportedTool1 = reimported.Tools.FirstOrDefault(t => t.Number == 1);
            var reimportedTool2 = reimported.Tools.FirstOrDefault(t => t.Number == 2);
            var reimportedTool3 = reimported.Tools.FirstOrDefault(t => t.Number == 3);

            if (reimportedTool1 == null || reimportedTool1.Comment != tool1.Comment)
            {
                throw new Exception($"Comment with comma not preserved. Expected: '{tool1.Comment}', Got: '{reimportedTool1?.Comment}'");
            }
            if (reimportedTool2 == null || reimportedTool2.Comment != tool2.Comment)
            {
                throw new Exception($"Comment with quote not preserved. Expected: '{tool2.Comment}', Got: '{reimportedTool2?.Comment}'");
            }
            if (reimportedTool3 == null || reimportedTool3.Comment != tool3.Comment)
            {
                throw new Exception($"Comment with newline not preserved. Expected: '{tool3.Comment}', Got: '{reimportedTool3?.Comment}'");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportUnitConversionInchesToMm()
        {
            Console.Write("Testing unit conversion inches to mm... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var testOutputPath = Path.Combine(outputDir, "test_inches_to_mm.csv");

            // Create a mapping that expects mm output
            var mapping = CsvMappingConfig.Load(mappingPath);
            var mmMappingPath = Path.Combine(outputDir, "test_mm_mapping.json");
            mapping.CsvInputUnits = "mm";
            mapping.AllowInvalidToolImport = true; // Allow import of test tools without all required fields

            // Save modified mapping
            using (var stream = new FileStream(mmMappingPath, FileMode.Create))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CsvMappingConfig));
                serializer.WriteObject(stream, mapping);
            }

            // Create database with tool in inches
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool = new Tool(true);
            tool.Number = 1;
            tool.Library = "Test Library";
            tool.Comment = "Test Tool";
            tool.Diameter = 0.5; // 0.5 inches
            tool.Diameter_m = false; // Inches
            tool.Tool_type_id = (Enums.ToolTypes)1;
            tool.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool);

            // Export to CSV with mm mapping
            CsvToolImporter.ExportToFile(testDb, testOutputPath, mmMappingPath);

            // Read exported CSV and check diameter value
            var csvLines = File.ReadAllLines(testOutputPath);
            var headerLine = csvLines[0];
            var dataLine = csvLines[1];

            var headers = headerLine.Split(',');
            var values = dataLine.Split(',');

            // Find diameter column (may be named "Diameter" or "(D) - Cutting Dia")
            int diameterIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i].Trim();
                if (header.Equals("Diameter", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("(D) - Cutting Dia", StringComparison.OrdinalIgnoreCase))
                {
                    diameterIndex = i;
                    break;
                }
            }

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column not found in CSV");
            }

            var exportedDiameter = double.Parse(values[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var expectedDiameter = 0.5 * 25.4; // Convert to mm

            if (Math.Abs(exportedDiameter - expectedDiameter) > 0.01)
            {
                throw new Exception($"Diameter not converted correctly. Expected: {expectedDiameter:F4} mm, Got: {exportedDiameter:F4} mm");
            }

            // Re-import and verify it converts back
            var reimported = CsvToolImporter.ImportFromFiles(testOutputPath, mmMappingPath, MessageFlags.None);
            var reimportedTool = reimported.Tools.FirstOrDefault();

            if (reimportedTool == null)
            {
                throw new Exception("Tool not reimported");
            }

            // After reimport with mm mapping, tool should be in mm
            if (!reimportedTool.Diameter_m)
            {
                throw new Exception("Reimported tool should have metric flag set");
            }

            if (Math.Abs(reimportedTool.Diameter - expectedDiameter) > 0.01)
            {
                throw new Exception($"Reimported diameter incorrect. Expected: {expectedDiameter:F4} mm, Got: {reimportedTool.Diameter:F4} mm");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportUnitConversionMmToInches()
        {
            Console.Write("Testing unit conversion mm to inches... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var testOutputPath = Path.Combine(outputDir, "test_mm_to_inches.csv");

            // Create a mapping that expects inches output
            var mapping = CsvMappingConfig.Load(mappingPath);
            var inchesMappingPath = Path.Combine(outputDir, "test_inches_mapping.json");
            mapping.CsvInputUnits = "in";
            mapping.AllowInvalidToolImport = true; // Allow import of test tools without all required fields

            // Save modified mapping
            using (var stream = new FileStream(inchesMappingPath, FileMode.Create))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CsvMappingConfig));
                serializer.WriteObject(stream, mapping);
            }

            // Create database with tool in mm
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool = new Tool(true);
            tool.Number = 1;
            tool.Library = "Test Library";
            tool.Comment = "Test Tool";
            tool.Diameter = 25.4; // 25.4 mm (= 1 inch)
            tool.Diameter_m = true; // Metric
            tool.Tool_type_id = (Enums.ToolTypes)1;
            tool.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool);

            // Export to CSV with inches mapping
            CsvToolImporter.ExportToFile(testDb, testOutputPath, inchesMappingPath);

            // Read exported CSV and check diameter value
            var csvLines = File.ReadAllLines(testOutputPath);
            var headerLine = csvLines[0];
            var dataLine = csvLines[1];

            var headers = headerLine.Split(',');
            var values = dataLine.Split(',');

            // Find diameter column (may be named "Diameter" or "(D) - Cutting Dia")
            int diameterIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i].Trim();
                if (header.Equals("Diameter", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("(D) - Cutting Dia", StringComparison.OrdinalIgnoreCase))
                {
                    diameterIndex = i;
                    break;
                }
            }

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column not found in CSV");
            }

            var exportedDiameter = double.Parse(values[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var expectedDiameter = 25.4 * 0.03937007874; // Convert to inches

            if (Math.Abs(exportedDiameter - expectedDiameter) > 0.0001)
            {
                throw new Exception($"Diameter not converted correctly. Expected: {expectedDiameter:F6} inches, Got: {exportedDiameter:F6} inches");
            }

            // Re-import and verify it converts back
            var reimported = CsvToolImporter.ImportFromFiles(testOutputPath, inchesMappingPath, MessageFlags.None);
            var reimportedTool = reimported.Tools.FirstOrDefault();

            if (reimportedTool == null)
            {
                throw new Exception("Tool not reimported");
            }

            // After reimport with inches mapping, tool should be in inches
            if (reimportedTool.Diameter_m)
            {
                throw new Exception("Reimported tool should not have metric flag set");
            }

            if (Math.Abs(reimportedTool.Diameter - expectedDiameter) > 0.0001)
            {
                throw new Exception($"Reimported diameter incorrect. Expected: {expectedDiameter:F6} inches, Got: {reimportedTool.Diameter:F6} inches");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestExportMixedUnitsMode()
        {
            Console.Write("Testing mixed units mode... ");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);
            var testOutputPath = Path.Combine(outputDir, "test_mixed_units.csv");

            // Create a mapping for mixed units
            var mapping = new CsvMappingConfig();
            mapping.LibraryName = "Test Library";
            mapping.CsvInputUnits = "mixed";
            mapping.AllowInvalidToolImport = true; // Allow import of test tools without all required fields
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Number", ToolField = "Number" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Comment", ToolField = "Comment" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Diameter", ToolField = "Diameter" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Input_units_m", ToolField = "_Input_units_m" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Tool Type", ToolField = "Tool_type_id", EnumType = "ToolTypes" });
            mapping.Mappings.Add(new CsvMapping { CsvColumn = "Tool Material", ToolField = "Tool_material_id", EnumType = "ToolMaterials" });

            var mixedMappingPath = Path.Combine(outputDir, "test_mixed_mapping.json");
            using (var stream = new FileStream(mixedMappingPath, FileMode.Create))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(CsvMappingConfig));
                serializer.WriteObject(stream, mapping);
            }

            // Create database with tools in different units
            var testDb = new DataBase();
            testDb.AddLibrary("Test Library");

            var tool1 = new Tool(true);
            tool1.Number = 1;
            tool1.Library = "Test Library";
            tool1.Comment = "Inch Tool";
            tool1.Diameter = 0.5; // Inches
            tool1.Diameter_m = false;
            tool1._Input_units_m = "false";
            tool1.Tool_type_id = (Enums.ToolTypes)1;
            tool1.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool1);

            var tool2 = new Tool(true);
            tool2.Number = 2;
            tool2.Library = "Test Library";
            tool2.Comment = "Metric Tool";
            tool2.Diameter = 25.4; // mm
            tool2.Diameter_m = true;
            tool2._Input_units_m = "true";
            tool2.Tool_type_id = (Enums.ToolTypes)1;
            tool2.Tool_material_id = (Enums.ToolMaterials)1;
            testDb.Tools.Add(tool2);

            // Export to CSV
            CsvToolImporter.ExportToFile(testDb, testOutputPath, mixedMappingPath);

            // Read exported CSV and verify no conversion occurred
            var csvLines = File.ReadAllLines(testOutputPath);
            var headerLine = csvLines[0];
            var dataLine1 = csvLines[1];
            var dataLine2 = csvLines[2];

            var headers = headerLine.Split(',');

            // Find diameter and Input_units_m columns
            int diameterIndex = -1;
            int unitsIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i].Trim();
                if (header.Equals("Diameter", StringComparison.OrdinalIgnoreCase))
                {
                    diameterIndex = i;
                }
                if (header.Equals("Input_units_m", StringComparison.OrdinalIgnoreCase))
                {
                    unitsIndex = i;
                }
            }

            if (diameterIndex == -1)
            {
                throw new Exception("Diameter column not found");
            }
            if (unitsIndex == -1)
            {
                throw new Exception("Input_units_m column not found - required for mixed units mode");
            }

            // Parse values
            var values1 = dataLine1.Split(',');
            var values2 = dataLine2.Split(',');

            var diameter1 = double.Parse(values1[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);
            var diameter2 = double.Parse(values2[diameterIndex], System.Globalization.CultureInfo.InvariantCulture);

            // Verify no conversion (should match original values)
            if (Math.Abs(diameter1 - 0.5) > 0.0001)
            {
                throw new Exception($"Tool 1 diameter should not be converted in mixed mode. Expected: 0.5, Got: {diameter1}");
            }
            if (Math.Abs(diameter2 - 25.4) > 0.01)
            {
                throw new Exception($"Tool 2 diameter should not be converted in mixed mode. Expected: 25.4, Got: {diameter2}");
            }

            // Verify unit flags are present
            var units1 = values1[unitsIndex].Trim().ToLower();
            var units2 = values2[unitsIndex].Trim().ToLower();

            if (units1 != "false" && units1 != "0")
            {
                throw new Exception($"Tool 1 Input_units_m should be false. Got: {units1}");
            }
            if (units2 != "true" && units2 != "1")
            {
                throw new Exception($"Tool 2 Input_units_m should be true. Got: {units2}");
            }

            // Re-import and verify units preserved
            var reimported = CsvToolImporter.ImportFromFiles(testOutputPath, mixedMappingPath, MessageFlags.None);
            var reimportedTool1 = reimported.Tools.FirstOrDefault(t => t.Number == 1);
            var reimportedTool2 = reimported.Tools.FirstOrDefault(t => t.Number == 2);

            if (reimportedTool1 == null || reimportedTool2 == null)
            {
                throw new Exception("Tools not reimported");
            }

            if (Math.Abs(reimportedTool1.Diameter - 0.5) > 0.0001 || reimportedTool1.Diameter_m)
            {
                throw new Exception($"Tool 1 not preserved correctly. Diameter: {reimportedTool1.Diameter}, Metric: {reimportedTool1.Diameter_m}");
            }

            if (Math.Abs(reimportedTool2.Diameter - 25.4) > 0.01 || !reimportedTool2.Diameter_m)
            {
                throw new Exception($"Tool 2 not preserved correctly. Diameter: {reimportedTool2.Diameter}, Metric: {reimportedTool2.Diameter_m}");
            }

            Console.WriteLine("PASS");
        }
    }
}
