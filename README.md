# HSMAdvisorPlugins

## This Solution contains core HSMAdvisor plugins:

* ExchangeHSMWorks -  Plugin to Import HSMWorks .hsmlib tool database files

## Pre-requisites
* Visual Studio 2019
* .NET Development Experience
* 

## How to Use:

* Create a .NET Framework v4.5 Class Library Project 
* Add refrenece to HSMAdvisorPlugin.dll ObjectToolDatabase.dll
* Create a class that ***implements*** ToolsPluginInterface
* Declare plugin capabilities like so:
```
        public override List<Capability> GetCapabilities()
        {
            var caps = new List<Capability>();
            caps.Add(new Capability("Import HSMWords Tool Database", (int)ToolsPluginCapabilityMethod.ImportTools));

            return caps;
        }
```
* Overwrite other ToolsPluginInterface methods to implement the declared capability functionality:
```
        public override DataBase ImportTools() {
          var targetDB = new DataBase();
          //Add sample tool with diameter 0.375
          targetDB.AddTool(new Tool() { Diameter= 0.375 });
          return targetDB
        }
```
* Build your plugin in release mode and copy it into the Plugins folder located in the HSMAdvisor AppData directory: ```C:\Users\{USERNAME}\AppData\Roaming\HSMAdvisor\Plugins```
* Restart HSMAdvisor and it should pick up your plugin
* Core plugins are installed into the Program Files and then copied by HSMAdvisor into the AppData folder

## Contributing

Anyone is welcome to create their own plugins. 