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
            // Base value columns
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)) { 
                _row["v"] = d;
                _row["vl"] = d;
                _row["vu"] = d;
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
                        _dt.Columns.Add(colName, typeof(string));

                    _row[colName] = kv.Value ?? "";
                }
            }

            _dt.Columns["R"].Expression = expression;
            return _row["R"];
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Expression evaluation failed for '{expression}' with value '{value}': {ex.Message}");

            if (raise_error) throw;
            return value;
        }
    }
}
