# ImportCsvTools Plugin

A flexible CSV import plugin for HSMAdvisor that allows importing tool libraries from CSV files using customizable JSON mapping configurations.

## Overview

The ImportCsvTools plugin enables importing tool data from CSV files into HSMAdvisor. It uses a JSON-based mapping system that provides flexibility in handling various CSV formats without code changes.

### Key Features

- **Flexible CSV Mapping** - JSON configuration defines column-to-field mappings
- **Expression Evaluation** - Support for calculated values and transformations
- **Value Mapping** - Translate external values to HSMAdvisor enums
- **Default Values** - Fallback values for missing CSV columns
- **Unit Handling** - Automatic metric/imperial unit conversion
- **Visual Mapping Editor** - GUI for creating and editing mappings
- **Validation** - Configurable import validation rules

## Installation

### From Pre-built DLL

1. Build the solution in Release mode or download the release DLL
2. Copy `ImportCsvTools.dll` to `%APPDATA%\HSMAdvisor\Plugins\`
3. Restart HSMAdvisor
4. Plugin capabilities will appear in the Tools menu

### Building from Source

1. Open the solution in Visual Studio
2. Ensure all references to HSMAdvisor DLLs are correct
3. Build in Release configuration
4. Output DLL will be in `bin/Release/`

## Usage

### Quick Start

1. In HSMAdvisor, go to **Tools** → **Import CSV Tools**
2. Select your CSV file
3. Choose or create a mapping configuration (JSON file)
4. Review and confirm the import
5. Tools will be added to your HSMAdvisor database

### Creating a Mapping File

The plugin includes a visual mapping editor, or you can create JSON mapping files manually.

## CSV Mapping Configuration

### Configuration File Structure

A mapping configuration is a JSON file that defines how CSV columns map to HSMAdvisor tool fields:

```json
{
  "LibraryName": "My Tool Library",
  "AllowInvalidToolImport": false,
  "CsvInputUnits": "in",
  "Mappings": [
    {
      "CsvColumn": "Tool Diameter",
      "ToolField": "Diameter",
      "ValueMap": {},
      "DefaultValue": "",
      "EnumType": "",
      "Expression": ""
    }
  ]
}
```

### Configuration Properties

| Property | Type | Description |
|----------|------|-------------|
| **LibraryName** | string | Name of the tool library to create in HSMAdvisor |
| **AllowInvalidToolImport** | bool | Whether to import tools that fail validation (default: false) |
| **CsvInputUnits** | string | Default units for dimensional values: "in" (inches) or "mm" (millimeters) |
| **Mappings** | array | Array of column mapping definitions |

### Mapping Properties

Each mapping in the `Mappings` array defines how one CSV column maps to a tool field:

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| **CsvColumn** | string | Yes | Name of the CSV column header |
| **ToolField** | string | Yes | Name of the HSMAdvisor Tool field (see [Tool Field Reference](../TOOL_FIELD_REFERENCE.md)) |
| **ValueMap** | object | No | Dictionary for translating CSV values to HSMAdvisor values |
| **DefaultValue** | string | No | Fallback value if CSV column is missing or empty |
| **EnumType** | string | No | Enum type name for validation (e.g., "ToolTypes", "ToolMaterials") |
| **Expression** | string | No | C# expression for calculated values |

## Required Field Mappings

Every mapping configuration **MUST** include mappings for these required fields:

```json
{
  "Mappings": [
    {
      "CsvColumn": "Tool Type",
      "ToolField": "Tool_type_id",
      "EnumType": "ToolTypes"
    },
    {
      "CsvColumn": "Material",
      "ToolField": "Tool_material_id",
      "EnumType": "ToolMaterials"
    },
    {
      "CsvColumn": "Coating",
      "ToolField": "Coating_id",
      "EnumType": "ToolCoatings"
    },
    {
      "CsvColumn": "Diameter",
      "ToolField": "Diameter"
    }
  ]
}
```

See [Tool Field Reference](../TOOL_FIELD_REFERENCE.md) for complete field documentation.

## Mapping Examples

### Basic Field Mapping

Direct column-to-field mapping:

```json
{
  "CsvColumn": "Flute Length",
  "ToolField": "Flute_Len"
}
```

### Enum Mapping with Value Translation

Map CSV values to HSMAdvisor enums:

```json
{
  "CsvColumn": "Material",
  "ToolField": "Tool_material_id",
  "EnumType": "ToolMaterials",
  "ValueMap": {
    "HSS Steel": "HSS",
    "Cobalt": "HSCobalt",
    "Carb": "Carbide",
    "Solid Carbide": "Carbide"
  }
}
```

### Tool Type Mapping

```json
{
  "CsvColumn": "Tool Type",
  "ToolField": "Tool_type_id",
  "EnumType": "ToolTypes",
  "ValueMap": {
    "End Mill": "SolidEndMill",
    "Ball End Mill": "SolidBallMill",
    "Twist Drill": "JobberTwistDrill",
    "Center Drill": "SpotDrill"
  }
}
```

### Default Values

Provide fallback values for missing data:

```json
{
  "CsvColumn": "Coating",
  "ToolField": "Coating_id",
  "EnumType": "ToolCoatings",
  "DefaultValue": "None"
}
```

### Expression-Based Mapping

Calculate values using C# expressions:

```json
{
  "CsvColumn": "Diameter_MM",
  "ToolField": "Diameter",
  "Expression": "value * 0.03937"
}
```

The `value` variable contains the CSV column value. You can use standard C# math operators and functions.

### Unit Flag Inference

Infer metric/imperial from CSV data:

```json
{
  "CsvColumn": "Units",
  "ToolField": "Input_units_m",
  "Expression": "value.ToLower() == \"mm\" || value.ToLower() == \"metric\""
}
```

### Conditional Mapping

Set values based on conditions:

```json
{
  "CsvColumn": "Flutes",
  "ToolField": "Flute_N",
  "Expression": "string.IsNullOrEmpty(value) ? 4 : int.Parse(value)"
}
```

## Complete Example

### Sample CSV File

```csv
Tool Type,Material,Coating,Diameter,Flutes,Flute Length,Overall Length,Shank Diameter
End Mill,Carbide,AlTiN,0.250,4,0.75,2.5,0.250
Ball End Mill,Carbide,TiAlN,0.375,4,1.0,3.0,0.375
Drill,HSS Steel,TiN,0.125,2,1.5,3.0,0.125
```

### Sample Mapping Configuration

```json
{
  "LibraryName": "Imported CSV Tools",
  "AllowInvalidToolImport": false,
  "CsvInputUnits": "in",
  "Mappings": [
    {
      "CsvColumn": "Tool Type",
      "ToolField": "Tool_type_id",
      "EnumType": "ToolTypes",
      "ValueMap": {
        "End Mill": "SolidEndMill",
        "Ball End Mill": "SolidBallMill",
        "Drill": "JobberTwistDrill"
      }
    },
    {
      "CsvColumn": "Material",
      "ToolField": "Tool_material_id",
      "EnumType": "ToolMaterials",
      "ValueMap": {
        "HSS Steel": "HSS",
        "Carbide": "Carbide"
      }
    },
    {
      "CsvColumn": "Coating",
      "ToolField": "Coating_id",
      "EnumType": "ToolCoatings"
    },
    {
      "CsvColumn": "Diameter",
      "ToolField": "Diameter"
    },
    {
      "CsvColumn": "Flutes",
      "ToolField": "Flute_N"
    },
    {
      "CsvColumn": "Flute Length",
      "ToolField": "Flute_Len"
    },
    {
      "CsvColumn": "Overall Length",
      "ToolField": "Depth"
    },
    {
      "CsvColumn": "Shank Diameter",
      "ToolField": "Shank_Dia"
    }
  ]
}
```

## Visual Mapping Editor

The plugin includes a Windows Forms-based mapping editor accessible through the plugin UI.

### Features

- **Column Preview** - View CSV columns before mapping
- **Field Dropdown** - Select from available Tool fields
- **Enum Editor** - Built-in enum value selection
- **Value Map Editor** - GUI for creating value translations
- **Expression Editor** - Syntax-highlighted expression editing
- **Validation** - Real-time mapping validation
- **Save/Load** - Manage multiple mapping configurations

### Using the Editor

1. Import CSV with the plugin
2. When prompted, click "Create New Mapping"
3. Use the editor to define mappings
4. Test with sample rows
5. Save configuration for reuse

## Advanced Features

### Handling Multiple CSV Formats

Create multiple mapping configurations for different CSV formats:

```
MyMappings/
├── vendor-a-mapping.json
├── vendor-b-mapping.json
└── legacy-format-mapping.json
```

### Dynamic Library Assignment

Use expressions to assign tools to different libraries based on CSV data:

```json
{
  "CsvColumn": "Vendor",
  "ToolField": "Library",
  "Expression": "\"Tools - \" + value"
}
```

### Boolean Value Handling

CSV boolean values can be handled flexibly:

```json
{
  "CsvColumn": "HSM Capable",
  "ToolField": "Hsm",
  "ValueMap": {
    "Yes": "true",
    "Y": "true",
    "1": "true",
    "No": "false",
    "N": "false",
    "0": "false"
  }
}
```

Or use expressions:

```json
{
  "CsvColumn": "HSM",
  "ToolField": "Hsm",
  "Expression": "value == \"1\" || value.ToLower() == \"yes\" || value.ToLower() == \"true\""
}
```

### Complex Type Mapping

Handle complex tool type logic:

```json
{
  "CsvColumn": "Type",
  "ToolField": "Tool_type_id",
  "EnumType": "ToolTypes",
  "Expression": "value.Contains(\"Ball\") ? \"SolidBallMill\" : (value.Contains(\"End Mill\") ? \"SolidEndMill\" : \"Unknown\")"
}
```

## Testing

### Unit Tests

The `ImportCsvTools.Tests` project includes tests:

```csharp
[TestMethod]
public void TestCsvImport()
{
    var importer = new CsvToolImporter();
    var mapping = LoadMapping("test-mapping.json");
    var result = importer.Import("test-tools.csv", mapping);
    
    Assert.IsTrue(result.Tools.Count > 0);
}
```

### Test Data

Sample files in `test-data/`:
- `sample-tools.csv` - Example CSV file
- `sample-mapping.json` - Example mapping configuration

### Manual Testing with Plugin Test Runner

1. Build the plugin in Debug mode
2. Run Plugin-Test-Runner-UI
3. Load ImportCsvTools.dll
4. Test ImportTools capability
5. Inspect imported tools in PropertyGrid

## Troubleshooting

### Import Issues

**Problem:** Required field validation errors
- Ensure Tool_type_id, Tool_material_id, Coating_id, and Diameter are mapped
- Check that CSV contains data for these columns
- Verify enum value mappings are correct

**Problem:** Enum values not matching
- Use ValueMap to translate CSV values to HSMAdvisor enum names
- Check [Tool Field Reference](../TOOL_FIELD_REFERENCE.md) for valid enum values
- Values are case-sensitive

**Problem:** Expression evaluation errors
- Verify C# expression syntax
- Ensure `value` variable is used correctly
- Check for null/empty value handling
- Review Visual Studio debug output for error details

### Mapping Issues

**Problem:** CSV columns not found
- Verify CSV column headers match CsvColumn values exactly (case-sensitive)
- Check for extra spaces in column headers
- Ensure CSV uses standard format (comma-separated)

**Problem:** Unit conversion not working
- Set CsvInputUnits to "in" or "mm" in configuration
- Or map individual unit flags (e.g., Diameter_m) per tool
- Ensure dimensional values are numeric

## Architecture

### Core Components

- **CsvToolImporter.cs** - Main import engine
- **CsvMapping.cs** - Mapping configuration classes
- **ExpressionEvaluator.cs** - Dynamic expression evaluation
- **CsvImportColumnInfo.cs** - CSV column metadata
- **ReflectionHelpers.cs** - Tool field reflection utilities
- **Forms/** - Visual mapping editor UI components

### Data Flow

```
CSV File → Parse → Apply Mappings → Evaluate Expressions → Create Tools → Validate → Import
```

## Enum Reference

For complete enum values and tool field documentation, see:

📖 [HSMAdvisor Tool Field Reference](../TOOL_FIELD_REFERENCE.md)

### Quick Reference: Common Enums

**ToolTypes:** Unknown, SolidEndMill, SolidBallMill, JobberTwistDrill, SpotDrill, Reamer, ThreadMill, Tap, etc.

**ToolMaterials:** Unknown, HSS, HSCobalt, Carbide, PCD, Ceramic

**ToolCoatings:** Unknown, None, TiN, TiCN, TiAlN, AlTiN, AlCrN, ZrN, nACRo, nACo

**ToolAngleModes:** Lead, Taper, Tip

## References

- [HSMAdvisor Tool Field Reference](../TOOL_FIELD_REFERENCE.md) - Complete field and enum documentation
- [Root README](../README.md) - General plugin development guide
- [Sample Mapping File](SampleMapping.json) - Example configuration

## License

This plugin is part of the HSMAdvisorPlugins project and follows the same MIT License.
