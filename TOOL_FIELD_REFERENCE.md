# HSMAdvisor Tool Field Reference

This document provides a comprehensive reference for the `ToolDatabase.Tool` class fields, used by all HSMAdvisor plugins for importing and exporting tool data.

## Tool Class Overview

The `Tool` class represents a cutting tool in HSMAdvisor. When building plugins, you'll create and populate `Tool` objects with data from external sources.

## Required Fields

These fields **MUST** be set for every tool:

| Field | Type | Description | Enum Values |
|-------|------|-------------|-------------|
| **Tool_type_id** | ToolTypes | Tool type classification | See ToolTypes enum below |
| **Tool_material_id** | ToolMaterials | Tool material | Unknown, HSS, HSCobalt, Carbide, PCD, Ceramic |
| **Coating_id** | ToolCoatings | Tool coating | Unknown, None, TiN, TiCN, TiAlN, AlTiN, AlCrN, ZrN, nACRo, nACo |
| **Diameter** | double | Tool diameter | Numeric value |

## All Tool Fields

### Tool Identification & Performance

| Field | Type | Description |
|-------|------|-------------|
| Number | int | Tool number |
| Comment | string | Tool description/comments |
| Library | string | Tool library name |
| Tool_material_id | ToolMaterials | Tool material (required) |
| Tool_type_id | ToolTypes | Tool type (required) |
| Coating_id | ToolCoatings | Tool coating (required) |
| Maxdeflection_pc | double | Maximum deflection percentage |
| Maxtorque_pc | double | Maximum torque percentage |
| Productivity | int | Productivity rating |
| Generic | bool | Generic tool flag |
| Custom_SF | bool | Custom surface footage |
| Rating | int | Tool rating |
| Tool_life | int | Tool life |

### Dimensional Properties

| Field | Type | Description | Unit Flag Field |
|-------|------|-------------|-----------------|
| Diameter | double | Tool diameter (required) | Diameter_m |
| Tip_Diameter | double | Tip diameter | Tip_Diameter_m |
| Shank_Dia | double | Shank diameter | Shank_Dia_m |
| Flute_Len | double | Flute length | Flute_len_m |
| Shoulder_Len | double | Shoulder length | Shoulder_len_m |
| Shoulder_Dia | double | Shoulder diameter | Shoulder_Dia_m |
| Stickout | double | Tool stickout | Stickout_m |
| Corner_rad | double | Corner radius | Corner_rad_m |
| Circle_dia | double | Circle diameter | Circle_dia_m |
| Depth | double | Cutting depth | Depth_m |
| Thread_drill_dia | double | Thread drill diameter | Thread_drill_dia_m |
| Pilot_Hole | double | Pilot hole diameter | Pilot_Hole_m |

### Cutting Parameters

| Field | Type | Description | Unit Flag Field |
|-------|------|-------------|-----------------|
| Flute_N | int | Number of flutes | - |
| Helix_angle | double | Helix angle (degrees) | - |
| Leadangle | double | Lead angle (degrees) | - |
| Toolangle_mode | ToolAngleModes | Tool angle mode | - |
| Toolangle | double | Tool angle (convenience property) | - |
| Doc | double | Depth of cut | Doc_m |
| Woc | double | Width of cut | Woc_m |
| Peck | double | Peck depth | Peck_m |
| Ramp | double | Ramp angle | - |
| Thread_pitch | double | Thread pitch | Thread_pitch_m |
| Tool_maxrpm | int | Maximum RPM | - |
| Sfm_adj | double | SFM adjustment | - |
| Ipt_adj | double | IPT adjustment | - |

### Geometry Flags

| Field | Type | Description |
|-------|------|-------------|
| Shoulder_Tapered | bool | Shoulder is tapered |
| Rampmilling | bool | Ramp milling capable |
| Circle_comp | bool | Circle compensation |
| Circle_external | int | Circle external flag |
| Hsm | bool | High speed machining |
| Hss | bool | High speed steel (deprecated, use Tool_material_id) |

### Tool Management

| Field | Type | Description |
|-------|------|-------------|
| Offset_Diameter | int | Offset diameter |
| Offset_Length | int | Offset length |
| Coolant | int | Coolant type |
| IsInsert | bool | Is insert tool |

### Inventory Management

| Field | Type | Description |
|-------|------|-------------|
| Instock | int | Quantity in stock |
| Stock_OrderQty | int | Stock order quantity |
| Stock_MinQty | int | Minimum stock quantity |
| Stock_MaxQty | int | Maximum stock quantity |
| Stock_OnOrderQty | int | Quantity on order |
| Stock_Received | int | Quantity received |
| Stock_Price | double | Stock price |

### Unit Flags (Metric/Imperial)

All dimensional properties have corresponding boolean flags to indicate metric (mm) units. When `false`, values are in imperial (inches) units.

| Flag Field | Controls Units For |
|------------|-------------------|
| Diameter_m | Diameter |
| Tip_Diameter_m | Tip_Diameter |
| Shank_Dia_m | Shank_Dia |
| Flute_len_m | Flute_Len |
| Shoulder_len_m | Shoulder_Len |
| Shoulder_Dia_m | Shoulder_Dia |
| Stickout_m | Stickout |
| Corner_rad_m | Corner_rad |
| Circle_dia_m | Circle_dia |
| Depth_m | Depth |
| Doc_m | Doc |
| Woc_m | Woc |
| Peck_m | Peck |
| Thread_pitch_m | Thread_pitch |
| Thread_drill_dia_m | Thread_drill_dia |
| Pilot_Hole_m | Pilot_Hole |
| Ipt_m | IPT (feed per tooth) |
| Sfm_m | SFM (surface speed) |
| Feed_m | Feed rate |
| Input_units_m | Input units preference |
| Result_units_m | Result units preference |

## Enumerations

### ToolTypes Enum

Classification of tool types supported by HSMAdvisor:

```
Unknown
SolidEndMill
SolidBallMill
HPEndMill
IndexedEndMill
IndexedBallMill
IndexedFaceMill
JobberTwistDrill
SpotDrill
CounterSink
Reamer
HighFeedMill
ParabolicDrill
StraightFluteDrill
SpadeDrill
ThreadMill
HelicalIndexedEndMill
VbitEngraver
TurningProfiling
TurningFacing
TurningGrooving
BoringBar
Tap
FormingTap
Counterbore
ChamferMill
IndexedDrill
BoringHead
WoodRuff
```

### ToolMaterials Enum

```
Unknown
HSS
HSCobalt
Carbide
PCD
Ceramic
```

### ToolCoatings Enum

```
Unknown
None
TiN
TiCN
TiAlN
AlTiN
AlCrN
ZrN
nACRo
nACo
```

### ToolAngleModes Enum

```
Lead
Taper
Tip
```

## Usage Guidelines

### Creating a Tool

```csharp
var tool = new Tool(true)
{
    Tool_type_id = Enums.ToolTypes.SolidEndMill,
    Tool_material_id = Enums.ToolMaterials.Carbide,
    Coating_id = Enums.ToolCoatings.AlTiN,
    Diameter = 0.375,
    Diameter_m = false, // inches
    Flute_N = 4,
    Flute_Len = 1.0,
    Flute_len_m = false,
    Comment = "3/8\" 4-Flute Carbide End Mill",
    Library = "My Tools"
};
```

### Unit Conversion Strategy

When importing tools, determine the source units and set all unit flags accordingly:

```csharp
bool isMetric = (sourceUnits == "mm" || sourceUnits == "millimeters");

tool.Diameter_m = isMetric;
tool.Flute_len_m = isMetric;
tool.Shank_Dia_m = isMetric;
// Set all relevant dimensional flags
```

### Handling Unknown Values

Use `-1` or appropriate default values for unknown properties:

```csharp
tool.Maxdeflection_pc = -1;  // Unknown
tool.Maxtorque_pc = -1;      // Unknown
tool.Productivity = -1;      // Unknown
```

### Data Preservation

For round-trip fidelity, consider storing original source data:

```csharp
// Store original XML/JSON in Aux_data field
tool.Aux_data = originalDataString;
```

This allows perfect conversion reversal and preserves format-specific properties.

## Best Practices

1. **Always set required fields** - Tool_type_id, Tool_material_id, Coating_id, Diameter
2. **Handle unit flags consistently** - Set all dimensional unit flags based on source data
3. **Provide meaningful defaults** - Use sensible defaults for optional fields
4. **Preserve original data** - Store source format data when possible for round-trip capability
5. **Validate enums** - Map external values to HSMAdvisor enums carefully
6. **Handle errors gracefully** - Provide fallback values for invalid data
7. **Document conversions** - Include comments explaining non-obvious mappings

## See Also

- [ExchangeHSMWorks Plugin](ExchangeHSMWorks/README.md) - Example of comprehensive tool type mapping
- [ImportCsvTools Plugin](ImportCsvTools/README.md) - CSV mapping configuration patterns
- Root README.md - Plugin development guide
