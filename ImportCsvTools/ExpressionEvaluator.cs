using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportCsvTools
{
    internal class ExpressionEvaluator
    {
        // Cached DataTable for expression evaluation
        private static readonly System.Data.DataTable _expressionEvaluator = new System.Data.DataTable();
        /// <summary>
        /// Evaluates a mathematical expression with the CSV value substituted for 'v'
        /// Uses cached DataTable.Compute() for performance
        /// </summary>
        public static object EvaluateExpression(string expression, string value, bool raise_error = false)
        {
            // If no expression, return original value
            if (string.IsNullOrWhiteSpace(expression))
            {
                return value;
            }

            try
            {
                // Replace 'v' with the actual value using invariant culture
                expression = expression.Replace("LOWER(v)", $"'{value.ToLower()}'");
                expression = expression.Replace("UPPER(v)", $"'{value.ToUpper()}'");

                var expr = expression.Replace("v", value);

                // Evaluate using cached DataTable
                var result = _expressionEvaluator.Compute(expr, null);

                // Convert result to double
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Expression evaluation failed for '{expression}' with value {value}: {ex.Message}");
                if (raise_error)
                    throw ex;

                return value; // Return original value on error
            }
        }

    }
}
