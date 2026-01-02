# Product Context

## Why This Project Exists

### Problem Statement
HSMAdvisor is a powerful machining optimization tool, but users often have existing tool libraries in other systems:

**HSMWorks/Fusion 360 Users:**
Without interoperability, users with HSMWorks tool libraries must manually recreate their tool databases.

**Generic CSV Data Users:**
Many organizations maintain tool data in spreadsheets, vendor-provided CSV files, or other systems that export to CSV. Without a flexible import mechanism, users face manual data entry for potentially hundreds of tools.

These issues lead to:
- **Time Waste**: Hours or days spent manually entering tool data
- **Data Loss**: Risk of losing detailed tool specifications during manual transfer
- **Adoption Barriers**: Reluctance to switch to HSMAdvisor due to existing investments in tool libraries
- **Workflow Disruption**: Inability to maintain consistent tool data across different systems
- **Vendor Lock-in**: Difficulty integrating data from multiple tool vendors

### Solution Approach
The HSMAdvisorPlugins project solves this by providing:

1. **HSMWorks Integration**: Direct import/export between HSMWorks and HSMAdvisor formats
2. **Flexible CSV Import**: JSON-based mapping system handles diverse CSV formats without code changes
3. **Data Preservation**: Maintains tool specifications, materials, geometries, and cutting parameters
4. **Extensible Architecture**: Plugin framework allows support for additional formats
5. **Visual Configuration**: GUI-based mapping editor eliminates manual JSON editing
6. **Developer Tools**: Testing infrastructure for plugin development and validation

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

#### HSMWorks Import Workflow
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

#### HSMWorks Export Workflow
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

#### CSV Import Workflow
```
User selects CSV file
↓
Plugin reads CSV structure
↓
User selects or creates mapping configuration
↓
   [First Time: Use Visual Mapping Editor]
   ↓
   Preview CSV columns
   ↓
   Map columns to Tool fields
   ↓
   Configure value translations
   ↓
   Add expressions for calculations
   ↓
   Save mapping configuration
↓
Plugin applies mapping rules:
  - Direct field assignments
  - Value map translations
  - Expression evaluations
  - Default value fallbacks
↓
Creates HSMAdvisor Tool objects
↓
Validates required fields
↓
Integrates into existing database
↓
User reviews imported tools
```

#### CSV Mapping Configuration Workflow
```
User needs to import CSV from new vendor
↓
Launch Visual Mapping Editor
↓
Load sample CSV file
↓
For each CSV column:
  - Select target Tool field
  - Configure value translation if needed
  - Add expression if calculation required
  - Set default value if column may be missing
↓
Test mapping with sample rows
↓
Save configuration as JSON file
↓
Reuse configuration for future imports
```

## Value Proposition

### For HSMAdvisor Users
- **Reduced Setup Time**: Import existing tool libraries in minutes instead of hours or days
- **Data Accuracy**: Eliminate manual entry errors
- **Workflow Continuity**: Maintain tool data consistency across CAM systems
- **Investment Protection**: Preserve existing tool library investments
- **Vendor Flexibility**: Import tools from any vendor providing CSV data
- **Bulk Operations**: Process hundreds of tools at once
- **Reusable Configurations**: Create mapping once, use for all future imports from same vendor

### For HSMAdvisor Ecosystem
- **Increased Adoption**: Lower barriers to HSMAdvisor adoption
- **Competitive Advantage**: Unique interoperability features
- **Community Growth**: Enable third-party plugin development
- **Market Expansion**: Access to HSMWorks/Fusion 360 user base

### For Plugin Developers
- **Clear Framework**: Well-defined interfaces and patterns
- **Development Tools**: Testing infrastructure and examples
- **Distribution Channel**: Plugin ecosystem for HSMAdvisor users
- **Technical Foundation**: Reusable patterns for other format integrations
- **Multiple Examples**: Both HSMWorks (XML) and CSV plugins demonstrate different approaches

## Success Metrics

### Technical Success
- Import/export operations complete without data loss
- Tool parameters accurately converted between formats
- Plugins load and integrate seamlessly with HSMAdvisor
- Performance acceptable for typical tool library sizes (100-1000 tools)
- CSV mapping system handles diverse vendor formats without code changes
- Expression evaluation provides flexibility for complex transformations

### User Success
- Users can import their HSMWorks libraries in under 5 minutes
- Users can import vendor CSV files in under 10 minutes (including mapping configuration)
- Reusable mapping configurations make subsequent imports take under 2 minutes
- Converted tools work correctly in HSMAdvisor calculations
- Export files load successfully in HSMWorks/Fusion 360
- Visual mapping editor enables non-programmers to create configurations
- Documentation enables successful plugin development by third parties

### CSV Import Specific Success
- Handle 90% of vendor CSV formats without custom code
- Mapping editor enables configuration creation in under 15 minutes
- Expression system handles unit conversions and value transformations
- Value maps correctly translate vendor-specific terminology
- Validation prevents import of incomplete tool data
