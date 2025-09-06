# HSMAdvisorPlugins Project Brief

## Project Overview
HSMAdvisorPlugins is a C# solution that provides extensible plugin architecture for HSMAdvisor, a machining optimization software. The project enables import/export of tool databases from various CAM systems into HSMAdvisor's native format.

## Core Requirements

### Primary Goals
1. **Tool Database Interoperability**: Enable HSMAdvisor to work with tool databases from different CAM systems
2. **Plugin Architecture**: Provide a standardized interface for creating HSMAdvisor plugins
3. **HSMWorks Integration**: Specifically support import/export of HSMWorks .hsmlib tool database files
4. **Developer Framework**: Offer clear documentation and examples for plugin development

### Key Components
1. **ExchangeHSMWorks Plugin**: Core plugin for HSMWorks tool database conversion
2. **Plugin Test Runner UI**: Development tool for testing plugins during development
3. **HSMAdvisor Core Integration**: Seamless integration with HSMAdvisor's plugin system

### Technical Requirements
- **.NET Framework 4.5**: Target framework for compatibility
- **Plugin Interface Compliance**: All plugins must implement `ToolsPluginInterface`
- **XML Schema Support**: Handle HSMWorks XML tool library format
- **Bidirectional Conversion**: Support both import and export operations
- **Tool Type Mapping**: Convert between HSMWorks and HSMAdvisor tool classifications

### Success Criteria
1. Successfully import HSMWorks .hsmlib files into HSMAdvisor
2. Export HSMAdvisor tool databases to HSMWorks format
3. Maintain tool data integrity during conversion
4. Provide clear plugin development documentation
5. Enable easy plugin installation and deployment

### Constraints
- Must work with existing HSMAdvisor plugin architecture
- Plugin deployment to `%APPDATA%\HSMAdvisor\Plugins` directory
- Visual Studio 2019 development environment requirement
- Backward compatibility with existing HSMAdvisor installations

## Project Scope
- **In Scope**: HSMWorks tool database conversion, plugin framework, testing tools
- **Out of Scope**: HSMAdvisor core application modifications, other CAM system integrations (beyond HSMWorks)

## Stakeholders
- **Primary Users**: HSMAdvisor users with HSMWorks tool libraries
- **Developers**: Plugin developers extending HSMAdvisor functionality
- **Maintainers**: HSMAdvisor development team
