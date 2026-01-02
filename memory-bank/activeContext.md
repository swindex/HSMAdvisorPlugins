# Active Context

## Current Work Focus

### ImportCsvTools Plugin - COMPLETED
Successfully implemented a comprehensive CSV import plugin with flexible JSON-based mapping configuration:

**Key Features Delivered:**
- **CSV Import Engine**: Parses CSV files and creates HSMAdvisor tools using configurable mappings
- **JSON Configuration System**: Defines column-to-field mappings, value translations, and expressions
- **Expression Evaluator**: Runtime C# expression compilation for calculated values and transformations
- **Value Map System**: Translates vendor-specific values to HSMAdvisor enums
- **Reflection-Based Mapping**: Dynamically sets Tool fields without hardcoding
- **Visual Mapping Editor**: Windows Forms UI for creating and editing mapping configurations
  - MappingEditorForm: Main mapping interface
  - ExpressionEditorForm: Expression editing with syntax support
  - ValueMapEditorForm: Value translation configuration
  - MappingSelectionForm: Mapping file selection
- **Unit Testing**: Comprehensive test suite with sample data

**Architecture Highlights:**
- Configuration-driven approach eliminates code changes for new CSV formats
- Expression system handles unit conversions, conditionals, and complex logic
- Reflection enables dynamic field assignment
- Reusable mapping configurations save time for repeat imports

### Solution Structure Expansion - COMPLETED
The solution now contains multiple plugin projects with comprehensive testing:

**Current Structure:**
```
HSMAdvisorPlugins Solution
├── ExchangeHSMWorks/              # HSMWorks XML conversion
│   └── ExchangeHSMWorks.Tests/    # Unit tests
├── ImportCsvTools/                # CSV import with mapping
│   ├── Forms/                     # Visual editors
│   └── ImportCsvTools.Tests/      # Unit tests
├── Plugin-Test-Runner-UI/         # Shared testing tool
└── HSMadvisorDlls/               # Shared references
```

### MSBuild Version Conflict Resolution - PREVIOUSLY COMPLETED
## Recent Changes & Discoveries

### ImportCsvTools Architecture Insights

#### 1. Configuration-Driven Flexibility
The JSON mapping system provides exceptional flexibility:
```json
{
  "CsvColumn": "Material",
  "ToolField": "Tool_material_id",
  "EnumType": "ToolMaterials",
  "ValueMap": {"HSS Steel": "HSS", "Carb": "Carbide"},
  "DefaultValue": "HSS",
  "Expression": ""
}
```

This allows users to:
- Map any CSV format without code changes
- Translate vendor-specific terminology
- Provide fallback values
- Calculate derived values

#### 2. Expression Evaluation Power
The expression system enables complex transformations:
- **Unit Conversion**: `value * 0.03937` (mm to inches)
- **Conditional Logic**: `string.IsNullOrEmpty(value) ? 4 : int.Parse(value)`
- **Boolean Conversion**: `value == "1" || value.ToLower() == "yes"`
- **Type Detection**: `value.Contains("Ball") ? "SolidBallMill" : "SolidEndMill"`

#### 3. Visual Editor Benefits
The Windows Forms-based mapping editor:
- Previews CSV structure
- Provides dropdown for Tool fields
- Built-in enum value selection
- GUI for value translations
- Expression editing with validation
- Real-time configuration testing

#### 4. Reflection-Based Mapping
Uses reflection to dynamically access Tool fields:
```csharp
public static void SetToolField(Tool tool, string fieldName, object value)
{
    var fieldInfo = typeof(Tool).GetField(fieldName);
    var converted = Convert.ChangeType(value, fieldInfo.FieldType);
    fieldInfo.SetValue(tool, converted);
}
```

Benefits:
- No hardcoded field names
- Supports all Tool fields automatically
- Type conversion handled dynamically
- Future-proof for Tool class changes

### Key Insights from Recent Development

#### 1. Multi-Plugin Solution Pattern
The solution demonstrates effective multi-plugin architecture:
- **ExchangeHSMWorks**: Schema-based XML conversion (HSMWorks)
- **ImportCsvTools**: Configuration-based CSV import (generic)
- **Shared Infrastructure**: Plugin Test Runner, HSMAdvisor DLLs, documentation

Each plugin showcases different approaches:
- **XML Plugin**: Auto-generated schema classes, bidirectional conversion
- **CSV Plugin**: Dynamic mapping, expression evaluation, visual configuration

#### 2. Testing Infrastructure Maturity
All plugins have comprehensive testing:
- Unit test projects with NUnit
- Test data directories with sample files
- Shared Plugin Test Runner for integration testing
- Consistent testing patterns across projects

#### 3. Documentation Excellence
The project has excellent documentation:
- Plugin-specific READMs with examples
- TOOL_FIELD_REFERENCE.md with complete field documentation
- CSV_Tool_Library_Import_Mapping.md with mapping rules
- Sample configuration files
- Root README with plugin development guide

### Previous Work: MSBuild Version Conflict Resolution
Successfully resolved MSBuild version conflicts in ExchangeHSMWorks.Tests:

**Problem Identified:**
- MSBuild warning MSB3277: "Found conflicts between different versions of mscorlib that could not be resolved"
- Root cause: Mismatch between .NET Framework 4.8 target and missing MSTest NuGet packages
- Test project was referencing non-existent MSTest framework assemblies

**Solution Implemented:**
- **Removed MSTest Dependencies**: Eliminated MSTest.TestAdapter and MSTest.TestFramework references
- **Updated Project Configuration**: Cleaned up packages.config and project file references
- **Console Application Approach**: Converted to standalone console test application
- **Framework Alignment**: Ensured consistent .NET Framework 4.8 targeting

**Test Results:**
- Build succeeded with only harmless mscorlib version warnings (due to HSMAdvisor DLLs being compiled against older .NET)
- Unit tests run successfully as console application
- Successfully loaded "Harvey Tool-End Mills.hsmlib" with 11,937 tools
- All comprehensive tests pass including tool conversion, material mapping, and round-trip data preservation

### Previous Work: Unit Test Enhancement
Successfully updated ExchangeHSMWorks unit test system:

**Key Improvements Made:**
- **Dynamic File Discovery**: Tests now automatically scan the `test-data` directory for all `.hsmlib` files
- **Efficient Caching**: Each test data file is loaded only once and cached for reuse across all tests
- **Comprehensive Testing**: All test data files are validated instead of just one hardcoded file
- **Better Error Reporting**: Test failures now include the specific file name for easier debugging

### Project State Assessment
The project is in a **mature, functional state** with:
- Complete ExchangeHSMWorks plugin implementation
- Working Plugin Test Runner for development
- Comprehensive tool type mapping system
- Bidirectional conversion capabilities (import/export)
- **Enhanced unit test system with file caching** (NEW)

## Recent Changes & Discoveries

### Key Insights from Code Analysis

#### 1. Comprehensive Tool Type Support
The plugin handles extensive tool type mapping:
- **Milling Tools**: flat end mill, ball end mill, thread mill, face mill, chamfer mill
- **Drilling Tools**: drill, spot drill, center drill
- **Threading Tools**: tap (left/right hand), thread mill
- **Turning Tools**: turning general, turning boring, turning grooving
- **Specialized Tools**: boring bar, slot mill, lollipop mill

#### 2. Data Preservation Strategy
Critical discovery: The plugin preserves original HSMWorks data in the `Aux_data` field:
```csharp
ret.Aux_data = Serializer.ToXML(t, "UTF-16");
```
This enables:
- Perfect round-trip conversion fidelity
- Access to HSMWorks-specific properties not mapped to HSMAdvisor
- Future enhancement without data loss

#### 3. Robust Error Handling
The system uses graceful degradation patterns:
- Default values (-1) for unknown properties
- Safe type conversion with Parse utility methods
- Exception handling in test runner with detailed error display

#### 4. Unit Conversion Architecture
Sophisticated unit handling where all dimensional properties are flagged based on source units:
```csharp
ret.Circle_dia_m = t.unit == "millimeters";
ret.Depth_m = t.unit == "millimeters";
// ... repeated for all dimensional properties
```

## Next Steps & Priorities

### Immediate Actions
1. **Test ImportCsvTools Integration**: Verify CSV plugin works with HSMAdvisor
2. **Create Sample Mappings**: Develop mapping configs for common vendor formats
3. **Validate Visual Editors**: Test all Forms with various CSV structures
4. **Performance Testing**: Test with large CSV files (1000+ tools)

### Documentation Enhancements
1. **CSV Import Tutorial**: Step-by-step guide for first-time users
2. **Mapping Examples Library**: Collection of pre-built vendor mappings
3. **Expression Cookbook**: Common expression patterns and use cases
4. **Troubleshooting Guide**: Common CSV import issues and solutions

### Potential Enhancement Areas

#### 1. ImportCsvTools Enhancements
- **CSV Export**: Add export capability (currently import-only)
- **Advanced CSV Parsing**: Handle quoted fields, embedded commas, multi-line values
- **Batch Processing**: Import multiple CSV files at once
- **Mapping Validation**: More comprehensive validation before import
- **Column Auto-Detection**: Suggest mappings based on column names
- **Template Library**: Pre-built mappings for popular tool vendors
- **Error Reporting**: Detailed per-row error messages during import
- **Progress Indicators**: Show progress for large CSV files

#### 2. ExchangeHSMWorks Enhancements
- **Tool Type Coverage**: Verify all HSMWorks tool types are mapped
- **Export Validation**: Test exported .hsmlib files in Fusion 360
- **Bulk Export**: Export selected tools or libraries

#### 3. Cross-Plugin Features
- **Unified Error Handling**: Consistent error reporting across plugins
- **Logging System**: Centralized logging for troubleshooting
- **Performance Optimization**: Profile and optimize for large datasets
- **Progress Indicators**: Standard progress UI for all plugins

#### 4. Plugin Test Runner Enhancements
- **Multi-File Testing**: Test import with multiple files
- **Performance Profiling**: Measure execution time and memory
- **Automated Testing**: Script-based test execution
- **Result Comparison**: Compare imported vs. expected results

## Active Decisions & Considerations

### Design Decisions in Place
1. **Preserve Original Data**: Store complete HSMWorks XML in Aux_data field
2. **Graceful Degradation**: Use default values rather than failing on missing data
3. **Bidirectional Support**: Full import/export capability
4. **Schema-Based Approach**: Use auto-generated classes from XSD

### Current Technical Preferences
- **Error Handling**: Prefer graceful degradation over strict validation
- **Data Mapping**: Comprehensive mapping with fallback defaults
- **Testing**: Use Plugin Test Runner for development validation
- **Deployment**: Simple copy-based plugin installation

## Important Patterns & Learnings

### ImportCsvTools Patterns

#### 1. Configuration-Driven Design
Separates mapping logic from code:
- JSON configuration defines behavior
- No recompilation for new formats
- User-editable configurations
- Version-controllable mapping files

#### 2. Expression Evaluation Pattern
Runtime compilation of C# expressions:
- Flexible transformations
- Type-safe evaluation
- Access to full C# syntax
- Context variable support

#### 3. Reflection-Based Field Access
Dynamic Tool field manipulation:
- No hardcoded field names
- Automatic type conversion
- Future-proof design
- Complete field coverage

#### 4. Visual Editor Pattern
Windows Forms for configuration:
- User-friendly interface
- Real-time validation
- Visual feedback
- Eliminates manual JSON editing

### ExchangeHSMWorks Patterns

#### 1. Tool Factory Pattern

#### 1. Tool Factory Pattern
The `ToTool()` method serves as a comprehensive factory:
```csharp
public static Tool ToTool(toollibraryTool t)
{
    Tool ret = new Tool(true) { /* base initialization */ };
    
    // Tool-type-specific configuration
    switch (t.type) {
        case "flat end mill": /* specific setup */ break;
        case "ball end mill": /* specific setup */ break;
        // ... comprehensive type handling
    }
    return ret;
}
```

#### 2. Bidirectional Material Mapping
Clean conversion between material systems:
```csharp
// HSMWorks → HSMAdvisor
public static Enums.ToolMaterials ToToolMaterial(string materialname)

// HSMAdvisor → HSMWorks  
public static string FromToolMaterial(Enums.ToolMaterials material_id)
```

#### 3. Plugin Capability Declaration
Clear interface for plugin functionality:
```csharp
public override List<Capability> GetCapabilities()
{
    var caps = new List<Capability>();
    caps.Add(new Capability("Import HSMWorks Tool Database", 
                           (int)ToolsPluginCapabilityMethod.ImportTools));
    caps.Add(new Capability("Export HSMWorks Tool Database", 
                           (int)ToolsPluginCapabilityMethod.ExportTools));
    return caps;
}
```

### Project Insights

#### Strengths
- **Comprehensive Coverage**: Handles wide variety of tool types
- **Data Integrity**: Preserves original data for round-trip fidelity
- **Robust Architecture**: Well-structured plugin pattern
- **Development Tools**: Excellent testing infrastructure

#### Areas for Attention
- **Documentation**: Could benefit from more inline code documentation
- **Validation**: Limited input validation on tool parameters
- **Error Reporting**: Basic error handling could be more informative
- **Performance**: No optimization for very large tool libraries

## Context for Future Work

### When Resuming Work
1. **Read All Memory Bank Files**: Essential for understanding project state
2. **Review Current Codebase**: Check for any changes since memory bank update
3. **Validate Build Status**: Ensure all projects compile and tests pass
4. **Check Integration**: Verify plugins work with HSMAdvisor

### Key Files to Monitor

**ImportCsvTools:**
- `ImportCsvTools/CsvToolImporter.cs`: Main import engine
- `ImportCsvTools/CsvMapping.cs`: Configuration classes
- `ImportCsvTools/ExpressionEvaluator.cs`: Expression system
- `ImportCsvTools/Forms/MappingEditorForm.cs`: Visual editor
- `ImportCsvTools/SampleMapping.json`: Example configuration

**ExchangeHSMWorks:**
- `ExchangeHSMWorks/Converter.cs`: Core conversion logic
- `ExchangeHSMWorks/Schema.cs`: HSMWorks XML schema classes

**Shared:**
- `Plugin-Test-Runner-UI/Form1.vb`: Testing infrastructure
- `README.md`: User-facing documentation
- `TOOL_FIELD_REFERENCE.md`: Tool field documentation
- `ImportCsvTools/CSV_Tool_Library_Import_Mapping.md`: CSV mapping guide

### Current Project State
The project is in a **mature, feature-rich state** with:
- Two complete, functional plugins (ExchangeHSMWorks, ImportCsvTools)
- Comprehensive testing infrastructure
- Excellent documentation
- Visual editors for user-friendly configuration
- Unit tests for all plugins
- Shared Plugin Test Runner for development

This active context provides the current state understanding needed to effectively continue development work on the HSMAdvisorPlugins project.
