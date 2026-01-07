using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HSMAdvisorDatabase;
using HSMAdvisorDatabase.ToolDataBase;

namespace ImportCsvTools
{
    /// <summary>
    /// Helper utilities for reflection-based discovery of Tool properties and Enum types
    /// </summary>
    internal static class ReflectionHelpers
    {
        /// <summary>
        /// Required fields that must be mapped for a valid tool import
        /// </summary>
        public static readonly HashSet<string> RequiredFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Tool_type_id",
            "Tool_material_id",
            "Coating_id",
            "Diameter"
        };

        /// <summary>
        /// Gets all writable public properties from the Tool class
        /// </summary>
        public static List<string> GetToolProperties()
        {
            return typeof(Tool)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Where(p => !p.Name.StartsWith("_"))
                .Select(p => p.Name)
                .OrderBy(n => n)
                .ToList();
        }

        /// <summary>
        /// Gets all nested enum types from the Enums class
        /// </summary>
        public static List<string> GetEnumTypes()
        {
            return typeof(Enums)
                .GetNestedTypes(BindingFlags.Public)
                .Where(t => t.IsEnum)
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToList();
        }

        /// <summary>
        /// Gets the property info for a Tool property by name
        /// </summary>
        public static PropertyInfo GetToolProperty(string propertyName)
        {
            return typeof(Tool).GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        }

        /// <summary>
        /// Checks if a Tool property represents an enum type (int that maps to an enum)
        /// </summary>
        public static bool IsEnumProperty(string propertyName, out string enumTypeName)
        {
            enumTypeName = null;

            // Common enum property patterns
            if (propertyName.EndsWith("_id", StringComparison.OrdinalIgnoreCase))
            {
                // Name_id -> ToolTypes
                // Tool_material_id -> ToolMaterials
                // Coating_id -> ToolCoatings
                var baseName = propertyName.Substring(0, propertyName.Length - 3);
                
                if (baseName.Equals("Tool_type", StringComparison.OrdinalIgnoreCase))
                {
                    enumTypeName = "ToolTypes";
                    return true;
                }
                else if (baseName.Equals("Tool_material", StringComparison.OrdinalIgnoreCase))
                {
                    enumTypeName = "ToolMaterials";
                    return true;
                }
                else if (baseName.Equals("Coating", StringComparison.OrdinalIgnoreCase))
                {
                    enumTypeName = "ToolCoatings";
                    return true;
                }
            }
            else if (propertyName.EndsWith("_mode", StringComparison.OrdinalIgnoreCase))
            {
                // Leadangle_mode -> ToolAngleModes
                enumTypeName = "ToolAngleModes";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all values from a specific enum type in the Enums class
        /// </summary>
        public static List<string> GetEnumValues(string enumTypeName)
        {
            if (string.IsNullOrWhiteSpace(enumTypeName))
                return new List<string>();

            var enumType = typeof(Enums).GetNestedType(enumTypeName, BindingFlags.Public);
            if (enumType == null || !enumType.IsEnum)
                return new List<string>();

            return Enum.GetNames(enumType).OrderBy(n => n).ToList();
        }

        /// <summary>
        /// Validates that all required fields are present in the mapping
        /// </summary>
        public static List<string> ValidateRequiredFields(List<CsvMapping> mappings, bool requireInputMapping = false)
        {
            var mappedFields = new HashSet<string>(
                mappings.Select(m => m.ToolField),
                StringComparer.OrdinalIgnoreCase);

            var reqs = RequiredFields.ToList();

            if (requireInputMapping)
            {
                reqs.Add("Input_units_m");
            }

            return reqs
                .Where(rf => !mappedFields.Contains(rf))
                .ToList();
        }

        /// <summary>
        /// Validates that a tool has all required fields populated with valid values
        /// </summary>
        public static bool ValidateToolHasRequiredFields(Tool tool)
        {
            if (tool == null)
                return false;

            // Check Tool_type_id (must be > 0)
            if (tool.Tool_type_id <= 0)
                return false;

            // Check Tool_material_id (must be > 0)
            if (tool.Tool_material_id <= 0)
                return false;

            // Check Coating_id (must be > 0)
            if (tool.Coating_id <= 0)
                return false;

            // Check Diameter (must be > 0)
            if (tool.Diameter <= 0)
                return false;

            return true;
        }
    }
}
