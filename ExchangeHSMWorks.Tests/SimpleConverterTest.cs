using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HSMAdvisorDatabase.ToolDataBase;
using HSMAdvisorDatabase;
using ExchangeHSMWorks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExchangeHSMWorks.Tests
{
    /// <summary>
    /// Simple test class that can be run without MSTest framework
    /// Demonstrates the plugin import functionality with all test data files
    /// </summary>
    [TestClass]
    public class SimpleConverterTest
    {
        private const string TestDataDirectory = @"ExchangeHSMWorks.Tests\test-data";

        // Cache for loaded test data to avoid reloading files multiple times
        private static readonly Dictionary<string, TestDataInfo> _testDataCache = new Dictionary<string, TestDataInfo>();

        /// <summary>
        /// Information about a loaded test data file
        /// </summary>
        private class TestDataInfo
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public DataBase Database { get; set; }
            public toollibrary OriginalData { get; set; }
        }

        /// <summary>
        /// Database cache information for tool count validation
        /// </summary>
        private class DatabaseCacheInfo
        {
            public string FileName { get; set; }
            public int OriginalToolCount { get; set; }
            public int ImportedToolCount { get; set; }
            public DataBase Database { get; set; }

            public toollibrary OriginalLibrary { get; set; }
            public bool ToolCountMatches => OriginalToolCount == ImportedToolCount;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("HSMAdvisor Plugin Import Test");
            Console.WriteLine("=============================");
            Console.WriteLine();

            try
            {
                var test = new SimpleConverterTest();
                test.RunAllTests();
                Console.WriteLine();
                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        [TestMethod]
        public void RunAllTests()
        {
            // Load all test data files first
            LoadAllTestData();

            TestFilesExist();
            TestImportToolCount();
            TestToolCountConsistency();
            TestLibraryCreation();
            TestToolDataPreservation();
            TestToolTypeMapping();
            TestMaterialMapping();
            TestUnitHandling();
            TestToolGeometry();
            TestManufacturerData();
            TestRoundTripData();
            TestMaterialConversion();
            TestCapabilities();

            // Demonstrate the side-by-side comparison functionality
            //DemonstrateComparisonFeature();
        }

        /// <summary>
        /// Load all .hsmlib files from the test-data directory into cache
        /// </summary>
        private void LoadAllTestData()
        {
            Console.WriteLine("Loading test data files...");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            if (!Directory.Exists(testDataDir))
            {
                throw new DirectoryNotFoundException($"Test data directory not found: {testDataDir}");
            }

            var hsmLibFiles = Directory.GetFiles(testDataDir, "*.hsmlib");
            if (hsmLibFiles.Length == 0)
            {
                throw new FileNotFoundException("No .hsmlib files found in test-data directory");
            }

            foreach (var filePath in hsmLibFiles)
            {
                var fileName = Path.GetFileName(filePath);
                Console.Write($"  Loading {fileName}... ");

                try
                {
                    var testData = LoadTestDataFile(filePath);
                    _testDataCache[fileName] = testData;
                    Console.WriteLine($"OK ({testData.OriginalData.tool} tools)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FAILED: {ex.Message}");
                    throw;
                }
            }

            Console.WriteLine($"Loaded {_testDataCache.Count} test data files");
            Console.WriteLine();
        }

        /// <summary>
        /// Load a single test data file and convert it to HSMAdvisor format
        /// </summary>
        private TestDataInfo LoadTestDataFile(string filePath)
        {
            // Read and parse the XML
            var xml = File.ReadAllText(filePath);
            var originalData = Serializer.FromXML<toollibrary>(xml, false);

            // Create database and convert tools
            var database = new DataBase();
            var libraryName = Path.GetFileNameWithoutExtension(filePath);
            var fileName = Path.GetFileName(filePath);

            // Add library
            database.AddLibrary(libraryName);

            // Convert all tools
            originalData.tool.ForEach(srcTool =>
            {
                var tool = Converter.ToTool(srcTool);
                tool.Library = libraryName;
                database.Tools.Add(tool);

                // Add holder if it has one
                if (srcTool.holder != null)
                {
                    var holder = database.Holders.FirstOrDefault(e =>
                        e.Comment == srcTool.holder.description && e.Library == tool.Library);
                    if (holder != null)
                        database.Holders.Remove(holder);

                    database.Holders.Add(new Holder()
                    {
                        Library = tool.Library,
                        Units_m = srcTool.unit == "millimeters",
                        Comment = srcTool.holder.description,
                        Brand_name = srcTool.holder.vendor,
                        Series_name = srcTool.holder.productid,
                        Shank_Dia = Parse.ToDouble(srcTool.body.shaftdiameter)
                    });
                }
            });

            // Populate database cache for tool count validation
            var originalToolCount = originalData.tool?.Count ?? 0;
            var importedToolCount = database.Tools.Count;

            return new TestDataInfo
            {
                FilePath = filePath,
                FileName = fileName,
                Database = database,
                OriginalData = originalData,
            };
        }

        [TestMethod]
        public void TestFilesExist()
        {
            Console.Write("Testing files exist... ");

            if (_testDataCache.Count == 0)
                throw new Exception("No test data files loaded");

            // Verify all expected files are present
            var expectedFiles = new[] { "Harvey Tool-End Mills.hsmlib", "Harvey Tool-Specialty Profiles.hsmlib" };
            foreach (var expectedFile in expectedFiles)
            {
                if (!_testDataCache.ContainsKey(expectedFile))
                    throw new FileNotFoundException($"Expected test file not found: {expectedFile}");
            }

            Console.WriteLine($"PASS ({_testDataCache.Count} files)");
        }

        [TestMethod]
        public void TestImportToolCount()
        {
            Console.Write("Testing import tool count... ");

            var totalTools = 0;
            foreach (var testData in _testDataCache.Values)
            {
                if (testData.Database == null)
                    throw new Exception($"Database is null for {testData.FileName}");

                if (testData.Database.Tools == null)
                    throw new Exception($"Tools collection is null for {testData.FileName}");

                if (testData.Database.Tools.Count == 0)
                    throw new Exception($"No tools found in {testData.FileName}");

                totalTools += testData.Database.Tools.Count;
            }

            Console.WriteLine($"PASS ({totalTools} total tools across {_testDataCache.Count} files)");
        }

        [TestMethod]
        public void TestToolCountConsistency()
        {
            Console.Write("Testing tool count consistency... ");

            var inconsistentFiles = new List<string>();
            var totalOriginal = 0;
            var totalImported = 0;

            foreach (var cacheInfo in _testDataCache.Values)
            {
                totalOriginal += cacheInfo.OriginalData.tool.Count;
                totalImported += cacheInfo.Database.Tools.Count;

                if (cacheInfo.OriginalData.tool.Count != cacheInfo.Database.Tools.Count)
                {
                    inconsistentFiles.Add($"{cacheInfo.FileName}: {cacheInfo.OriginalData.tool.Count} original → {cacheInfo.Database.Tools.Count} imported");
                }
            }

            if (inconsistentFiles.Any())
            {
                Console.WriteLine();
                Console.WriteLine("TOOL COUNT MISMATCH DETECTED:");
                foreach (var inconsistency in inconsistentFiles)
                {
                    Console.WriteLine($"  {inconsistency}");
                }
                throw new Exception($"Tool count mismatch in {inconsistentFiles.Count} file(s). Each original tool library should result in the same number of imported tools in the database.");
            }

            Console.WriteLine($"PASS (Original: {totalOriginal}, Imported: {totalImported} across {_testDataCache.Count} files)");
        }

        [TestMethod]
        public void TestLibraryCreation()
        {
            Console.Write("Testing library creation... ");

            foreach (var testData in _testDataCache.Values)
            {
                var expectedLibraryName = Path.GetFileNameWithoutExtension(testData.FileName);

                if (testData.Database.Libraries == null)
                    throw new Exception($"Libraries collection is null for {testData.FileName}");

                if (!testData.Database.Libraries.Any(l => l.Name == expectedLibraryName))
                    throw new Exception($"Library '{expectedLibraryName}' was not created for {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Library == expectedLibraryName))
                    throw new Exception($"Not all tools are assigned to correct library in {testData.FileName}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestToolDataPreservation()
        {
            Console.Write("Testing tool data preservation... ");

            foreach (var testData in _testDataCache.Values)
            {
                // Check for missing GUIDs
                var toolsWithoutGuids = testData.Database.Tools.Where(t => string.IsNullOrEmpty(t.Guid)).ToList();
                if (toolsWithoutGuids.Any())
                {
                    var firstTool = toolsWithoutGuids.First();
                    var originalTool = testData.OriginalData.tool.FirstOrDefault(t => t.productid == firstTool.Series_name);
                    ShowToolComparison("GUID Missing", originalTool, firstTool, testData.FileName);
                    throw new Exception($"Some tools are missing GUIDs in {testData.FileName}");
                }

                var toolsWithEmptyNameID = testData.Database.Tools.Where(t => t.Tool_type_id == 0).ToList();
                if (toolsWithEmptyNameID.Any())
                {
                    var firstTool = toolsWithEmptyNameID.First();
                    var originalTool = testData.OriginalData.tool.FirstOrDefault(t => t.productid == firstTool.Series_name);
                    ShowToolComparison("Tool_type_id Missing", originalTool, firstTool, testData.FileName);
                    throw new Exception($"Some tools have invalid Tool_type_id in {testData.FileName}");
                }

                // Check for invalid diameters
                var toolsWithInvalidDiameters = testData.Database.Tools.Where(t =>
                {
                    return t.Diameter <= 0;
                }).ToList();
                if (toolsWithInvalidDiameters.Any())
                {
                    var firstTool = toolsWithInvalidDiameters.First();
                    var originalTool = testData.OriginalData.tool.FirstOrDefault(t => t.productid == firstTool.Series_name);
                    ShowToolComparison("Invalid Diameter", originalTool, firstTool, testData.FileName);
                    throw new Exception($"Some tools have invalid diameters in {testData.FileName}");
                }

                // Check for missing Aux_data
                var toolsWithoutAuxData = testData.Database.Tools.Where(t => string.IsNullOrEmpty(t.Aux_data)).ToList();
                if (toolsWithoutAuxData.Any())
                {
                    var firstTool = toolsWithoutAuxData.First();
                    var originalTool = testData.OriginalData.tool.FirstOrDefault(t => t.productid == firstTool.Series_name);
                    ShowToolComparison("Missing Aux_data", originalTool, firstTool, testData.FileName);
                    throw new Exception($"Some tools are missing Aux_data in {testData.FileName}");
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestToolTypeMapping()
        {
            Console.Write("Testing tool type mapping... ");

            var allToolTypes = new HashSet<Enums.ToolTypes>();
            var hasEndMills = false;
            var hasBallMills = false;

            foreach (var testData in _testDataCache.Values)
            {
                var toolTypes = testData.Database.Tools.Select(t => (Enums.ToolTypes)t.Tool_type_id).Distinct().ToList();

                if (!toolTypes.Any())
                    throw new Exception($"No tool types found in {testData.FileName}");

                foreach (var toolType in toolTypes)
                {
                    allToolTypes.Add(toolType);
                }

                if (testData.Database.Tools.Any(t => (Enums.ToolTypes)t.Tool_type_id == Enums.ToolTypes.SolidEndMill))
                    hasEndMills = true;

                if (testData.Database.Tools.Any(t => (Enums.ToolTypes)t.Tool_type_id == Enums.ToolTypes.SolidBallMill))
                    hasBallMills = true;
            }

            if (!hasEndMills)
                throw new Exception("No flat end mills found across all test files");

            if (!hasBallMills)
                throw new Exception("No ball end mills found across all test files");

            Console.WriteLine($"PASS ({allToolTypes.Count} different tool types across all files)");
        }

        [TestMethod]
        public void TestMaterialMapping()
        {
            Console.Write("Testing material mapping... ");

            var allMaterials = new HashSet<Enums.ToolMaterials>();
            var hasCarbide = false;

            foreach (var testData in _testDataCache.Values)
            {
                var materials = testData.Database.Tools.Select(t => (Enums.ToolMaterials)t.Tool_material_id).Distinct().ToList();

                if (!materials.Any())
                    throw new Exception($"No materials found in {testData.FileName}");

                foreach (var material in materials)
                {
                    allMaterials.Add(material);
                }

                if (testData.Database.Tools.Any(t => (Enums.ToolMaterials)t.Tool_material_id == Enums.ToolMaterials.Carbide))
                    hasCarbide = true;
            }

            if (!hasCarbide)
                throw new Exception("No carbide tools found across all test files");

            Console.WriteLine($"PASS ({allMaterials.Count} different materials across all files)");
        }

        [TestMethod]
        public void TestUnitHandling()
        {
            Console.Write("Testing unit handling... ");

            var totalMetricTools = 0;
            var totalImperialTools = 0;

            foreach (var testData in _testDataCache.Values)
            {
                // Check that unit flags are set based on source data
                // The converter sets all unit flags based on the source unit
                var toolsWithInconsistentUnits = testData.Database.Tools.Where(t =>
                    t.Input_units_m != t.Diameter_m ||
                    t.Input_units_m != t.Circle_dia_m ||
                    t.Input_units_m != t.Depth_m).ToList();

                if (toolsWithInconsistentUnits.Any())
                {
                    var firstInconsistent = toolsWithInconsistentUnits.First();
                    throw new Exception($"Unit flags inconsistent for tool {firstInconsistent.Series_name} in {testData.FileName}: " +
                        $"Input_units_m={firstInconsistent.Input_units_m}, " +
                        $"Diameter_m={firstInconsistent.Diameter_m}, " +
                        $"Circle_dia_m={firstInconsistent.Circle_dia_m}");
                }

                // Count tools by unit type
                totalMetricTools += testData.Database.Tools.Count(t => t.Input_units_m);
                totalImperialTools += testData.Database.Tools.Count(t => !t.Input_units_m);
            }

            Console.WriteLine($"PASS ({totalMetricTools} metric, {totalImperialTools} imperial tools across all files)");
        }

        [TestMethod]
        public void TestToolGeometry()
        {
            Console.Write("Testing tool geometry... ");

            foreach (var testData in _testDataCache.Values)
            {
                if (!testData.Database.Tools.All(t => t.Diameter > 0))
                    throw new Exception($"Some tools have non-positive diameter in {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Flute_Len >= 0))
                    throw new Exception($"Some tools have negative flute length in {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Stickout >= 0))
                    throw new Exception($"Some tools have negative stickout in {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Flute_N > 0))
                    throw new Exception($"Some tools have no flutes in {testData.FileName}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestManufacturerData()
        {
            Console.Write("Testing manufacturer data... ");

            foreach (var testData in _testDataCache.Values)
            {
                if (!testData.Database.Tools.All(t => !string.IsNullOrEmpty(t.Brand_name)))
                    throw new Exception($"Some tools missing manufacturer name in {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Brand_name == "HARVEY TOOL"))
                    throw new Exception($"Not all tools are from Harvey Tool in {testData.FileName}");

                if (!testData.Database.Tools.All(t => !string.IsNullOrEmpty(t.Series_name)))
                    throw new Exception($"Some tools missing product ID in {testData.FileName}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestRoundTripData()
        {
            Console.Write("Testing round-trip data... ");

            foreach (var testData in _testDataCache.Values)
            {
                // Test first 10 tools per file for performance
                foreach (var tool in testData.Database.Tools.Take(10))
                {
                    if (string.IsNullOrEmpty(tool.Aux_data))
                        throw new Exception($"Tool {tool.Series_name} missing Aux_data in {testData.FileName}");

                    try
                    {
                        var originalTool = Serializer.FromXML<toollibraryTool>(tool.Aux_data, false);
                        if (originalTool == null)
                            throw new Exception($"Failed to deserialize Aux_data for tool {tool.Series_name} in {testData.FileName}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to deserialize Aux_data for tool {tool.Series_name} in {testData.FileName}: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestMaterialConversion()
        {
            Console.Write("Testing material conversion... ");

            // Test material mapping
            if (Converter.ToToolMaterial("carbide") != Enums.ToolMaterials.Carbide)
                throw new Exception("Carbide material mapping failed");

            if (Converter.ToToolMaterial("ceramics") != Enums.ToolMaterials.Ceramic)
                throw new Exception("Ceramics material mapping failed");

            if (Converter.ToToolMaterial("cobalt") != Enums.ToolMaterials.HSCobalt)
                throw new Exception("Cobalt material mapping failed");

            if (Converter.ToToolMaterial("hss") != Enums.ToolMaterials.HSS)
                throw new Exception("HSS material mapping failed");

            if (Converter.ToToolMaterial("unknown") != Enums.ToolMaterials.HSS)
                throw new Exception("Unknown material should default to HSS");

            // Test reverse material mapping
            if (Converter.FromToolMaterial(Enums.ToolMaterials.Carbide) != "carbide")
                throw new Exception("Reverse carbide material mapping failed");

            if (Converter.FromToolMaterial(Enums.ToolMaterials.Ceramic) != "ceramics")
                throw new Exception("Reverse ceramics material mapping failed");

            if (Converter.FromToolMaterial(Enums.ToolMaterials.HSCobalt) != "cobalt")
                throw new Exception("Reverse cobalt material mapping failed");

            if (Converter.FromToolMaterial(Enums.ToolMaterials.HSS) != "hss")
                throw new Exception("Reverse HSS material mapping failed");

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestCapabilities()
        {
            Console.Write("Testing capabilities... ");
            var converter = new Converter();
            var capabilities = converter.GetCapabilities();

            if (capabilities == null)
                throw new Exception("Capabilities is null");

            if (capabilities.Count != 2)
                throw new Exception("Should have exactly 2 capabilities");

            if (!capabilities.Any(c => c.Name.Contains("Import")))
                throw new Exception("Missing import capability");

            if (!capabilities.Any(c => c.Name.Contains("Export")))
                throw new Exception("Missing export capability");

            var filter = converter.GetReadFileFilter();
            if (string.IsNullOrEmpty(filter))
                throw new Exception("File filter is empty");

            if (!filter.Contains("*.hsmlib"))
                throw new Exception("Filter missing .hsmlib extension");

            if (!filter.Contains("*.xml"))
                throw new Exception("Filter missing .xml extension");

            Console.WriteLine("PASS");
        }

        /// <summary>
        /// Demonstrate the side-by-side comparison functionality with a sample tool
        /// </summary>
        private void DemonstrateComparisonFeature()
        {
            Console.WriteLine();
            Console.WriteLine("=== SIDE-BY-SIDE COMPARISON DEMONSTRATION ===");
            Console.WriteLine("This demonstrates how the comparison table appears when unit tests fail:");
            Console.WriteLine();

            // Get the first tool from the first test data file for demonstration
            var firstTestData = _testDataCache.Values.First();
            var firstConvertedTool = firstTestData.Database.Tools.First();
            var firstOriginalTool = firstTestData.OriginalData.tool.FirstOrDefault(t => t.productid == firstConvertedTool.Series_name);

            // Show the comparison table
            ShowToolComparison("DEMONSTRATION", firstOriginalTool, firstConvertedTool, firstTestData.FileName);

            Console.WriteLine("NOTE: In actual test failures, differences would be highlighted in yellow.");
            Console.WriteLine("This comparison helps identify exactly what data was lost or incorrectly converted.");
            Console.WriteLine();
        }

        /// <summary>
        /// Display a side-by-side comparison of source HSMWorks tool vs converted HSMAdvisor tool
        /// </summary>
        /// <param name="failureReason">The reason for the comparison (what failed)</param>
        /// <param name="originalTool">Original HSMWorks tool data</param>
        /// <param name="convertedTool">Converted HSMAdvisor tool data</param>
        /// <param name="fileName">Source file name</param>
        private void ShowToolComparison(string failureReason, toollibraryTool originalTool, Tool convertedTool, string fileName)
        {
            Console.WriteLine();
            Console.WriteLine("================================================================================");
            Console.WriteLine($"TOOL CONVERSION COMPARISON - {failureReason}");
            Console.WriteLine($"File: {fileName}");
            Console.WriteLine("================================================================================");
            Console.WriteLine();

            // Create table with fixed column widths
            const int propertyWidth = 25;
            const int sourceWidth = 30;
            const int convertedWidth = 30;

            var separator = new string('=', propertyWidth + sourceWidth + convertedWidth + 6);
            var headerFormat = $"{{0,-{propertyWidth}}} | {{1,-{sourceWidth}}} | {{2,-{convertedWidth}}}";
            var rowFormat = $"{{0,-{propertyWidth}}} | {{1,-{sourceWidth}}} | {{2,-{convertedWidth}}}";

            Console.WriteLine(headerFormat, "Property", "HSMWorks Source", "HSMAdvisor Converted");
            Console.WriteLine(separator);

            // Basic identification
            ShowComparisonRow("GUID", originalTool?.guid ?? "NULL", convertedTool?.Guid ?? "NULL", rowFormat);
            ShowComparisonRow("Product ID", originalTool?.productid ?? "NULL", convertedTool?.Series_name ?? "NULL", rowFormat);
            ShowComparisonRow("Description", originalTool?.description ?? "NULL", convertedTool?.Comment ?? "NULL", rowFormat);
            ShowComparisonRow("Manufacturer", originalTool?.manufacturer ?? "NULL", convertedTool?.Brand_name ?? "NULL", rowFormat);
            ShowComparisonRow("Type", originalTool?.type ?? "NULL", GetToolTypeName(convertedTool), rowFormat);

            Console.WriteLine(separator);

            // Material and units
            ShowComparisonRow("Material", originalTool?.material?.name ?? "NULL", GetMaterialName(convertedTool), rowFormat);
            ShowComparisonRow("Units", originalTool?.unit ?? "NULL", convertedTool?.Input_units_m == true ? "millimeters" : "inches", rowFormat);

            Console.WriteLine(separator);

            // Geometry data
            if (originalTool?.body != null)
            {
                ShowComparisonRow("Diameter", originalTool.body.diameter ?? "NULL", convertedTool?.Diameter.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Flute Length", originalTool.body.flutelength ?? "NULL", convertedTool?.Flute_Len.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Body Length", originalTool.body.bodylength ?? "NULL", convertedTool?.Stickout.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Shaft Diameter", originalTool.body.shaftdiameter ?? "NULL", convertedTool?.Shank_Dia.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Number of Flutes", originalTool.body.numberofflutes ?? "NULL", convertedTool?.Flute_N.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Corner Radius", originalTool.body.cornerradius ?? "NULL", convertedTool?.Corner_rad.ToString() ?? "NULL", rowFormat);
            }
            else
            {
                ShowComparisonRow("Body Data", "NULL", "Converted values present", rowFormat);
            }

            Console.WriteLine(separator);

            // NC data
            if (originalTool?.nc != null)
            {
                ShowComparisonRow("NC Number", originalTool.nc.number ?? "NULL", convertedTool?.Number.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Diameter Offset", originalTool.nc.diameteroffset ?? "NULL", convertedTool?.Offset_Diameter.ToString() ?? "NULL", rowFormat);
                ShowComparisonRow("Length Offset", originalTool.nc.lengthoffset ?? "NULL", convertedTool?.Offset_Length.ToString() ?? "NULL", rowFormat);
            }

            Console.WriteLine(separator);

            // Aux data preservation
            ShowComparisonRow("Aux Data Present", originalTool != null ? "YES" : "NO", !string.IsNullOrEmpty(convertedTool?.Aux_data) ? "YES" : "NO", rowFormat);

            Console.WriteLine("================================================================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Show a single comparison row with highlighting for differences
        /// </summary>
        private void ShowComparisonRow(string property, string sourceValue, string convertedValue, string format)
        {
            // Normalize values for comparison
            var normalizedSource = (sourceValue ?? "NULL").Trim();
            var normalizedConverted = (convertedValue ?? "NULL").Trim();

            // Truncate long values for display
            var displaySource = normalizedSource.Length > 28 ? normalizedSource.Substring(0, 25) + "..." : normalizedSource;
            var displayConverted = normalizedConverted.Length > 28 ? normalizedConverted.Substring(0, 25) + "..." : normalizedConverted;

            // Check if values are different (with some tolerance for numeric values)
            bool isDifferent = !AreValuesEquivalent(normalizedSource, normalizedConverted);

            if (isDifferent)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(format, property, displaySource, displayConverted);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(format, property, displaySource, displayConverted);
            }
        }

        /// <summary>
        /// Check if two values are equivalent, accounting for numeric precision and null handling
        /// </summary>
        private bool AreValuesEquivalent(string value1, string value2)
        {
            if (value1 == value2) return true;
            if (value1 == "NULL" || value2 == "NULL") return false;

            // Try numeric comparison with tolerance
            if (double.TryParse(value1, out double num1) && double.TryParse(value2, out double num2))
            {
                return Math.Abs(num1 - num2) < 0.0001;
            }

            // String comparison (case insensitive)
            return string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get the tool type name from the converted tool
        /// </summary>
        private string GetToolTypeName(Tool tool)
        {
            if (tool == null) return "NULL";

            try
            {
                var toolType = (Enums.ToolTypes)tool.Tool_type_id;
                return toolType.ToString();
            }
            catch
            {
                return $"Unknown ({tool.Tool_type_id})";
            }
        }

        /// <summary>
        /// Get the material name from the converted tool
        /// </summary>
        private string GetMaterialName(Tool tool)
        {
            if (tool == null) return "NULL";

            try
            {
                var material = (Enums.ToolMaterials)tool.Tool_material_id;
                return material.ToString();
            }
            catch
            {
                return $"Unknown ({tool.Tool_material_id})";
            }
        }

    }
}
