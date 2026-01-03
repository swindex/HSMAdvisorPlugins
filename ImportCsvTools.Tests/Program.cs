using System;

namespace ImportCsvTools.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ImportCsvTools Test Suite");
            Console.WriteLine("=========================");
            Console.WriteLine();

            bool allPassed = true;

            try
            {
                Console.WriteLine("Running Import Tests...");
                Console.WriteLine("----------------------");
                var importTest = new CsvImportTest();
                importTest.RunAllTests();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import tests failed: {ex.Message}");
                allPassed = false;
            }

            try
            {
                Console.WriteLine("Running Export Tests...");
                Console.WriteLine("----------------------");
                var exportTest = new CsvExportTest();
                exportTest.RunAllTests();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Export tests failed: {ex.Message}");
                allPassed = false;
            }

            if (allPassed)
            {
                Console.WriteLine("=========================");
                Console.WriteLine("ALL TESTS PASSED!");
                Console.WriteLine("=========================");
            }
            else
            {
                Console.WriteLine("=========================");
                Console.WriteLine("SOME TESTS FAILED");
                Console.WriteLine("=========================");
                Environment.Exit(1);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
