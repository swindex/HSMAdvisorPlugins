# ExchangeHSMWorks Plugin

A bidirectional converter plugin for importing and exporting HSMWorks (.hsmlib) tool library files to and from HSMAdvisor.

## Overview

The ExchangeHSMWorks plugin enables seamless data exchange between Autodesk HSMWorks (and Fusion 360) tool libraries and HSMAdvisor. It provides full round-trip conversion capability, preserving original HSMWorks data for perfect fidelity.

### Key Features

- **Import .hsmlib files** - Read HSMWorks tool libraries into HSMAdvisor
- **Export to .hsmlib** - Convert HSMAdvisor tools to HSMWorks format
- **Round-trip fidelity** - Preserves complete HSMWorks data for reversible conversion
- **Comprehensive type mapping** - Supports extensive tool type conversions
- **Unit handling** - Automatic metric/imperial unit conversion
- **Data preservation** - Original XML stored in Aux_data field

## Installation

### From Pre-built DLL

1. Build the solution in Release mode or download the release DLL
2. Copy `ExchangeHSMWorks.dll` to `%APPDATA%\HSMAdvisor\Plugins\`
3. Restart HSMAdvisor
4. Plugin capabilities will appear in the Tools menu

### Building from Source

1. Open the solution in Visual Studio
2. Ensure all references to HSMAdvisor DLLs are correct
3. Build in Release configuration
4. Output DLL will be in `bin/Release/`

## Usage

### Importing HSMWorks Tools

1. In HSMAdvisor, go to **Tools** → **Import HSMWorks Tool Database**
2. Select a `.hsmlib` file from HSMWorks or Fusion 360
3. Tools will be imported into HSMAdvisor database
4. Original HSMWorks data is preserved in each tool's Aux_data field

### Exporting to HSMWorks

1. In HSMAdvisor, select tools to export
2. Go to **Tools** → **Export HSMWorks Tool Database**
3. Choose output location and filename
4. If tools were originally imported from HSMWorks, original data is restored
5. New tools are converted with best-match settings

## Architecture

### Core Components

- **Converter.cs** - Main plugin class implementing ToolsPluginInterface
- **Schema.cs** - Auto-generated C# classes from HSMWorks XML schema
- **XMLSchema.xsd** - HSMWorks tool library schema definition

### Data Flow

```
Import: .hsmlib (XML) → Parse → HSMWorks Objects → Convert → HSMAdvisor Tools
Export: HSMAdvisor Tools → Convert → HSMWorks Objects → Serialize → .hsmlib (XML)
```

## Tool Type Mapping

The plugin provides comprehensive mapping between HSMWorks and HSMAdvisor tool types:

### Milling Tools

| HSMWorks Type | HSMAdvisor Type |
|---------------|-----------------|
| flat end mill | SolidEndMill |
| ball end mill | SolidBallMill |
| bull nose end mill | SolidBallMill |
| face mill | IndexedFaceMill |
| slot mill | SolidEndMill |
| chamfer mill | ChamferMill |
| thread mill | ThreadMill |

### Drilling Tools

| HSMWorks Type | HSMAdvisor Type |
|---------------|-----------------|
| drill | JobberTwistDrill |
| center drill | SpotDrill |
| spot drill | SpotDrill |
| countersink | CounterSink |
| counterbore | Counterbore |
| reamer | Reamer |

### Turning Tools

| HSMWorks Type | HSMAdvisor Type |
|---------------|-----------------|
| turning general | TurningProfiling |
| turning boring | BoringBar |
| turning grooving | TurningGrooving |

### Specialty Tools

| HSMWorks Type | HSMAdvisor Type |
|---------------|-----------------|
| tap right hand | Tap |
| tap left hand | Tap |
| form mill | HPEndMill |
| engraving tool | VbitEngraver |

## Advanced Features

### Data Preservation Pattern

The plugin stores complete HSMWorks XML data in the `Aux_data` field:

```csharp
// During import - preserve original HSMWorks data
ret.Aux_data = Serializer.ToXML(hsmWorksTool, "UTF-16");

// During export - restore original data if available
if (!string.IsNullOrEmpty(tool.Aux_data))
{
    var originalTool = Serializer.FromXML<tool>(tool.Aux_data);
    // Use original data for perfect round-trip
}
```

**Benefits:**
- Perfect conversion reversal
- Preserves HSMWorks-specific properties not supported by HSMAdvisor
- Enables future enhancements without data loss
- Maintains tool vendor metadata

### Unit Conversion

The plugin automatically handles unit conversions:

```csharp
// Determine source units from HSMWorks file
bool isMetric = (hsmTool.unit == "millimeters");

// Set all dimensional unit flags
tool.Diameter_m = isMetric;
tool.Flute_len_m = isMetric;
tool.Shank_Dia_m = isMetric;
tool.Depth_m = isMetric;
// ... etc for all dimensional properties
```

### Property Mapping Examples

**Threading Tools:**
```csharp
if (hsmTool.type == "thread mill")
{
    ret.Tool_type_id = Enums.ToolTypes.ThreadMill;
    ret.Thread_pitch = Parse.ToDouble(hsmTool.body.threadpitch);
    ret.Thread_pitch_m = isMetric;
}
```

**Turning Tools:**
```csharp
if (hsmTool.type == "turning general")
{
    ret.Tool_type_id = Enums.ToolTypes.TurningProfiling;
    ret.Leadangle = Parse.ToDouble(hsmTool.post_process.relief_angle);
}
```

**Ball End Mills:**
```csharp
if (hsmTool.type == "ball end mill")
{
    ret.Tool_type_id = Enums.ToolTypes.SolidBallMill;
    ret.Tip_Diameter = Parse.ToDouble(hsmTool.geometry.diameter);
    ret.Tip_Diameter_m = isMetric;
}
```

## HSMWorks File Format

### Structure

HSMWorks libraries use XML format with this basic structure:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<toollibrary>
  <tool guid="..." version="...">
    <description>Tool Name</description>
    <nc>
      <number>1</number>
      <diameter>6.35</diameter>
      <length>50</length>
    </nc>
    <geometry>
      <diameter>6.35</diameter>
      <shoulder-length>25</shoulder-length>
      <flute-length>20</flute-length>
    </geometry>
    <holder>
      <!-- Holder information -->
    </holder>
    <material>carbide</material>
    <type>flat end mill</type>
  </tool>
</toollibrary>
```

### Schema Generation

The `Schema.cs` file is auto-generated from `XMLSchema.xsd` using Visual Studio's xsd.exe tool:

```cmd
xsd.exe XMLSchema.xsd /classes /namespace:ExchangeHSMWorks
```

## Testing

### Unit Tests

The `ExchangeHSMWorks.Tests` project includes comprehensive tests:

```csharp
[TestMethod]
public void TestImportHSMWorksFile()
{
    var converter = new Converter();
    var result = converter.ImportTools();
    
    Assert.IsNotNull(result);
    Assert.IsTrue(result.Tools.Count > 0);
}
```

### Test Data

Sample .hsmlib files are included in `test-data/`:
- `Harvey Tool-End Mills.hsmlib`
- `Harvey Tool-Holemaking and Threading.hsmlib`
- `Harvey Tool-Specialty Profiles.hsmlib`

### Manual Testing

1. Use Plugin Test Runner to load ExchangeHSMWorks.dll
2. Execute ImportTools capability
3. Select a test .hsmlib file
4. Inspect returned DataBase object
5. Execute ExportTools with the imported data
6. Compare exported file with original

## Troubleshooting

### Import Issues

**Problem:** Tools not importing
- Verify .hsmlib file is valid XML
- Check HSMWorks format version compatibility
- Review Visual Studio debug output for parsing errors

**Problem:** Missing tool properties
- Some HSMWorks properties may not have HSMAdvisor equivalents
- Check if data is preserved in Aux_data field
- Review type mapping table for supported conversions

### Export Issues

**Problem:** Exported file not loading in HSMWorks
- Ensure XML encoding is UTF-8 or UTF-16
- Validate against XMLSchema.xsd
- Check required fields are populated

**Problem:** Tool properties changed after round-trip
- Verify Aux_data preservation is working
- Check unit conversion logic
- Review type mapping for specific tool type

## Extending the Plugin

### Adding New Tool Type Mappings

To support additional tool types:

1. Update the type mapping switch statement in `ToTool()`:
```csharp
case "new_hsmworks_type":
    ret.Tool_type_id = Enums.ToolTypes.NewHSMAdvisorType;
    // Map additional properties
    break;
```

2. Update the reverse mapping in `ExportTools()` if needed

3. Add test cases with sample data

### Supporting New HSMWorks Properties

1. Update `XMLSchema.xsd` if HSMWorks schema changes
2. Regenerate `Schema.cs` using xsd.exe
3. Map new properties in conversion methods
4. Update data preservation logic if needed

## References

- [HSMAdvisor Tool Field Reference](../TOOL_FIELD_REFERENCE.md) - Complete field documentation
- [HSMWorks Documentation](https://www.autodesk.com/products/hsmworks/) - HSMWorks product info
- [Root README](../README.md) - General plugin development guide

## License

This plugin is part of the HSMAdvisorPlugins project and follows the same MIT License.
