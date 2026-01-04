# Plugin Test Runner

A Windows Forms application for testing and debugging HSMAdvisor plugins during development.

## Overview

The Plugin Test Runner provides a standalone testing environment for HSMAdvisor plugins without requiring a full HSMAdvisor installation. It allows developers to load plugins, execute their capabilities, and inspect results interactively.

### Key Features

- **Dynamic Plugin Loading** - Load plugins from any directory
- **Capability Execution** - Test ImportTools, ExportTools, and ModifyTools methods
- **Result Inspection** - View returned DataBase objects with PropertyGrid
- **Tool Browsing** - Examine individual tools and their properties
- **No HSMAdvisor Required** - Test plugins independently during development
- **Debug Support** - Attach debugger for troubleshooting

## Installation

### Running Pre-built Version

1. Build the solution or download the release
2. Navigate to `Plugin-Test-Runner-UI/bin/Debug/` or `bin/Release/`
3. Run `Plugin-Test-Runner-UI.exe`

### Building from Source

1. Open the solution in Visual Studio
2. Ensure VB.NET support is installed
3. Build the Plugin-Test-Runner-UI project
4. Output executable will be in `bin/Debug/` or `bin/Release/`

## Usage

### Basic Workflow

1. **Launch the Application**
   ```cmd
   cd Plugin-Test-Runner-UI\bin\Debug
   Plugin-Test-Runner-UI.exe
   ```

2. **Select Plugin Directory**
   - Click "Browse" or enter path to your plugin's output folder
   - Example: `C:\Projects\HSMAdvisorPlugins\ExchangeHSMWorks\bin\Debug`

3. **Load Plugin**
   - Select the plugin DLL from the dropdown
   - Plugin capabilities will appear in the list

4. **Execute Capability**
   - Double-click a capability (e.g., "Import HSMWorks Tool Database")
   - Follow any file dialogs or prompts from the plugin
   - Results will appear in the PropertyGrid

5. **Inspect Results**
   - Browse imported tools in the tree view
   - Examine tool properties in the PropertyGrid
   - Verify all fields are populated correctly

### Interface Overview

```
┌─────────────────────────────────────────────┐
│ Plugin Path: [________________] [Browse]    │
│ Plugin: [ExchangeHSMWorks.dll ▼]           │
├─────────────────────────────────────────────┤
│ Capabilities:                               │
│  • Import HSMWorks Tool Database            │
│  • Export HSMWorks Tool Database            │
├─────────────────────────────────────────────┤
│ Results (PropertyGrid):                     │
│  ├─ DataBase                               │
│  │   ├─ Tools [Collection]                │
│  │   │   ├─ [0] Tool                      │
│  │   │   │   ├─ Diameter: 0.375          │
│  │   │   │   ├─ Flute_N: 4               │
│  │   │   │   └─ ...                       │
│  │   │   └─ [1] Tool                      │
│  │   └─ Libraries [Collection]            │
└─────────────────────────────────────────────┘
```

## Testing Plugins

### Testing Import Functionality

1. Load your plugin in the Test Runner
2. Double-click the Import capability
3. Select a test input file
4. Verify results:
   - Check Tools count is correct
   - Inspect individual tool properties
   - Verify required fields are populated
   - Check unit flags are set correctly
   - Confirm tool types are mapped properly

### Testing Export Functionality

1. First import some test data
2. Double-click the Export capability
3. Choose output file location
4. Open the exported file in appropriate tool to verify

### Testing Round-Trip Conversion

1. Import tools from external format
2. Immediately export to same format
3. Compare exported file with original
4. Verify data preservation and fidelity

## Debugging Plugins

### Attaching Debugger

1. **Build plugin in Debug mode**
   ```cmd
   msbuild YourPlugin.csproj /p:Configuration=Debug
   ```

2. **Launch Test Runner**
   ```cmd
   Plugin-Test-Runner-UI.exe
   ```

3. **Attach Visual Studio Debugger**
   - In Visual Studio: Debug → Attach to Process
   - Select `Plugin-Test-Runner-UI.exe`
   - Set breakpoints in your plugin code

4. **Execute Plugin Capability**
   - Debugger will break at your breakpoints
   - Step through code and inspect variables

### Debug Output

The Test Runner captures console output from plugins:
- Check Visual Studio Output window
- Look for Debug.WriteLine() messages
- Review exception details

### Common Debugging Scenarios

**Investigating Conversion Logic:**
```csharp
public override DataBase ImportTools()
{
    Debug.WriteLine("Starting import...");
    var db = new DataBase();
    
    foreach (var item in sourceData)
    {
        Debug.WriteLine($"Converting: {item.Name}");
        var tool = ConvertTool(item);
        db.Tools.Add(tool);
    }
    
    Debug.WriteLine($"Imported {db.Tools.Count} tools");
    return db;
}
```

**Testing Error Handling:**
```csharp
try
{
    var result = ParseInputFile(fileName);
}
catch (Exception ex)
{
    Debug.WriteLine($"Parse error: {ex.Message}");
    MessageBox.Show($"Error: {ex.Message}");
    return null;
}
```

## Test-Driven Development

### Unit Test Integration

Use the Test Runner alongside unit tests:

1. **Write Unit Tests**
   ```csharp
   [TestMethod]
   public void TestImport()
   {
       var converter = new MyConverter();
       var result = converter.ImportTools();
       Assert.IsNotNull(result);
   }
   ```

2. **Manual Verification**
   - Run unit tests for automated validation
   - Use Test Runner for visual inspection
   - Verify edge cases interactively

### Test Data Management

Organize test files for different scenarios:

```
test-data/
├── valid/
│   ├── simple-tools.ext
│   ├── complex-geometry.ext
│   └── large-library.ext
├── invalid/
│   ├── missing-required-field.ext
│   └── corrupt-file.ext
└── edge-cases/
    ├── empty-library.ext
    └── special-characters.ext
```

## Workflows

### New Plugin Development

1. Create plugin project
2. Implement ToolsPluginInterface
3. Build in Debug mode
4. Load in Test Runner
5. Test each capability
6. Iterate based on results
7. Add unit tests
8. Test edge cases
9. Build Release version

### Investigating Issues

1. Reproduce issue in Test Runner
2. Attach debugger
3. Set breakpoints at suspected locations
4. Execute problematic capability
5. Step through code
6. Identify root cause
7. Implement fix
8. Verify fix in Test Runner
9. Update unit tests

### Regression Testing

1. Maintain set of test files
2. After changes, load plugin
3. Test all capabilities with known-good files
4. Compare results with previous versions
5. Verify no functionality broken

## Architecture

### Technology Stack

- **Language:** VB.NET
- **Framework:** .NET Framework 4.8
- **UI:** Windows Forms
- **Plugin Loading:** Reflection-based dynamic loading

### Key Components

- **Form1.vb** - Main application form and logic
- **Plugin Loading** - Dynamic assembly loading
- **Capability Execution** - Reflection-based method invocation
- **PropertyGrid Integration** - Result visualization

### Plugin Loading Process

```
1. User selects plugin directory
2. Application scans for DLL files
3. Loads assembly using Reflection
4. Searches for ToolsPluginInterface implementations
5. Instantiates plugin class
6. Calls GetCapabilities() to populate list
7. User selects capability
8. Application invokes corresponding method
9. Results displayed in PropertyGrid
```

## Limitations

- **No HSMAdvisor Context** - Plugins run outside full application context
- **Limited Database Operations** - Can't test integration with HSMAdvisor database
- **UI Only** - Not suitable for automated testing (use unit tests)
- **Windows Only** - Windows Forms application requires Windows

## Tips and Best Practices

### Effective Testing

1. **Test Early and Often** - Use Test Runner throughout development
2. **Multiple Test Files** - Test with various input scenarios
3. **Property Verification** - Always inspect returned tool properties
4. **Unit Flags Check** - Verify metric/imperial flags are correct
5. **Enum Validation** - Ensure enum values are valid

### Performance Testing

1. Test with large libraries (1000+ tools)
2. Monitor import/export time
3. Check memory usage
4. Verify UI remains responsive

### Error Handling

1. Test with invalid files
2. Test with missing required fields
3. Test with corrupted data
4. Verify graceful error messages

## Troubleshooting

### Plugin Not Loading

**Problem:** Plugin doesn't appear in dropdown
- Verify DLL is built successfully
- Check plugin implements ToolsPluginInterface
- Ensure all dependencies are in same directory
- Try different build configuration (Debug vs Release)

**Problem:** "Could not load assembly" error
- Check .NET Framework version matches
- Verify HSMAdvisor DLLs are present
- Ensure plugin isn't corrupted

### Capability Execution Failures

**Problem:** Exception when executing capability
- Attach debugger to see exact error
- Check file paths are valid
- Verify input file format is correct
- Review plugin error handling

**Problem:** Nothing happens when double-clicking capability
- Check if plugin shows file dialog (might be hidden)
- Look for exceptions in debug output
- Verify capability implementation isn't empty

### Result Inspection Issues

**Problem:** PropertyGrid is empty
- Check if plugin returns null
- Verify DataBase object is created
- Ensure Tools collection is populated

**Problem:** Can't see tool properties
- Expand tree nodes in PropertyGrid
- Check if Tool objects are properly initialized
- Verify properties are public

## Advanced Usage

### Batch Testing

Create scripts to test multiple files:

```vbnet
' Pseudo-code for automation
For Each testFile In testFiles
    plugin.ImportTools(testFile)
    ValidateResults()
Next
```

### Custom Test Harnesses

Extend the Test Runner for specific needs:
- Add logging functionality
- Implement comparison tools
- Create automated test sequences
- Export results to reports

## References

- [Root README](../README.md) - Plugin development guide
- [ExchangeHSMWorks Plugin](../ExchangeHSMWorks/README.md) - Example plugin
- [ImportCsvTools Plugin](../ImportCsvTools/README.md) - CSV import plugin

## License

This tool is part of the HSMAdvisorPlugins project and follows the same MIT License.
