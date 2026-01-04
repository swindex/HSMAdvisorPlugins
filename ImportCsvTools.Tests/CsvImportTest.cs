using System;
using System.IO;
using System.Linq;
using HSMAdvisorDatabase.ToolDataBase;
using ImportCsvTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImportCsvTools.Tests
{
    [TestClass]
    public class CsvImportTest
    {
        // Get path to the project root
        private const string TestDataDirectory = @"..\..\test-data";

        [TestMethod]
        public void RunAllTests()
        {
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");

            EnsureTestFilesExist(csvPath, mappingPath);

            var database = CsvToolImporter.ImportFromFiles(csvPath, mappingPath, MessageFlags.None);

            TestToolCount(database);
            TestLibraryName(database);
            TestToolMappings(database);
        }

        [TestMethod]
        public void TestFilesExist()
        {
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");

            EnsureTestFilesExist(csvPath, mappingPath);
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

        [TestMethod]
        public void TestImportedToolCount()
        {
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var database = CsvToolImporter.ImportFromFiles(csvPath, mappingPath, MessageFlags.None);

            TestToolCount(database);
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

            if (database.Tools.Count != 288)
            {
                throw new Exception($"Expected 3 tools, found {database.Tools.Count}");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestImportedLibraryName()
        {
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var database = CsvToolImporter.ImportFromFiles(csvPath, mappingPath, MessageFlags.None);

            TestLibraryName(database);
        }

        private static void TestLibraryName(DataBase database)
        {
            Console.Write("Testing library name... ");

            var tlibname = "Tool Master Import for HSMA";

            if (database.Libraries == null || !database.Libraries.Any(library => library.Name == tlibname))
            {
                throw new Exception($"Expected library name '{tlibname}' was not created");
            }

            if (database.Tools.Any(tool => tool.Library != tlibname))
            {
                throw new Exception($"Not all tools are assigned to the '{tlibname}' library");
            }

            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestImportedToolMappings()
        {
            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var csvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            var mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            var database = CsvToolImporter.ImportFromFiles(csvPath, mappingPath, MessageFlags.None);

            TestToolMappings(database);
        }

        private static void TestToolMappings(DataBase database)
        {
            Console.Write("Testing mapped tool values... ");

            var firstTool = database.Tools.FirstOrDefault(tool => tool.Number == 10);
            if (firstTool == null)
            {
                throw new Exception("Tool number 1 was not imported");
            }

            if (firstTool.Comment != "1/32 x 3/32 4 Flt Carb End Mill")
            {
                throw new Exception($"Unexpected comment for tool 1: {firstTool.Comment}");
            }

            if (Math.Abs(firstTool.Diameter - 0.03125) > 0.0001)
            {
                throw new Exception($"Unexpected diameter for tool 1: {firstTool.Diameter}");
            }

            if ((Enums.ToolMaterials)firstTool.Tool_material_id != Enums.ToolMaterials.Carbide)
            {
                throw new Exception("Tool material mapping for tool 1 failed");
            }

            var thirdTool = database.Tools.FirstOrDefault(tool => tool.Number == 60);
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
