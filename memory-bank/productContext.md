# Product Context

## Why This Project Exists

### Problem Statement
HSMAdvisor is a powerful machining optimization tool, but users often have existing tool libraries in other CAM systems like HSMWorks (now Fusion 360 CAM). Without interoperability, users must manually recreate their tool databases, leading to:

- **Time Waste**: Hours spent manually entering tool data
- **Data Loss**: Risk of losing detailed tool specifications during manual transfer
- **Adoption Barriers**: Reluctance to switch to HSMAdvisor due to existing investments in tool libraries
- **Workflow Disruption**: Inability to maintain consistent tool data across different CAM systems

### Solution Approach
The HSMAdvisorPlugins project solves this by providing:

1. **Automated Conversion**: Direct import/export between HSMWorks and HSMAdvisor formats
2. **Data Preservation**: Maintains tool specifications, materials, geometries, and cutting parameters
3. **Extensible Architecture**: Plugin framework allows support for additional CAM systems
4. **Developer Tools**: Testing infrastructure for plugin development and validation

## How It Should Work

### User Experience Goals

#### For End Users
1. **Simple Import Process**:
   - File → Import → Select HSMWorks .hsmlib file
   - Automatic conversion and integration into HSMAdvisor database
   - Preview and validation before final import

2. **Seamless Export**:
   - File → Export → Choose HSMWorks format
   - Select tools/libraries to export
   - Generate .hsmlib file compatible with HSMWorks/Fusion 360

3. **Data Integrity**:
   - All tool parameters preserved during conversion
   - Material properties correctly mapped
   - Tool geometries accurately translated
   - Cutting parameters maintained

#### For Plugin Developers
1. **Clear Interface**: Well-documented `ToolsPluginInterface` with examples
2. **Testing Tools**: Plugin Test Runner UI for development and debugging
3. **Easy Deployment**: Simple copy-to-folder installation process
4. **Extensibility**: Framework supports additional CAM system integrations

### Core Workflows

#### Import Workflow
```
User selects .hsmlib file
↓
Plugin reads XML structure
↓
Maps HSMWorks tool types to HSMAdvisor equivalents
↓
Converts units and parameters
↓
Creates HSMAdvisor Tool objects
↓
Integrates into existing database
↓
User reviews imported tools
```

#### Export Workflow
```
User selects tools to export
↓
Plugin converts HSMAdvisor tools to HSMWorks format
↓
Generates XML structure per HSMWorks schema
↓
Creates .hsmlib file
↓
User saves file for use in HSMWorks/Fusion 360
```

## Value Proposition

### For HSMAdvisor Users
- **Reduced Setup Time**: Import existing tool libraries in minutes instead of hours
- **Data Accuracy**: Eliminate manual entry errors
- **Workflow Continuity**: Maintain tool data consistency across CAM systems
- **Investment Protection**: Preserve existing tool library investments

### For HSMAdvisor Ecosystem
- **Increased Adoption**: Lower barriers to HSMAdvisor adoption
- **Competitive Advantage**: Unique interoperability features
- **Community Growth**: Enable third-party plugin development
- **Market Expansion**: Access to HSMWorks/Fusion 360 user base

### For Plugin Developers
- **Clear Framework**: Well-defined interfaces and patterns
- **Development Tools**: Testing infrastructure and examples
- **Distribution Channel**: Plugin ecosystem for HSMAdvisor users
- **Technical Foundation**: Reusable patterns for other CAM integrations

## Success Metrics

### Technical Success
- Import/export operations complete without data loss
- Tool parameters accurately converted between formats
- Plugin loads and integrates seamlessly with HSMAdvisor
- Performance acceptable for typical tool library sizes (100-1000 tools)

### User Success
- Users can import their HSMWorks libraries in under 5 minutes
- Converted tools work correctly in HSMAdvisor calculations
- Export files load successfully in HSMWorks/Fusion 360
- Documentation enables successful plugin development by third parties
