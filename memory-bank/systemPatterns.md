# System Patterns & Architecture

## Overall Architecture

### Plugin Architecture Pattern
The system follows a **Plugin Architecture** where HSMAdvisor acts as the host application and plugins extend functionality through well-defined interfaces.

```
HSMAdvisor Core Application
├── Plugin Manager
│   ├── Plugin Discovery (%APPDATA%\HSMAdvisor\Plugins)
│   ├── Plugin Loading (Reflection-based)
│   └── Interface Validation
├── Database Engine
│   ├── Tool Database Management
│   ├── Library Organization
│   └── Data Persistence
└── User Interface
    ├── Import/Export Dialogs
    ├── Plugin Capability Display
    └── Tool Management UI
```

### Key Design Patterns

#### 1. Strategy Pattern - Plugin Capabilities
Each plugin implements `ToolsPluginInterface` and declares its capabilities:
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

#### 2. Factory Pattern - Tool Creation
The `ToTool()` method acts as a factory, creating HSMAdvisor `Tool` objects from HSMWorks `toollibraryTool` objects:
```csharp
public static Tool ToTool(toollibraryTool t)
{
    Tool ret = new Tool(true) { /* initialization */ };
    // Complex mapping logic based on tool type
    switch (t.type) {
        case "flat end mill": /* specific configuration */ break;
        case "ball end mill": /* specific configuration */ break;
        // ... other tool types
    }
    return ret;
}
```

#### 3. Adapter Pattern - Format Conversion
The plugin acts as an adapter between HSMWorks XML format and HSMAdvisor's internal format:
- **HSMWorks Format**: XML-based with specific schema
- **HSMAdvisor Format**: .NET objects with different property names and structures
- **Adapter**: `Converter` class handles bidirectional translation

## Component Relationships

### Core Components

#### 1. ExchangeHSMWorks Plugin
- **Primary Class**: `Converter : ToolsPluginInterface`
- **Responsibilities**:
  - XML serialization/deserialization
  - Tool type mapping
  - Unit conversion
  - Material mapping
  - File dialog management

#### 2. ImportCsvTools Plugin
- **Primary Class**: `CsvToolImporter : ToolsPluginInterface`
- **Responsibilities**:
  - CSV parsing and import
  - JSON-based mapping configuration
  - Expression evaluation for calculated values
  - Value translation (ValueMap)
  - Visual mapping editor
- **Key Components**:
  - `CsvToolImporter`: Main import engine
  - `CsvMapping`: Configuration classes
  - `ExpressionEvaluator`: Dynamic value calculation
  - `ReflectionHelpers`: Tool field introspection
  - Visual Forms: Mapping editor UI

#### 3. Schema Classes (ExchangeHSMWorks)
- **Generated from XSD**: Auto-generated classes for HSMWorks XML schema
- **Key Classes**:
  - `toollibrary`: Root container
  - `toollibraryTool`: Individual tool definition
  - `toollibraryToolBody`: Tool geometry
  - `toollibraryToolMaterial`: Material properties
  - `toollibraryToolHolder`: Holder information

#### 4. Plugin Test Runner
- **Purpose**: Development and testing tool
- **Architecture**: Windows Forms application
- **Key Features**:
  - Plugin discovery and loading
  - Capability enumeration
  - Method execution testing
  - Property grid for result inspection

### Data Flow Patterns

#### Import Flow
```
.hsmlib File (XML)
↓ [File.ReadAllText()]
XML String
↓ [Serializer.FromXML<toollibrary>()]
HSMWorks Objects
↓ [ToTool() mapping]
HSMAdvisor Tool Objects
↓ [DataBase.AddTool()]
HSMAdvisor Database
```

#### Export Flow
```
HSMAdvisor Database
↓ [Tool enumeration]
HSMAdvisor Tool Objects
↓ [FromTool() mapping]
HSMWorks Objects
↓ [Serializer.ToXML()]
XML String
↓ [File.WriteAllText()]
.hsmlib File (XML)
```

## Critical Implementation Patterns

### 1. Tool Type Mapping Strategy
Complex mapping between different tool classification systems:

**HSMWorks Types** → **HSMAdvisor Types**
- `"flat end mill"` → `Enums.ToolTypes.SolidEndMill`
- `"ball end mill"` → `Enums.ToolTypes.SolidBallMill`
- `"thread mill"` → `Enums.ToolTypes.ThreadMill`
- `"drill"` → `Enums.ToolTypes.JobberTwistDrill`
- `"turning general"` → `Enums.ToolTypes.TurningProfiling`

### 2. Unit Conversion Pattern
Consistent unit handling across the conversion:
```csharp
// All unit flags set based on source unit
ret.Circle_dia_m = t.unit == "millimeters";
ret.Depth_m = t.unit == "millimeters";
ret.Diameter_m = t.unit == "millimeters";
// ... (repeated for all dimensional properties)
```

### 3. Data Preservation Pattern
Original HSMWorks data preserved in `Aux_data` field:
```csharp
ret.Aux_data = Serializer.ToXML(t, "UTF-16");
```
This enables:
- Round-trip conversion fidelity
- Access to HSMWorks-specific properties
- Future enhancement without data loss

### 4. Error Handling Pattern
Graceful degradation with default values:
```csharp
ret.Maxdeflection_pc = -1;  // Default for unknown values
ret.Maxtorque_pc = -1;
ret.Productivity = -1;
```

### 5. Material Mapping Pattern
Bidirectional material type conversion:
```csharp
public static Enums.ToolMaterials ToToolMaterial(string materialname)
{
    switch (materialname)
    {
        case "carbide": return Enums.ToolMaterials.Carbide;
        case "ceramics": return Enums.ToolMaterials.Ceramic;
        case "cobalt": return Enums.ToolMaterials.HSCobalt;
        case "hss":
        default: return Enums.ToolMaterials.HSS;
    }
}
```

## Extensibility Patterns

### Plugin Interface Contract
```csharp
public abstract class ToolsPluginInterface : HSMAdvisorPluginInterface
{
    public abstract List<Capability> GetCapabilities();
    public abstract DataBase ImportTools();
    public abstract void ExportTools(DataBase db);
    public abstract void ModifyTools(DataBase db);
}
```

### Capability Declaration Pattern
Plugins declare what they can do, UI adapts accordingly:
```csharp
var caps = new List<Capability>();
caps.Add(new Capability("Human-readable name", MethodEnum));
```

### File Filter Pattern
Consistent file dialog behavior:
```csharp
public string GetReadFileFilter()
{
    return "HSMWorks Tool Database Files (*.hsmlib)|*.hsmlib|XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
}
```

## ImportCsvTools Architecture Patterns

### Configuration-Driven Mapping Pattern
The CSV import uses a flexible JSON configuration that defines mappings without code changes:

```json
{
  "LibraryName": "My Tools",
  "CsvInputUnits": "in",
  "Mappings": [
    {
      "CsvColumn": "Tool Type",
      "ToolField": "Tool_type_id",
      "EnumType": "ToolTypes",
      "ValueMap": {"End Mill": "SolidEndMill"}
    }
  ]
}
```

### Expression Evaluation Pattern
Dynamic expression evaluation for calculated values:
```csharp
public class ExpressionEvaluator
{
    public object Evaluate(string expression, Dictionary<string, object> variables)
    {
        // Compiles and evaluates C# expressions at runtime
        // Supports: math operations, string manipulation, conditionals
    }
}
```

Example usage:
```json
{
  "CsvColumn": "Diameter_MM",
  "ToolField": "Diameter",
  "Expression": "value * 0.03937"  // Convert mm to inches
}
```

### Value Translation Pattern
ValueMap translates external values to HSMAdvisor enums:
```json
{
  "ValueMap": {
    "HSS Steel": "HSS",
    "Cobalt": "HSCobalt",
    "Carb": "Carbide"
  }
}
```

### Reflection-Based Mapping Pattern
Uses reflection to dynamically set Tool properties:
```csharp
public static class ReflectionHelpers
{
    public static void SetToolField(Tool tool, string fieldName, object value)
    {
        var field = typeof(Tool).GetField(fieldName);
        field?.SetValue(tool, ConvertValue(value, field.FieldType));
    }
}
```

### Visual Editor Pattern
Windows Forms-based mapping editor with:
- **Column Discovery**: Reads CSV headers automatically
- **Field Dropdown**: Shows available Tool fields
- **Enum Selection**: Built-in enum value picker
- **Value Map Editor**: GUI for translation rules
- **Expression Editor**: Syntax-highlighted expression editing
- **Real-time Validation**: Checks mappings before save

## Solution Structure

### Multi-Project Organization
```
HSMAdvisorPlugins Solution
├── ExchangeHSMWorks/              # HSMWorks plugin + tests
│   └── ExchangeHSMWorks.Tests/
├── ImportCsvTools/                # CSV plugin + tests
│   └── ImportCsvTools.Tests/
├── Plugin-Test-Runner-UI/         # Shared testing tool
└── HSMadvisorDlls/               # Shared references
```

### Shared Testing Infrastructure
Both plugins use the same testing patterns:
- **Unit Test Projects**: NUnit-based test suites
- **Test Data Directories**: Sample files for validation
- **Plugin Test Runner**: Shared UI testing tool
- **Integration Tests**: Test with real data files

## Testing Patterns

### Plugin Testing Architecture
The Plugin Test Runner follows these patterns:
- **Dynamic Loading**: Uses reflection to load plugins at runtime
- **Interface Validation**: Ensures plugins implement required interfaces
- **Method Invocation**: Dynamically calls plugin methods based on capabilities
- **Result Inspection**: Uses PropertyGrid for detailed result examination

### Unit Testing Pattern (ImportCsvTools)
```csharp
[TestMethod]
public void TestCsvImport()
{
    var importer = new CsvToolImporter();
    var mapping = LoadMapping("Tool_Master_Import_for_HSMA.mapping.json");
    var csv = File.ReadAllText("Tool Master Import for HSMA.csv");
    
    var result = importer.Import(csv, mapping);
    
    Assert.IsNotNull(result);
    Assert.IsTrue(result.Tools.Count > 0);
    // Validate required fields
    foreach (var tool in result.Tools)
    {
        Assert.IsNotNull(tool.Tool_type_id);
        Assert.IsTrue(tool.Diameter > 0);
    }
}
```

This architecture enables rapid plugin development and testing without requiring full HSMAdvisor integration.
