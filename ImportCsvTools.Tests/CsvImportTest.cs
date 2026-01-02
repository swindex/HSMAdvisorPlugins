using System;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase.ToolDataBase;
using ImportCsvTools;

namespace ImportCsvTools.Tests
{
    public class CsvImportTest
    {
        private const string TestDataDirectory = @"ImportCsvTools.Tests\test-data";

        public static void Main(string[] args)
        {
            Console.WriteLine("ImportCsvTools CSV Import Test");
            Console.WriteLine("===============================");
            Console.WriteLine();

            try
            {
                var test = new CsvImportTest();
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
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "sample-tools.csv");
            var mappingPath = Path.Combine(testDataDir, "sample-mapping.json");

            EnsureTestFilesExist(csvPath, mappingPath);

            var database = CsvToolImporter.ImportFromFiles(csvPath, mappingPath);

            TestToolCount(database);
            TestLibraryName(database);
            TestToolMappings(database);
        }

        private static void EnsureTestFilesExist(string csvPath, string mappingPath)
        {
            Console.Write("Testing sample files exist... ");

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException($"Sample CSV file not found: {csvPath}");
            }

            if (!File.Exists(mappingPath))
            {
                throw new FileNotFoundException($"Sample mapping file not found: {mappingPath}");
            }

            Console.WriteLine("PASS");
        }

        private static void TestToolCount(DataBase database)
        {
            Console.Write("Testing imported tool count... ");

            if (database == null)
            {
                throw new Exception("Database result is null");
            }

            if (database.Tools == null || database.Tools.Count == 0)
            {
                throw new Exception("No tools were imported");
            }

            if (database.Tools.Count != 3)
            {
                throw new Exception($"Expected 3 tools, found {database.Tools.Count}");
            }

            Console.WriteLine("PASS");
        }

        private static void TestLibraryName(DataBase database)
        {
            Console.Write("Testing library name... ");

            if (database.Libraries == null || !database.Libraries.Any(library => library.Name == "CSV Import"))
            {
                throw new Exception("Expected library name 'CSV Import' was not created");
            }

            if (database.Tools.Any(tool => tool.Library != "CSV Import"))
            {
                throw new Exception("Not all tools are assigned to the 'CSV Import' library");
            }

            Console.WriteLine("PASS");
        }

        private static void TestToolMappings(DataBase database)
        {
            Console.Write("Testing mapped tool values... ");

            var firstTool = database.Tools.FirstOrDefault(tool => tool.Number == 1);
            if (firstTool == null)
            {
                throw new Exception("Tool number 1 was not imported");
            }

            if (firstTool.Comment != "Sample End Mill")
            {
                throw new Exception($"Unexpected comment for tool 1: {firstTool.Comment}");
            }

            if (Math.Abs(firstTool.Diameter - 0.5) > 0.0001)
            {
                throw new Exception($"Unexpected diameter for tool 1: {firstTool.Diameter}");
            }

            if ((Enums.ToolMaterials)firstTool.Tool_material_id != Enums.ToolMaterials.Carbide)
            {
                throw new Exception("Tool material mapping for tool 1 failed");
            }

            var thirdTool = database.Tools.FirstOrDefault(tool => tool.Number == 3);
            if (thirdTool == null)
            {
                throw new Exception("Tool number 3 was not imported");
            }

            if ((Enums.ToolMaterials)thirdTool.Tool_material_id != Enums.ToolMaterials.HSS)
            {
                throw new Exception("Tool material mapping for tool 3 failed");
            }

            Console.WriteLine("PASS");
        }
    }
}
