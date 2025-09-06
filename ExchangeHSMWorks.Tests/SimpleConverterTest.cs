using System;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase.ToolDataBase;
using HSMAdvisorDatabase;
using ExchangeHSMWorks;

namespace ExchangeHSMWorks.Tests
{
    /// <summary>
    /// Simple test class that can be run without MSTest framework
    /// Demonstrates the plugin import functionality with the test data
    /// </summary>
    public class SimpleConverterTest
    {
        private const string TestDataPath = @"test-data\Harvey Tool-End Mills.hsmlib";
        private const int ExpectedToolCount = 11937; // Based on our analysis of the test file

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
            TestFileExists();
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

        private void TestFileExists()
        {
            Console.Write("Testing file exists... ");
            var testFilePath = Path.GetFullPath(TestDataPath);

            if (!File.Exists(testFilePath))
            {
                throw new FileNotFoundException($"Test data file not found at: {testFilePath}");
            }

            Console.WriteLine("PASS");
        }

        private void TestImportToolCount()
        {
            Console.Write("Testing import tool count... ");
            var result = ImportToolsFromFile();

            if (result == null)
                throw new Exception("ImportTools returned null");

            if (result.Tools == null)
                throw new Exception("Tools collection is null");

            if (result.Tools.Count != ExpectedToolCount)
                throw new Exception($"Expected {ExpectedToolCount} tools, got {result.Tools.Count}");

            Console.WriteLine($"PASS ({result.Tools.Count} tools imported)");
        }

        private void TestLibraryCreation()
        {
            Console.Write("Testing library creation... ");
            var result = ImportToolsFromFile();
            var expectedLibraryName = Path.GetFileNameWithoutExtension(TestDataPath);

            if (result.Libraries == null)
                throw new Exception("Libraries collection is null");

            if (!result.Libraries.Any(l => l.Name == expectedLibraryName))
                throw new Exception($"Library '{expectedLibraryName}' was not created");

            if (!result.Tools.All(t => t.Library == expectedLibraryName))
                throw new Exception("Not all tools are assigned to the correct library");

            Console.WriteLine("PASS");
        }

        private void TestToolDataPreservation()
        {
            Console.Write("Testing tool data preservation... ");
            var result = ImportToolsFromFile();

            if (!result.Tools.All(t => !string.IsNullOrEmpty(t.Guid)))
                throw new Exception("Some tools are missing GUIDs");

            if (!result.Tools.All(t => t.Diameter > 0))
                throw new Exception("Some tools have invalid diameters");

            if (!result.Tools.All(t => !string.IsNullOrEmpty(t.Aux_data)))
                throw new Exception("Some tools are missing Aux_data");

            Console.WriteLine("PASS");
        }

        private void TestToolTypeMapping()
        {
            Console.Write("Testing tool type mapping... ");
            var result = ImportToolsFromFile();

            var toolTypes = result.Tools.Select(t => (Enums.ToolTypes)t.Name_id).Distinct().ToList();

            if (!toolTypes.Any())
                throw new Exception("No tool types found");

            if (!result.Tools.Any(t => (Enums.ToolTypes)t.Name_id == Enums.ToolTypes.SolidEndMill))
                throw new Exception("No flat end mills found");

            if (!result.Tools.Any(t => (Enums.ToolTypes)t.Name_id == Enums.ToolTypes.SolidBallMill))
                throw new Exception("No ball end mills found");

            Console.WriteLine($"PASS ({toolTypes.Count} different tool types)");
        }

        private void TestMaterialMapping()
        {
            Console.Write("Testing material mapping... ");
            var result = ImportToolsFromFile();

            var materials = result.Tools.Select(t => (Enums.ToolMaterials)t.Tool_material_id).Distinct().ToList();

            if (!materials.Any())
                throw new Exception("No materials found");

            if (!result.Tools.Any(t => (Enums.ToolMaterials)t.Tool_material_id == Enums.ToolMaterials.Carbide))
                throw new Exception("No carbide tools found");

            Console.WriteLine($"PASS ({materials.Count} different materials)");
        }

        private void TestUnitHandling()
        {
            Console.Write("Testing unit handling... ");
            var result = ImportToolsFromFile();

            // Check that unit flags are set based on source data
            // The converter sets all unit flags based on the source unit
            var toolsWithInconsistentUnits = result.Tools.Where(t =>
                t.Input_units_m != t.Diameter_m ||
                t.Input_units_m != t.Circle_dia_m ||
                t.Input_units_m != t.Depth_m).ToList();

            if (toolsWithInconsistentUnits.Any())
            {
                var firstInconsistent = toolsWithInconsistentUnits.First();
                throw new Exception($"Unit flags inconsistent for tool {firstInconsistent.Series_name}: " +
                    $"Input_units_m={firstInconsistent.Input_units_m}, " +
                    $"Diameter_m={firstInconsistent.Diameter_m}, " +
                    $"Circle_dia_m={firstInconsistent.Circle_dia_m}");
            }

            // Count tools by unit type
            var metricTools = result.Tools.Count(t => t.Input_units_m);
            var imperialTools = result.Tools.Count(t => !t.Input_units_m);

            Console.WriteLine($"PASS ({metricTools} metric, {imperialTools} imperial tools)");
        }

        private void TestToolGeometry()
        {
            Console.Write("Testing tool geometry... ");
            var result = ImportToolsFromFile();

            if (!result.Tools.All(t => t.Diameter > 0))
                throw new Exception("Some tools have non-positive diameter");

            if (!result.Tools.All(t => t.Flute_Len >= 0))
                throw new Exception("Some tools have negative flute length");

            if (!result.Tools.All(t => t.Stickout >= 0))
                throw new Exception("Some tools have negative stickout");

            if (!result.Tools.All(t => t.Flute_N > 0))
                throw new Exception("Some tools have no flutes");

            Console.WriteLine("PASS");
        }

        private void TestManufacturerData()
        {
            Console.Write("Testing manufacturer data... ");
            var result = ImportToolsFromFile();

            if (!result.Tools.All(t => !string.IsNullOrEmpty(t.Brand_name)))
                throw new Exception("Some tools missing manufacturer name");

            if (!result.Tools.All(t => t.Brand_name == "HARVEY TOOL"))
                throw new Exception("Not all tools are from Harvey Tool");

            if (!result.Tools.All(t => !string.IsNullOrEmpty(t.Series_name)))
                throw new Exception("Some tools missing product ID");

            Console.WriteLine("PASS");
        }

        private void TestRoundTripData()
        {
            Console.Write("Testing round-trip data... ");
            var result = ImportToolsFromFile();

            // Test first 10 tools for performance
            foreach (var tool in result.Tools.Take(10))
            {
                if (string.IsNullOrEmpty(tool.Aux_data))
                    throw new Exception($"Tool {tool.Series_name} missing Aux_data");

                try
                {
                    var originalTool = Serializer.FromXML<toollibraryTool>(tool.Aux_data, false);
                    if (originalTool == null)
                        throw new Exception($"Failed to deserialize Aux_data for tool {tool.Series_name}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to deserialize Aux_data for tool {tool.Series_name}: {ex.Message}");
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

        /// <summary>
        /// Helper method to import tools from the test file
        /// </summary>
        private DataBase ImportToolsFromFile()
        {
            var testFilePath = Path.GetFullPath(TestDataPath);

            // Read the XML directly
            var xml = File.ReadAllText(testFilePath);
            var src = Serializer.FromXML<toollibrary>(xml, false);

            // Create a new database
            var targetDB = new DataBase();
            var libname = Path.GetFileNameWithoutExtension(testFilePath);

            // Add library
            targetDB.AddLibrary(libname);

            // Add tools one by one (mimicking the ImportTools method logic)
            src.tool.ForEach(srcTool =>
            {
                var tool = Converter.ToTool(srcTool);
                tool.Library = libname;
                targetDB.Tools.Add(tool);

                // Add holder if it has one
                if (srcTool.holder != null)
                {
                    var holder = targetDB.Holders.FirstOrDefault(e =>
                        e.Comment == srcTool.holder.description && e.Library == tool.Library);
                    if (holder != null)
                        targetDB.Holders.Remove(holder);

                    targetDB.Holders.Add(new Holder()
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

            return targetDB;
        }
    }
}
