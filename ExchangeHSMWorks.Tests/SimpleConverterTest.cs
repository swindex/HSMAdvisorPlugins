using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using HSMAdvisorDatabase.ToolDataBase;
using HSMAdvisorDatabase;
using ExchangeHSMWorks;

namespace ExchangeHSMWorks.Tests
{
    /// <summary>
    /// Simple test class that can be run without MSTest framework
    /// Demonstrates the plugin import functionality with all test data files
    /// </summary>
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
            public int ToolCount { get; set; }
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

        public void RunAllTests()
        {
            // Load all test data files first
            LoadAllTestData();

            TestFilesExist();
            TestImportToolCount();
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
                    Console.WriteLine($"OK ({testData.ToolCount} tools)");
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

            return new TestDataInfo
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath),
                Database = database,
                OriginalData = originalData,
                ToolCount = database.Tools.Count
            };
        }

        private void TestFilesExist()
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

        private void TestImportToolCount()
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

        private void TestLibraryCreation()
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

        private void TestToolDataPreservation()
        {
            Console.Write("Testing tool data preservation... ");

            foreach (var testData in _testDataCache.Values)
            {
                if (!testData.Database.Tools.All(t => !string.IsNullOrEmpty(t.Guid)))
                    throw new Exception($"Some tools are missing GUIDs in {testData.FileName}");

                if (!testData.Database.Tools.All(t => t.Diameter > 0))
                    throw new Exception($"Some tools have invalid diameters in {testData.FileName}");

                if (!testData.Database.Tools.All(t => !string.IsNullOrEmpty(t.Aux_data)))
                    throw new Exception($"Some tools are missing Aux_data in {testData.FileName}");
            }

            Console.WriteLine("PASS");
        }

        private void TestToolTypeMapping()
        {
            Console.Write("Testing tool type mapping... ");

            var allToolTypes = new HashSet<Enums.ToolTypes>();
            var hasEndMills = false;
            var hasBallMills = false;

            foreach (var testData in _testDataCache.Values)
            {
                var toolTypes = testData.Database.Tools.Select(t => (Enums.ToolTypes)t.Name_id).Distinct().ToList();

                if (!toolTypes.Any())
                    throw new Exception($"No tool types found in {testData.FileName}");

                foreach (var toolType in toolTypes)
                {
                    allToolTypes.Add(toolType);
                }

                if (testData.Database.Tools.Any(t => (Enums.ToolTypes)t.Name_id == Enums.ToolTypes.SolidEndMill))
                    hasEndMills = true;

                if (testData.Database.Tools.Any(t => (Enums.ToolTypes)t.Name_id == Enums.ToolTypes.SolidBallMill))
                    hasBallMills = true;
            }

            if (!hasEndMills)
                throw new Exception("No flat end mills found across all test files");

            if (!hasBallMills)
                throw new Exception("No ball end mills found across all test files");

            Console.WriteLine($"PASS ({allToolTypes.Count} different tool types across all files)");
        }

        private void TestMaterialMapping()
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

        private void TestUnitHandling()
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

        private void TestToolGeometry()
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

        private void TestManufacturerData()
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

        private void TestRoundTripData()
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

        private void TestMaterialConversion()
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

        private void TestCapabilities()
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

    }
}
