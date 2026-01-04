using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HSMAdvisorDatabase.ToolDataBase;
using ImportCsvTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImportCsvTools.Tests
{
    [TestClass]
    public class CsvCompletenessTest
    {
        // Get path to the project root
        private const string TestDataDirectory = @"..\..\test-data";
        private const string OutputDirectory = @"..\..\test-output";

        // Paths to source and exported CSV files
        private static string sourceCsvPath;
        private static string exportedCsvPath;
        private static string mappingPath;

        [ClassInitialize]
        public static void SetupTestData(TestContext context)
        {
            SetupTestDataInternal();
        }

        // Internal setup method that can be called from both MSTest and manual test runner
        private static void SetupTestDataInternal()
        {
            // Only setup once
            if (sourceCsvPath != null)
                return;

            Console.WriteLine("=== Setting up CSV Completeness Test ===");

            var testDataDir = Path.GetFullPath(TestDataDirectory);
            var outputDir = Path.GetFullPath(OutputDirectory);

            // Ensure output directory exists
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            sourceCsvPath = Path.Combine(testDataDir, "Tool Master Import for HSMA.csv");
            mappingPath = Path.Combine(testDataDir, "Tool_Master_Import_for_HSMA.mapping.json");
            exportedCsvPath = Path.Combine(outputDir, "test_completeness_export.csv");

            Console.WriteLine($"Source CSV: {sourceCsvPath}");
            Console.WriteLine($"Mapping: {mappingPath}");
            Console.WriteLine($"Export CSV: {exportedCsvPath}");

            // Import source CSV
            Console.WriteLine("Importing source CSV...");
            var database = CsvToolImporter.ImportFromFiles(sourceCsvPath, mappingPath, MessageFlags.None);
            if (database == null || database.Tools.Count == 0)
            {
                throw new Exception("Failed to import tools from source CSV");
            }
            Console.WriteLine($"  Imported {database.Tools.Count} tools");

            // Delete old export file if it exists
            if (File.Exists(exportedCsvPath))
            {
                File.Delete(exportedCsvPath);
            }

            // Export to CSV
            Console.WriteLine("Exporting to CSV...");
            CsvToolImporter.ExportToFile(database, exportedCsvPath, mappingPath);
            Console.WriteLine($"  Exported to: {exportedCsvPath}");

            Console.WriteLine("=== Setup Complete ===\n");
        }

        // Manual test runner method (for Program.cs)
        public void RunAllTests()
        {
            // Setup once
            SetupTestDataInternal();

            // Run the completeness test
            TestCsvCompletenessComparison();
        }

        [TestMethod]
        public void TestCsvCompletenessComparison()
        {
            Console.WriteLine("=== Testing CSV Completeness Comparison ===");

            // Parse both CSV files
            Console.WriteLine("Reading source CSV...");
            var sourceCsv = CsvFileHandler.ReadCsvFile(sourceCsvPath);
            Console.WriteLine($"  Source: {sourceCsv.Headers.Count} columns, {sourceCsv.Rows.Count} rows");

            Console.WriteLine("Reading exported CSV...");
            var exportedCsv = CsvFileHandler.ReadCsvFile(exportedCsvPath);
            Console.WriteLine($"  Exported: {exportedCsv.Headers.Count} columns, {exportedCsv.Rows.Count} rows");

            var differences = new List<string>();

            // Compare column count
            Console.WriteLine("\nComparing column count...");
            if (sourceCsv.Headers.Count != exportedCsv.Headers.Count)
            {
                differences.Add($"Column count mismatch: Source has {sourceCsv.Headers.Count} columns, Export has {exportedCsv.Headers.Count} columns");
            }

            // Compare column names and order
            Console.WriteLine("Comparing column names and order...");
            var maxColumns = Math.Max(sourceCsv.Headers.Count, exportedCsv.Headers.Count);
            var missingColumns = new List<string>();
            var extraColumns = new List<string>();
            var orderDifferences = new List<string>();

            for (int i = 0; i < maxColumns; i++)
            {
                var sourceHeader = i < sourceCsv.Headers.Count ? sourceCsv.Headers[i] : null;
                var exportedHeader = i < exportedCsv.Headers.Count ? exportedCsv.Headers[i] : null;

                if (sourceHeader != null && exportedHeader == null)
                {
                    missingColumns.Add(sourceHeader);
                }
                else if (sourceHeader == null && exportedHeader != null)
                {
                    extraColumns.Add(exportedHeader);
                }
                else if (sourceHeader != null && exportedHeader != null)
                {
                    if (!sourceHeader.Equals(exportedHeader, StringComparison.Ordinal))
                    {
                        orderDifferences.Add($"Column {i}: Expected '{sourceHeader}', Found '{exportedHeader}'");
                    }
                }
            }

            // Check for columns present in source but missing in export (regardless of order)
            var sourceHeaderSet = new HashSet<string>(sourceCsv.Headers, StringComparer.Ordinal);
            var exportedHeaderSet = new HashSet<string>(exportedCsv.Headers, StringComparer.Ordinal);

            var missingInExport = new List<string>();
            foreach (var header in sourceHeaderSet)
            {
                if (!exportedHeaderSet.Contains(header))
                {
                    missingInExport.Add(header);
                }
            }

            var extraInExport = new List<string>();
            foreach (var header in exportedHeaderSet)
            {
                if (!sourceHeaderSet.Contains(header))
                {
                    extraInExport.Add(header);
                }
            }

            if (missingInExport.Count > 0)
            {
                differences.Add($"\nColumns present in source but MISSING in export ({missingInExport.Count}):");
                foreach (var column in missingInExport)
                {
                    differences.Add($"  - '{column}'");
                }
            }

            if (extraInExport.Count > 0)
            {
                differences.Add($"\nColumns in export but NOT in source ({extraInExport.Count}):");
                foreach (var column in extraInExport)
                {
                    differences.Add($"  - '{column}'");
                }
            }

            if (orderDifferences.Count > 0)
            {
                differences.Add($"\nColumn order differences ({orderDifferences.Count}):");
                foreach (var diff in orderDifferences.Take(10))
                {
                    differences.Add($"  - {diff}");
                }
                if (orderDifferences.Count > 10)
                {
                    differences.Add($"  ... and {orderDifferences.Count - 10} more");
                }
            }

            // Compare row count
            Console.WriteLine("Comparing row count...");
            if (sourceCsv.Rows.Count != exportedCsv.Rows.Count)
            {
                differences.Add($"\nRow count mismatch: Source has {sourceCsv.Rows.Count} rows, Export has {exportedCsv.Rows.Count} rows");
            }

            // Compare cell values (only for columns that exist in both and are in the same position)
            Console.WriteLine("Comparing cell values...");
            var valueDifferences = new List<string>();
            var minRows = Math.Min(sourceCsv.Rows.Count, exportedCsv.Rows.Count);
            var minColumns = Math.Min(sourceCsv.Headers.Count, exportedCsv.Headers.Count);

            // Only compare values if columns match exactly in order
            bool canCompareValues = orderDifferences.Count == 0 && missingInExport.Count == 0 && extraInExport.Count == 0;

            if (canCompareValues)
            {
                for (int rowIndex = 0; rowIndex < minRows; rowIndex++)
                {
                    var sourceRow = sourceCsv.Rows[rowIndex];
                    var exportedRow = exportedCsv.Rows[rowIndex];

                    for (int colIndex = 0; colIndex < minColumns; colIndex++)
                    {
                        var sourceValue = colIndex < sourceRow.Length ? sourceRow[colIndex] : "";
                        var exportedValue = colIndex < exportedRow.Length ? exportedRow[colIndex] : "";

                        // Trim and normalize values for comparison
                        sourceValue = sourceValue?.Trim() ?? "";
                        exportedValue = exportedValue?.Trim() ?? "";

                        if (!sourceValue.Equals(exportedValue, StringComparison.Ordinal))
                        {
                            var columnName = sourceCsv.Headers[colIndex];
                            valueDifferences.Add(
                                $"Row {rowIndex + 1}, Column '{columnName}': " +
                                $"Expected '{TruncateForDisplay(sourceValue)}', " +
                                $"Found '{TruncateForDisplay(exportedValue)}'");

                            // Limit to first 20 value differences
                            if (valueDifferences.Count >= 20)
                            {
                                break;
                            }
                        }
                    }

                    if (valueDifferences.Count >= 20)
                    {
                        break;
                    }
                }

                if (valueDifferences.Count > 0)
                {
                    differences.Add($"\nCell value differences (showing first {Math.Min(20, valueDifferences.Count)}):");
                    foreach (var diff in valueDifferences)
                    {
                        differences.Add($"  - {diff}");
                    }

                    // Continue counting total differences
                    if (valueDifferences.Count == 20)
                    {
                        int totalDifferences = valueDifferences.Count;
                        for (int rowIndex = 0; rowIndex < minRows && totalDifferences < 1000; rowIndex++)
                        {
                            var sourceRow = sourceCsv.Rows[rowIndex];
                            var exportedRow = exportedCsv.Rows[rowIndex];

                            for (int colIndex = 0; colIndex < minColumns && totalDifferences < 1000; colIndex++)
                            {
                                var sourceValue = (colIndex < sourceRow.Length ? sourceRow[colIndex] : "")?.Trim() ?? "";
                                var exportedValue = (colIndex < exportedRow.Length ? exportedRow[colIndex] : "")?.Trim() ?? "";

                                if (!sourceValue.Equals(exportedValue, StringComparison.Ordinal))
                                {
                                    totalDifferences++;
                                }
                            }
                        }
                        if (totalDifferences > 20)
                        {
                            differences.Add($"  ... and approximately {totalDifferences - 20} more value differences");
                        }
                    }
                }
            }
            else
            {
                differences.Add("\nSkipping cell value comparison due to column structure differences");
            }

            // Report results
            Console.WriteLine("\n=== Comparison Results ===");
            if (differences.Count == 0)
            {
                Console.WriteLine("✓ PASS - Source and Export CSVs match completely!");
            }
            else
            {
                Console.WriteLine($"✗ FAIL - Found {differences.Count} categories of differences:");
                foreach (var diff in differences)
                {
                    Console.WriteLine(diff);
                }

                // Build detailed failure message
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("CSV Completeness Test Failed!");
                errorMessage.AppendLine();
                errorMessage.AppendLine("The exported CSV does not match the source CSV structure and content.");
                errorMessage.AppendLine();
                foreach (var diff in differences)
                {
                    errorMessage.AppendLine(diff);
                }

                throw new Exception(errorMessage.ToString());
            }
        }

        /// <summary>
        /// Truncates long strings for display in error messages
        /// </summary>
        private static string TruncateForDisplay(string value, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength) + "...";
        }
    }
}
