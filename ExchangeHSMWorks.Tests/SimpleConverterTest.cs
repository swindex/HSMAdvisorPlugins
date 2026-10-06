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

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
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
            TestToToolFromToolLossless();
            TestTaperAngleImport();
            TestTaperAngleRoundTrip();
            TestTaperAngleRoundTripNoAuxData();
            TestTaperAngleModifiedExport();
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

            var testDataDir = GetTestDataDirectory();
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

        private void EnsureTestDataLoaded()
        {
            if (_testDataCache.Count == 0)
                LoadAllTestData();
        }

        private string GetTestDataDirectory()
        {
            var baseDirectories = new[]
            {
                Directory.GetCurrentDirectory(),
                AppDomain.CurrentDomain.BaseDirectory
            };

            foreach (var baseDirectory in baseDirectories)
            {
                var directory = new DirectoryInfo(baseDirectory);
                while (directory != null)
                {
                    var candidate = Path.Combine(directory.FullName, TestDataDirectory);
                    if (Directory.Exists(candidate))
                        return candidate;

                    candidate = Path.Combine(directory.FullName, "test-data");
                    if (Directory.Exists(candidate))
                        return candidate;

                    directory = directory.Parent;
                }
            }

            return Path.GetFullPath(TestDataDirectory);
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

            EnsureTestDataLoaded();

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

            EnsureTestDataLoaded();

            foreach (var testData in _testDataCache.Values)
            {
                foreach (var originalTool in testData.OriginalData.tool)
                {
                    var convertedTool = testData.Database.Tools.FirstOrDefault(t => t.Guid == originalTool.guid);
                    if (convertedTool == null)
                        throw new Exception($"Converted tool not found for source GUID '{originalTool.guid}' in {testData.FileName}");

                    if (!string.Equals(convertedTool.Brand_name, originalTool.manufacturer, StringComparison.Ordinal))
                        throw new Exception($"Manufacturer changed for tool '{originalTool.productid}' in {testData.FileName}");

                    if (!string.Equals(convertedTool.Series_name, originalTool.productid, StringComparison.Ordinal))
                        throw new Exception($"Product ID changed for tool '{originalTool.productid}' in {testData.FileName}");
                }
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
        public void TestToToolFromToolLossless()
        {
            Console.Write("Testing ToTool/FromTool lossless round-trip... ");

            EnsureTestDataLoaded();

            foreach (var testData in _testDataCache.Values)
            {
                foreach (var originalTool in testData.OriginalData.tool)
                {
                    var convertedTool = Converter.ToTool(originalTool);
                    var exportedTool = Converter.FromTool(convertedTool);

                    var expectedXml = Serializer.ToXML(originalTool, "UTF-16");
                    var actualXml = Serializer.ToXML(exportedTool, "UTF-16");

                    if (!string.Equals(expectedXml, actualXml, StringComparison.Ordinal))
                    {
                        throw new Exception($"ToTool/FromTool round-trip changed tool '{originalTool.productid}' in {testData.FileName}.");
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
        /// Verifies that taper/flute/tip angles imported from HSMLib are stored from the
        /// correct vertical reference (see swindex/HSMAdvisor#140).
        /// HSMWorks "taper-angle" semantics per tool type:
        ///   - chamfer mill / counter sink: included tip angle
        ///   - drill / spot drill / counter bore: included point angle
        ///   - tapered mill (Taper mode): side taper from the tool axis
        ///   - dovetail mill: included angle, stored negative in HSMAdvisor
        /// After import, Tool.Toolangle must equal the source taper-angle (per mode), and
        /// the calculator-facing Leadangle must remain in a valid range (0, 180].
        /// </summary>
        [TestMethod]
        public void TestTaperAngleImport()
        {
            Console.Write("Testing taper angle import (vertical reference)... ");

            EnsureTestDataLoaded();

            var testData = _testDataCache["Milling Tools (Inch).hsmlib"];

            // guid -> (expected Toolangle, expected mode)
            var expected = new Dictionary<string, Tuple<double, Enums.ToolAngleModes>>
            {
                { "39052c62-d9a2-42d4-b7c0-5f11cfcb7ffd", Tuple.Create(45d, Enums.ToolAngleModes.Taper) }, // .425" x 45 Chamfer Mill
                { "2df7b1c1-1e4c-4b59-8023-bb213d775b9c", Tuple.Create(60d, Enums.ToolAngleModes.Taper) }, // .425" x 60 Chamfer Mill
                { "9001e5e0-20d9-442b-b1ec-3830de668786", Tuple.Create(90d, Enums.ToolAngleModes.Tip) },   // 0.25 DIA Counter Sink
                { "5bcd0a85-bbae-41d1-b849-d6287df62a12", Tuple.Create(120d, Enums.ToolAngleModes.Tip) },  // 0.5 DIA X 120 deg inc Spot Drill
                { "38702099-97e9-4e4b-9f60-5cff1a19f700", Tuple.Create(90d, Enums.ToolAngleModes.Tip) },   // 0.5 DIA X 90 deg inc Spot Drill
                { "8edd1c42-315a-44ca-9303-7c928412ec70", Tuple.Create(-45d, Enums.ToolAngleModes.Taper) }, // Dovetail Mill (taper-angle 45, stored negative)
                { "ce74e212-05a2-489c-84ba-6d2ed29d9a90", Tuple.Create(-30d, Enums.ToolAngleModes.Taper) }, // Dovetail Mill (taper-angle 30, stored negative)
                { "a9972dfd-3d73-4a12-b135-b351ec087f86", Tuple.Create(10d, Enums.ToolAngleModes.Taper) },  // Tapered Mill (10 deg side taper)
                { "cbbe8505-1358-4a65-a7d6-c75dc40389fb", Tuple.Create(5d, Enums.ToolAngleModes.Taper) },   // Tapered Mill (5 deg side taper)
            };

            foreach (var kv in expected)
            {
                var tool = testData.Database.Tools.FirstOrDefault(t => t.Guid == kv.Key);
                if (tool == null)
                    throw new Exception($"Test tool not found in database by guid '{kv.Key}'");

                if (tool.Toolangle_mode != kv.Value.Item2)
                    throw new Exception($"Tool '{kv.Key}' has angle mode {tool.Toolangle_mode}, expected {kv.Value.Item2}");

                if (!AreClose(tool.Toolangle, kv.Value.Item1))
                    throw new Exception($"Tool '{kv.Key}' imported Toolangle {tool.Toolangle}, expected {kv.Value.Item1} " +
                                        "(angle must be referenced from the vertical tool axis, swindex/HSMAdvisor#140)");

                // The stored Leadangle must be valid for the calculator (0, 180]
                if (tool.Leadangle <= 0d || tool.Leadangle > 180d)
                    throw new Exception($"Tool '{kv.Key}' has out-of-range Leadangle {tool.Leadangle} (expected within (0, 180])");
            }

            // Cross-check every imported tool that carries a source taper angle:
            // Toolangle must match the source taper-angle for Taper-mode tools,
            // and 2*(90 - source) == source (i.e. Toolangle == source) for Tip-mode tools.
            foreach (var originalTool in testData.OriginalData.tool.Where(t => t.body != null && !string.IsNullOrEmpty(t.body.taperangle)))
            {
                var imported = testData.Database.Tools.FirstOrDefault(t => t.Guid == originalTool.guid);
                if (imported == null) continue;

                var sourceAngle = Parse.ToDouble(originalTool.body.taperangle);
                if (imported.Toolangle_mode == Enums.ToolAngleModes.Taper)
                {
                    // dovetail mills: HSMWorks stores the side taper angle, HSMAdvisor stores it negative
                    var expectedToolangle = originalTool.type == "dovetail mill" ? -sourceAngle : sourceAngle;
                    if (!AreClose(imported.Toolangle, expectedToolangle))
                        throw new Exception($"Tool '{originalTool.guid}' ({originalTool.type}) Toolangle {imported.Toolangle}, expected {expectedToolangle} from source taper-angle {sourceAngle}");
                }
                else if (imported.Toolangle_mode == Enums.ToolAngleModes.Tip)
                {
                    if (!AreClose(imported.Toolangle, sourceAngle))
                        throw new Exception($"Tool '{originalTool.guid}' ({originalTool.type}) Toolangle {imported.Toolangle}, expected {sourceAngle} (included angle from source taper-angle)");
                }
            }

            Console.WriteLine($"PASS ({expected.Count} known tools verified)");
        }

        /// <summary>
        /// Round-trip test: HSMLib tool -> Tool -> HSMLib tool must preserve the source
        /// taper-angle exactly, for every angle-carrying tool type. This catches the
        /// horizontal/vertical reference complementation bug (swindex/HSMAdvisor#140)
        /// on both the import and export paths.
        /// </summary>
        [TestMethod]
        public void TestTaperAngleRoundTrip()
        {
            Console.Write("Testing taper angle round-trip... ");

            EnsureTestDataLoaded();

            int checkedTools = 0;
            foreach (var testData in _testDataCache.Values)
            {
                foreach (var originalTool in testData.OriginalData.tool)
                {
                    if (originalTool.body == null || string.IsNullOrEmpty(originalTool.body.taperangle))
                        continue;

                    var sourceAngle = Parse.ToDouble(originalTool.body.taperangle);
                    if (sourceAngle == 0d)
                        continue;

                    var imported = Converter.ToTool(originalTool);
                    var exported = Converter.FromTool(imported);

                    var exportedAngle = Parse.ToDouble(exported.body?.taperangle);
                    if (!AreClose(exportedAngle, sourceAngle))
                        throw new Exception($"Round-trip changed taper-angle of tool '{originalTool.productid}' ({originalTool.type}) in {testData.FileName}: " +
                                            $"source {sourceAngle}, exported {exportedAngle}");

                    // And the imported Tool must keep reporting the same angle via Toolangle
                    var expectedToolangle = originalTool.type == "dovetail mill" ? -sourceAngle : sourceAngle;
                    if (!AreClose(imported.Toolangle, expectedToolangle))
                        throw new Exception($"Imported Toolangle {imported.Toolangle} != source {expectedToolangle} for tool '{originalTool.productid}' ({originalTool.type}) in {testData.FileName}");

                    checkedTools++;
                }
            }

            if (checkedTools == 0)
                throw new Exception("No taper-angle tools found in test data - test data may be incomplete");

            Console.WriteLine($"PASS ({checkedTools} tools round-tripped)");
        }

        /// <summary>
        /// Same round-trip as <see cref="TestTaperAngleRoundTrip"/> but with the tool's
        /// Aux_data cleared before export. With Aux_data present, FromTool deserializes
        /// the original tool from that XML and SetIfChanged skips fields whose values are
        /// unchanged, so the taper-angle would be copied through verbatim instead of being
        /// recomputed. Nulling Aux_data forces originalTool == null, so the full switch-case
        /// conversion path (including ToHsmWorksAngle from Leadangle) is actually exercised.
        /// </summary>
        [TestMethod]
        public void TestTaperAngleRoundTripNoAuxData()
        {
            Console.Write("Testing taper angle round-trip (no Aux_data, real conversion)... ");

            EnsureTestDataLoaded();

            int checkedTools = 0;
            foreach (var testData in _testDataCache.Values)
            {
                foreach (var originalTool in testData.OriginalData.tool)
                {
                    if (originalTool.body == null || string.IsNullOrEmpty(originalTool.body.taperangle))
                        continue;

                    var sourceAngle = Parse.ToDouble(originalTool.body.taperangle);
                    if (sourceAngle == 0d)
                        continue;

                    var imported = Converter.ToTool(originalTool);
                    imported.Aux_data = null; // force FromTool to rebuild via real conversion
                    var exported = Converter.FromTool(imported);

                    var exportedAngle = Parse.ToDouble(exported.body?.taperangle);
                    if (!AreClose(exportedAngle, sourceAngle))
                        throw new Exception($"Round-trip (no Aux_data) changed taper-angle of tool '{originalTool.productid}' ({originalTool.type}) in {testData.FileName}: " +
                                            $"source {sourceAngle}, exported {exportedAngle}");

                    checkedTools++;
                }
            }

            if (checkedTools == 0)
                throw new Exception("No taper-angle tools found in test data - test data may be incomplete");

            Console.WriteLine($"PASS ({checkedTools} tools round-tripped)");
        }

        /// <summary>
        /// Round-trip for a user-modified angle: when the user changes the tool angle in
        /// HSMAdvisor (via Toolangle), the exported HSMLib taper-angle must carry the new
        /// value - not a complemented one.
        /// </summary>
        [TestMethod]
        public void TestTaperAngleModifiedExport()
        {
            Console.Write("Testing modified angle export... ");

            EnsureTestDataLoaded();

            var testData = _testDataCache["Milling Tools (Inch).hsmlib"];

            // 45 deg chamfer mill
            var originalTool = testData.OriginalData.tool.First(t => t.guid == "39052c62-d9a2-42d4-b7c0-5f11cfcb7ffd");
            var tool = Converter.ToTool(originalTool);
            tool.Toolangle = 90d; // user doubles the included angle
            var exported = Converter.FromTool(tool);
            var exportedAngle = Parse.ToDouble(exported.body?.taperangle);
            if (!AreClose(exportedAngle, 90d))
                throw new Exception($"Modified chamfer mill exported taper-angle {exportedAngle}, expected 90");

            // 10 deg tapered mill (Taper mode)
            var originalTaper = testData.OriginalData.tool.First(t => t.guid == "a9972dfd-3d73-4a12-b135-b351ec087f86");
            var taperTool = Converter.ToTool(originalTaper);
            taperTool.Toolangle = 15d;
            var exportedTaper = Converter.FromTool(taperTool);
            var exportedTaperAngle = Parse.ToDouble(exportedTaper.body?.taperangle);
            if (!AreClose(exportedTaperAngle, 15d))
                throw new Exception($"Modified tapered mill exported taper-angle {exportedTaperAngle}, expected 15");

            // Dovetail mill must export a positive angle (HSMAdvisor stores it negative)
            var originalDovetail = testData.OriginalData.tool.First(t => t.guid == "8edd1c42-315a-44ca-9303-7c928412ec70");
            var dovetail = Converter.ToTool(originalDovetail);
            if (!AreClose(dovetail.Toolangle, -45d))
                throw new Exception($"Dovetail mill imported Toolangle {dovetail.Toolangle}, expected -45");
            var exportedDovetail = Converter.FromTool(dovetail);
            var exportedDovetailAngle = Parse.ToDouble(exportedDovetail.body?.taperangle);
            if (!AreClose(exportedDovetailAngle, 45d))
                throw new Exception($"Dovetail mill exported taper-angle {exportedDovetailAngle}, expected 45 (HSMWorks stores positive)");

            Console.WriteLine("PASS");
        }

        private static bool AreClose(double a, double b, double tolerance = 0.0001d)
        {
            return Math.Abs(a - b) < tolerance;
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
