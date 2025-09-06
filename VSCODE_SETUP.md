# VSCode Setup for HSMAdvisorPlugins

This document explains how to build and debug the HSMAdvisorPlugins solution using Visual Studio Code.

## Required Extensions

VSCode will automatically recommend the following extensions when you open this workspace. Install them for the best development experience:

### Essential Extensions
- **C# for Visual Studio Code** (`ms-dotnettools.csharp`) - C# language support and debugging
- **.NET Install Tool** (`ms-dotnettools.vscode-dotnet-runtime`) - .NET runtime management
- **XML** (`redhat.vscode-xml`) - XML file support for project files and schemas
- **JSON** (`ms-vscode.vscode-json`) - JSON configuration file support
- **PowerShell** (`ms-vscode.powershell`) - PowerShell script support

### Optional but Recommended
- **.NET Core Test Explorer** (`formulahendry.dotnet-test-explorer`) - Test discovery and execution
- **C# Extensions** (`jchannon.csharpextensions`) - Additional C# productivity features
- **C# Extensions** (`kreativ-software.csharpextensions`) - More C# code generation tools

## Building the Solution

### Using Command Palette (Ctrl+Shift+P)
1. Open Command Palette: `Ctrl+Shift+P`
2. Type "Tasks: Run Task"
3. Select one of the available build tasks:
   - **build-debug** (default) - Build entire solution in Debug configuration
   - **build-release** - Build entire solution in Release configuration
   - **build-plugin-only** - Build only the ExchangeHSMWorks plugin
   - **clean** - Clean all build outputs
   - **rebuild** - Clean and rebuild solution

### Using Keyboard Shortcuts
- **Build (Default)**: `Ctrl+Shift+P` → "Tasks: Run Build Task" or `Ctrl+Shift+B`
- **Quick Build**: The default build task is "build-debug"

### Using Terminal
You can also run the MSBuild commands directly in the integrated terminal:
```cmd
# Build Debug
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" HSMAdvisor-Plugins.sln /p:Configuration=Debug /p:Platform="Any CPU"

# Build Release  
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" HSMAdvisor-Plugins.sln /p:Configuration=Release /p:Platform="Any CPU"
```

## Debugging

### Debug Configurations Available

1. **Debug Plugin Test Runner**
   - Launches the Plugin Test Runner application with debugging
   - Automatically builds the solution first
   - Best for testing plugin functionality

2. **Debug Plugin Test Runner (No Build)**
   - Launches the Plugin Test Runner without building first
   - Faster startup if you know the build is current

3. **Attach to HSMAdvisor Process**
   - Attaches debugger to a running HSMAdvisor.exe process
   - Useful for debugging the plugin when loaded in HSMAdvisor

### Starting Debug Session
1. **F5** - Start debugging with the default configuration
2. **Ctrl+F5** - Run without debugging
3. **Debug Panel** - Click the debug icon in the sidebar, select configuration, and click the play button

### Setting Breakpoints
- Click in the left margin of any C# or VB.NET file to set breakpoints
- Breakpoints work in both the plugin code and the test runner
- Use conditional breakpoints for complex debugging scenarios

## Project Structure in VSCode

```
HSMAdvisorPlugins/
├── .vscode/                    # VSCode configuration
│   ├── tasks.json             # Build tasks
│   ├── launch.json            # Debug configurations  
│   ├── settings.json          # Workspace settings
│   └── extensions.json        # Recommended extensions
├── ExchangeHSMWorks/          # Main plugin project (C#)
├── Plugin-Test-Runner-UI/     # Test runner (VB.NET)
├── HSMadvisorDlls/           # HSMAdvisor dependencies
└── memory-bank/              # Project documentation
```

## Build Outputs

### Debug Build
- **Plugin DLL**: `ExchangeHSMWorks/bin/Debug/ExchangeHSMWorks.dll`
- **Test Runner**: `Plugin-Test-Runner-UI/bin/Debug/Plugin-Test-Runner-UI.exe`

### Release Build  
- **Plugin DLL**: Deployed to HSMAdvisor plugins directory
- **Test Runner**: `Plugin-Test-Runner-UI/bin/Release/Plugin-Test-Runner-UI.exe`

## Testing the Plugin

### Using the Plugin Test Runner
1. Build the solution: `Ctrl+Shift+B`
2. Run task: "run-plugin-test-runner" or press `F5` to debug
3. The test runner will launch and allow you to:
   - Load the ExchangeHSMWorks plugin
   - Test import/export functionality
   - Inspect plugin capabilities
   - Debug conversion issues

### Manual Testing
1. Build in Release configuration
2. Copy `ExchangeHSMWorks.dll` to `%APPDATA%\HSMAdvisor\Plugins`
3. Launch HSMAdvisor to test the plugin integration

## Troubleshooting

### Build Issues
- **MSBuild not found**: Ensure Visual Studio 2022 is installed
- **Reference errors**: Check that HSMAdvisor DLLs are present in `HSMadvisorDlls/`
- **Permission errors**: Run VSCode as administrator if needed

### Debug Issues
- **Debugger won't attach**: Install the C# extension and .NET runtime
- **Breakpoints not hit**: Ensure you're building in Debug configuration
- **Process not found**: Make sure HSMAdvisor.exe is running for attach scenarios

### IntelliSense Issues
- **No code completion**: Install the C# extension and restart VSCode
- **Project not loaded**: Check that OmniSharp is running (bottom status bar)
- **References not resolved**: Try "OmniSharp: Restart OmniSharp" from Command Palette

## Tips for Development

1. **Use the integrated terminal** for quick commands and Git operations
2. **Enable format on save** (already configured) for consistent code style
3. **Use the Problems panel** to see build errors and warnings
4. **Leverage IntelliSense** for code completion and documentation
5. **Use the Explorer panel** to navigate the project structure efficiently

## Additional Resources

- [C# programming in VS Code](https://code.visualstudio.com/docs/languages/csharp)
- [Debugging in VS Code](https://code.visualstudio.com/docs/editor/debugging)
- [Tasks in VS Code](https://code.visualstudio.com/docs/editor/tasks)
