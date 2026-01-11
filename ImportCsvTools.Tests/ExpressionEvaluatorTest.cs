using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImportCsvTools.Tests
{
    /// <summary>
    /// Comprehensive tests for the ExpressionEvaluator class.
    /// Tests expression evaluation, value type handling, parameters, conditionals, error handling, and edge cases.
    /// </summary>
    [TestClass]
    public class ExpressionEvaluatorTest
    {
        #region Basic Expression Evaluation Tests

        [TestMethod]
        public void TestNullExpression()
        {
            Console.Write("Testing null expression returns original value... ");
            var result = ExpressionEvaluator.EvaluateExpression(null, "TestValue");
            Assert.AreEqual("TestValue", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestEmptyExpression()
        {
            Console.Write("Testing empty expression returns original value... ");
            var result = ExpressionEvaluator.EvaluateExpression("", "TestValue");
            Assert.AreEqual("TestValue", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestWhitespaceExpression()
        {
            Console.Write("Testing whitespace-only expression returns original value... ");
            var result = ExpressionEvaluator.EvaluateExpression("   ", "TestValue");
            Assert.AreEqual("TestValue", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestArithmeticMultiplication()
        {
            Console.Write("Testing arithmetic multiplication (v * 2)... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 2", "5.5");
            Assert.AreEqual(11.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestArithmeticAddition()
        {
            Console.Write("Testing arithmetic addition (v + 10)... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + 10", "15.5");
            Assert.AreEqual(25.5, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestArithmeticDivision()
        {
            Console.Write("Testing arithmetic division (v / 25.4)... ");
            var result = ExpressionEvaluator.EvaluateExpression("v / 25.4", "25.4");
            Assert.AreEqual(1.0, Convert.ToDouble(result), 0.0001);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestArithmeticSubtraction()
        {
            Console.Write("Testing arithmetic subtraction (v - 5)... ");
            var result = ExpressionEvaluator.EvaluateExpression("v - 5", "10");
            Assert.AreEqual(5.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestComplexArithmetic()
        {
            Console.Write("Testing complex arithmetic ((v * 2) + 10) / 3... ");
            var result = ExpressionEvaluator.EvaluateExpression("((v * 2) + 10) / 3", "5");
            Assert.AreEqual(6.666666, Convert.ToDouble(result), 0.0001);
            Console.WriteLine("PASS");
        }

        #endregion

        #region Value Type Handling Tests

        [TestMethod]
        public void TestNumericValueParsing()
        {
            Console.Write("Testing numeric value is parsed for math operations... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 25.4", "1.5");
            Assert.AreEqual((1.5 * 25.4), Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestStringValueHandling()
        {
            Console.Write("Testing string value with concatenation... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + '_suffix'", "prefix");
            Assert.AreEqual("prefix_suffix", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestLowercaseTransformation()
        {
            Console.Write("Testing vl (lowercase) transformation... ");
            var result = ExpressionEvaluator.EvaluateExpression("vl", "MixedCase");
            Assert.AreEqual("mixedcase", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestUppercaseTransformation()
        {
            Console.Write("Testing vu (uppercase) transformation... ");
            var result = ExpressionEvaluator.EvaluateExpression("vu", "MixedCase");
            Assert.AreEqual("MIXEDCASE", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNumericValueInStringContext()
        {
            Console.Write("Testing numeric value treated as string when needed... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + '.0'", "25");
            Assert.AreEqual("25.0", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNegativeNumber()
        {
            Console.Write("Testing negative number handling... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * -1", "5.5");
            Assert.AreEqual(-5.5, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestZeroValue()
        {
            Console.Write("Testing zero value handling... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + 10", "0");
            Assert.AreEqual(10.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestScientificNotation()
        {
            Console.Write("Testing scientific notation parsing... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 2", "1.5e2");
            Assert.AreEqual(300.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        #endregion

        #region Parameter Handling Tests

        [TestMethod]
        public void TestSingleParameter()
        {
            Console.Write("Testing single parameter usage... ");
            var parameters = new Dictionary<string, string> { { "Material", "Carbide" } };
            var result = ExpressionEvaluator.EvaluateExpression("row_Material", "10", parameters);
            Assert.AreEqual("Carbide", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestMultipleParameters()
        {
            Console.Write("Testing multiple parameter concatenation... ");
            var parameters = new Dictionary<string, string>
            {
                { "Type", "EndMill" },
                { "Material", "Carbide" }
            };
            var result = ExpressionEvaluator.EvaluateExpression("row_Type + '_' + row_Material", "value", parameters);
            Assert.AreEqual("EndMill_Carbide", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestParameterWithSpecialCharacters()
        {
            Console.Write("Testing parameter name with special characters is sanitized... ");
            var parameters = new Dictionary<string, string> { { "Tool-Type", "EndMill" } };
            // The column name will be sanitized to row_Tool_Type
            var result = ExpressionEvaluator.EvaluateExpression("row_Tool_Type", "value", parameters);
            Assert.AreEqual("EndMill", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestParameterWithSpaces()
        {
            Console.Write("Testing parameter name with spaces is sanitized... ");
            var parameters = new Dictionary<string, string> { { "Tool Type", "EndMill" } };
            // The column name will be sanitized to row_Tool_Type
            var result = ExpressionEvaluator.EvaluateExpression("row_Tool_Type", "value", parameters);
            Assert.AreEqual("EndMill", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNullParameter()
        {
            Console.Write("Testing null parameter value is converted to empty string... ");
            var parameters = new Dictionary<string, string> { { "Material", null } };
            var result = ExpressionEvaluator.EvaluateExpression("row_Material + '_suffix'", "value", parameters);
            Assert.AreEqual("_suffix", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestEmptyParameter()
        {
            Console.Write("Testing empty parameter value... ");
            var parameters = new Dictionary<string, string> { { "Material", "" } };
            var result = ExpressionEvaluator.EvaluateExpression("row_Material + 'test'", "value", parameters);
            Assert.AreEqual("test", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestParameterInArithmetic()
        {
            Console.Write("Testing parameter in arithmetic expression... ");
            var parameters = new Dictionary<string, string> { { "Multiplier", "2.5" } };
            var result = ExpressionEvaluator.EvaluateExpression("v * row_Multiplier", "10", parameters, true);
            Assert.AreEqual(25.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        #endregion

        #region Conditional Logic Tests

        [TestMethod]
        public void TestSimpleIIF()
        {
            Console.Write("Testing simple IIF expression... ");
            var result = ExpressionEvaluator.EvaluateExpression("IIF(v > 0.5, 'Large', 'Small')", "1.0");
            Assert.AreEqual("Large", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestIIFFalseCondition()
        {
            Console.Write("Testing IIF with false condition... ");
            var result = ExpressionEvaluator.EvaluateExpression("IIF(v > 0.5, 'Large', 'Small')", "0.25");
            Assert.AreEqual("Small", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestIIFWithParameters()
        {
            Console.Write("Testing IIF with parameter comparison... ");
            var parameters = new Dictionary<string, string> { { "Material", "HSS" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Material = 'HSS', v * 1.5, v)",
                "10",
                parameters);
            Assert.AreEqual(15.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestComplexConditional()
        {
            Console.Write("Testing complex conditional with AND... ");
            var parameters = new Dictionary<string, string> { { "Type", "EndMill" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Type = 'EndMill' AND v < 0.25, v * 25.4, v)",
                "0.125",
                parameters);
            Assert.AreEqual(3.175, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestComplexConditionalWithOR()
        {
            Console.Write("Testing complex conditional with OR... ");
            var parameters = new Dictionary<string, string> { { "Material", "Carbide" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Material = 'HSS' OR row_Material = 'Carbide', 'Valid', 'Invalid')",
                "HSS",
                parameters);
            Assert.AreEqual("Valid", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNestedIIF()
        {
            Console.Write("Testing nested IIF expressions... ");
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(v < 0.25, 'Small', IIF(v < 0.5, 'Medium', 'Large'))",
                "0.4");
            Assert.AreEqual("Medium", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestIIFWithStringComparison()
        {
            Console.Write("Testing IIF with string comparison using vl... ");
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(vl = 'carbide', 1, IIF(vl = 'hss', 2, 0))",
                "Carbide");
            Assert.AreEqual(1, Convert.ToInt32(result));
            Console.WriteLine("PASS");
        }

        #endregion

        #region Error Handling Tests

        [TestMethod]
        public void TestInvalidExpressionWithoutRaisingError()
        {
            Console.Write("Testing invalid expression returns original value when raise_error=false... ");
            var result = ExpressionEvaluator.EvaluateExpression("invalid_column", "TestValue", null, false);
            Assert.AreEqual("TestValue", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        [ExpectedException(typeof(EvaluateException))]
        public void TestInvalidExpressionWithRaisingError()
        {
            Console.Write("Testing invalid expression throws exception when raise_error=true... ");
            try
            {
                ExpressionEvaluator.EvaluateExpression("invalid_column", "TestValue", null, true);
            }
            catch (Exception)
            {
                Console.WriteLine("PASS (Exception thrown as expected)");
                throw;
            }
        }

        [TestMethod]
        public void TestDivisionByZeroWithoutRaisingError()
        {
            Console.Write("Testing division by zero returns original value when raise_error=false... ");
            var result = ExpressionEvaluator.EvaluateExpression("v / 0", "10", null, false);
            Assert.AreEqual("10", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        [ExpectedException(typeof(EvaluateException))]
        public void TestDivisionByZeroWithRaisingError()
        {
            Console.Write("Testing division by zero throws exception when raise_error=true... ");
            try
            {
                ExpressionEvaluator.EvaluateExpression("v / 0", "10", null, true);
            }
            catch (Exception)
            {
                Console.WriteLine("PASS (Exception thrown as expected)");
                throw;
            }
        }

        [TestMethod]
        public void TestTypeMismatchWithoutRaisingError()
        {
            Console.Write("Testing type mismatch returns original value when raise_error=false... ");
            // Trying to do arithmetic on a non-numeric string
            var result = ExpressionEvaluator.EvaluateExpression("v * 2", "NotANumber", null, false);
            Assert.AreEqual("NotANumber", result);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestUndefinedParameterWithoutRaisingError()
        {
            Console.Write("Testing undefined parameter reference returns original value when raise_error=false... ");
            var result = ExpressionEvaluator.EvaluateExpression("row_NonExistent", "TestValue", null, false);
            Assert.AreEqual("TestValue", result);
            Console.WriteLine("PASS");
        }

        #endregion

        #region Edge Cases & Boundary Tests

        [TestMethod]
        public void TestNullValue()
        {
            Console.Write("Testing null value input... ");
            var result = ExpressionEvaluator.EvaluateExpression("v", null);
            // Null should be treated as empty string
            Assert.AreEqual(result,"");
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNullCompareValueTrue()
        {
            Console.Write("Testing null value input... ");
            var result = ExpressionEvaluator.EvaluateExpression("IIF(v = '', TRUE, FALSE)", null);
            Assert.AreEqual(result, true);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestNullCompareValueFalse()
        {
            Console.Write("Testing null value input... ");
            var result = ExpressionEvaluator.EvaluateExpression("IIF(v = 0, TRUE, FALSE)", "1");
            Assert.AreEqual(result, false);
            Console.WriteLine("PASS");
        }


        [TestMethod]
        public void TestEmptyValue()
        {
            Console.Write("Testing empty value input... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + '_test'", "");
            Assert.AreEqual("_test", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestWhitespaceValue()
        {
            Console.Write("Testing whitespace value input... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + 'x'", "   ");
            Assert.AreEqual("   x", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestVeryLargeNumber()
        {
            Console.Write("Testing very large number... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 2", "999999999.99");
            Assert.AreEqual(1999999999.98, Convert.ToDouble(result), 0.01);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestVerySmallNumber()
        {
            Console.Write("Testing very small number... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 1000", "0.000001");
            Assert.AreEqual(0.001, Convert.ToDouble(result), 0.000001);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestUnicodeCharacters()
        {
            Console.Write("Testing unicode characters in value... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + '_suffix'", "测试");
            Assert.AreEqual("测试_suffix", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestSpecialCharactersInValue()
        {
            Console.Write("Testing special characters in value... ");
            var result = ExpressionEvaluator.EvaluateExpression("v + '_test'", "a@b#c$");
            Assert.AreEqual("a@b#c$_test", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestLongExpression()
        {
            Console.Write("Testing long expression... ");
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(v > 0, IIF(v > 1, IIF(v > 2, IIF(v > 3, 'VeryLarge', 'Large'), 'Medium'), 'Small'), 'Zero')",
                "2.5");
            Assert.AreEqual("Large", result.ToString());
            Console.WriteLine("PASS");
        }

        #endregion

        #region Real-World Scenario Tests

        [TestMethod]
        public void TestUnitConversionInchesToMM()
        {
            Console.Write("Testing unit conversion: inches to mm... ");
            var result = ExpressionEvaluator.EvaluateExpression("v * 25.4", "0.5");
            Assert.AreEqual(12.7, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestUnitConversionMMToInches()
        {
            Console.Write("Testing unit conversion: mm to inches... ");
            var result = ExpressionEvaluator.EvaluateExpression("v / 25.4", "25.4");
            Assert.AreEqual(1.0, Convert.ToDouble(result), 0.0001);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestValueMappingMaterialToID()
        {
            Console.Write("Testing value mapping: material name to ID... ");
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(vl = 'carbide', 1, IIF(vl = 'hss', 2, IIF(vl = 'cobalt', 3, 0)))",
                "HSS");
            Assert.AreEqual(2, Convert.ToInt32(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestStringFormattingWithMultipleParams()
        {
            Console.Write("Testing string formatting with multiple parameters... ");
            var parameters = new Dictionary<string, string>
            {
                { "Diameter", "0.5" },
                { "Flutes", "4" },
                { "Material", "Carbide" }
            };
            var result = ExpressionEvaluator.EvaluateExpression(
                "row_Diameter + 'in ' + row_Flutes + 'FL ' + row_Material",
                "value",
                parameters);
            Assert.AreEqual("0.5in 4FL Carbide", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestDataCleanupUppercase()
        {
            Console.Write("Testing data cleanup: uppercase transformation... ");
            var result = ExpressionEvaluator.EvaluateExpression("vu", "mixed CASE value");
            Assert.AreEqual("MIXED CASE VALUE", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestDataCleanupLowercase()
        {
            Console.Write("Testing data cleanup: lowercase transformation... ");
            var result = ExpressionEvaluator.EvaluateExpression("vl", "MIXED case VALUE");
            Assert.AreEqual("mixed case value", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestConditionalUnitConversion()
        {
            Console.Write("Testing conditional unit conversion based on parameter... ");
            var parameters = new Dictionary<string, string> { { "Units", "mm" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Units = 'mm', v / 25.4, v)",
                "25.4",
                parameters);
            Assert.AreEqual(1.0, Convert.ToDouble(result), 0.0001);
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestComplexToolDescriptionBuilder()
        {
            Console.Write("Testing complex tool description builder... ");
            var parameters = new Dictionary<string, string>
            {
                { "Type", "End Mill" },
                { "Material", "Carbide" },
                { "Coating", "TiAlN" }
            };
            var result = ExpressionEvaluator.EvaluateExpression(
                "v + ' ' + row_Type + ' - ' + row_Material + ' (' + row_Coating + ')'",
                "0.5in",
                parameters);
            Assert.AreEqual("0.5in End Mill - Carbide (TiAlN)", result.ToString());
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestMaterialBasedMultiplier()
        {
            Console.Write("Testing material-based calculation multiplier... ");
            var parameters = new Dictionary<string, string> { { "Material", "HSS" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Material = 'Carbide', v * 1.2, IIF(row_Material = 'HSS', v * 0.8, v))",
                "100",
                parameters);
            Assert.AreEqual(80.0, Convert.ToDouble(result));
            Console.WriteLine("PASS");
        }

        [TestMethod]
        public void TestDefaultValueHandling()
        {
            Console.Write("Testing default value when parameter is empty... ");
            var parameters = new Dictionary<string, string> { { "Material", "" } };
            var result = ExpressionEvaluator.EvaluateExpression(
                "IIF(row_Material = '', 'Unknown', row_Material)",
                "value",
                parameters);
            Assert.AreEqual("Unknown", result.ToString());
            Console.WriteLine("PASS");
        }

        #endregion
    }
}
