# Technical Context

## Technology Stack

### Core Technologies
- **.NET Framework 4.8**: Target framework for compatibility with HSMAdvisor
- **C# 6.0+**: Primary development language
- **Visual Basic .NET**: Used for Plugin Test Runner UI
- **Windows Forms**: UI framework for test runner and mapping editors
- **XML Serialization**: For HSMWorks format handling
- **JSON Serialization**: For CSV mapping configurations
- **System.Reflection**: For dynamic plugin loading and Tool field mapping
- **C# Expression Evaluation**: Runtime expression compilation for CSV mappings
- **CSV Parsing**: Text parsing for CSV file import

### Development Environment
- **Visual Studio 2019**: Required IDE
- **Windows 10/11**: Development and target platform
- **Git**: Version control (GitHub repository)
- **.NET Development Experience**: Required skill set

### Key Dependencies

#### HSMAdvisor Core Libraries
Located in `HSMadvisorDlls/`:
- **HSMAdvisorDatabase.dll**: Database and tool management
- **HSMAdvisorPlugin.dll**: Plugin interface definitions
- **Newtonsoft.Json.dll**: JSON serialization support

#### System Dependencies
- **System.Windows.Forms**: File dialogs and UI components
- **System.Xml.Serialization**: XML schema handling
- **System.IO**: File operations
- **System.Linq**: Data querying and manipulation

## Project Structure

### Solution Organization
```
HSMAdvisor-Plugins.sln
├── ExchangeHSMWorks/                 # HSMWorks plugin project
│   ├── Converter.cs                  # Core conversion logic
│   ├── Schema.cs                     # HSMWorks XML schema classes
│   ├── XMLSchema.xsd                 # HSMWorks schema definition
│   └── ExchangeHSMWorks.csproj       # C# class library project
├── ExchangeHSMWorks.Tests/           # HSMWorks unit tests
│   ├── SimpleConverterTest.cs        # Unit test suite
│   ├── test-data/                    # Sample .hsmlib files
│   └── ExchangeHSMWorks.Tests.csproj
├── ImportCsvTools/                   # CSV import plugin project
│   ├── CsvToolImporter.cs            # Main import engine
│   ├── CsvMapping.cs                 # Mapping configuration classes
│   ├── ExpressionEvaluator.cs        # Expression evaluation engine
│   ├── ReflectionHelpers.cs          # Tool field reflection utilities
│   ├── CsvImportColumnInfo.cs        # CSV column metadata
│   ├── Forms/                        # Visual mapping editor forms
│   │   ├── MappingEditorForm.cs      # Main mapping editor
│   │   ├── ExpressionEditorForm.cs   # Expression editor
│   │   ├── ValueMapEditorForm.cs     # Value translation editor
│   │   └── MappingSelectionForm.cs   # Mapping file selector
│   ├── SampleMapping.json            # Example mapping config
│   └── ImportCsvTools.csproj         # C# class library project
├── ImportCsvTools.Tests/             # CSV import unit tests
│   ├── CsvImportTest.cs              # Unit test suite
│   ├── test-data/                    # Sample CSV and mapping files
│   └── ImportCsvTools.Tests.csproj
├── Plugin-Test-Runner-UI/            # Testing application
│   ├── Form1.vb                      # Main test UI
│   ├── Form1.Designer.vb             # UI designer code
│   └── Plugin-Test-Runner-UI.vbproj  # VB.NET Windows Forms project
└── HSMadvisorDlls/                   # Reference assemblies
    ├── HSMAdvisorDatabase.dll
    ├── HSMAdvisorPlugin.dll
    └── Newtonsoft.Json.dll
```

### Build Configuration
- **Target Framework**: .NET Framework 4.8
- **Platform**: Any CPU
- **Configuration**: Release (for deployment)
- **Output**: Class library DLL files
- **Test Framework**: NUnit for unit testing

### Project Dependencies
**ExchangeHSMWorks:**
- HSMAdvisorDatabase.dll
- HSMAdvisorPlugin.dll
- System.Xml.Serialization

**ImportCsvTools:**
- HSMAdvisorDatabase.dll
- HSMAdvisorPlugin.dll
- Newtonsoft.Json.dll
- System.Windows.Forms (for editors)
- Microsoft.CSharp (for expression evaluation)

**Plugin-Test-Runner-UI:**
- HSMAdvisorDatabase.dll
- HSMAdvisorPlugin.dll
- System.Windows.Forms
- System.Reflection

## Technical Constraints

### Compatibility Requirements
- **HSMAdvisor Integration**: Must work with existing HSMAdvisor plugin architecture
- **XML Schema Compliance**: Must handle HSMWorks XML format exactly
- **.NET Framework 4.5**: Cannot use newer framework features
- **Windows Platform**: Windows-specific file paths and dialogs

### Performance Considerations
- **Memory Usage**: Handle large tool libraries (1000+ tools) efficiently
- **File I/O**: Minimize disk operations during conversion
- **XML Processing**: Efficient serialization/deserialization
- **UI Responsiveness**: Non-blocking operations in test runner

### Deployment Constraints
- **Plugin Directory**: Must deploy to `%APPDATA%\HSMAdvisor\Plugins`
- **Dependencies**: All dependencies must be available or bundled
- **Permissions**: Standard user permissions (no admin required)
- **Installation**: Copy-based deployment (no installer)

## Development Patterns

### Code Organization Patterns

#### Namespace Structure
```csharp
// ExchangeHSMWorks
namespace ExchangeHSMWorks
{
    public class Converter : ToolsPluginInterface  // Main plugin class
    // Schema classes (auto-generated)
    public partial class toollibrary
    public partial class toollibraryTool
    // ... other schema classes
}

// ImportCsvTools
namespace ImportCsvTools
{
    public class CsvToolImporter : ToolsPluginInterface  // Main plugin class
    public class CsvMapping                              // Configuration model
    public class CsvMappingConfig                        // Root config
    public class CsvImportColumnInfo                     // Column metadata
    public class ExpressionEvaluator                     // Expression engine
    public static class ReflectionHelpers                // Reflection utilities
    
    // Forms namespace
    namespace ImportCsvTools.Forms
    {
        public class MappingEditorForm : Form
        public class ExpressionEditorForm : Form
        public class ValueMapEditorForm : Form
        public class MappingSelectionForm : Form
    }
}
```

#### Method Organization
```csharp
public class Converter : ToolsPluginInterface
{
    // Interface implementation
    public override List<Capability> GetCapabilities()
    public override DataBase ImportTools()
    public override void ExportTools(DataBase src)
    public override void ModifyTools(DataBase db)
    
    // Conversion methods
    public static Tool ToTool(toollibraryTool t)
    private toollibraryTool FromTool(Tool srcTool)
    
    // Utility methods
    public static Enums.ToolMaterials ToToolMaterial(string materialname)
    public static string FromToolMaterial(Enums.ToolMaterials material_id)
    
    // UI methods
    public string ShowOpenFileDialog()
    public string ShowSaveAsFileDialog()
}
```

### Error Handling Patterns

#### Graceful Degradation
```csharp
// Default values for unknown properties
ret.Maxdeflection_pc = -1;
ret.Maxtorque_pc = -1;
ret.Productivity = -1;
```

#### Exception Handling in Test Runner
```vb
Try
    currDataBase = tPlugin.ImportTools()
    PGrid.SelectedObject = currDataBase
Catch ex As Exception
    PGrid.SelectedObject = ex  ' Display exception details
End Try
```

#### Safe Type Conversion
```csharp
ret.Number = Parse.ToInteger(t.nc.number);  // Custom parsing with defaults
ret.Diameter = Parse.ToDouble(t.body.diameter);
```

### XML Handling Patterns

#### Schema-Based Serialization
```csharp
// Deserialize from XML
toollibrary src = Serializer.FromXML<toollibrary>(xml, false);

// Serialize to XML
File.WriteAllText(FileName, Serializer.ToXML(targetDB, "UTF-16"));
```

#### Attribute Mapping
```csharp
[System.Xml.Serialization.XmlAttribute("corner-radius")]
public string cornerradius { get; set; }

[System.Xml.Serialization.XmlElement("product-id")]
public string productid { get; set; }
```

### JSON Configuration Patterns

#### Mapping Configuration Structure
```csharp
public class CsvMappingConfig
{
    public string LibraryName { get; set; }
    public bool AllowInvalidToolImport { get; set; }
    public string CsvInputUnits { get; set; }  // "in" or "mm"
    public List<CsvMapping> Mappings { get; set; }
}

public class CsvMapping
{
    public string CsvColumn { get; set; }
    public string ToolField { get; set; }
    public Dictionary<string, string> ValueMap { get; set; }
    public string DefaultValue { get; set; }
    public string EnumType { get; set; }
    public string Expression { get; set; }
}
```

#### JSON Serialization
```csharp
// Load mapping configuration
var json = File.ReadAllText(mappingFilePath);
var config = JsonConvert.DeserializeObject<CsvMappingConfig>(json);

// Save mapping configuration
var json = JsonConvert.SerializeObject(config, Formatting.Indented);
File.WriteAllText(mappingFilePath, json);
```

### CSV Processing Patterns

#### CSV Parsing
```csharp
var lines = File.ReadAllLines(csvFilePath);
var headers = lines[0].Split(',');
var dataRows = lines.Skip(1);

foreach (var row in dataRows)
{
    var values = row.Split(',');
    var tool = CreateToolFromRow(headers, values, mapping);
    database.Tools.Add(tool);
}
```

#### Reflection-Based Field Setting
```csharp
public static void SetToolField(Tool tool, string fieldName, object value)
{
    var fieldInfo = typeof(Tool).GetField(fieldName);
    if (fieldInfo != null)
    {
        var convertedValue = Convert.ChangeType(value, fieldInfo.FieldType);
        fieldInfo.SetValue(tool, convertedValue);
    }
}
```

### Expression Evaluation Patterns

#### Runtime Expression Compilation
```csharp
public class ExpressionEvaluator
{
    public object Evaluate(string expression, Dictionary<string, object> context)
    {
        // Build C# code with expression
        var code = $"return {expression};";
        
        // Compile and execute at runtime
        // Uses Microsoft.CSharp.CSharpCodeProvider
        // or Roslyn scripting APIs
    }
}
```

#### Expression Usage Examples
```csharp
// Unit conversion
"value * 0.03937"  // mm to inches

// Conditional logic
"string.IsNullOrEmpty(value) ? 4 : int.Parse(value)"

// Boolean conversion
"value == \"1\" || value.ToLower() == \"yes\""

// Complex mapping
"value.Contains(\"Ball\") ? \"SolidBallMill\" : \"SolidEndMill\""
```

## Testing Infrastructure

### Plugin Test Runner Architecture
```vb
Public Class Form1
    Private currDataBase As HSMAdvisorDatabase.ToolDataBase.DataBase
    Private _plugins As List(Of HSMAdvisorPluginInterface)
    
    ' Plugin discovery and loading
    Me.Plugins = HSMAdvisorPlugin.PluginsReader.ReadPlugins(txt_pluginPath.Text)
    
    ' Dynamic method invocation
    Select Case cap.CapabilityMethod
        Case ToolsPluginCapabilityMethod.ImportTools
            currDataBase = tPlugin.ImportTools()
        Case ToolsPluginCapabilityMethod.ExportTools
            tPlugin.ExportTools(currDataBase)
    End Select
End Class
```

### Testing Workflow
1. **Plugin Discovery**: Scan directory for plugin DLLs
2. **Plugin Loading**: Use reflection to load and instantiate
3. **Capability Enumeration**: Display available plugin methods
4. **Method Execution**: Invoke selected plugin methods
5. **Result Inspection**: Use PropertyGrid to examine results

## Build and Deployment

### Build Process
1. **Restore NuGet Packages**: If any (currently none)
2. **Build Solution**: Visual Studio or MSBuild
3. **Copy Dependencies**: Ensure HSMAdvisor DLLs are available
4. **Output Verification**: Check DLL generation

### Deployment Process
1. **Build Release Configuration**: Optimized for production
2. **Copy Plugin DLL**: To `%APPDATA%\HSMAdvisor\Plugins`
3. **Copy Dependencies**: If not already present
4. **Restart HSMAdvisor**: To load new plugin
5. **Verify Integration**: Test import/export functionality

### Development Tools Usage

#### Visual Studio Features
- **IntelliSense**: Code completion and error detection
- **Debugger**: Step-through debugging for complex conversions
- **NuGet Package Manager**: Dependency management
- **Solution Explorer**: Project organization
- **Properties Window**: Project configuration

#### Git Workflow
- **Repository**: GitHub hosted
- **Branching**: Feature branches for development
- **Commits**: Atomic commits with clear messages
- **Pull Requests**: Code review process

This technical foundation provides the infrastructure for reliable HSMWorks tool database conversion while maintaining compatibility with the HSMAdvisor ecosystem.
