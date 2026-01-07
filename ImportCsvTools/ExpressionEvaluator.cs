using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;

internal static class ExpressionEvaluator
{
    private static readonly DataTable _dt;
    private static readonly DataRow _row;

    static ExpressionEvaluator()
    {
        _dt = new DataTable();

        // Base columns
        _dt.Columns.Add("v", typeof(object));
        _dt.Columns.Add("vl", typeof(string));
        _dt.Columns.Add("vu", typeof(string));

        _dt.Columns.Add("R", typeof(object));

        _row = _dt.NewRow();
        _dt.Rows.Add(_row);
    }
    /// <summary>
    /// Evaluates a DataColumn expression with a given value and optional parameters.
    /// Value in expression is accessible as 'v', lowercase as 'vl', uppercase as 'vu'.
    /// Numeric values are treated as doubles, others as strings.
    /// Null values are treated as empty strings.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="value"></param>
    /// <param name="parameters"></param>
    /// <param name="raise_error"></param>
    /// <returns></returns>
    public static object EvaluateExpression(
        string expression,
        string value,
        Dictionary<string, string> parameters = null,
        bool raise_error = false)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return value;

        try
        {
            _dt.Columns["R"].Expression = null;
            // Base value columns
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) { 
                _row["v"] = d;
                _row["vl"] = d;
                _row["vu"] = d;
            }
            else if (value is null)
            {
                _row["v"] = "";
                _row["vl"] = "";
                _row["vu"] = "";
            }
            else
            {
                _row["v"] = value;
                _row["vl"] = value.ToLowerInvariant();
                _row["vu"] = value.ToUpperInvariant();
            }

            // Parameter columns (string only)
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    var colName = "row_" + kv.Key;
                    //strip all special characters from colName except underscore
                    colName = System.Text.RegularExpressions.Regex.Replace(colName, @"[^a-zA-Z0-9_]", "_");

                    if (!_dt.Columns.Contains(colName))
                        _dt.Columns.Add(colName, typeof(object));

                    if (double.TryParse(kv.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dv))
                    {
                        _row[colName] = dv;
                    }
                    else if (kv.Value is null)
                    {
                        _row[colName] = "";
                    }
                    else
                    {
                        _row[colName] = kv.Value;
                    }
                }
            }

            _dt.Columns["R"].Expression = expression;

            if (_row["R"] == DBNull.Value)
                return "";

            //check if infinite or NaN
            if (_row["R"] is double dr)
            {
                if (double.IsInfinity(dr) || double.IsNaN(dr))
                {
                    throw new EvaluateException("Result is not a valid number.");
                }
            }

            return _row["R"];
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Expression evaluation failed for '{expression}' with value '{value}': {ex.Message}");

            if (raise_error) throw ex;
            return value;
        }
    }
}
